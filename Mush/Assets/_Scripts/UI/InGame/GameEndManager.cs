using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameEndManager : MonoBehaviour
{ //(OBS: algumas checagens e acoes estao no UIInGameManager pois ele ja tem varias das referencias necessarias (depois arrumar se necessario))

    //public
    public string selectSceneName; //nome da proxima cena que carrega depois dessa cena //(nesse caso SelectScene)
    public string mainMenuName; //nome da proxima cena que carrega depois dessa cena //(nesse caso MainMenu)

    [Header("Buttons")]
    public int restartButtonNumber; //numero do botao de re iniciar //(contagem da ordem comecando do 0)
    public int selectSceneButtonNumber; //numero do botao de ir para a tela de selecao //(contagem da ordem comecando do 0)
    public int mainMenuButtonNumber; //numero do botao de ir para o menu principal //(contagem da ordem comecando do 0)
    public int exitButtonNumber; //numero do botao de sair //(contagem da ordem comecando do 0)

    [Space(5)]
    public Text[] buttonText; //referencia aos botoes da cena //(na ordem)
    public int buttonHoverOffFontSize; //tamanho dos botoes quando nao "selecionados"
    public int buttonHoverOnFontSize; //tamanho dos botoes quando "selecionados"
    public Color buttonHoverOffColor; //cor dos botoes quando nao "selecionados"
    public Color buttonHoverOnColor; //cor dos botoes quando "selecionados"

    [Header("FadeInOut")]
    public Animator whiteBackgroundImage; //objeto/imagem/animator que contem a imagem de fundo branca 
    public Animator blackBackgroundImage; //objeto/imagem/animator que contem a imagem de fundo preta

    [Space(5)]
    public Text gameTimerText; //referencia ao texto de quando acaba o jogo
    public int gameEndWaitTime; //tempo que espera para liberar o painel

    public Text gameEndText; //referencia ao texto de quando acaba o jogo
    public Image gameEnd2Panel; //objeto do painel com os textos/tempo de quando acaba o jogo

    [Header("Sounds")] //nomes dos sons que vai tocar em cada parte
    public string menuMusicSound;
    public string gameMusicSound;
    [Space(3)]
    public string navSound;
    public string selectSound;
    [Space(3)]
    public string gameEndSound;

    [HideInInspector]
    public bool isNavigationEnabled; //se a navegacao nesse menu esta habilitada
    [HideInInspector]
    public bool isGameEnd; //se o jogo ja terminou

    //private
    private RectTransform rectTransform; // componente rectTransform 
    private RectTransformStruct originalRectTransform; // struct rectTransform (para guardar os dados originais do rectTransform) //(para poder habilitar/desabilitar soh escondendo o painel) 

    private struct RectTransformStruct //struct para armazenar dados de um recttransform
    {
        public Vector2 anchorMin;
        public Vector2 anchorMax;
        public Vector2 offsetMax;
        public Vector2 offsetMin;
    }

    private int currentSelectedButton; //item atual "selecionado"

    private string thisSceneName; //nome dessa cena

    private bool isStartExitSelected; //se o "botao" de start ja foi selecionado

    private System.Action actionFunction; //funcao que ira executar quando a animacao acabar

    private SoundsGeneral soundsGeneral; //script soundsManager do objeto soundsManager

    private bool isVerticalInUse;

    //inicializar antes do start
    private void Awake()
    {
        soundsGeneral = GameObject.FindObjectOfType<SoundsGeneral>(); //inicializar valores

        rectTransform = gameObject.GetComponent<RectTransform>(); //inicializar valores

        originalRectTransform.anchorMin = rectTransform.anchorMin;
        originalRectTransform.anchorMax = rectTransform.anchorMax;
        originalRectTransform.offsetMax = rectTransform.offsetMax;
        originalRectTransform.offsetMin = rectTransform.offsetMin;

        gameEnd2Panel.enabled = false;

        gameTimerText.text = "";
        gameEndText.text = "";

        isVerticalInUse = false;
    }

    // Use this for initialization
    void Start()
    {
        soundsGeneral.FindAndPlay(gameMusicSound); //encontrar e tocar som

        currentSelectedButton = 0; //inicializar valores
        thisSceneName = SceneManager.GetActiveScene().name; //pegar nome da cena atual 
        //buttonText[currentSelectedButton].fontSize = buttonHoverOnFontSize; //inicializar dados
        //buttonText[currentSelectedButton].color = buttonHoverOnColor;

        isStartExitSelected = false; //inicializar valores
        isNavigationEnabled = false;
        isGameEnd = false;

        actionFunction = null; //inicializar valores

        //gameObject.SetActive(false); //desativar o painel de gameEnd //(nao esta funcionando com setActive)
        SetActiveFalse();
    }

    // Update is called once per frame
    void Update()
    {
        if (isStartExitSelected || !isNavigationEnabled) return; //se estiver comecando/saindo do jogo

        float moveVertical = Input.GetAxisRaw("NavigationVertical"); //pegar navegacao vertical //(-1, 0, 1)

        if (/*Input.GetButtonDown("NavigationVertical")*/!isVerticalInUse && moveVertical != 0) //se apertar o botao de navegar na vertical
        {
            isVerticalInUse = true;
            ChooseNextSelectionVertical((int)moveVertical); //escolher proxima selecao de botao na tela
        }
        else if (Input.GetButtonDown("Select1")) //se apertar o botao de selecionar
        {
            SelectCurrentButton(); //verifica em qual painel/boato se encontra e executa acao de "selecionar" no botao/painel atual 
        }

        if (moveVertical == 0)
        {
            isVerticalInUse = false;
        }
    }

    public void ChooseNextSelectionVertical(int direction) //escolher proxima selecao na tela (botoes) //(direction -> 1 cima, -1 baixo, 0 nao fazer nada)
    {
        soundsGeneral.FindAndPlay(navSound); //encontrar e tocar som

        //if (currentOnPanel == 0) //se o jogador estiver no painel inicial
        //{
        int newButtonSelection = (currentSelectedButton + direction + buttonText.Length) % buttonText.Length; //setar proxima posicao da selecao de botoes
        ActualizeButtonSelected(newButtonSelection);
        //}
    }

    public void ActualizeButtonSelected(int newButtonSelection) //mudar selecao do botao para proximo selecionado
    {
        buttonText[currentSelectedButton].fontSize = buttonHoverOffFontSize; //atualizar dados do botao selecionado antigo
        buttonText[currentSelectedButton].color = buttonHoverOffColor;

        buttonText[newButtonSelection].fontSize = buttonHoverOnFontSize; //atualizar dados do botao selecionado novo
        buttonText[newButtonSelection].color = buttonHoverOnColor;

        currentSelectedButton = newButtonSelection; //atualizar dados
    }

    public void SelectCurrentButton() //verifica em qual painel/boato se encontra e executa acao de "selecionar" no botao/painel atual 
    {
        soundsGeneral.FindAndPlay(selectSound); //encontrar e tocar som
        soundsGeneral.FindAndStop(gameMusicSound); //encontrar e tocar som

        isStartExitSelected = true;
        if (currentSelectedButton == restartButtonNumber) //se for o botao de re iniciar
        {
            actionFunction = RestartGame; //setar funcao para chamar no final da animacao
            blackBackgroundImage.SetTrigger("FadeIn"); //triggerar animacao para realizar a transicao de cena
        }
        else if (currentSelectedButton == selectSceneButtonNumber) //se for o botao de ir para a tela de selecao
        {
            soundsGeneral.FindAndPlay(menuMusicSound); //encontrar e tocar som

            actionFunction = GoToSelectScene; //setar funcao para chamar no final da animacao
            blackBackgroundImage.SetTrigger("FadeIn"); //triggerar animacao para realizar a transicao de cena
        }
        else if (currentSelectedButton == mainMenuButtonNumber) //se for o botao de ir para o menu principal
        {
            soundsGeneral.FindAndPlay(menuMusicSound); //encontrar e tocar som

            actionFunction = GoToMainMenu; //setar funcao para chamar no final da animacao
            blackBackgroundImage.SetTrigger("FadeIn"); //triggerar animacao para realizar a transicao de cena
        }
        else if (currentSelectedButton == exitButtonNumber) //se for o botao de sair
        {
            actionFunction = ExitGame; //setar funcao para chamar no final da animacao
            blackBackgroundImage.SetTrigger("FadeIn"); //triggerar animacao para realizar a transicao de cena
        }
    }

    public void GameEnded(string winnerString) //"terminar" o jogo
    {
        if (isGameEnd) return;

        isGameEnd = true;
        SetActiveTrue();

        soundsGeneral.FindAndStop(gameMusicSound); //encontrar e tocar som
        soundsGeneral.FindAndPlay(gameEndSound); //encontrar e tocar som

        //isNavigationEnabled = true;
        gameEndText.text = winnerString;
        StartCoroutine(WaitFewSeconds_Enable()); //espera alguns segundos e reativa a navegacao no painel
    }

    public IEnumerator WaitFewSeconds_Enable() //espera alguns segundos e reativa a navegacao no painel
    {
        int endWaitTime = 0;
        while (gameEndWaitTime > endWaitTime) //esperar um tempo antes de liberar o painel //(enquanto atualiza o texto com o tempo que precisa ser esperado) 
        {
            gameTimerText.text = "." + (gameEndWaitTime - endWaitTime) + ".";

            yield return new WaitForSeconds(1f);

            endWaitTime += 1;
        }
        gameTimerText.text = ".0.";

        buttonText[currentSelectedButton].fontSize = buttonHoverOnFontSize; //atualizar/ajustar dados
        buttonText[currentSelectedButton].color = buttonHoverOnColor;

        isNavigationEnabled = true;
    }

    public void SetActiveTrue() //"ativa" o painel
    {
        //rectTransform = originalRectTransform;
        rectTransform.anchorMin = originalRectTransform.anchorMin;
        rectTransform.anchorMax = originalRectTransform.anchorMax;
        rectTransform.offsetMax = originalRectTransform.offsetMax;
        rectTransform.offsetMin = originalRectTransform.offsetMin;

        gameEnd2Panel.enabled = true;
    }

    public void SetActiveFalse() //"desativa" o painel
    {
        rectTransform.anchorMin = Vector2.zero;
        rectTransform.anchorMax = Vector2.zero;
        rectTransform.offsetMax = new Vector2(-1, 0);
        rectTransform.offsetMin = Vector2.zero;
    }

    public void RestartGame() //re comecar o jogo
    {
        Debug.Log("Restarting game!");

        SceneManager.LoadScene(thisSceneName); //carregar proxima cena (cena atual)
    }

    public void GoToSelectScene() //chamar cena de selecao de personagens
    {
        Debug.Log("Starting Select Scene!");

        SceneManager.LoadScene(selectSceneName); //carregar proxima cena (SelectScene)
    }

    public void GoToMainMenu() //chamar o menu principal
    {
        Debug.Log("Starting Main Menu!");

        SceneManager.LoadScene(mainMenuName); //carregar proxima cena (MainMenu)
    }

    public void ExitGame() //sair/fechar jogo
    {
        Debug.Log("Exiting Game");

#if UNITY_EDITOR //se estiver no editor da unity
        UnityEditor.EditorApplication.isPlaying = false;
#else //se nao (/se estiver na aplicacao)
        Application.Quit(); //sair do jogo
#endif
    }

    public void CallActionFunction() //chama funcao actionFunction //(para usar pelas animacoes no caso)
    {
        if (actionFunction != null)
        {
            actionFunction();
        }
    }
}
