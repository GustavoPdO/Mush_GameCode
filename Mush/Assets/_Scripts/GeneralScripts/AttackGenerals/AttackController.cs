using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[System.Serializable] //para poder ser usado como objeto e aparecer no editor
public abstract class AttackController : MonoBehaviour
{ //classe base para ataques basicos (ex: melee, ranged, etc.) //(tambem para controle e uso de diferentes scripts de ataque como unico "AttackController" [+this.GetType().Name])
    //public
    public CharacterScriptableObject characterSO; //referencia para os dados do personagem

    [HideInInspector]
    public bool canAttack; //se pode ou nao atacar
    [HideInInspector]
    public int skillDamage; //dano da skill que for ser ativada (ajustar pelo script da skill)

    //scriptable public - private
    public int[] attackDamage; //ataque do character/da bala (para cada ataque ate o maxQtdAttacks)
    public int maxQtdAttacks; //quantidade maxima de ataques (seguir quantidade de um combo)

    public bool canStopMovement; //para parar ou nao movimento do player enquanto ataca

    //private/protected
    protected Animator anim; //animator
    protected HealthController healthController; //script healthController
    protected PlayerController playerController; //script playerController

    protected GameObject player; //player referente a esse "ataque"

    protected bool isAttacking; //para verificar se esta atacando ou nao

    protected int playerNumber; //numero do player relacionado a este script (para a selecao de controles, etc)

    protected int attackCount; //contar numeros de ataques dados ate o momento no combo (ex: max 3 por combo)

    //(eh necessario para verificar em qual animacao/ataque esta no animator, pois apesar do codigo presente no update abaixo, ele nao atualizava (ou provavelmente nao rapido o suficiente, nem nos outros tipos de updates) quando mudava de animacao para o OnTriggerEnter2D computar o dano do novo ataque)
    protected int attackNumber; //para o mesmo final que o attackCount 

    //functions
    public abstract void StartAttack(string name = "", int attackNumber = 0); //seta preparacoes para iniciar o ataque/dar dano (chamar pela animacao [comeco]) //(argumento opcional, para testes)

    //OBS: talvez para casos de cast de skill no meio do ataque seja necessario resetar as variaveis do ataque (talvez precise chamar EndAttack quando entrar na animacao de uma skill e desabilitar ataques)
    public abstract void EndAttack(string name = ""); //seta preparacoes para finalizar o ataque/dar dano (chamar pela animacao [final]) //(para cancelar/finalizar ataques tambem) //(argumento opcional, para testes)

    // Update is called once per frame
    void Update()
    {
        if (healthController.actualNumberOfLives < 1) return; //se o jogador nao tiver mais "vidas"

        isAttacking = false;
        for (int i = 1; i <= maxQtdAttacks; i++) //verificar se esta atacando / animando ataque
        {
            if (anim.GetCurrentAnimatorStateInfo(0).IsName("Attack" + i)) //se estiver em uma das animacoes de ataque
            {
                isAttacking = true;
                attackCount = i; //em qual das animacoes de ataque se encontra
            }
        }
        if (isAttacking) //se estiver atacando 
        {
            if (canStopMovement) playerController.canMove = false; //se puder parar o movimento do player parar 
            //else playerController.canMove = true; //se nao, manter/retomar movimentacao do player
        }
        else if (canStopMovement) playerController.canMove = true; //se nao, manter/retomar movimentacao do player
        //else anim.SetBool("doAttack", false); //senao, resetar bool de ataque

        playerController.isAttacking = isAttacking; //informar o isAttacking para o playerController //(para afetar velocidade de movimento enquanto ataca, etc.)

        if (canAttack && Input.GetButtonDown("Attack" + playerNumber)) //se puder atacar e apertar o botao de ataque
        {
            //Debug.Log("Attack Count " + attackCount + " : isAttacking " + isAttacking + " : doAttack " + anim.GetBool("doAttack"));
            if (!isAttacking) //se nao estiver atacando
            {
                //anim.SetBool("doAttack", true); //atacar
                attackCount = 1; //resetar attackCount
            }
            /*else */
            if (/*isAttacking && */attackCount < maxQtdAttacks) //se ja estiver atacando e ainda nao atacou a quantidade maxima de vezes
            {
                anim.SetBool("doAttack", true); //atacar
                //attackCount += 1; //somar quantidade de ataques realizados
            }
            else /*if (isAttacking && attackCount > maxQtdAttacks)*/ //caso nao possa realizar um ataque por causa do maximo de ataques permitidos, 
            { // resetar trigger/bool de ataque para nao ficar "estocando ataques" e nao comecar uma animacao de ataque logo em seguida do ultimo ataque sem apertar o botao
                anim.SetBool("doAttack", false); //resetar bool de ataque
            }
        }

        //Debug.Log(anim.GetCurrentAnimatorStateInfo(0).IsName("Attack1") + " : " + anim.GetCurrentAnimatorStateInfo(0).IsName("Attack2") + " : " +
        //    anim.GetCurrentAnimatorStateInfo(0).IsName("Attack3") + " : " + anim.GetCurrentAnimatorStateInfo(0).IsName("Attacking") + " : " +
        //    anim.GetCurrentAnimatorStateInfo(0).IsName("Idle")); //para testes
    }
}
