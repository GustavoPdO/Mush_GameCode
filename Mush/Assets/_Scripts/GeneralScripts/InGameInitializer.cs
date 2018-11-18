using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InGameInitializer : MonoBehaviour
{ //provavelmente este script deve rodar antes dos outros para inicializar a cena corretamente (se necessario ajustar ordem de execucao depois)
    //public
    public GameObject multipleMainCamera; //referencia a camera do cenario
    public GameObject UIInGame; //referencia a interface dentro de jogo (/UI in game) do cenario

    public Transform[] playersInitialPosition; //posicao inicial dos jogadores na cena (/posicao de spawn)

    //[HideInInspector]
    public GameObject[] players; //referencia aos jogadores na cena //(setar quando trocar de cena para a cena do jogo (inGame)) //(obs: colocar players na ordem (P1, P2, ...))
    //[HideInInspector]
    public int playersNumber; //numero de jogadores na cena

    //private


    //inicializar antes do start
    private void Awake()
    {
        InitializeStartGame(); //inicializar dados

        //players = new GameObject[playersNumber]; //inicializar variaveis
        Initialize_Players(); //inicializar tags e outras coisas necessarias dos players da cena
        Initialize_Camera(); //inicializar o que for necessario da camera do cenario
        Initialize_UIInGame(); //inicializar o que for necessario da interface dentro de jogo (/UI in game) do cenario
    }

    public void InitializeStartGame() //seta os dados necessarios para inicializar uma partida/um jogo
    {
        GameDataGeneral gameDataGeneral = GameObject.FindObjectOfType<GameDataGeneral>(); //encontrar objeto com os dados do jogo na cena

        playersNumber = gameDataGeneral.playersNumber; //inicializar dados
        players = new GameObject[playersNumber];

        for (int i = 0; i < playersNumber; i++) //instanciar jogadores/players
        {
            players[i] = Instantiate(gameDataGeneral.playersPrefab[gameDataGeneral.characterSelected[i]], playersInitialPosition[i].position, playersInitialPosition[i].rotation);
            players[i].name = gameDataGeneral.playersPrefab[gameDataGeneral.characterSelected[i]].name + i;
            SpriteRenderer spriteRenderer = players[i].gameObject.transform.Find("Arrow").GetComponent<SpriteRenderer>();
            spriteRenderer.color = new Color(gameDataGeneral.playerColor[i].r, gameDataGeneral.playerColor[i].g, gameDataGeneral.playerColor[i].b);
        }

        //gameDataGeneral.SetDoGameData(false); //re setar doGameData //(para criar quando voltar da tela de jogo)
        //Destroy(gameDataGeneral.gameObject); //destruir objeto com os dados gerais (ja foram inicializados os dados)
    }

    public void Initialize_Players() //inicializar tags e outras coisas necessarias dos players da cena
    {
        for (int i = 0; i < playersNumber; i++) //para cada player
        {
            Transform[] allChildren = players[i].transform.GetComponentsInChildren<Transform>(); //pegar todos os objetos do jogador i e inicializar tag e layer 
            foreach (Transform child in allChildren) //para cada objeto filho (/child) do jogador (incluindo o objeto pai))
            {
                child.gameObject.tag = "Player" + (i + 1); //setar tag
                child.gameObject.layer = LayerMask.NameToLayer("Player" + (i + 1)); //setar layer
            }

            CharacterScriptableObject playerSO = players[i].transform.GetComponent<PlayerController>().characterSO; //pegar scriptable object do jogador
            GameObject playerWeapon = players[i].transform.Find(playerSO.weaponPath).GetChild(0).gameObject; //pegar arma do jogador
            allChildren = playerWeapon.transform.GetComponentsInChildren<Transform>(); //pegar todos os objetos da arma do jogador i e inicializar tag e layer 
            foreach (Transform child in allChildren) //para cada objeto filho (/child) da arma do jogador (incluindo o objeto pai))
            {
                child.gameObject.tag = "Damage" + (i + 1); //setar tag
                child.gameObject.layer = LayerMask.NameToLayer("Damage" + (i + 1)); //setar layer
            }
        }
    }

    public void Initialize_Camera() //inicializar o que for necessario da camera do cenario
    {
        MultipleTargetCameraController multipleTargetCameraController = multipleMainCamera.GetComponent<MultipleTargetCameraController>(); //pegar script multipleTargetCameraController
        multipleTargetCameraController.targets = new List<Transform>(); //inicialziar lista de alvos (/targets) da camera

        for (int i = 0; i < playersNumber; i++) //para cada player
        {
            multipleTargetCameraController.targets.Add(players[i].transform); //adicionar referencia do jogador na camera
        }
    }

    public void Initialize_UIInGame() //inicializar o que for necessario da interface dentro de jogo (/UI in game) do cenario
    {
        UIInGameManager uiInGameManager = UIInGame.GetComponent<UIInGameManager>(); //pegar script InGameManager
        uiInGameManager.players = new GameObject[playersNumber]; //inicializar array de jogadores da interface (/ui)

        for (int i = 0; i < playersNumber; i++) //para cada jogador
        {
            uiInGameManager.players[i] = players[i]; //adicionar referencia do jogador na interface
        }
    }

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
