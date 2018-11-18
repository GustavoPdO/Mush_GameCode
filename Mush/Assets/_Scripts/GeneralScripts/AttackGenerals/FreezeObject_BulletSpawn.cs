using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FreezeObject_BulletSpawn : MonoBehaviour
{
    //public

    //private
    private Transform player; //referencia ao transform do player

    private Vector3 newPosition; //vector3 com a posicao que sera atribuida ao objeto
    private Quaternion newRotation; //vector3 com a rotacao que sera atribuida ao objeto

    // Use this for initialization
    void Start()
    {
        player = gameObject.transform.root; //pegar referencia ao player/root
        //newPosition = gameObject.transform.position - player.position; //diferenca de distancia do player //inicializar valores
        //newRotation = Quaternion.Euler(new Vector3(0, 0, 0));
    }

    // Update is called once per frame
    void Update()
    {
        //gameObject.transform.SetPositionAndRotation(player.position + newLocalPosition, newLocalRotation); //setar/manter a posicao e a rotacao do objeto //(manter a posicao com relacao ao player)
    }

    //executar no final do frame (apos todos os updates)
    private void LateUpdate()
    {
        Vector3.Scale(gameObject.transform.position, new Vector3(1, 0, 1)); //multiplicacao posicao por posicao do vetor //(zerar movimentacao em y)
        gameObject.transform.rotation = player.rotation; //manter rotacao do bulletSpawn como a do player //(para apenas virar/flipar junto)
    }
}
