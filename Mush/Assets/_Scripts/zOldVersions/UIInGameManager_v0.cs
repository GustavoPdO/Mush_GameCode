//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEngine.UI;

//public class UIInGameManager_v0 : MonoBehaviour
//{ //(Obs: manter os paineis de status dos players desativados e as vidas tambem, etc. quando for "testar"/"buildar" o jogo (ativados e geradas por script dependendo da quantidade necessaria))
//  //(Obs2: eh importante ressaltar que a referencia dos jogadores terao que ser passadas na inicializacao da cena quando for feito o menu/cena de selecao (+ verificar se algum outro objeto tambem nao depende de referenciacao ao inicializar a cena, como os delimitadores de cenario para a camera se necessario, etc.)

//    //public
//    public GameObject[] players; //referencia aos jogadores na tela

//    [Space(5)]
//    public float lifeChangeDurationTime; //tempo que a vida leva para mudar para a nova quantidade

//    [Space(5)]
//    public GameObject lifePrefab; //prefab de uma vida
//    public Color enabledLifeColor; //cor de uma vida ativada/com a vida
//    public Color disabledLifeColor; //cor de uma vida desativada/sem a vida

//    //private
//    private struct Player // struct com UI e dados necessarios de cada jogador
//    {
//        //Dados da UI
//        public Transform playerStatsPanel; // transform do painel de status

//        public Slider playerHealthBarSlider; // slider de vida
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

//    private float lifeChangeStartTime; //velocidade inicial (desde o comeco da cena)
//    private bool isReducingLife; //relacionada a co rotina ReduceLifeBar

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

//            playersStruct[i].playerHealthBarSlider = playersStruct[i].playerStatsPanel.Find("HealthBarSlider").GetComponent<Slider>();
//            playersStruct[i].playerHealthBarSlider.maxValue = playersStruct[i].characterSO.maxHealth;
//            playersStruct[i].playerHpNumberText = playersStruct[i].playerHealthBarSlider.transform.Find("HpNumberText").GetComponent<Text>();

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
//            int livesCount = playersStruct[i].characterSO.maxNumberOfLives; //pegar numero de vidas de um jogador
//            playersStruct[i].playerLives = new Image[livesCount]; //inicializar array da struct
//            for (int j = 0; j < livesCount; j++) //para cada vida, criar e inicializar valores necessarios
//            {
//                GameObject newLife = Instantiate(lifePrefab, playerLivesPanel); //criar e referenciar cada uma das vidas do jogador
//                newLife.name = "Life" + (j + 1); //renomear cada uma das vidas
//                RectTransform newLifeRectTransform = newLife.GetComponent<RectTransform>(); //inicializar valores
//                newLifeRectTransform.anchorMin = new Vector2((float)j / livesCount, newLifeRectTransform.anchorMin.y);
//                newLifeRectTransform.anchorMax = new Vector2((float)(j + 1) / livesCount, newLifeRectTransform.anchorMax.y);
//                newLifeRectTransform.offsetMin = Vector2.zero;
//                newLifeRectTransform.offsetMax = Vector2.zero;

//                playersStruct[i].playerLives[j] = newLife.GetComponent<Image>(); //referenciar nova vida criada
//            }
//        }

//        //inicializar valores
//        for (int i = 0; i < numberOfPlayers; i++) //para cada jogador setar os dados necessarios na interface (UI in game)
//        {
//            SetCurrentLife(i);
//            SetCurrentNumberOfLives(i);
//            SetCurrentSkillsState(i);
//        }
//    }

//    // Use this for initialization
//    void Start()
//    {
//        lifeChangeStartTime = Time.time; //inicializar valores
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

//    //(atualizar para versoes "melhores" depois) //(+ barras, e fazer as skills, etc.)
//    public void SetCurrentLife(int playerNumber) //seta a vida atual do jogador
//    {
//        //playersStruct[playerNumber].playerHealthBarSlider.value = playersStruct[playerNumber].healthController.actualHealth; //setar valor no slider de vida
//        //playersStruct[playerNumber].playerHealthBarSlider.value = Mathf.FloorToInt(Mathf.Lerp(playersStruct[playerNumber].playerHealthBarSlider.value, playersStruct[playerNumber].healthController.actualHealth, Time.deltaTime * lifeChangeSpeed)); //setar valor no slider de vida lentamente
//        if (!isReducingLife && playersStruct[playerNumber].playerHealthBarSlider.value != playersStruct[playerNumber].healthController.actualHealth)
//        {
//            StartCoroutine(ReduceLifeBar(playerNumber, (int)playersStruct[playerNumber].playerHealthBarSlider.value)); //reduzir gradualmente a vida na barra de vida
//        }
//        playersStruct[playerNumber].playerHpNumberText.text = playersStruct[playerNumber].playerHealthBarSlider.value + "/" + playersStruct[playerNumber].characterSO.maxHealth; //setar valor no texto de vida
//    }

//    public IEnumerator ReduceLifeBar(int playerNumber, int initialLife) //reduz gradualmente a vida na barra de vida
//    {
//        isReducingLife = true;

//        float multiplierAux = 1;
//        if (initialLife > playersStruct[playerNumber].healthController.actualHealth) multiplierAux = -1;
//        Debug.Log("Pass -- " + initialLife + " - " + playersStruct[playerNumber].playerHealthBarSlider.value + " : " + playersStruct[playerNumber].healthController.actualHealth);

//        while (playersStruct[playerNumber].playerHealthBarSlider.value != playersStruct[playerNumber].healthController.actualHealth) //enquanto a barra nao tiver atingido a vida que deve atingir
//        {
//            float newTime = (Time.time - lifeChangeStartTime) / lifeChangeDurationTime;

//            playersStruct[playerNumber].playerHealthBarSlider.value = Mathf.FloorToInt(Mathf.Abs(Mathf.Lerp(initialLife * multiplierAux, playersStruct[playerNumber].healthController.actualHealth * multiplierAux, newTime))); //setar valor no slider de vida lentamente

//            yield return null; //passar frame
//        }

//        isReducingLife = false;
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
///*
//    public IEnumerator ReduceLifeBar(int playerNumber, int initialLife) //reduz gradualmente a vida na barra de vida
//    {
//        isReducingLife = true;

//        float multiplierAux = 1;
//        if (initialLife > playersStruct[playerNumber].healthController.actualHealth) multiplierAux = -1;
//        Debug.Log("Pass -- " + initialLife + " - " + playersStruct[playerNumber].playerHealthBarSlider.value + " : " + playersStruct[playerNumber].healthController.actualHealth);

//        while (playersStruct[playerNumber].playerHealthBarSlider.value != playersStruct[playerNumber].healthController.actualHealth) //enquanto a barra nao tiver atingido a vida que deve atingir
//        {
//            float newTime = (Time.time - lifeChangeStartTime) / lifeChangeDurationTime;

//            playersStruct[playerNumber].playerHealthBarSlider.value = Mathf.FloorToInt(Mathf.Abs(Mathf.SmoothStep(initialLife * multiplierAux, playersStruct[playerNumber].healthController.actualHealth * multiplierAux, newTime))); //setar valor no slider de vida lentamente

//            yield return null; //passar frame
//        }

//        isReducingLife = false;
//    }
//*/
