using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;

public class HealthController_v0 : MonoBehaviour
{ //controlador de vida e receber/dar dano
    //public
    public CharacterScriptableObject characterSO; //referencia para os dados do personagem

    public int actualHealth; //vida atual
    public int actualNumberOfLives; //numero de vidas atual
    public int actualDefense; //defesa atual

    public bool infiniteLife; //vida infinita?
    public bool canReceiveDamage; //pode receber dano?

    [HideInInspector]
    public int damageReceived; //armazena dano recebido ate o momento (pode ser zerado/controlado por fora) //(para ser utilizado em skills, como parry, que devolve parte do dano recebido)

    //scriptable public - private
    public int maxHealth; //vida maxima de uma vida do character
    public int maxNumberOfLives; //numero maximo de vida do character
    public int defense; //defesa padrao
    public float damageDelay; //delay para poder levar dano novamente

    //private
    //private Rigidbody2D rb2d; //rigdbody2d
    //private string targetOppositeTag; //tag de quem vai ser acertado
    private Animator anim; //animator

    private bool isDecreasingHealth; //verifica se acabou de receber dano / ainda esta recebendo dano (relacionado ao damage delay) //para a coroutine DecreaseHealth()

    //inicializar antes do start
    private void Awake()
    {
        //rb2d = gameObject.GetComponent<Rigidbody2D>(); //inicializar valores
        anim = gameObject.GetComponent<Animator>();

        Initialize(); //inicializar variaveis
    }

    public void Initialize() //inicializa variaveis do player
    {
        maxHealth = characterSO.maxHealth;
        maxNumberOfLives = characterSO.maxNumberOfLives;
        defense = characterSO.defense;
        damageDelay = characterSO.damageDelay;
    }

    // Use this for initialization
    void Start()
    {
        //if (gameObject.CompareTag("Player1")) targetOppositeTag = "Player2"; //quem vai ser o alvos
        //else if (gameObject.CompareTag("Player2")) targetOppositeTag = "Player1";

        actualDefense = defense; //inicalizar valores
        actualHealth = maxHealth;
        actualNumberOfLives = maxNumberOfLives;
        damageReceived = 0;

        isDecreasingHealth = false;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void DoDamage(GameObject other, int damage) //tentar enviar dano
    {
        other.SendMessageUpwards("ReceiveDamage", damage/*, SendMessageOptions.DontRequireReceiver*/); //tenta enviar dano //nao requisitar msg de retorno
    }

    public void ReceiveDamage(int damage)
    {
        damageReceived += (damage - actualDefense); //contador de dano recebido
        //Debug.Log(gameObject.name + " : damage received " + damageReceived);

        if (canReceiveDamage && damage > 0) //se puder receber dano e o dano for maior que 0
        {
            if (!isDecreasingHealth) StartCoroutine(DecreaseHealth(damage - actualDefense)); //diminuir health //(alterar formula de defesa depois)
        }
    }

    public IEnumerator DecreaseHealth(int damage) //diminuir health e esperar x segs para poder diminuir de novo
    {
        if (damageDelay > 0) isDecreasingHealth = true; //se tiver delay, lockar zona de receber dano

        if (actualHealth - damage <= 0) //diminuir health
        {
            //actualHealth = 0;
            if (actualNumberOfLives > 1) //se o numero atual de vidas for maior que um //(talvez alterar para simular a barras de vida)
            {
                actualNumberOfLives -= 1; //diminuir quantidade de vidas atual
                actualHealth += maxHealth - damage; //resetar vida atual (com a vida maxima [contabilizando dano anterior recebido (se tiver recebido algo) -> (actualHealth + maxHealth - damage)])
            }
            else
            {
                actualHealth = 0;

                //if (infiniteLife) actualHealth = maxHealth;
                if (infiniteLife) //se tiver vidas infinitas
                {
                    actualNumberOfLives = maxNumberOfLives; //resetar quantidade de vidas (com a quantidade maxima)
                    actualHealth = maxHealth; //resetar vida atual (com a vida maxima)
                }
                else
                {
                    anim.SetTrigger("noHealth"); //triggerar animacao //anim.ResetTrigger("noHealth"); //resetar trigger 
                    //anim.Rebind(); //restar animator (mudar de lugar depois)
                    //Destroy(gameObject); //"Destroy" //alterar depois //+ efeitos, etc
                }
            }
        }
        else
        {
            actualHealth -= damage;
            //+ efeitos
        }

        Debug.Log("Health " + gameObject.name + ": " + actualHealth + ". Damage taken: " + damage + ". Lives: " + actualNumberOfLives);

        if (damageDelay > 0) yield return new WaitForSeconds(damageDelay); //se tiver delay, eseperar antes de poder levar dano novamente

        if (damageDelay > 0) isDecreasingHealth = false;
    }
}
