using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;

public class HealthController : MonoBehaviour
{ //controlador de vida e receber/dar dano
    //public
    public CharacterScriptableObject characterSO; //referencia para os dados do personagem

    public int[] actualHealth; //quantidade de vida atual em cada uma das vidas (em cada uma das N MaxNumberOfLives)
    public int actualNumberOfLives; //numero de vidas atual
    public int actualDefense; //defesa atual

    public bool infiniteLife; //vida infinita?
    public bool canReceiveDamage; //pode receber dano?

    public ParticleSystem lifeTakenParticleSystem; //sistema de particulas de quando leva dano

    [HideInInspector]
    public int damageReceived; //armazena dano recebido ate o momento (pode ser zerado/controlado por fora) //(para ser utilizado em skills, como parry, que devolve parte do dano recebido)

    //scriptable public - private
    public int maxHealth; //vida maxima de uma vida do character
    public int maxNumberOfLives; //numero maximo de vida do character
    public int defense; //defesa padrao
    public float damageDelay; //delay para poder levar dano novamente

    [Header("Sounds")] //nomes dos sons que vai tocar em cada parte
    public string receiveDamageSound;
    public string lifeLostSound;

    //private
    //private Rigidbody2D rb2d; //rigdbody2d
    //private string targetOppositeTag; //tag de quem vai ser acertado
    private Animator anim; //animator

    private bool isDecreasingHealth; //verifica se acabou de receber dano / ainda esta recebendo dano (relacionado ao damage delay) //para a coroutine DecreaseHealth()

    private SoundsGeneral soundsGeneral; //script soundsManager do objeto soundsManager

    //inicializar antes do start
    private void Awake()
    {
        soundsGeneral = GameObject.FindObjectOfType<SoundsGeneral>(); //inicializar valores

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
        for (int i = 0; i < maxNumberOfLives; i++)
        { //para cada vida, setar quantidade de vida maxima
            actualHealth[i] = maxHealth;
        }
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
        lifeTakenParticleSystem.Play(); //dar play no sistema de particulas de quando levar dano

        soundsGeneral.FindAndPlay(receiveDamageSound); //encontrar e tocar som

        if (canReceiveDamage && damage > 0) //se puder receber dano e o dano for maior que 0
        {
            if (!isDecreasingHealth) StartCoroutine(DecreaseHealth(damage - actualDefense)); //diminuir health //(alterar formula de defesa depois)
        }
    }

    public IEnumerator DecreaseHealth(int damage) //diminuir health e esperar x segs para poder diminuir de novo
    {
        if (damageDelay > 0) isDecreasingHealth = true; //se tiver delay, lockar zona de receber dano //(talvez colocar lock independente do delay por garantia)

        if (actualNumberOfLives > 0) //se o jogador ainda tiver mais "vidas"
        {
            if (actualHealth[actualNumberOfLives - 1] - damage <= 0) //diminuir health
            {
                soundsGeneral.FindAndPlay(lifeLostSound); //encontrar e tocar som

                //actualHealth = 0;
                if (actualNumberOfLives > 1) //se o numero atual de vidas for maior que um //(talvez alterar para simular a barras de vida)
                {
                    int decreaseHealthAux = actualHealth[actualNumberOfLives - 1]; //guardar valor anterior para contar corretamente dano levado na proxima barra de vida
                    actualHealth[actualNumberOfLives - 1] = 0; //setar barra de vida atual como zero
                    actualNumberOfLives -= 1; //diminuir quantidade de vidas atual
                    actualHealth[actualNumberOfLives - 1] = maxHealth + decreaseHealthAux - damage; //contabilizar dano tomado na nova barra de vida tambem (contando tambem dano tomado na barra de vida anterior antes de zerar)
                }
                else
                {
                    actualHealth[actualNumberOfLives - 1] = 0;
                    actualNumberOfLives = 0;

                    //if (infiniteLife) actualHealth = maxHealth;
                    if (infiniteLife) //se tiver vidas infinitas
                    {
                        actualNumberOfLives = maxNumberOfLives; //resetar quantidade de vidas (com a quantidade maxima)
                        for (int i = 0; i < maxNumberOfLives; i++)
                        { //para cada vida, resetar vida atual (com a vida maxima)
                            actualHealth[i] = maxHealth;
                        }
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
                actualHealth[actualNumberOfLives - 1] -= damage;
                //+ efeitos
            }

            if (actualNumberOfLives > 0)
                Debug.Log("Health " + gameObject.name + ": " + actualHealth[actualNumberOfLives - 1] + ". Damage taken: " + damage + ". Lives: " + actualNumberOfLives);

            if (damageDelay > 0) yield return new WaitForSeconds(damageDelay); //se tiver delay, eseperar antes de poder levar dano novamente
        }

        if (damageDelay > 0) isDecreasingHealth = false;
    }
}
