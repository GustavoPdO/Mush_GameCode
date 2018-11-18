using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class HomingBulletController : MonoBehaviour
{

    public float bulletSpeed;
    public Transform enemy;

    private Rigidbody2D bulletRB;
    private float angle;

    void Awake()
    {
        bulletRB = GetComponent<Rigidbody2D>();
    }

    // Use this for initialization
    void Start()
    {
        bulletRB.transform.eulerAngles = new Vector3(0, 0, getAngle(this.gameObject.transform.position, enemy.position));
        angle = getAngle(this.gameObject.transform.position, enemy.position);
    }

    // Update is called once per frame
    void Update()
    {

        //bulletRB.velocity = Vector2.right * bulletSpeed;
        //bulletRB.AddRelativeForce(Vector2.right * bulletSpeed, ForceMode2D.Impulse); //inicializar velocidade da bala
        bulletRB.velocity = (Quaternion.Euler(0, 0, angle) * Vector3.right) * bulletSpeed;

    }

    private float getAngle(Vector2 v1, Vector2 v2)
    {
        float x = v2.x - v1.x;
        float y = v2.y - v1.y;
        float angle = Mathf.Atan2(y, x);
        return angle * Mathf.Rad2Deg;
    }
}
