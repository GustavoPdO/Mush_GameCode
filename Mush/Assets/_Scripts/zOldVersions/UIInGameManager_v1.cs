//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEngine.UI;

//public class UIInGameManager_v1 : MonoBehaviour
//{ //(Obs: manter os paineis de status dos players desativados e as vidas tambem, etc. quando for "testar"/"buildar" o jogo (ativados e geradas por script dependendo da quantidade necessaria))
//  //(Obs2: eh importante ressaltar que a referencia dos jogadores terao que ser passadas na inicializacao da cena quando for feito o menu/cena de selecao (+ verificar se algum outro objeto tambem nao depende de referenciacao ao inicializar a cena, como os delimitadores de cenario para a camera se necessario, etc.)

//    //public
//    public GameObject[] players; //referencia aos jogadores na tela

//    [Space(5)]
//    public float healthAnimatingDelay; //atraso (/delay) /tempo em segundos que a animacao da barra leva para diminuir cada 1 de vida //(0 -> frame a frame)
//    public float healthAnimatingBackgroundInitialDelay; //atraso (/delay) /tempo em segundos inicial que a animacao da barra "background" leva para diminuir cada 1 de vida //(0 -> frame a frame) //aplicar em funcao depois

//    public GameObject healthBarPrefab; //prefab de uma barra de vida
//    public Color[] healthBarSlidersColors; //cor dos sliders de vida (na ordem do primeiro para o ultimo) //(do mesmo tamanho da quantidade de vidas de preferencia)
//    public float healthBackgroundShaderProportion; //proporcao da cor original que gera a cor shaded dela para sua barra "background"

//    [Space(5)]
//    public GameObject lifePrefab; //prefab de uma vida
//    public Color enabledLifeColor; //cor de uma vida ativada/com a vida
//    public Color disabledLifeColor; //cor de uma vida desativada/sem a vida

//    //private
//    private struct Player // struct com UI e dados necessarios de cada jogador
//    {
//        //Dados da UI
//        public Transform playerStatsPanel; // transform do painel de status

//        public Slider[] playerHealthBarSlider; // slider de vida
//        public Slider[] playerBackgroundHealthBarSlider;
//        public Text playerHpNumberText; // numero da vida

//        public Skill[] playerSkills; // skills de um jogador

//        public Image[] playerLives; //vidas de um jogador

//        //Dados do Objeto
//        public CharacterScriptableObject characterSO; //referencia para os dados do personagem

//        public HealthController healthController; //script healthController do jogador
//        public SkillManager skillManager; //script skillManager do jogador
//    }
//    private Player[] playersStruct; // struct com UI e dados necessarios de cada jogador

//    private struct Skill // struct com os componentes/objetos (necessarios) de uma skill
//    {
//        public Image iconBackgroundImage; //icone de background da skill (quando a skill estiver disponivel)
//        public Image iconSkillImage; //icone/imagem da skill 
//        public Image skillBlurEffectImage; //efeito de blur (quando a skill estiver em recarga)
//        public Text skillCdTimeText; //tempo de recarga (quando a skill estiver em recarga)
//        public Image iconSkillLockImage; //icone de skill bloqueada (quando a skill estiver bloqueada)

//        public bool isUsable; //para verificar se uma skill esta ou nao disponivel
//    }

//    private int numberOfPlayers; //numero de jogadores referenciados (/na cena)

//    private bool isAnimatingHealth; //se esta ou nao animando a barra de vida //(relacionada a coroutine Health_Animating)
//    private bool isAnimatingHealthBackground; //se esta ou nao animando a barra "background" de vida //(relacionada a coroutine HealthBackground_Animating)

//    //inicializar antes do start
//    private void Awake()
//    {
//        numberOfPlayers = players.Length; //inicializar valores 

//        playersStruct = new Player[numberOfPlayers]; //inicializar array

//        for (int i = 0; i < numberOfPlayers; i++) //para cada jogador referenciado (/na cena) //referenciar objetos
//        {
//            //Dados do Objeto
//            playersStruct[i].characterSO = players[i].GetComponent<PlayerController>().characterSO; //inicializar scriptableObject do jogador
//            playersStruct[i].healthController = players[i].GetComponent<HealthController>(); //script healthController do jogador
//            playersStruct[i].skillManager = players[i].GetComponent<SkillManager>(); //script skillManager do jogador

//            //Dados da UI
//            playersStruct[i].playerStatsPanel = gameObject.transform.Find("Player" + (i + 1) + "StatsPanel");

//            playersStruct[i].playerStatsPanel.gameObject.SetActive(true); // ativar painel de status do player //(para cada player na referenciado (/na cena))


//            playersStruct[i].playerHpNumberText = playersStruct[i].playerStatsPanel.Find("HealthBarPanel/HpNumberText").GetComponent<Text>();

//            Transform playerSkillsPanel = playersStruct[i].playerStatsPanel.Find("PlayerSkillsPanel");
//            playersStruct[i].playerSkills = new Skill[playerSkillsPanel.childCount]; //inicializar array da struct
//            for (int j = 0; j < playerSkillsPanel.childCount; j++) //para cada skill inicializar valores necessarios
//            {
//                Transform playerSkill = playerSkillsPanel.Find("Skill" + (j + 1)); //encontrar skill e inicializar valores
//                playersStruct[i].playerSkills[j].iconBackgroundImage = playerSkill.Find("IconBackgroundImage").GetComponent<Image>();
//                playersStruct[i].playerSkills[j].iconSkillImage = playerSkill.Find("IconCircleImage/IconSkillImage").GetComponent<Image>();
//                playersStruct[i].playerSkills[j].skillBlurEffectImage = playerSkill.Find("IconCircleImage/SkillBlurEffectImage").GetComponent<Image>();
//                playersStruct[i].playerSkills[j].skillCdTimeText = playerSkill.Find("IconCircleImage/SkillCdTimeText").GetComponent<Text>();
//                playersStruct[i].playerSkills[j].iconSkillLockImage = playerSkill.Find("IconSkillLockImage").GetComponent<Image>();

//                playersStruct[i].playerSkills[j].iconSkillImage.sprite = playersStruct[i].characterSO.skillsSprites[j]; //ajustar sprite da skill
//            }

//            Transform playerLivesPanel = playersStruct[i].playerStatsPanel.Find("PlayerLivesPanel");
//            Transform healthBarSlidersPanel = playersStruct[i].playerStatsPanel.Find("HealthBarPanel/HealthBarSlidersPanel");

//            int livesCount = playersStruct[i].characterSO.maxNumberOfLives; //pegar numero de vidas de um jogador
//            playersStruct[i].playerLives = new Image[livesCount]; //inicializar array da struct
//            for (int j = 0; j < livesCount; j++) //para cada vida, criar e inicializar valores necessarios
//            {
//                //criar barras de vida
//                GameObject newHealthBar = Instantiate(healthBarPrefab, healthBarSlidersPanel); //criar e referenciar cada uma das barras de vida (completa) do jogador
//                newHealthBar.name = healthBarPrefab.name + (j + 1); //renomear cada uma das barras de vidas

//                playersStruct[i].playerHealthBarSlider[j] = newHealthBar.transform.Find("HealthBarSlider").GetComponent<Slider>(); //inicializar valores e referenciar barras criadas
//                playersStruct[i].playerHealthBarSlider[j].maxValue = playersStruct[i].characterSO.maxHealth;
//                playersStruct[i].playerHealthBarSlider[j].value = playersStruct[i].healthController.actualHealth;
//                playersStruct[i].playerHealthBarSlider[j].gameObject.transform.Find("Fill Area/Fill").GetComponent<Image>().color = healthBarSlidersColors[j]; //setar cor da barra de vida

//                playersStruct[i].playerBackgroundHealthBarSlider[j] = newHealthBar.transform.Find("BackgroundHealthBarSlider").GetComponent<Slider>();
//                playersStruct[i].playerBackgroundHealthBarSlider[j].maxValue = playersStruct[i].characterSO.maxHealth;
//                playersStruct[i].playerBackgroundHealthBarSlider[j].value = playersStruct[i].playerHealthBarSlider[j].value;
//                playersStruct[i].playerBackgroundHealthBarSlider[j].gameObject.transform.Find("Fill Area/Fill").GetComponent<Image>().color = new Color(healthBarSlidersColors[j].r, healthBarSlidersColors[j].g, healthBarSlidersColors[j].b, 230f / healthBackgroundShaderProportion) * healthBackgroundShaderProportion; //setar cor da barra "backround" de vida com a proporcao de "escurecer" aplicada

//                //criar vidas
//                GameObject newLife = Instantiate(lifePrefab, playerLivesPanel); //criar e referenciar cada uma das vidas do jogador
//                newLife.name = lifePrefab.name + (j + 1); //renomear cada uma das vidas
//                RectTransform newLifeRectTransform = newLife.GetComponent<RectTransform>(); //inicializar valores
//                newLifeRectTransform.anchorMin = new Vector2((float)j / livesCount, newLifeRectTransform.anchorMin.y);
//                newLifeRectTransform.anchorMax = new Vector2((float)(j + 1) / livesCount, newLifeRectTransform.anchorMax.y);
//                newLifeRectTransform.offsetMin = Vector2.zero;
//                newLifeRectTransform.offsetMax = Vector2.zero;

//                playersStruct[i].playerLives[j] = newLife.GetComponent<Image>(); //referenciar nova vida criada
//            }
//        }
//    }

//    // Use this for initialization
//    void Start()
//    {
//        //inicializar valores
//        for (int i = 0; i < numberOfPlayers; i++) //para cada jogador setar os dados necessarios na interface (UI in game)
//        {
//            SetCurrentLife(i);
//            SetCurrentNumberOfLives(i);
//            SetCurrentSkillsState(i);
//        }
//    }

//    // Update is called once per frame
//    void Update()
//    {
//        for (int i = 0; i < numberOfPlayers; i++) //para cada jogador setar os dados necessarios na interface (UI in game)
//        {
//            SetCurrentLife(i);
//            SetCurrentNumberOfLives(i);
//            SetCurrentSkillsState(i);
//        }
//    }

//    //(atualizar para versoes "melhores" depois) //(+ barras, e fazer as skills, +scripts, etc.)
//    public void SetCurrentLife(int playerNumber) //seta a vida atual do jogador
//    {
//        //playersStruct[playerNumber].playerHealthBarSlider.value = playersStruct[playerNumber].healthController.actualHealth; //setar valor no slider de vida
//        if (!isAnimatingHealth && playersStruct[playerNumber].playerHealthBarSlider.value != playersStruct[playerNumber].healthController.actualHealth)
//        {
//            StartCoroutine(Health_Animating(playerNumber));
//        }

//        if (!isAnimatingHealthBackground && !isAnimatingHealth && playersStruct[playerNumber].playerBackgroundHealthBarSlider.value != playersStruct[playerNumber].healthController.actualHealth) //se nao estiver animando backround
//        {
//            StartCoroutine(HealthBackground_Animating(playerNumber));
//        }
//        else if (isAnimatingHealthBackground && isAnimatingHealth) //se estiver executando ambos ao mesmo tempo, cancelar
//        {
//            StopCoroutine(HealthBackground_Animating(playerNumber)); //parar co rotina de animar barra "background" de vida
//        }

//        playersStruct[playerNumber].playerHpNumberText.text = playersStruct[playerNumber].playerHealthBarSlider.value + "/" + playersStruct[playerNumber].characterSO.maxHealth; //setar valor no texto de vida
//    }

//    public IEnumerator Health_Animating(int playerNumber) //anima barra de vida (barra normal + "background")
//    {
//        isAnimatingHealth = true;

//        float initialHealthBarValue = playersStruct[playerNumber].playerHealthBarSlider.value; //valor inicial da vida a ser usada como parametro de maximo

//        //if/else para testes (quando regenera a vida, etc.) //ajustar depois
//        if (playersStruct[playerNumber].playerHealthBarSlider.value > playersStruct[playerNumber].healthController.actualHealth)
//        {
//            while (playersStruct[playerNumber].playerHealthBarSlider.value > playersStruct[playerNumber].healthController.actualHealth) //se a vida atual na barra for maior do que a vida atual do player 
//            {
//                yield return new WaitForSeconds(healthAnimatingDelay); //esperar um tempo e reduzir "lentamente" a barra de vida
//                playersStruct[playerNumber].playerHealthBarSlider.value -= 1;
//            }
//        }
//        else if (playersStruct[playerNumber].playerHealthBarSlider.value < playersStruct[playerNumber].healthController.actualHealth)
//        {
//            initialHealthBarValue = playersStruct[playerNumber].healthController.actualHealth; //valor inicial da vida a ser usada como parametro de maximo

//            while (playersStruct[playerNumber].playerHealthBarSlider.value < playersStruct[playerNumber].healthController.actualHealth) //se a vida atual na barra for maior do que a vida atual do player 
//            {
//                yield return new WaitForSeconds(healthAnimatingDelay); //esperar um tempo e reduzir "lentamente" a barra de vida
//                playersStruct[playerNumber].playerHealthBarSlider.value += 1;
//            }
//        }

//        isAnimatingHealth = false;
//    }

//    public IEnumerator HealthBackground_Animating(int playerNumber) //anima barra de vida (barra "background")
//    {
//        isAnimatingHealthBackground = true;

//        //if/else para testes (quando regenera a vida, etc.) //ajustar depois
//        if (playersStruct[playerNumber].playerBackgroundHealthBarSlider.value > playersStruct[playerNumber].playerHealthBarSlider.value)
//        {
//            while (playersStruct[playerNumber].playerBackgroundHealthBarSlider.value > playersStruct[playerNumber].playerHealthBarSlider.value) //se a vida atual na barra "background" for maior do que a vida atual da barra de vida
//            {
//                yield return new WaitForSeconds(healthAnimatingDelay); //esperar um tempo e reduzir "lentamente" a barra "background" de vida
//                playersStruct[playerNumber].playerBackgroundHealthBarSlider.value -= 1;
//            }
//        }
//        else if (playersStruct[playerNumber].playerBackgroundHealthBarSlider.value < playersStruct[playerNumber].playerHealthBarSlider.value)
//        {
//            while (playersStruct[playerNumber].playerBackgroundHealthBarSlider.value < playersStruct[playerNumber].playerHealthBarSlider.value) //se a vida atual na barra "background" for maior do que a vida atual da barra de vida
//            {
//                yield return new WaitForSeconds(healthAnimatingDelay); //esperar um tempo e reduzir "lentamente" a barra "background" de vida
//                playersStruct[playerNumber].playerBackgroundHealthBarSlider.value += 1;
//            }
//        }

//        isAnimatingHealthBackground = false;
//    }

//    public void SetCurrentNumberOfLives(int playerNumber) //seta a quantidade de vidas atual do jogador
//    {
//        int actualNumberOfLives = playersStruct[playerNumber].healthController.actualNumberOfLives; //encontrar quantidade de vidas atual
//        for (int i = 0; i < playersStruct[playerNumber].characterSO.maxNumberOfLives; i++) //setar vidas como ativadas ou desativadas (baseando-se na cor)
//        {
//            if (i < actualNumberOfLives) //se estiver ativa, ativar
//            {
//                playersStruct[playerNumber].playerLives[i].color = enabledLifeColor;
//            }
//            else //senao, desativar
//            {
//                playersStruct[playerNumber].playerLives[i].color = disabledLifeColor;
//            }
//        }
//    }

//    //(falta fazer cds, etc. das skills)
//    public void SetCurrentSkillsState(int playerNumber) //seta o estado atual das skills do jogador (bloqueada, em recarga, ativa, etc.)
//    {
//        //inicializar valores das skills de acordo com o nome dado no skillManager //(talvez padronizar nomes depois)
//        playersStruct[playerNumber].playerSkills[0].isUsable = playersStruct[playerNumber].skillManager.Skill1Usable;
//        playersStruct[playerNumber].playerSkills[1].isUsable = playersStruct[playerNumber].skillManager.Skill2Usable;
//        playersStruct[playerNumber].playerSkills[2].isUsable = playersStruct[playerNumber].skillManager.MushUsable;

//        int actualNumberOfLives = playersStruct[playerNumber].healthController.actualNumberOfLives; //encontrar quantidade de vidas atual
//        for (int i = 0; i < playersStruct[playerNumber].playerSkills.Length; i++) //para cada skill do player
//        {
//            if (playersStruct[playerNumber].characterSO.skillsLifeUnlock[i] < actualNumberOfLives) //verificar se a skill esta ou nao desbloqueada
//            {
//                playersStruct[playerNumber].playerSkills[i].iconSkillLockImage.enabled = true; //se estiver bloqueada, mostrar icone de bloqueada
//                playersStruct[playerNumber].playerSkills[i].iconBackgroundImage.enabled = false; //esconder icone/imagem de fundo da skill

//                playersStruct[playerNumber].playerSkills[i].skillBlurEffectImage.fillAmount = 1f; //mostrar "efeito de blur" completo
//                //playersStruct[playerNumber].playerSkills[i].skillBlurEffectImage.enabled = true; //mostrar "efeito de blur"

//                playersStruct[playerNumber].playerSkills[i].skillCdTimeText.text = ""; //esconder texto do tempo de recarga da skill
//            }
//            else //se nao estiver bloqueada
//            {
//                playersStruct[playerNumber].playerSkills[i].iconSkillLockImage.enabled = false; //esconder icone de bloqueada
//                playersStruct[playerNumber].playerSkills[i].iconBackgroundImage.enabled = true; //mostrar icone/imagem de fundo da skill

//                if (playersStruct[playerNumber].playerSkills[i].isUsable)
//                {
//                    playersStruct[playerNumber].playerSkills[i].skillBlurEffectImage.fillAmount = 0f; //esconder "efeito de blur"
//                    playersStruct[playerNumber].playerSkills[i].skillCdTimeText.text = ""; //esconder texto do tempo de recarga da skill
//                }
//            }
//        }
//    }
//}
