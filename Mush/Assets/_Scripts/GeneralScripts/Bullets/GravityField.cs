using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityField : MonoBehaviour
{

    [SerializeField]
    private Rigidbody2D target;
    private CircleCollider2D circleCollider;
    public JorgeSK2 skill;

    public float fieldSize;
    public float fieldForce;
    public int fieldDamage;

    public GameObject enemy;
    private HealthController enemyHealth;

    public float timer = 0;

    public GameObject fieldEffect;

    private GameObject particle;

    // Use this for initialization
    void Start()
    {

        circleCollider = GetComponent<CircleCollider2D>();
        transform.localScale *= new Vector2(fieldSize, fieldSize);
        target = enemy.GetComponent<Rigidbody2D>();
        enemyHealth = enemy.GetComponent<HealthController>();
        particle = Instantiate(fieldEffect, this.transform.position, Quaternion.identity);

    }

    private void Update()
    {
        skill.skillManager.skill2CD = 0;
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag(enemy.tag) && other.tag.Contains("Player") || other.CompareTag("Ground"))
        {
            Vector3 gravityField = enemy.transform.position - transform.position;
            float index = (circleCollider.radius - gravityField.magnitude) / circleCollider.radius;
            target.AddForce(fieldForce * 2 * gravityField * index);

            if (timer <= 1)
            {
                timer += Time.deltaTime;
            }
            else
            {
                enemyHealth.ReceiveDamage(fieldDamage);
                timer = 0;
            }
        }
    }

    private void OnDestroy()
    {
        skill.skillManager.skill2CD = 0;
        skill.skillManager.restrictAll = false;
        Destroy(particle);
    }
}
