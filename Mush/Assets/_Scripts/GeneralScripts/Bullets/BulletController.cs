using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletController : MonoBehaviour
{
    //public
    public float bulletSpeed; //velocidade da bala (default)

    //private
    private Rigidbody2D rb2d; //rigdbody2D

    //inicializar antes do start
    private void Awake()
    {
        rb2d = gameObject.GetComponent<Rigidbody2D>(); //inicializar valores 
    }

    // Use this for initialization
    void Start()
    {
        rb2d.velocity = gameObject.transform.right * bulletSpeed * Time.deltaTime; //inicializar velocidade da bala
    }

    // Update is called once per frame
    void Update()
    {

    }
}
