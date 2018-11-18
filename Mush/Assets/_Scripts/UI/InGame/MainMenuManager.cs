using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    //public
    public string nextSceneName; //nome da proxima cena que carrega depois dessa cena //(nesse caso SelectScene)

    [Header("Buttons")]
    public int startButtonNumber; //numero do botao de iniciar //(contagem da ordem comecando do 0)
    public int exitButtonNumber; //numero do botao de sair //(contagem da ordem comecando do 0)

    [Space(5)]
    public Text[] buttonText; //referencia aos botoes da cena //(na ordem)
    public int buttonHoverOffFontSize; //tamanho dos botoes quando nao "selecionados"
    public int buttonHoverOnFontSize; //tamanho dos botoes quando "selecionados"
    public Color buttonHoverOffColor; //cor dos botoes quando nao "selecionados"
    public Color buttonHoverOnColor; //cor dos botoes quando "selecionados"

    [Header("Panels")]
    public GameObject[] itemPanel; //referencia aos paineis dos itens da cena //(na ordem) //(0 -> default, colocar o default (nesse caso o de instrucoes) para preencher posicoes de botoes sem painel)
    public bool[] itemHasPanel; //se o botao selecionado tem um painel para ser ativado

    [Header("SubPanels")]
    public string defaultSubPanelName; //nome default dos subPanels (ex: SubPanelX -> SubPanel, X = 0, 1, ...)
    public int[] numberOfSubPanels; //numero de sub paineis que cada painel que contem sub paineis tem //(numerados na ordem -> 0, 1, ...)

    [Header("FadeInOut")]
    public Animator whiteBackgroundImage; //objeto/imagem/animator que contem a imagem de fundo branca 
    public Animator blackBackgroundImage; //objeto/imagem/animator que contem a imagem de fundo preta

    [Header("Sounds")] //nomes dos sons que vai tocar em cada parte
    public string menuMusicSound;
    [Space(3)]
    public string menuNavSound;
    public string menuSelectSound;
    [Space(3)]
    public string panelNavSound;
    public string panelBackSound;
    [Space(3)]
    public string gameStartSound;
    public string gameExitSound;

    //private
    private int currentSelectedButton; //item atual "selecionado"
    private int currentOnPanel; //painel atual ativado
    private int currentOnSubPanel; //sub painel atual ativado

    private bool isStartExitSelected; //se o "botao" de start ja foi selecionado

    private System.Action actionFunction; //funcao que ira executar quando a animacao acabar

    private SoundsGeneral soundsGeneral; //script soundsManager do objeto soundsManager

    private bool isVerticalInUse;
    private bool isHorizontalInUse;

    //inicializar antes do start
    private void Awake()
    {
        soundsGeneral = GameObject.FindObjectOfType<SoundsGeneral>(); //inicializar valores

        currentSelectedButton = 0; //inicializar valores
        currentOnPanel = 0;
        currentOnSubPanel = 0;

        buttonText[currentSelectedButton].fontSize = buttonHoverOnFontSize; //inicializar dados
        buttonText[currentSelectedButton].color = buttonHoverOnColor;

        isStartExitSelected = false; //inicializar valores
        isVerticalInUse = false;
        isHorizontalInUse = false;
    }

    // Use this for initialization
    void Start()
    {
        whiteBackgroundImage.SetTrigger("FadeOut"); //"iniciar jogo" (menu principal) 
        actionFunction = null;

        //for (int i = 0; i < itemPanel.Length; i++) //iniciar paineis escondidos na lateral esquerda
        //{
        //    RectTransform newPanelRectTransform = itemPanel[i].GetComponent<RectTransform>(); //inicializar valores
        //    newPanelRectTransform.anchorMax = new Vector2(newPanelRectTransform.anchorMin.x, newPanelRectTransform.anchorMax.y);
        //    newPanelRectTransform.offsetMax = new Vector2(-1, 0);
        //    newPanelRectTransform.offsetMin = Vector2.zero;
        //}

        itemPanel[0].GetComponent<Animator>().SetTrigger("OpenLeft"); //abrir painel 0 pela esquerda //(iniciar)

        //if(soundsGeneral != null)
        soundsGeneral.FindAndPlay(menuMusicSound); //encontrar e tocar som
    }

    // Update is called once per frame
    void Update()
    {
        if (isStartExitSelected) return; //se estiver comecando/saindo do jogo

        float moveHorizontal = Input.GetAxisRaw("NavigationHorizontal1"); //pegar navegacao horizontal //(-1, 0, 1)
        float moveVertical = Input.GetAxisRaw("NavigationVertical"); //pegar navegacao vertical //(-1, 0, 1)

        if (/*Input.GetButtonDown("NavigationVertical")*/!isVerticalInUse && moveVertical != 0) //se apertar o botao de navegar na vertical
        {
            isVerticalInUse = true;
            ChooseNextSelectionVertical((int)moveVertical); //escolher proxima selecao de botao na tela
        }
        else if (/*Input.GetButtonDown("NavigationHorizontal1")*/!isHorizontalInUse && moveHorizontal != 0) //se apertar o botao de navegar na horizontal
        {
            isHorizontalInUse = true;
            ChooseNextSelectionHorizontal((int)moveHorizontal); //escolher proxima selecao de painel na tela
        }
        else if (Input.GetButtonDown("Select1")) //se apertar o botao de selecionar
        {
            SelectCurrentButton(); //verifica em qual painel/boato se encontra e executa acao de "selecionar" no botao/painel atual 
        }
        else if (Input.GetButtonDown("Back")) //se apertar o botao de selecionar
        {
            BackToItems(); //volta a selecao para os botoes do "menu principal"
        }

        if (moveVertical == 0)
        {
            isVerticalInUse = false;
        }
        if (moveHorizontal == 0)
        {
            isHorizontalInUse = false;
        }
    }

    public void ChooseNextSelectionVertical(int direction) //escolher proxima selecao na tela (botoes) //(direction -> 1 cima, -1 baixo, 0 nao fazer nada)
    {
        soundsGeneral.FindAndPlay(menuNavSound); //encontrar e tocar som

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

    public void ChooseNextSelectionHorizontal(int direction) //escolher proxima selecao na tela (paineis) //(direction -> 1 cima, -1 baixo, 0 nao fazer nada)
    {
        soundsGeneral.FindAndPlay(panelNavSound); //encontrar e tocar som

        int newSubPanelSelection = (currentOnSubPanel + direction + numberOfSubPanels[currentOnPanel]) % numberOfSubPanels[currentOnPanel]; //setar proxima posicao da selecao de sub paineis
        ActualizeSubPanelSelected(newSubPanelSelection, direction);
    }

    public void ActualizeSubPanelSelected(int newSubPanelSelection, int direction) //mudar selecao do sub painel para proximo selecionado
    {
        if (newSubPanelSelection == currentOnSubPanel) return; //se o sub painel selecionado ja estiver ativado

        itemPanel[currentOnPanel].transform.Find(defaultSubPanelName + newSubPanelSelection).gameObject.SetActive(true); //ativar proximo sub painel do botao selecionado atual
        SubPanel_AnimateTransition(currentOnSubPanel, newSubPanelSelection, direction); //anima a transicao do sub painel atual para o proximo sub painel
        //itemPanel[currentOnPanel].transform.Find(defaultSubPanelName + currentOnSubPanel).gameObject.SetActive(false); //desativar sub painel do botao selecionado atual

        currentOnSubPanel = newSubPanelSelection; //atualizar dados
    }

    public void SelectCurrentButton() //verifica em qual painel/boato se encontra e executa acao de "selecionar" no botao/painel atual 
    {
        if (currentSelectedButton == startButtonNumber) //se for o botao de iniciar
        {
            soundsGeneral.FindAndPlay(gameStartSound); //encontrar e tocar som

            isStartExitSelected = true;
            //StartGame(); //comecar o jogo / chamar cena de selecao de personagens
            whiteBackgroundImage.Rebind(); //resetar animator
            actionFunction = StartGame; //setar funcao para chamar no final da animacao
            whiteBackgroundImage.SetTrigger("FadeIn");

            //StartCoroutine(Scene_AnimateTransition(whiteBackgroundImage, "FadeIn", StartGame)); //comecar o jogo
        }
        else if (currentSelectedButton == exitButtonNumber) //se for o botao de sair
        {
            soundsGeneral.FindAndPlay(gameExitSound); //encontrar e tocar som

            isStartExitSelected = true;
            actionFunction = ExitGame; //setar funcao para chamar no final da animacao
            blackBackgroundImage.SetTrigger("FadeIn");

            //ExitGame(); //sair/fechar jogo
            //StartCoroutine(Scene_AnimateTransition(blackBackgroundImage, "FadeIn", ExitGame)); //sair/fechar o jogo
        }
        else if (itemHasPanel[currentSelectedButton] && currentOnPanel != currentSelectedButton) //se o botao selecionado tem algum painel para ser ativado //e se nao for o painel dele que estiver ativado
        {
            soundsGeneral.FindAndPlay(menuSelectSound); //encontrar e tocar som

            itemPanel[currentSelectedButton].SetActive(true); //ativar painel do botao selecionado novo
            Panel_AnimateTransition(currentOnPanel, currentSelectedButton, 1); //anima a transicao do painel atual para o proximo painel

            //itemPanel[currentOnPanel].SetActive(false); //desativar painel do botao selecionado atual
            ResetCurrentPanel(); //reseta o painel atual

            currentOnPanel = currentSelectedButton; //atualizar dados
        }
    }

    public void BackToItems() //volta a selecao para os botoes do "menu principal"
    {
        if (currentOnPanel == 0) return; //se ja for o painel 0, retornar

        soundsGeneral.FindAndPlay(panelBackSound); //encontrar e tocar som

        itemPanel[0].SetActive(true); //ativar painel inicial
        Panel_AnimateTransition(currentOnPanel, 0, -1); //anima a transicao do painel atual para o proximo painel

        //itemPanel[currentOnPanel].SetActive(false); //desativar painel do botao selecionado atual
        ResetCurrentPanel(); //reseta o painel atual

        currentOnPanel = 0; //re setar painel atual
    }

    public void ResetCurrentPanel() //reseta o painel atual //(com relacao aos sub paineis)
    {
        itemPanel[currentOnPanel].transform.Find(defaultSubPanelName + 0).gameObject.SetActive(true); //ativar sub painel inicials
        SubPanel_AnimateTransition(currentOnSubPanel, 0, -1); //anima a transicao do sub painel atual para o proximo sub painel
        //itemPanel[currentOnPanel].transform.Find(defaultSubPanelName + currentOnSubPanel).gameObject.SetActive(false); //desativar sub painel do botao selecionado atual
        currentOnSubPanel = 0; //re setar sub painel atual
    }

    public void Panel_AnimateTransition(int currentPanelNumber, int newPanelNumber, int direction) //anima a transicao do painel atual para o proximo painel //(direcao -> (<- -1, 0, 1 ->))
    {
        if (direction == 1) //animar paineis para a direita
        {
            itemPanel[currentPanelNumber].GetComponent<Animator>().SetTrigger("CloseRight"); //fechar painel antigo para a direita
            itemPanel[newPanelNumber].GetComponent<Animator>().SetTrigger("OpenLeft"); //abrir painel novo pela esquerda
        }
        else if (direction == -1) //animar paineis para a esquerda
        {
            itemPanel[currentPanelNumber].GetComponent<Animator>().SetTrigger("CloseLeft"); //fechar painel antigo para a direita
            itemPanel[newPanelNumber].GetComponent<Animator>().SetTrigger("OpenRight"); //abrir painel novo pela esquerda
        }
    }

    public void SubPanel_AnimateTransition(int currentSubPanelNumber, int newSubPanelNumber, int direction) //anima a transicao do sub painel atual para o proximo sub painel //(direcao -> (<- -1, 0, 1 ->))
    {
        if (direction == 1) //animar sub paineis para a direita
        {
            itemPanel[currentOnPanel].transform.Find(defaultSubPanelName + currentSubPanelNumber).gameObject.GetComponent<Animator>().SetTrigger("CloseRight"); //fechar painel antigo para a direita
            itemPanel[currentOnPanel].transform.Find(defaultSubPanelName + newSubPanelNumber).gameObject.GetComponent<Animator>().SetTrigger("OpenLeft"); //abrir painel novo pela esquerda
        }
        else if (direction == -1) //animar sub paineis para a esquerda
        {
            itemPanel[currentOnPanel].transform.Find(defaultSubPanelName + currentSubPanelNumber).gameObject.GetComponent<Animator>().SetTrigger("CloseLeft"); //fechar painel antigo para a direita
            itemPanel[currentOnPanel].transform.Find(defaultSubPanelName + newSubPanelNumber).gameObject.GetComponent<Animator>().SetTrigger("OpenRight"); //abrir painel novo pela esquerda
        }

        itemPanel[currentOnPanel].transform.Find("NumberText").GetComponent<Text>().text = (newSubPanelNumber + 1) + "/" + numberOfSubPanels[currentOnPanel]; //setar texto da pagina do menu
    }

    public void StartGame() //comecar o jogo / chamar cena de selecao de personagens
    {
        Debug.Log("Starting Select Scene!");

        SceneManager.LoadScene(nextSceneName); //carregar proxima cena (SelectScene)
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

    //public IEnumerator Scene_AnimateTransition(Animator backgroundImageAnim, string triggerName, System.Action function) //animar transicao de cenas //(esperar animacao acabar e chamar funcao passada)
    //{
    //    isStartExitSelected = true;

    //    backgroundImageAnim.SetTrigger(triggerName); //"iniciar jogo" (menu principal) 

    //    yield return new WaitWhile(() => !(backgroundImageAnim.GetCurrentAnimatorStateInfo(0).IsName("ImageFadeIn") || backgroundImageAnim.GetCurrentAnimatorStateInfo(0).IsName("ImageFadeOut")) || backgroundImageAnim.GetCurrentAnimatorStateInfo(0).normalizedTime < 1); //enquanto estiver nao estiver rodando uma das animations esperar, e quando comecar esperar acabar //(/esperar animation acabar)
    //    //yield return new WaitForSeconds(backgroundImage.GetCurrentAnimatorStateInfo(0).length); //esperar duracao da animacao

    //    function(); //chamar funcao passada
    //}
}
