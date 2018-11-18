using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarDamageController : MonoBehaviour
{
    //public
    public int damage; //dano causado

    public float impactForce; //forca da colisao 

    //private
    private GameEndManager gameEndManager; //script/objeto gameEndManaged

    // Use this for initialization
    void Start()
    {
        gameEndManager = GameObject.FindObjectOfType<GameEndManager>(); //inicializar valores
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!gameEndManager.isGameEnd && other.gameObject.tag.Contains("Player")) //se for um player
        {
            Attack_DoDamage(other.gameObject); //tentar enviar dano

            //Vector3 direction = (transform.position - other.transform.position).normalized;

            other.GetComponent<Rigidbody2D>().AddForce(Vector2.up * impactForce, ForceMode2D.Impulse); //(para outras direcoes que nao sao na vertical nao funciona por causa do playerController)
        }
    }

    public void Attack_DoDamage(GameObject other) //tenta enviar dano causado
    {
        other.SendMessageUpwards("ReceiveDamage", damage/*, SendMessageOptions.DontRequireReceiver*/); //tenta enviar dano //nao requisitar msg de retorno
    }
}
