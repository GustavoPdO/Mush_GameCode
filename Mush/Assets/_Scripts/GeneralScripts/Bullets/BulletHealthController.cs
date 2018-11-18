using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletHealthController : MonoBehaviour
{
    //public
    public int bulletDamage; //ataque/dano da bala
    public float bulletLifeTime; //tempo de vida da bala

    [HideInInspector]
    public HealthController healthController; //healthController do player

    [HideInInspector]
    public bool canDestroyBullet; //True - destruir bala, False - nao fazer nada

    //private
    private float destroyDelay; //delay de tempo para a destruicao da bala (se estiver fazendo som, colocar tempo do som, por exemplo [sound.length, etc.])

    private bool canDoDamage; //para auxiliar o canDestroyBullet (enquanto a bala estive sendo destruida)
    private bool isDestroyingBullet; //para auxiliar o canDestroyBullet (referente a coroutine BulletDestroy)

    // Use this for initialization
    void Start()
    {
        destroyDelay = 0; //inicializar variaveis

        canDestroyBullet = false; //inicializar variaveis
        canDoDamage = true;
        isDestroyingBullet = false;

        StartCoroutine(BulletLifeTimeCount()); //espera o lifetime da bullet e seta ela como pronta para ser destruida
    }

    // Update is called once per frame
    void Update()
    {
        if (canDestroyBullet && !isDestroyingBullet) //se puder destruir a bala e ja nao estiver destruindo
        {
            StartCoroutine(BulletDestroy());
        }
    }

    //se o trigger entrar em outro objeto
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (canDoDamage && !other.CompareTag(healthController.tag) && other.tag.Contains("Player")) //se nao for esse player que estiver sendo atingido e se for outro player //e se puder dar dano
        {
            Attack_DoDamage(other.gameObject); //tentar enviar dano
        }
    }

    public void Attack_DoDamage(GameObject other) //tenta enviar dano causado
    {
        healthController.DoDamage(other, bulletDamage); //enviar dano
        canDestroyBullet = true; //"pode destruir a bala"
    }

    public IEnumerator BulletDestroy() //faz algo se tiver que fazer, tipo tocar um som, e destroi a bala
    {
        isDestroyingBullet = true;
        canDoDamage = false;

        gameObject.GetComponent<SpriteRenderer>().enabled = false; //"remover"/esconder bala do cenario

        yield return new WaitForSeconds(destroyDelay); //esperar delay para poder destruir

        Destroy(gameObject); //destruir bala    
    }

    public IEnumerator BulletLifeTimeCount() //espera o lifetime da bullet e seta ela como pronta para ser destruida
    {
        yield return new WaitForSeconds(bulletLifeTime);
        canDestroyBullet = true;
    }
}
