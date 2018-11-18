using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CounterField : MonoBehaviour {

    private CircleCollider2D areaCollider;

    public int fieldScale;
    public float fieldSpeed;
    public int fieldDamage;

    public GameObject enemy;
    private HealthController enemyHealth;


	// Use this for initialization
	void Start () {

        areaCollider = GetComponent<CircleCollider2D>();
        enemyHealth = enemy.GetComponent<HealthController>();
		
	}

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(enemy.tag) && other.tag.Contains("Player"))
        {
            enemyHealth.ReceiveDamage(fieldDamage);
            Destroy(this.gameObject);
        }
    }

    // Update is called once per frame
    void Update () {

        if(this.transform.localScale.x <= fieldScale)
        {
            this.transform.localScale += new Vector3(fieldSpeed, fieldSpeed);
        }
        else
        {
            Destroy(this.gameObject);
        }
		
	}
}
