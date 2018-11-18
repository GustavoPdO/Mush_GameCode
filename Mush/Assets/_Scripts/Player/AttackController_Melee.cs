using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackController_Melee : AttackController
{ //para controlar o ataque melee de um personagem (falta testar com as animacoes e as chamadas de funcao nas animations)
    //public

    //scriptable public - private

    //private
    private Collider2D col2d; //collider2D

    private bool[] wasDamageDealt; //para verificar se o ataque corrente ja deu dano no player X

    //inicializar antes do start
    private void Awake()
    {
        col2d = gameObject.GetComponent<Collider2D>(); //inicializar valores 

        player = gameObject.transform.root.gameObject; //gameObject.transform.parent.gameObject; //pegar referencia do parent (player)

        anim = player.GetComponent<Animator>(); //pegar animator do player
        healthController = player.GetComponent<HealthController>(); //pegar script health controller do parent (player)
        playerController = player.GetComponent<PlayerController>(); //pegar script player controller do parent (player)

        col2d.enabled = false;

        Initialize(); //inicializar variaveis
    }

    public void Initialize() //inicializa variaveis do player
    {
        attackDamage = characterSO.attackDamage;
        maxQtdAttacks = characterSO.maxQtdAttacks;

        canStopMovement = characterSO.canStopMovement;
    }

    // Use this for initialization
    void Start()
    {
        if (player.CompareTag("Player1")) playerNumber = 1; //encontrar numero do player
        else /*if(gameObject.CompareTag("Player2"))*/ playerNumber = 2;

        attackCount = 0; //inicializar valores
        wasDamageDealt = new bool[4] { false, false, false, false }; //inicializar array de dano causado

        canAttack = true; //inicializar valores
        isAttacking = false;
    }

    //Update esta no AttackController

    public override void StartAttack(string name = "", int attackNumber = 0) //seta preparacoes para iniciar o ataque/dar dano (chamar pela animacao [comeco]) //(argumento string opcional, para testes) //(argumento int obrigatorio para ataques basicos)
    {
        //Debug.Log(player.tag + " : " + name); //para testes
        this.attackNumber = attackNumber;

        //isAttacking = true;
        anim.SetBool("doAttack", false); //resetar bool de atacar

        wasDamageDealt = new bool[4] { false, false, false, false }; //re inicializar array de dano causado

        col2d.enabled = true; //habilitar collider
    }

    //OBS: talvez para casos de cast de skill no meio do ataque seja necessario resetar as variaveis do ataque (talvez precise chamar EndAttack quando entrar na animacao de uma skill e desabilitar ataques)
    public override void EndAttack(string name = "") //seta preparacoes para finalizar o ataque/dar dano (chamar pela animacao [final]) //(para cancelar/finalizar ataques tambem) //(argumento opcional, para testes)
    {
        Debug.Log(player.tag + " : " + name); //para testes

        col2d.enabled = false; //desabilitar collider

        //wasDamageDealt = new bool[4] { false, false, false, false }; //re inicializar array de dano causado

        //ativar para conectar ultimo ataque com primeiro
        //if (/*!anim.GetBool("doAttack")*/attackCount >= maxQtdAttacks) isAttacking = false; //se nao for atacar novamente/nao estiver atacando //(obs: antes era trigger, mas nao dava pra fazer essa verificacao, ou alterar para soh chavear no final da animacao de ataque)
    }

    //se o trigger entrar em outro objeto
    private void OnTriggerEnter2D(Collider2D other)
    {
        //Debug.Log("T attackCount: " + attackNumber); //para testes
        //Debug.Log("T attackCount: " + anim.GetCurrentAnimatorClipInfo(0)[0].clip.name); //(mostrar que nao computa troca de animacoes rapido o suficiente, fazendo parecer que pula do ataque 1 para o 3)
        if (!other.CompareTag(player.tag) && other.tag.Contains("Player")) //se nao for esse player que estiver sendo atingido e se for outro player
        {
            Attack_DoDamage(other.gameObject); //tentar enviar dano
        }
    }

    public void Attack_DoDamage(GameObject other) //tenta enviar dano causado
    {
        for (int i = 1; i <= 4; i++) //verificar qual player for acertado
        {
            if (other.tag.Contains("" + i) && wasDamageDealt[i]) return; //se esse player ja foi acertado
            else if (other.tag.Contains("" + i)) wasDamageDealt[i] = true; //se nao, marcar como acertado
        }

        if (isAttacking) healthController.DoDamage(other, attackDamage[attackNumber]); //enviar dano //se estiver atacando
        else healthController.DoDamage(other, skillDamage); //enviar dano caso nao esteja atacando (no caso, vai estar usando skill)
    }
}
