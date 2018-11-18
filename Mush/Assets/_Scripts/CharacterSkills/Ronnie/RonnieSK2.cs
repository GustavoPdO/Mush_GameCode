using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RonnieSK2 : MonoBehaviour {

    public SkillManager skillManager;
    public BoxCollider2D skillAttack;
    public AttackController_Melee attackController;

    [Header("Definições Gerais")]
    [SerializeField]
    private int damage = 20;
    [SerializeField]
    private int jumpDistance = 1600;
    [SerializeField]
    private float jumpHeight = 350f;
    [SerializeField]
    [Range(0, 10)]
    private float jumpTime = 0.35f;
    [SerializeField]
    [Tooltip("Se torna invulneravel?")]
    private bool invulnerability;

    private void Awake()
    {
        skillManager = GetComponentInParent<SkillManager>();
        skillAttack = skillManager.GetComponentInChildren<BoxCollider2D>(true);
        skillManager.charAnim = GetComponentInParent<Animator>();
        attackController = skillManager.GetComponentInChildren<AttackController_Melee>();
    }

    // Use this for initialization
    void Start ()
    {
        attackController.skillDamage = damage;
        skillManager.skill2CD = skillManager.character.skill2Cooldown;
    }
	
	// Update is called once per frame
	void Update () {

        if (skillManager.healthController.actualNumberOfLives < 3)
        {

            if (skillManager.skill2CD < skillManager.character.skill2Cooldown)
            {
                skillManager.skill2Usable = false;
                skillManager.skill2CD += Time.deltaTime;
            }
            else if (!skillManager.restrictAll)
            {
                skillManager.skill2Usable = true;
            }

            if (Input.GetButtonDown("Offense" + skillManager.playerNumber) && skillManager.skill2Usable)
            {
                StartCoroutine(Dash());
                skillManager.skill2CD = 0;
            }
        }
    }

    

    public IEnumerator Dash() //pular para cima (player)
    {
        skillManager.restrictAll = true;
        skillManager.skill2CD = 0;

        attackController.StartAttack();
        attackController.canAttack = false;
        skillManager.playerController.enabled = false;

        skillManager.charAnim.SetInteger("SkillNumber", 2);
        Vector2 jumpControl = skillManager.charRB.transform.position;
        skillManager.charRB.velocity = new Vector2(0,0);
        if(skillManager.playerController.isFacingRight)
        {
            skillManager.charRB.AddForce(new Vector2(jumpDistance, jumpHeight), ForceMode2D.Force);
        }
        else
        {
            skillManager.charRB.AddForce(new Vector2(-jumpDistance, jumpHeight), ForceMode2D.Force);
        }
        

        yield return new WaitForSecondsRealtime(jumpTime);
        if (jumpControl.x <= skillManager.charRB.transform.position.x)
        {
            skillManager.charRB.constraints = RigidbodyConstraints2D.FreezePosition;
        }

        skillManager.charRB.constraints = RigidbodyConstraints2D.FreezeRotation;
        skillManager.playerController.enabled = true;
        attackController.canAttack = true;

        yield return new WaitForSecondsRealtime(0.5f);
        
        attackController.EndAttack();
        skillManager.restrictAll = false;
    }

}
