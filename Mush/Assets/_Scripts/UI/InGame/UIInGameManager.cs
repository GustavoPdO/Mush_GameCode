using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIInGameManager : MonoBehaviour
{ //(Obs: manter os paineis de status dos players desativados e as vidas tambem, etc. quando for "testar"/"buildar" o jogo (ativados e geradas por script dependendo da quantidade necessaria))
  //(Obs2: eh importante ressaltar que a referencia dos jogadores terao que ser passadas na inicializacao da cena quando for feito o menu/cena de selecao (+ verificar se algum outro objeto tambem nao depende de referenciacao ao inicializar a cena, como os delimitadores de cenario para a camera se necessario, etc.)

    //public
    public GameObject[] players; //referencia aos jogadores na tela

    //[Space(5)]
    [Header("Health Bars")]
    public int healthQtdPerDelay; //quantia de life (barra normal) que altera por delay
    public float healthAnimatingDelay; //atraso (/delay) /tempo em segundos que a animacao da barra leva para diminuir cada 1 de vida //(0 -> frame a frame)
    public int healthBackgroundQtdPerDelay; //quantia de life (barra "background") que altera por delay
    public float healthAnimatingBackgroundInitialDelay; //atraso (/delay) /tempo em segundos inicial que a animacao da barra "background" leva para diminuir cada 1 de vida //(0 -> frame a frame) //aplicar em funcao depois

    public GameObject healthBarPrefab; //prefab de uma barra de vida
    public Color[] healthBarSlidersColors; //cor dos sliders de vida (na ordem do primeiro (barra mais em baixo) para o ultimo (barra mais em cima)) //(do mesmo tamanho da quantidade de vidas de preferencia)
    [Range(0.01f, 1)]
    public float healthBackgroundShaderProportion; //proporcao da cor original que gera a cor shaded dela para sua barra "background"
    [Range(0.01f, 1)]
    public float healthBackgroundTintProportion; //proporcao da cor original que gera a cor tint dela para sua barra normal enquanto leva dano

    public float blinkColorTime; //tempo que uma barra de vida leva piscando //(0 -> frame atual)

    //[Space(5)]
    [Header("Lives")]
    public GameObject lifePrefab; //prefab de uma vida
    public Color enabledLifeColor; //cor de uma vida ativada/com a vida
    public Color disabledLifeColor; //cor de uma vida desativada/sem a vida

    [Header("Timer")]
    public float fullTime; //tempo total de partida //(em segundos)
    [Range(0.01f, 10)]
    public float timerSpeedMultiplier; //multilpicador de velocidade do timer (1 -> velocidade normal, < 1 -> mais devagar, > 1 -> mais rapido)

    //[Header("Sounds")] //nomes dos sons que vai tocar em cada parte
    //public string menuMusicSound;
    //public string gameMusicSound;

    [HideInInspector]
    public bool isGameTimerEnd; //o jogo ja acabou? (com relacao ao tempo de partida)

    //private
    private struct Player // struct com UI e dados necessarios de cada jogador
    {
        //Dados da UI
        public Transform playerStatsPanel; // transform do painel de status

        public Slider[] playerHealthBarSlider; // slider de vida
        public Slider[] playerBackgroundHealthBarSlider;
        public Image[] playerHealthBarFillImage; // imagem do slider de vida
        public Text playerHpNumberText; // numero da vida

        public Skill[] playerSkills; // skills de um jogador

        public Image[] playerLives; //vidas de um jogador

        //Dados do Objeto
        public CharacterScriptableObject characterSO; //referencia para os dados do personagem

        public HealthController healthController; //script healthController do jogador
        public SkillManager skillManager; //script skillManager do jogador

        public bool[] isAnimatingHealth; //se esta ou nao animando a barra de vida //(relacionadas a coroutine Health_Animating)
        public bool[] isAnimatingHealthBackground; //se esta ou nao animando a barra "background" de vida //(relacionadas a coroutine HealthBackground_Animating)
        public bool[] isBlinkingColor; //se esta ou nao piscando a barra de vida //(relacionadas a coroutine Health_BlinkingColor)
        public Coroutine[] healthBackGroundCoroutineInstances; //coroutines que serao/podem ser iniciadas //(para serem paradas se necessario) //(relacionadas as barras "background" de vida, etc.)
        public int[] actualHealthAux; //vetor para controlar vida atual do player //(para piscar a barra de vida quando levar dano) 
    }
    private Player[] playersStruct; // struct com UI e dados necessarios de cada jogador

    private struct Skill // struct com os componentes/objetos (necessarios) de uma skill
    {
        public Image iconBackgroundImage; //icone de background da skill (quando a skill estiver disponivel)
        public Image iconSkillImage; //icone/imagem da skill 
        public Image skillBlurEffectImage; //efeito de blur (quando a skill estiver em recarga)
        public Text skillCdTimeText; //tempo de recarga (quando a skill estiver em recarga)
        public Image iconSkillLockImage; //icone de skill bloqueada (quando a skill estiver bloqueada)

        public float skillCd; //para verificar quanto tempo de recarga tem a skill
        public float skillActualCd; //para verificar em quanto tempo de recarga esta a skill
        public bool isUsable; //para verificar se uma skill esta ou nao disponivel
    }

    private int numberOfPlayers; //numero de jogadores referenciados (/na cena)

    private Text timerText; //texto com o tempo da partida
    private float currentTime; //tempo atual da partida

    private GameEndManager gameEndManager; //script gameEndManager

    private SoundsGeneral soundsGeneral; //script soundsManager do objeto soundsManager

    //inicializar antes do start
    private void Awake()
    {
        soundsGeneral = GameObject.FindObjectOfType<SoundsGeneral>(); //inicializar valores

        numberOfPlayers = players.Length; //inicializar valores 

        playersStruct = new Player[numberOfPlayers]; //inicializar array

        for (int i = 0; i < numberOfPlayers; i++) //para cada jogador referenciado (/na cena) //referenciar objetos
        {
            //Dados do Objeto
            playersStruct[i].characterSO = players[i].GetComponent<PlayerController>().characterSO; //inicializar scriptableObject do jogador
            playersStruct[i].healthController = players[i].GetComponent<HealthController>(); //script healthController do jogador
            playersStruct[i].skillManager = players[i].GetComponent<SkillManager>(); //script skillManager do jogador

            //Dados da UI
            playersStruct[i].playerStatsPanel = gameObject.transform.Find("Player" + (i + 1) + "StatsPanel");

            playersStruct[i].playerStatsPanel.transform.Find("PlayerImage").GetComponent<Image>().sprite = playersStruct[i].characterSO.playerInGameSprite; //setar a image do jogador na interface dentro do jogo (/UIInGame)
            playersStruct[i].playerStatsPanel.gameObject.SetActive(true); // ativar painel de status do player //(para cada player na referenciado (/na cena))

            playersStruct[i].playerHpNumberText = playersStruct[i].playerStatsPanel.Find("HealthBarPanel/HpNumberText").GetComponent<Text>();

            Transform playerSkillsPanel = playersStruct[i].playerStatsPanel.Find("PlayerSkillsPanel");
            playersStruct[i].playerSkills = new Skill[playerSkillsPanel.childCount]; //inicializar array da struct
            for (int j = 0; j < playerSkillsPanel.childCount; j++) //para cada skill inicializar valores necessarios
            {
                Transform playerSkill = playerSkillsPanel.Find("Skill" + (j + 1)); //encontrar skill e inicializar valores
                playersStruct[i].playerSkills[j].iconBackgroundImage = playerSkill.Find("IconBackgroundImage").GetComponent<Image>();
                playersStruct[i].playerSkills[j].iconSkillImage = playerSkill.Find("IconCircleImage/IconSkillImage").GetComponent<Image>();
                playersStruct[i].playerSkills[j].skillBlurEffectImage = playerSkill.Find("IconCircleImage/SkillBlurEffectImage").GetComponent<Image>();
                playersStruct[i].playerSkills[j].skillCdTimeText = playerSkill.Find("IconCircleImage/SkillCdTimeText").GetComponent<Text>();
                playersStruct[i].playerSkills[j].iconSkillLockImage = playerSkill.Find("IconSkillLockImage").GetComponent<Image>();

                playersStruct[i].playerSkills[j].iconSkillImage.sprite = playersStruct[i].characterSO.skillsSprites[j]; //ajustar sprite da skill
            }
            //inicializar valores das skills de acordo com o nome dado no skillManager //(talvez padronizar nomes depois)
            playersStruct[i].playerSkills[0].skillCd = playersStruct[i].characterSO.skill1Cooldown;
            playersStruct[i].playerSkills[1].skillCd = playersStruct[i].characterSO.skill2Cooldown;
            playersStruct[i].playerSkills[2].skillCd = playersStruct[i].characterSO.mushCooldown;

            Transform playerLivesPanel = playersStruct[i].playerStatsPanel.Find("PlayerLivesPanel");
            Transform healthBarSlidersPanel = playersStruct[i].playerStatsPanel.Find("HealthBarPanel/HealthBarSlidersPanel");

            int livesCount = playersStruct[i].characterSO.maxNumberOfLives; //pegar numero de vidas de um jogador

            playersStruct[i].playerHealthBarSlider = new Slider[livesCount]; //inicializar array da struct
            playersStruct[i].playerBackgroundHealthBarSlider = new Slider[livesCount]; //inicializar array da struct
            playersStruct[i].playerHealthBarFillImage = new Image[livesCount];
            playersStruct[i].isAnimatingHealth = new bool[livesCount];
            playersStruct[i].isAnimatingHealthBackground = new bool[livesCount];
            playersStruct[i].isBlinkingColor = new bool[livesCount];
            playersStruct[i].healthBackGroundCoroutineInstances = new Coroutine[livesCount];
            playersStruct[i].actualHealthAux = new int[livesCount];

            playersStruct[i].playerLives = new Image[livesCount]; //inicializar array da struct

            for (int j = 0; j < livesCount; j++) //para cada vida, criar e inicializar valores necessarios
            {
                //criar barras de vida
                GameObject newHealthBar = Instantiate(healthBarPrefab, healthBarSlidersPanel); //criar e referenciar cada uma das barras de vida (completa) do jogador
                newHealthBar.name = healthBarPrefab.name + (j + 1); //renomear cada uma das barras de vidas

                playersStruct[i].playerHealthBarSlider[j] = newHealthBar.transform.Find("HealthBarSlider").GetComponent<Slider>(); //inicializar valores e referenciar barras criadas
                playersStruct[i].playerHealthBarSlider[j].maxValue = playersStruct[i].characterSO.maxHealth;
                playersStruct[i].playerHealthBarSlider[j].value = playersStruct[i].healthController.actualHealth[j]; //inicializar com valores atuais de vida
                playersStruct[i].playerHealthBarFillImage[j] = playersStruct[i].playerHealthBarSlider[j].gameObject.transform.Find("Fill Area/Fill").GetComponent<Image>(); //referenciar imagem que contem cor da barra de vida
                playersStruct[i].playerHealthBarFillImage[j].color = healthBarSlidersColors[j]; //setar cor da barra de vida

                playersStruct[i].playerBackgroundHealthBarSlider[j] = newHealthBar.transform.Find("BackgroundHealthBarSlider").GetComponent<Slider>();
                playersStruct[i].playerBackgroundHealthBarSlider[j].maxValue = playersStruct[i].characterSO.maxHealth;
                playersStruct[i].playerBackgroundHealthBarSlider[j].value = playersStruct[i].playerHealthBarSlider[j].value; //inicializar com valores atuais de vida
                playersStruct[i].playerBackgroundHealthBarSlider[j].gameObject.transform.Find("Fill Area/Fill").GetComponent<Image>().color = new Color(healthBarSlidersColors[j].r, healthBarSlidersColors[j].g, healthBarSlidersColors[j].b, 230f / healthBackgroundShaderProportion) * healthBackgroundShaderProportion; //setar cor da barra "backround" de vida com a proporcao de "escurecer" aplicada

                playersStruct[i].actualHealthAux[j] = playersStruct[i].healthController.actualHealth[j];

                //criar vidas
                GameObject newLife = Instantiate(lifePrefab, playerLivesPanel); //criar e referenciar cada uma das vidas do jogador
                newLife.name = lifePrefab.name + (j + 1); //renomear cada uma das vidas
                RectTransform newLifeRectTransform = newLife.GetComponent<RectTransform>(); //inicializar valores
                newLifeRectTransform.anchorMin = new Vector2(/*(float)j / livesCount*/0.175f * j, newLifeRectTransform.anchorMin.y);
                newLifeRectTransform.anchorMax = new Vector2(/*(float)(j + 1) / livesCount*/0.25f + 0.175f * j, newLifeRectTransform.anchorMax.y);
                newLifeRectTransform.offsetMin = Vector2.zero;
                newLifeRectTransform.offsetMax = Vector2.zero;

                playersStruct[i].playerLives[j] = newLife.GetComponent<Image>(); //referenciar nova vida criada
            }
        }

        //dados do timer
        timerText = gameObject.transform.Find("TimerPanel/TimerText").GetComponent<Text>(); //inicializar valores //pegar text do timer
    }

    // Use this for initialization
    void Start()
    {
        //dados do gameEndManager
        gameEndManager = gameObject.transform.Find("GameEndPanel").GetComponent<GameEndManager>(); //inicializar valores
        //gameEndManager.isNavigationEnabled = false; //desativar navegacao no painel de gameEnd
        //gameEndManager.gameObject.SetActive(false);

        //inicializar valores
        for (int i = 0; i < numberOfPlayers; i++) //para cada jogador setar os dados necessarios na interface (UI in game)
        {
            SetCurrentLife(i);
            SetCurrentNumberOfLives(i);
            SetCurrentSkillsState(i);
        }

        StartCoroutine(TimeCounter()); //iniciar temporizador
    }

    public IEnumerator TimeCounter() //iniciar/gerenciar temporizador
    {
        isGameTimerEnd = false; //o jogo ja acabou?
        Debug.Log("Game Timer Start!");

        currentTime = fullTime; //inicializar tempo atual com o tmepo total

        while (currentTime > 0 && !gameEndManager.isGameEnd) //enquanto o tempo nao acabar //e o jogo nao tiver acabado
        {
            timerText.text = "" + currentTime; //setar texto do tempo atual

            yield return new WaitForSeconds(1 / timerSpeedMultiplier); //esperar um tempo

            currentTime -= 1; //tirar "1 segundo" do contador
        }
        if (!gameEndManager.isGameEnd) //se o jogo nao tiver acabado por outra causa, zerar o tempo
            timerText.text = "0"; //setar texto do tempo atual

        isGameTimerEnd = true; //o jogo ja acabou?
        Debug.Log("Game Timer End!");

        //gameEndManager.gameObject.SetActive(true); //re ativar o painel de gameEnd //(nao esta funcionando com setActive)
        //gameEndManager.GameEnded();
        CheckGameEnd();
    }

    // Update is called once per frame
    void Update()
    {
        //if (gameEndManager.isGameEnd) return; //se o jogo ja acabou, retornar
        if (!gameEndManager.isGameEnd)
            CheckGameEnd(); //verifica se o jogo acabou e toma as acoes necessarias
        else
            StopAllPlayers();

        for (int i = 0; i < numberOfPlayers; i++) //para cada jogador setar os dados necessarios na interface (UI in game)
        {
            SetCurrentLife(i);
            SetCurrentNumberOfLives(i);
            SetCurrentSkillsState(i);
        }
    }

    //(atualizar para versoes "melhores" depois) //(+ barras, e fazer as skills, +scripts, etc.)
    public void SetCurrentLife(int playerNumber) //seta a vida atual do jogador
    {
        int actualTotalLife = 0;
        for (int i = 0; i < playersStruct[playerNumber].characterSO.maxNumberOfLives; i++) //para cada vida / em cada barra de vida, verificar quantias e dados necessarios da vida atual
        {
            //playersStruct[playerNumber].playerHealthBarSlider.value = playersStruct[playerNumber].healthController.actualHealth; //setar valor no slider de vida
            if (!playersStruct[playerNumber].isAnimatingHealth[i] && playersStruct[playerNumber].playerHealthBarSlider[i].value != playersStruct[playerNumber].healthController.actualHealth[i])
            {
                StartCoroutine(Health_Animating(playerNumber, i));
            }
            else if (!playersStruct[playerNumber].isAnimatingHealthBackground[i] && !playersStruct[playerNumber].isAnimatingHealth[i] && playersStruct[playerNumber].playerBackgroundHealthBarSlider[i].value != playersStruct[playerNumber].healthController.actualHealth[i]) //se nao estiver animando backround
            {
                playersStruct[playerNumber].healthBackGroundCoroutineInstances[i] = StartCoroutine(HealthBackground_Animating(playerNumber, i));
            }
            else if (playersStruct[playerNumber].isAnimatingHealthBackground[i] && playersStruct[playerNumber].isAnimatingHealth[i]) //se estiver executando ambos ao mesmo tempo, cancelar
            {
                StopCoroutine(playersStruct[playerNumber].healthBackGroundCoroutineInstances[i]); //parar co rotina de animar barra "background" de vida
                playersStruct[playerNumber].isAnimatingHealthBackground[i] = false; //resetar valor da variavel que indica se a coroutine esta ou nao rodando
            }

            if (playersStruct[playerNumber].actualHealthAux[i] != playersStruct[playerNumber].healthController.actualHealth[i]) //se levar dano, piscar cor da barra de vida
            {
                playersStruct[playerNumber].actualHealthAux[i] = playersStruct[playerNumber].healthController.actualHealth[i]; //atualizar vida atual
                                                                                                                               //if (!isBlinkingColor[lifeBarNumber]) //ativar if caso queira piscar apenas se ja nao estiver piscando
                StartCoroutine(Health_BlinkColor(playerNumber, i)); //piscar cor da barra de vida quando levar dano
            }

            actualTotalLife += (int)playersStruct[playerNumber].playerHealthBarSlider[i].value; //somar valores de vida das barras para obter o valor de vida (total) atual
        }

        playersStruct[playerNumber].playerHpNumberText.text = actualTotalLife + "/" + (playersStruct[playerNumber].characterSO.maxHealth * playersStruct[playerNumber].characterSO.maxNumberOfLives); //setar valor no texto de vida (referente a vida total) //(para testes, etc.)
    }

    public IEnumerator Health_BlinkColor(int playerNumber, int lifeBarNumber) //piscar cor da barra de vida (barra normal)
    {
        playersStruct[playerNumber].isBlinkingColor[lifeBarNumber] = true;

        playersStruct[playerNumber].playerHealthBarFillImage[lifeBarNumber].color = healthBarSlidersColors[lifeBarNumber] + new Color(1 - healthBarSlidersColors[lifeBarNumber].r, 1 - healthBarSlidersColors[lifeBarNumber].g, 1 - healthBarSlidersColors[lifeBarNumber].b, 0f) * healthBackgroundTintProportion; //setar cor da barra de vida

        yield return new WaitForSeconds(blinkColorTime);

        playersStruct[playerNumber].playerHealthBarFillImage[lifeBarNumber].color = healthBarSlidersColors[lifeBarNumber]; //re setar cor da barra de vida

        playersStruct[playerNumber].isBlinkingColor[lifeBarNumber] = false;
    }

    public IEnumerator Health_Animating(int playerNumber, int lifeBarNumber) //anima barra de vida "lifeBarNumber" (barra normal) do jogador "playerNumber"
    {
        playersStruct[playerNumber].isAnimatingHealth[lifeBarNumber] = true;

        //if (!isBlinkingColor[lifeBarNumber]) StartCoroutine(Health_BlinkColor(playerNumber, lifeBarNumber)); //piscar cor da barra de vida quando comecar a levar dano
        float initialHealthBarValue = playersStruct[playerNumber].playerHealthBarSlider[lifeBarNumber].value; //valor inicial da vida a ser usada como parametro de maximo

        //if/else para testes (quando regenera a vida, etc.) //ajustar depois
        if (playersStruct[playerNumber].playerHealthBarSlider[lifeBarNumber].value > playersStruct[playerNumber].healthController.actualHealth[lifeBarNumber])
        {
            while (playersStruct[playerNumber].playerHealthBarSlider[lifeBarNumber].value > playersStruct[playerNumber].healthController.actualHealth[lifeBarNumber]) //se a vida atual na barra for maior do que a vida atual do player 
            {
                yield return new WaitForSeconds(healthAnimatingDelay); //esperar um tempo e reduzir "lentamente" a barra de vida
                playersStruct[playerNumber].playerHealthBarSlider[lifeBarNumber].value -= healthQtdPerDelay;
            }
        }
        else if (playersStruct[playerNumber].playerHealthBarSlider[lifeBarNumber].value < playersStruct[playerNumber].healthController.actualHealth[lifeBarNumber])
        {
            initialHealthBarValue = playersStruct[playerNumber].healthController.actualHealth[lifeBarNumber]; //valor inicial da vida a ser usada como parametro de maximo

            while (playersStruct[playerNumber].playerHealthBarSlider[lifeBarNumber].value < playersStruct[playerNumber].healthController.actualHealth[lifeBarNumber]) //se a vida atual na barra for maior do que a vida atual do player 
            {
                yield return new WaitForSeconds(healthAnimatingDelay); //esperar um tempo e reduzir "lentamente" a barra de vida
                playersStruct[playerNumber].playerHealthBarSlider[lifeBarNumber].value += healthQtdPerDelay;
            }
        }
        playersStruct[playerNumber].playerHealthBarSlider[lifeBarNumber].value = playersStruct[playerNumber].healthController.actualHealth[lifeBarNumber];

        playersStruct[playerNumber].isAnimatingHealth[lifeBarNumber] = false;
    }

    public IEnumerator HealthBackground_Animating(int playerNumber, int lifeBarNumber) //anima barra de vida "lifeBarNumber" (barra "background") do jogador "playerNumber"
    {
        playersStruct[playerNumber].isAnimatingHealthBackground[lifeBarNumber] = true;

        //if/else para testes (quando regenera a vida, etc.) //ajustar depois
        if (playersStruct[playerNumber].playerBackgroundHealthBarSlider[lifeBarNumber].value > playersStruct[playerNumber].playerHealthBarSlider[lifeBarNumber].value)
        {
            while (playersStruct[playerNumber].playerBackgroundHealthBarSlider[lifeBarNumber].value > playersStruct[playerNumber].playerHealthBarSlider[lifeBarNumber].value) //se a vida atual na barra "background" for maior do que a vida atual da barra de vida
            {
                yield return new WaitForSeconds(healthAnimatingDelay); //esperar um tempo e reduzir "lentamente" a barra "background" de vida
                playersStruct[playerNumber].playerBackgroundHealthBarSlider[lifeBarNumber].value -= 1;
            }
        }
        else if (playersStruct[playerNumber].playerBackgroundHealthBarSlider[lifeBarNumber].value < playersStruct[playerNumber].playerHealthBarSlider[lifeBarNumber].value)
        {
            playersStruct[playerNumber].playerBackgroundHealthBarSlider[lifeBarNumber].value = playersStruct[playerNumber].playerHealthBarSlider[lifeBarNumber].value; //se a vida atual na barra "background" for maior do que a vida atual da barra de vida, apenas setar mesmo valor da barra (barra normal) atual
        }
        playersStruct[playerNumber].playerBackgroundHealthBarSlider[lifeBarNumber].value = playersStruct[playerNumber].playerHealthBarSlider[lifeBarNumber].value;

        playersStruct[playerNumber].isAnimatingHealthBackground[lifeBarNumber] = false;
    }

    public void SetCurrentNumberOfLives(int playerNumber) //seta a quantidade de vidas atual do jogador
    {
        int actualNumberOfLives = playersStruct[playerNumber].healthController.actualNumberOfLives; //encontrar quantidade de vidas atual
        for (int i = 0; i < playersStruct[playerNumber].characterSO.maxNumberOfLives; i++) //setar vidas como ativadas ou desativadas (baseando-se na cor)
        {
            if (i < actualNumberOfLives) //se estiver ativa, ativar
            {
                playersStruct[playerNumber].playerLives[i].color = enabledLifeColor;
            }
            else //senao, desativar
            {
                playersStruct[playerNumber].playerLives[i].color = disabledLifeColor;
            }
        }
    }

    //(falta fazer cds, etc. das skills)
    public void SetCurrentSkillsState(int playerNumber) //seta o estado atual das skills do jogador (bloqueada, em recarga, ativa, etc.)
    {
        //inicializar valores das skills de acordo com o nome dado no skillManager //(talvez padronizar nomes depois)
        playersStruct[playerNumber].playerSkills[0].isUsable = playersStruct[playerNumber].skillManager.skill1Usable;
        playersStruct[playerNumber].playerSkills[1].isUsable = playersStruct[playerNumber].skillManager.skill2Usable;
        playersStruct[playerNumber].playerSkills[2].isUsable = playersStruct[playerNumber].skillManager.mushUsable;

        playersStruct[playerNumber].playerSkills[0].skillActualCd = playersStruct[playerNumber].skillManager.skill1CD;
        playersStruct[playerNumber].playerSkills[1].skillActualCd = playersStruct[playerNumber].skillManager.skill2CD;
        playersStruct[playerNumber].playerSkills[2].skillActualCd = playersStruct[playerNumber].skillManager.mushCD;

        int actualNumberOfLives = playersStruct[playerNumber].healthController.actualNumberOfLives; //encontrar quantidade de vidas atual
        for (int i = 0; i < playersStruct[playerNumber].playerSkills.Length; i++) //para cada skill do player
        {
            if (playersStruct[playerNumber].characterSO.skillsLifeUnlock[i] < actualNumberOfLives) //verificar se a skill esta ou nao desbloqueada
            {
                playersStruct[playerNumber].playerSkills[i].iconSkillLockImage.enabled = true; //se estiver bloqueada, mostrar icone de bloqueada
                playersStruct[playerNumber].playerSkills[i].iconBackgroundImage.enabled = false; //esconder icone/imagem de fundo da skill

                playersStruct[playerNumber].playerSkills[i].skillBlurEffectImage.fillAmount = 1f; //mostrar "efeito de blur" completo
                                                                                                  //playersStruct[playerNumber].playerSkills[i].skillBlurEffectImage.enabled = true; //mostrar "efeito de blur"

                playersStruct[playerNumber].playerSkills[i].skillCdTimeText.text = ""; //esconder texto do tempo de recarga da skill
            }
            else //se nao estiver bloqueada
            {
                playersStruct[playerNumber].playerSkills[i].iconSkillLockImage.enabled = false; //esconder icone de bloqueada
                playersStruct[playerNumber].playerSkills[i].iconBackgroundImage.enabled = true; //mostrar icone/imagem de fundo da skill

                if (playersStruct[playerNumber].playerSkills[i].isUsable) //se a skill estiver disponivel / puder ser usada
                {
                    playersStruct[playerNumber].playerSkills[i].skillBlurEffectImage.fillAmount = 0f; //esconder "efeito de blur"
                    playersStruct[playerNumber].playerSkills[i].skillCdTimeText.text = ""; //esconder texto do tempo de recarga da skill
                }
                else //se a skill nao estiver disponivel / estiver em recarga
                {
                    int currentCdTime = Mathf.CeilToInt(playersStruct[playerNumber].playerSkills[i].skillCd - playersStruct[playerNumber].playerSkills[i].skillActualCd); //relacao do tempo de recarga atual com o tempo de recarga total
                    float currentNormalizedCdTime = (playersStruct[playerNumber].playerSkills[i].skillCd - playersStruct[playerNumber].playerSkills[i].skillActualCd) / playersStruct[playerNumber].playerSkills[i].skillCd; //relacao do tempo de recarga atual com o tempo de recarga total (normalizado "de 0 a 1")

                    playersStruct[playerNumber].playerSkills[i].skillBlurEffectImage.fillAmount = currentNormalizedCdTime; //ajustar "efeito de blur" (de acordo com o tempo de recarga atual)
                    playersStruct[playerNumber].playerSkills[i].skillCdTimeText.text = (currentCdTime != 0) ? "" + currentCdTime : ""; //mostrar texto do tempo de recarga da skill com o tempo atual
                }
            }
        }
    }

    //------Checagens de fim de jogo e acoes necessarias------ 
    //(ver de arrumar depois essas coisas relacionadas ao final de um jogo/partida e de separar os Managers de interface em partes menores para facilitar)

    public void CheckGameEnd() //verifica se o jogo acabou e toma as acoes necessarias
    {
        string winnerText = "";

        if (isGameTimerEnd) //se o tempo acabou
        {
            TimeEndCheckPlayersLife(); //verifica se o tempo acabou, tentando escolher quem "ganhou"
            winnerText += "Time Over\n";
        }

        int[] numberOfPlayersAlive = CheckPlayersLife(); //verifica se a vida dos jogadores ja acabou (//0 -> numberOfPlayersAlive, 1, 2, ... -> playersAliveNumber)

        if (numberOfPlayersAlive[0] <= 1 || isGameTimerEnd) //se tiver sobrado soh um jogador //ou se o tempo tiver acabado
        {
            winnerText += "Winner - Player ";
            for (int i = 0; i < numberOfPlayersAlive[0]; i++) //encontrar e preeencher texto de jogadores que ganharam
            {
                winnerText += (numberOfPlayersAlive[i + 1] + 1);
                if (i < numberOfPlayersAlive[0] - 1) winnerText += ", ";
            }
            gameEndManager.GameEnded(winnerText);
        }
    }

    public void TimeEndCheckPlayersLife() //verifica se o tempo acabou, tentando escolher quem "ganhou"
    {
        int maxPlayersLife = 0; //numero de jogadores "vivos"
        for (int i = 0; i < numberOfPlayers; i++) //para cada jogador na cena
        {
            if (playersStruct[i].healthController.actualNumberOfLives > 0) //se esse jogador ainda tiver mais vidas
            {
                int currentLife = 0; //encontrar maior vida entre os jogadores
                for (int j = 0; j < playersStruct[i].healthController.maxNumberOfLives; j++)
                {
                    currentLife += playersStruct[i].healthController.actualHealth[j];
                }
                if (currentLife > maxPlayersLife) maxPlayersLife = currentLife;
            }
        }

        for (int i = 0; i < numberOfPlayers; i++) //para cada jogador na cena
        {
            if (playersStruct[i].healthController.actualNumberOfLives > 0) //se esse jogador ainda tiver mais vidas
            {
                int currentLife = 0; //encontrar vida total do jogador
                for (int j = 0; j < playersStruct[i].healthController.maxNumberOfLives; j++)
                {
                    currentLife += playersStruct[i].healthController.actualHealth[j];
                }

                if (currentLife < maxPlayersLife) //se esse jogador tiver menos vida que o jogador com mais vida
                {
                    for (int j = 0; j < playersStruct[i].healthController.actualNumberOfLives; j++) //remover vidas do jogador
                    {
                        playersStruct[i].healthController.actualHealth[j] = 0;
                    }
                    playersStruct[i].healthController.actualNumberOfLives = 1;
                    playersStruct[i].healthController.ReceiveDamage(1);
                }
            }
        }
    }

    public int[] CheckPlayersLife() //verifica se a vida dos jogadores ja acabou
    {
        int numberOfPlayersAlive = 0; //numero de jogadores "vivos"
        for (int i = 0; i < numberOfPlayers; i++) //para cada jogador na cena
        {
            if (playersStruct[i].healthController.actualNumberOfLives > 0) //se esse jogador ainda tiver mais vidas
            {
                numberOfPlayersAlive += 1;
            }
        }

        int[] nOfPlayersAlive = new int[numberOfPlayersAlive + 1]; //encontrar jogadores que ganharam
        nOfPlayersAlive[0] = numberOfPlayersAlive;
        int j = 1;
        for (int i = 0; i < numberOfPlayers; i++) //para cada jogador na cena
        {
            if (playersStruct[i].healthController.actualNumberOfLives > 0) //se esse jogador ainda tiver mais vidas
            {
                nOfPlayersAlive[j++] = i;
            }
        }

        return nOfPlayersAlive;
    }

    public void StopAllPlayers() //para todos os jogadores da tela //(cuidado, soh usar no final da cena para parar tudo)
    {
        for (int i = 0; i < numberOfPlayers; i++) //para cada jogador na cena
        {
            if (playersStruct[i].healthController.gameObject.GetComponent<SkillManager>().enabled == false) return; //criterio para nao rodar varias vezes //(arrumar depois)

            playersStruct[i].healthController.gameObject.GetComponent<PlayerController>().canMove = false;
            playersStruct[i].healthController.gameObject.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezeRotation;
            playersStruct[i].healthController.gameObject.GetComponent<SkillManager>().enabled = false;
            //playersStruct[i].healthController.gameObject.GetComponent<Collider2D>().enabled = false;

            playersStruct[i].healthController.gameObject.GetComponentInChildren<AttackController>().canAttack = false;

            playersStruct[i].healthController.gameObject.transform.Find("Skills").gameObject.SetActive(false);
        }
    }
}
