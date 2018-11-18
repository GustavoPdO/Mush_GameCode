using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpikesController : MonoBehaviour
{
    //public
    public int damage; //dano causado

    public float timePerDamage; //tempo entre cada dano

    //private
    private Collider2D col2d; //collider2D do objeto (generico)

    private bool isWaiting; //se esta esperando

    private GameEndManager gameEndManager; //script/objeto gameEndManaged

    // Use this for initialization
    void Start()
    {
        gameEndManager = GameObject.FindObjectOfType<GameEndManager>();
        col2d = gameObject.GetComponent<Collider2D>(); //inicializar valores

        isWaiting = false;
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnCollisionStay2D(Collision2D other)
    {
        if (!gameEndManager.isGameEnd && !isWaiting && other.gameObject.tag.Contains("Player")) //se for um player
        {
            Attack_DoDamage(other.gameObject); //tentar enviar dano
            StartCoroutine(WaitSeconds());
        }
    }

    public void Attack_DoDamage(GameObject other) //tenta enviar dano causado
    {
        other.SendMessageUpwards("ReceiveDamage", damage/*, SendMessageOptions.DontRequireReceiver*/); //tenta enviar dano //nao requisitar msg de retorno
    }

    public IEnumerator WaitSeconds() //pra esperar um tempo e voltar a andar entre os pontos
    {
        isWaiting = true;

        yield return new WaitForSeconds(timePerDamage); //apos se movimentar, ficar um tempo parado

        isWaiting = false;
    }
}
