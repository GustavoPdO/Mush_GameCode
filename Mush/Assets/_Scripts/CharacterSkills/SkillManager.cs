using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillManager : MonoBehaviour {

    public GameObject enemy;
    public HealthController healthController;
    public PlayerController playerController;
    public Rigidbody2D charRB;
    public Animator charAnim;

    public CharacterScriptableObject character;

    public bool skill1Usable;
    public bool skill2Usable;
    public bool mushUsable;
    public bool restrictAll;

    public float skill1CD;
    public float skill2CD;
    public float mushCD;
    public int playerNumber;

    // Use this for initialization
    void Start () {

        playerController = GetComponent<PlayerController>();
        healthController = GetComponent<HealthController>();
        charRB = GetComponent<Rigidbody2D>();
        charAnim = GetComponent<Animator>();

        if (gameObject.CompareTag("Player1")) playerNumber = 1; //encontrar numero do player
        else /*if(gameObject.CompareTag("Player2"))*/ playerNumber = 2;

        if (playerNumber == 1)
        {
            enemy = GameObject.FindGameObjectWithTag("Player2").GetComponentInParent<PlayerController>().gameObject;
        }
        else
        {
            enemy = GameObject.FindGameObjectWithTag("Player1").GetComponentInParent<PlayerController>().gameObject;
        }

    }
	
	// Update is called once per frame
	void Update () {

        if (restrictAll)
        {
            skill1Usable = false;
            skill2Usable = false;
            mushUsable = false;
        }
    }
}
