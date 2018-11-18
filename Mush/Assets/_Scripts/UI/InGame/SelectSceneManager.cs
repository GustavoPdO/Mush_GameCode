using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SelectSceneManager : MonoBehaviour
{ //(Obs: apesar de provavelmente com alguns ajuste o jogo conseguir comportar mais de 2 jogadores, provavelmente sera necessaria uma reformulacao da interface comportar a adicao de mais de 2 jogadores)

    //public
    public string previousSceneName; //nome da cena anterior a essa cena //(nesse caso MainMenu)
    public string nextSceneName; //nome da proxima cena que carrega depois dessa cena //(nesse caso ArenaX, X = 1, 2, ...)

    public int numberOfPlayers; //numero de jogadores na tela de selecao //(por enquanto manter numberOfPlayers = 2)
    public Color[] playerSelectedColor; //cor dos "quadros" de selecao de cada jogador
    //public Color playerMultipleSelectedColor; //cor dos "quadros" de selecao quando ambos os jogadores estao no mesmo quadrado
    [Range(0.01f, 1)]
    public float selectedPortraitTintProportion; //proporcao da cor original que gera a cor tint dela quando o personagem for selecionado

    [Space(5)]
    public CharacterScriptableObject[] characterSO; //referencia para os dados dos personagens //(colocar na orde da tela de selecao)
    public string[] scenarioName; //nomes dos cenarios //(colocar na orde da tela de selecao)

    [Header("FadeInOut")]
    public Animator whiteBackgroundImage; //objeto/imagem/animator que contem a imagem de fundo branca 
    public Animator blackBackgroundImage; //objeto/imagem/animator que contem a imagem de fundo preta

    [Header("Sounds")] //nomes dos sons que vai tocar em cada parte
    public string menuMusicSound;
    [Space(3)]
    public string navSound;
    public string selectSound;
    [Space(3)]
    public string backToMenuSound;
    public string gameStartSound;

    //private
    private struct Player // struct com UI e dados necessarios de cada jogador
    {
        //Dados da UI
        public Image characterSelectedImage; //imagem do personagem
        public Text characterSelectedText; //texto/nome do personagem
        public Image characterSelectedPortrait; //"quadro" de selecao do personagem

        public int skillsQuantity; //quantidade de skills
        public Skill[] descriptionSelectedSkills; // referencia para dados de cada skill na UI

        //Dados de controle da UI
        public int currentCharacterSelection; //personagem "selecionado" pelo jogador
        public bool isCharacterSelected; //o personagem "selecionado" foi selecionado? 
    }
    private Player[] playersStruct; //jogadores na tela de selecao

    private struct Skill // struct com UI e dados necessarios de cada skill
    {
        public Image skillPanelImage; // imagem da skill
        public Text skillPanelText; // texto da skill
    }

    private Text selectSceneText; //texto da tela de selecao

    private Image[] scenarioImage; //referencia para as imagens de todos os cenarios

    private Image scenarioSelectedImage; //referencia para a imagem do cenario selecionado
    private Text scenarioSelectedtext; //referencia para o texto/nome do cenario selecionado

    private Image[] characterSelectPortraits; //referencia para os portraits de selecionado de todos os personagens
    private Image[] scenarioSelectPortraits; //referencia para os portraits de selecionado de todos os cenarios

    private int playerSelectingScenario; //jogador que esta selecionando o cenario
    private int currentScenarioSelection; //cenario "selecionado" pelo jogador
    private bool isScenarioSelected; //o cenario "selecionado" foi selecionado? 
    private bool isAllCharactersSelected; //todos os jogadores selecionaram seus personagens?

    private System.Action actionFunction; //funcao que ira executar quando a animacao acabar

    private SoundsGeneral soundsGeneral; //script soundsManager do objeto soundsManager

    private bool[] isHorizontalInUse;

    //inicializar antes do start
    private void Awake()
    {
        soundsGeneral = GameObject.FindObjectOfType<SoundsGeneral>(); //inicializar valores

        playersStruct = new Player[numberOfPlayers]; //inicializar valores
        isAllCharactersSelected = false;
        actionFunction = null;

        //Dados da UI
        Transform selectScenePanel = gameObject.transform.Find("SelectScenePanel");
        Transform charactersPanel = selectScenePanel.Find("CharactersPanel");
        Transform charactersSelectPanel = selectScenePanel.Find("CharactersSelectPanel");
        Transform scenarioPanel = selectScenePanel.Find("ScenarioPanel");
        Transform scenarioSelectPanel = selectScenePanel.Find("ScenarioSelectPanel");
        Transform initalTextPanel = selectScenePanel.Find("InitialTextPanel");

        //Interface geral
        selectSceneText = initalTextPanel.Find("SelectSceneText").GetComponent<Text>(); //inicializar valores

        characterSelectPortraits = new Image[charactersSelectPanel.childCount]; //inicializar array
        for (int j = 0; j < charactersSelectPanel.childCount; j++) //para cada personagem inicializar valores necessarios
        {
            characterSelectPortraits[j] = charactersSelectPanel.Find("Character_" + (j + 1) + "/Portrait").GetComponent<Image>(); //encontrar personagem e inicializar valores

            characterSelectPortraits[j].enabled = false;
        }

        playerSelectingScenario = 0; //jogador 1 que esta selecionando o cenario
        currentScenarioSelection = 0; //cenario 1 "selecionado" pelo jogador
        isScenarioSelected = false; //o cenario "selecionado" ainda nao foi selecionado

        scenarioSelectedImage = scenarioPanel.gameObject.transform.Find("Scenario_Selected/Image").GetComponent<Image>(); //referencia para a imagem do cenario selecionado //inicializar valores
        scenarioSelectedtext = scenarioPanel.gameObject.transform.Find("Scenario_Selected/Text").GetComponent<Text>(); //referencia para o texto/nome do cenario selecionado

        scenarioSelectPortraits = new Image[scenarioSelectPanel.childCount]; //inicializar array
        scenarioImage = new Image[scenarioSelectPanel.childCount]; //inicializar array
        for (int j = 0; j < scenarioSelectPanel.childCount; j++) //para cada cenario inicializar valores necessarios
        {
            Transform scenarioArena = scenarioSelectPanel.Find("Scenario_Arena" + (j + 1)); //encontrar cenario e inicializar valores

            scenarioImage[j] = scenarioArena.Find("Image").GetComponent<Image>(); //inicializar valores
            scenarioSelectPortraits[j] = scenarioArena.Find("Portrait").GetComponent<Image>();

            scenarioSelectPortraits[j].enabled = false;
        }

        //Interface de cada jogador
        for (int i = 0; i < numberOfPlayers; i++) //para cada jogador //inicializar valores
        {
            //Dados do jogador
            playersStruct[i].currentCharacterSelection = i; //personagem "selecionado" pelo jogador
            playersStruct[i].isCharacterSelected = false; //o personagem "selecionado" ainda nao foi selecionado

            //Dados da UI
            Transform characterSelected = charactersPanel.Find("Character_Selected" + (i + 1));
            playersStruct[i].characterSelectedImage = characterSelected.Find("Image").GetComponent<Image>(); //imagem do personagem
            playersStruct[i].characterSelectedText = characterSelected.Find("Text").GetComponent<Text>(); //texto/nome do personagem
            playersStruct[i].characterSelectedPortrait = characterSelected.Find("Portrait").GetComponent<Image>(); //"quadro" de selecao do personagem

            playersStruct[i].characterSelectedImage.sprite = characterSO[playersStruct[i].currentCharacterSelection].playerSelectSceneSprite;
            playersStruct[i].characterSelectedText.text = characterSO[playersStruct[i].currentCharacterSelection].characterName;
            playersStruct[i].characterSelectedPortrait.color = playerSelectedColor[i]; //inicializar valores

            Transform descriptionSelectedPanel = charactersPanel.Find("Description_Selected" + (i + 1));
            playersStruct[i].skillsQuantity = descriptionSelectedPanel.childCount;
            playersStruct[i].descriptionSelectedSkills = new Skill[descriptionSelectedPanel.childCount]; //inicializar array da struct
            for (int j = 0; j < playersStruct[i].skillsQuantity; j++) //para cada skill inicializar valores necessarios
            {
                Transform playerSkill = descriptionSelectedPanel.Find("Skill" + (j + 1) + "Panel"); //encontrar skill e inicializar valores
                playersStruct[i].descriptionSelectedSkills[j].skillPanelImage = playerSkill.Find("Image").GetComponent<Image>();
                playersStruct[i].descriptionSelectedSkills[j].skillPanelText = playerSkill.Find("Text").GetComponent<Text>();

                playersStruct[i].descriptionSelectedSkills[j].skillPanelImage.sprite = characterSO[playersStruct[i].currentCharacterSelection].skillsSprites[j]; //ajustar imagem da skill
                playersStruct[i].descriptionSelectedSkills[j].skillPanelText.text = characterSO[playersStruct[i].currentCharacterSelection].skillsDescription[j]; //ajustar texto/descricao da skill
            }

            characterSelectPortraits[playersStruct[i].currentCharacterSelection].color = playerSelectedColor[i]; //inicializar valores
            characterSelectPortraits[playersStruct[i].currentCharacterSelection].enabled = true;

            if (i == playerSelectingScenario) //se for este jogador selecionando o cenario //inicializar valores
            {
                scenarioSelectPortraits[currentScenarioSelection].color = playerSelectedColor[playerSelectingScenario];
                scenarioSelectPortraits[currentScenarioSelection].enabled = false; //true;

                scenarioSelectedImage.sprite = scenarioImage[currentScenarioSelection].sprite;
                scenarioSelectedtext.text = scenarioName[currentScenarioSelection];
            }
        }
    }

    // Use this for initialization
    void Start()
    {
        isHorizontalInUse = new bool[numberOfPlayers];
        for (int i = 0; i < numberOfPlayers; i++)
        {
            isHorizontalInUse[i] = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < numberOfPlayers; i++)  //para cada jogador
        {
            float moveHorizontal = Input.GetAxisRaw("NavigationHorizontal" + (i + 1)); //pegar navegacao horizontal //(-1, 0, 1)

            if (Input.GetButtonDown("Back")) //se apertar o botao de voltar
            {
                BackToMainMenu(); //voltar para o menu principal
            }
            else if (/*Input.GetButtonDown("NavigationHorizontal" + (i + 1))*/!isHorizontalInUse[i] && moveHorizontal != 0) //se apertar o botao de navegar na horiontal
            {
                isHorizontalInUse[i] = true;
                ChooseNextSelection(i, (int)moveHorizontal); //escolher proxima selecao na tela
            }
            else if (Input.GetButtonDown("Select" + (i + 1))) //se apertar o botao de selecionar
            {
                if (playersStruct[i].isCharacterSelected == false) // se o jogador estiver clicando para selecionar o personagem e o personagem ainda nao estiver selecionado
                {
                    SelectCharacter(i); //marca jogador como tendo selecionado seu personagem
                    CheckAllPlayersSelectedCharacter(); //verifica se todos os players selecionaram seu personagem  e atualiza os dados se necessario
                }
                else if (isAllCharactersSelected == true && i == playerSelectingScenario && isScenarioSelected == false) // se o jogador responsavel por selecionar o cenario estiver clicando para selecionar o cenario e o cenario ainda nao estiver selecionado
                {
                    isScenarioSelected = true; //marcar cenario como selecionado
                    //ChangeToScenarioSelection(); //mudar selecao dos personagens para selecao dos cenarios

                    soundsGeneral.FindAndPlay(gameStartSound); //encontrar e tocar som
                    soundsGeneral.FindAndStop(menuMusicSound); //encontrar e parar som

                    actionFunction = StartGame;
                    blackBackgroundImage.SetTrigger("FadeIn");
                    //StartGame(); //comecar o jogo 
                }
                //else if (i == playerSelectingScenario) //se o jogador responsavel por selecionar o cenario estiver clicando para selecionar o cenario e o cenario ja estiver selecionado
                //{
                //    StartGame(); //comecar o jogo 
                //}
            }

            if (moveHorizontal == 0)
            {
                isHorizontalInUse[i] = false;
            }
        }
    }

    public void SelectCharacter(int playerNumber) //marca jogador como tendo selecionado seu personagem
    {
        soundsGeneral.FindAndPlay(selectSound); //encontrar e tocar som

        playersStruct[playerNumber].isCharacterSelected = true; //marcar personagem como selecionado

        playersStruct[playerNumber].characterSelectedPortrait.color = playerSelectedColor[playerNumber] + new Color(1 - playerSelectedColor[playerNumber].r, 1 - playerSelectedColor[playerNumber].g, 1 - playerSelectedColor[playerNumber].b, 0f) * selectedPortraitTintProportion; //setar cor do portrait do player par aum pouco mais clara (como selecionado)
    }

    public void CheckAllPlayersSelectedCharacter() //verifica se todos os players selecionaram seu personagem e atualiza os dados se necessario
    {
        bool isAllCharactersSelectedAux = true; //todos os jogadores selecionaram seus personagens?
        for (int i = 0; i < numberOfPlayers; i++)  //para cada jogador
        {
            if (playersStruct[i].isCharacterSelected == false) isAllCharactersSelectedAux = false; //verificar se algum jogador ainda nao selecionou seu personagem
        }
        if (isAllCharactersSelectedAux) //se todos os jogadores selecionaram seu personagem
        {
            ActualizeScenarioSelected(0);
            selectSceneText.text = "Select the Arena";
            isAllCharactersSelected = true;
        }
    }

    public void ChooseNextSelection(int playerNumber, int direction) //escolher proxima selecao na tela //(direction -> 1 direita, -1 esquerda, 0 nao fazer nada)
    {
        if (playersStruct[playerNumber].isCharacterSelected == false) //se o jogador ainda nao tiver escolhido o personagem
        {
            soundsGeneral.FindAndPlay(navSound); //encontrar e tocar som

            int newCharacterSelection = (playersStruct[playerNumber].currentCharacterSelection + direction + characterSO.Length) % characterSO.Length; //setar proxima posicao da selecao de personagem
            ActualizeCharacterSelected(playerNumber, newCharacterSelection);
        }
        else if (isAllCharactersSelected == true && playerNumber == playerSelectingScenario && isScenarioSelected == false)
        {
            soundsGeneral.FindAndPlay(navSound); //encontrar e tocar som

            int newScenarioSelection = (currentScenarioSelection + direction + scenarioName.Length) % scenarioName.Length; //setar proxima posicao da selecao do cenario
            ActualizeScenarioSelected(newScenarioSelection);
        }
    }

    public void ActualizeCharacterSelected(int playerNumber, int newCharacterSelection) //mudar selecao do personagem para proxima selecionada
    {
        Color newColor = playerSelectedColor[playerNumber];
        if (CheckAllPlayersColor(characterSelectPortraits[playersStruct[playerNumber].currentCharacterSelection].color)) //verifica se algum outro jogador ja "tem" essa cor //(se nao esta selecionado por mais de um jogador)
        {
            characterSelectPortraits[playersStruct[playerNumber].currentCharacterSelection].enabled = false; //desativer selecao antiga
        }
        else //se estiver selecionado por mais de um jogador
        {
            newColor = (characterSelectPortraits[playersStruct[playerNumber].currentCharacterSelection].color * 1.15f) - new Color(newColor.r, newColor.g, newColor.b, 0f); //pegar nova cor do "quadro" que nao esta mais selecionado por esse jogador
            characterSelectPortraits[playersStruct[playerNumber].currentCharacterSelection].color = newColor; //atualizar dados
        }

        playersStruct[playerNumber].characterSelectedImage.sprite = characterSO[newCharacterSelection].playerSelectSceneSprite; //atualizar dados
        playersStruct[playerNumber].characterSelectedText.text = characterSO[newCharacterSelection].characterName;

        for (int j = 0; j < playersStruct[playerNumber].skillsQuantity; j++) //para cada skill atualizar dados necessarios
        {
            playersStruct[playerNumber].descriptionSelectedSkills[j].skillPanelImage.sprite = characterSO[newCharacterSelection].skillsSprites[j]; //ajustar imagem da skill
            playersStruct[playerNumber].descriptionSelectedSkills[j].skillPanelText.text = characterSO[newCharacterSelection].skillsDescription[j]; //ajustar texto/descricao da skill
        }

        newColor = playerSelectedColor[playerNumber];
        if (characterSelectPortraits[newCharacterSelection].enabled == true) //se ja estiver selecionado
        {
            newColor = (characterSelectPortraits[newCharacterSelection].color + new Color(newColor.r, newColor.g, newColor.b, 0f)) / 1.15f; //combinar cores
        }

        characterSelectPortraits[newCharacterSelection].color = newColor; //atualizar dados
        characterSelectPortraits[newCharacterSelection].enabled = true; //ativar selecao nova

        playersStruct[playerNumber].currentCharacterSelection = newCharacterSelection; //atualizar selecao
    }

    public bool CheckAllPlayersColor(Color color) //verifica se algum player "tem" essa cor
    {
        bool isPlayerColor = false; //algum jogador "tem" essa cor?

        for (int i = 0; i < numberOfPlayers; i++)  //para cada jogador
        {
            if (playerSelectedColor[i] == color) isPlayerColor = true; //verificar se algum jogador "tem" essa cor
        }

        return isPlayerColor;
    }

    public void ActualizeScenarioSelected(int newScenarioSelection) //mudar selecao do cenario para proxima selecionada
    {
        scenarioSelectPortraits[currentScenarioSelection].enabled = false; //desativar selecao antiga

        scenarioSelectedImage.sprite = scenarioImage[newScenarioSelection].sprite; //atualizar dados
        scenarioSelectedtext.text = scenarioName[newScenarioSelection];

        scenarioSelectPortraits[newScenarioSelection].color = playerSelectedColor[playerSelectingScenario]; //atualizar dados
        scenarioSelectPortraits[newScenarioSelection].enabled = true; //ativar nova selecao

        currentScenarioSelection = newScenarioSelection; //atualizar selecao
    }

    //public void ChangeToScenarioSelection() //mudar selecao dos personagens para selecao dos cenarios
    //{

    //}

    public void StartGame() //comecar o jogo / chamar cena principal do jogo
    {
        Debug.Log("Starting Game!");

        GameDataGeneral gameDataGeneral = GameObject.FindObjectOfType<GameDataGeneral>(); //encontrar objeto com os dados do jogo na cena

        gameDataGeneral.playersNumber = numberOfPlayers; //inicializar dados
        gameDataGeneral.characterSelected = new int[numberOfPlayers];
        gameDataGeneral.playerColor = new Color[numberOfPlayers];
        for (int i = 0; i < numberOfPlayers; i++)
        {
            gameDataGeneral.characterSelected[i] = playersStruct[i].currentCharacterSelection;
            gameDataGeneral.playerColor[i] = playerSelectedColor[i];
        }

        gameDataGeneral.scenarioSelected = currentScenarioSelection;

        SceneManager.LoadScene(nextSceneName + (currentScenarioSelection + 1)); //carregar proxima cena (ArenaX, X = 1, 2, 3...)
    }

    public void BackToMainMenu() //voltar para o menu principal / chamar cena do menu do jogo
    {
        soundsGeneral.FindAndPlay(backToMenuSound); //encontrar e tocar som

        GameDataGeneral gameDataGeneral = GameObject.FindObjectOfType<GameDataGeneral>(); //encontrar objeto com os dados do jogo na cena

        //Destroy(gameDataGeneral.gameObject); //destruir objeto com os dados do jogo

        SceneManager.LoadScene(previousSceneName); //carregar cena anterior (MainMenu)
    }

    public void CallActionFunction() //chama funcao actionFunction //(para usar pelas animacoes no caso)
    {
        if (actionFunction != null)
        {
            actionFunction();
        }
    }
}
