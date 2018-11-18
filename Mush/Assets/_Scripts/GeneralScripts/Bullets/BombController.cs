using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombController : MonoBehaviour {

    private Rigidbody2D bombRB;
    public JorgeSK2 skill;
    public GameObject enemy;

	// Use this for initialization
	void Start () {

        bombRB = GetComponent<Rigidbody2D>();
		
	}
    
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(enemy.tag) && other.tag.Contains("Player"))
        {
            skill.Detonate();
        }

    }
}
