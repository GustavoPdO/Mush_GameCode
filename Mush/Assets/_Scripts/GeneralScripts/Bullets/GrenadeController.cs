using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrenadeController : MonoBehaviour {

    [HideInInspector]
    public int horizontal;
    [HideInInspector]
    public int vertical;
    [HideInInspector]
    public int stunDuration;
    [SerializeField]
    private int grenadeLifeTime = 4;

    private Rigidbody2D grenadeRB;
    private SpriteRenderer sprite;

    

    public GameObject enemy;
    public AttackController attackController;
    public PlayerController playerController;
    public SkillManager skillManager;
    public GameObject stunEffect;

    private GameObject stunParticle;
    public Transform groundCheck;

    // Use this for initialization
    void Start () {

        grenadeRB = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();

        grenadeRB.AddForce(new Vector2(horizontal, vertical), ForceMode2D.Impulse);
        playerController = enemy.GetComponentInParent<PlayerController>();
        skillManager = enemy.GetComponent<SkillManager>();
        groundCheck = enemy.transform.Find("GroundCheck");

        attackController = enemy.GetComponentInChildren<AttackController_Melee>();
        if (!attackController)
        {
            attackController = enemy.GetComponentInChildren<AttackController_Ranged>();
        }         

    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if ((other.CompareTag(enemy.tag) && other.tag.Contains("Player")))
        {
            sprite.enabled = false;
            StartCoroutine(Stun());            
        }

        if(other.CompareTag("Ground") || other.CompareTag("GroundNotPlataform"))
        {
            sprite.enabled = false;
            Destroy(this.gameObject, grenadeLifeTime + stunDuration);
        }

    }


    private void Update()
    {
        if(stunParticle != null)
        {
            stunParticle.transform.position = new Vector3(stunParticle.transform.position.x, groundCheck.position.y);

            if(enemy.transform.position.x > stunParticle.transform.position.x + 1 || enemy.transform.position.x < stunParticle.transform.position.x - 1)
            {
                Destroy(stunParticle);
            }

        }
    }

    private IEnumerator Stun()
    {

        stunParticle = Instantiate(stunEffect, groundCheck.position + new Vector3(0, 1, 0), Quaternion.identity);

        playerController.GetComponent<Rigidbody2D>().velocity = new Vector2(0, 0);
        playerController.Move_Animating(new Vector3(0, 0, 0));
        playerController.enabled = false;   
        //attackController.canAttack = false;
        //skillManager.restrictAll = true; bugado, preciso compatibilizar
        yield return new WaitForSecondsRealtime(stunDuration);

        playerController.enabled = true;
        playerController.canMove = true;
        //attackController.canAttack = true;
        //skillManager.restrictAll = false;
        Debug.Log("Teste");
        Destroy(stunParticle);
        Destroy(this.gameObject, grenadeLifeTime);

    }



}
