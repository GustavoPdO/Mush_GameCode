using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChildCaller_Attack : MonoBehaviour
{ //funcao que sera responsavel por passar chamadas de funcao das animations do parent para o respectivo child (usado para o ataque no caso - script no bone da sword/espada) 
  //(depois ajustar localizacao das bones e nome [vai ter que alterar todos os animations] para ficar mais organizado e geral)

    //public
    public CharacterScriptableObject characterSO; //referencia para os dados do personagem

    //scriptable public - private
    public string weaponPath; //caminho de onde se encontra a arma/colisor (em qual child) partindo do player
    //ex: Ronnie -> Bones/Hip/Torso/R Upper Arm/R Arm/R Hand --> .GetChild(0) = Sword

    //private
    private AttackController attackController; //scprit attackController 


    //inicializar antes do start
    private void Awake()
    {
        Initialize(); //inicializar variaveis
    }

    public void Initialize() //inicializa variaveis do player
    {
        weaponPath = characterSO.weaponPath;
    }


    // Use this for initialization
    void Start()
    {
        attackController = gameObject.transform.Find(weaponPath).GetChild(0).GetComponent<AttackController>(); //inicializar valores
        if (attackController == null) Debug.Log("ERRO: Objeto de ataque nao encontrado!"); //para melhor controle e teste
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void StartAttack(int attackNumber = 0) //seta preparacoes para iniciar o ataque/dar dano (chamar pela animacao [comeco]) //(argumento int obrigatorio para ataques basicos)
    {
        attackController.StartAttack(gameObject.name, attackNumber - 1); //(argumento string opcional, para testes) 
    }

    //OBS: talvez para casos de cast de skill no meio do ataque seja necessario resetar as variaveis do ataque (talvez precise chamar EndAttack quando entrar na animacao de uma skill e desabilitar ataques)
    public void EndAttack(string name = "") //seta preparacoes para finalizar o ataque/dar dano (chamar pela animacao [final]) //(para cancelar/finalizar ataques tambem) //(argumento opcional, para testes)
    {
        attackController.EndAttack(name);
    }
}
