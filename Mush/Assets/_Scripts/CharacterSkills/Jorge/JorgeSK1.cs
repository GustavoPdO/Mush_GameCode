using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JorgeSK1 : MonoBehaviour {

    public SkillManager skillManager;
    public AttackController_Ranged attackController;

    [Header("Definições Gerais")]
    [SerializeField]
    private int jumpDistance = 1600;
    [SerializeField]
    private float jumpHeight = 800f;
    [SerializeField]
    [Range(0, 10)]
    private float jumpTime = 0.35f;
    /*
    [SerializeField]
    [Tooltip("Bloqueia ataques?")]
    private bool blockAttacks;
    [SerializeField]
    [Tooltip("Bloqueia habilidades?")]
    private bool blockSkills;
    [SerializeField]
    [Tooltip("Se torna invulneravel?")]
    private bool invulnerability;
    */

    [Space(5)]
    [Header("Definições da Granada")]
    [SerializeField]
    private int stunDuration = 3;
    [SerializeField]
    private int horizontalThrow = 15;
    [SerializeField]
    private int verticalThrow = 5;
    [SerializeField]
    private GameObject grenadePrefab;
    [SerializeField]
    private Transform grenadeSpawn;

    private void Awake()
    {
        skillManager = GetComponentInParent<SkillManager>();
        attackController = skillManager.GetComponentInChildren<AttackController_Ranged>();
    }
    // Use this for initialization
    void Start () {

        skillManager.skill1CD = skillManager.character.skill1Cooldown;	
	}
	
	// Update is called once per frame
	void Update () {

        if (skillManager.healthController.actualNumberOfLives < 4)
        {

            if (skillManager.skill1CD < skillManager.character.skill1Cooldown)
            {
                skillManager.skill1Usable = false;
                skillManager.skill1CD += Time.deltaTime;
            }
            else if (!skillManager.restrictAll)
            {
                skillManager.skill1Usable = true;
            }


            if (Input.GetButtonDown("Defense" + skillManager.playerNumber) && skillManager.skill1Usable)
            {
                Debug.Log("foi");
                StartCoroutine(BackJump());
                skillManager.skill1CD = 0;
            }
        }

    }

    private void Throw(int direction)
    {
        GameObject grenade = Instantiate(grenadePrefab, grenadeSpawn.position, grenadeSpawn.rotation);
        grenade.layer = LayerMask.NameToLayer("Damage" + skillManager.playerNumber);
        GrenadeController jorgeGrenade = grenade.GetComponent<GrenadeController>();

        jorgeGrenade.horizontal = horizontalThrow * direction;
        jorgeGrenade.vertical = verticalThrow;
        jorgeGrenade.enemy = skillManager.enemy;
        jorgeGrenade.stunDuration = stunDuration;
    }

    public IEnumerator BackJump() //pular para cima (player)
    {
        skillManager.restrictAll = true;
        skillManager.skill1CD = 0;

        attackController.canAttack = false;
        skillManager.playerController.enabled = false;

        skillManager.charAnim.SetInteger("SkillNumber", 1);
        Vector2 jumpControl = skillManager.charRB.transform.position;

        skillManager.charRB.velocity = new Vector2(0, 0);
        if (skillManager.playerController.isFacingRight)
        {
            skillManager.charRB.AddForce(new Vector2(-jumpDistance, jumpHeight), ForceMode2D.Force);
            Throw(1);
        }
        else
        {
            skillManager.charRB.AddForce(new Vector2(jumpDistance, jumpHeight), ForceMode2D.Force);
            Throw(-1);
        }

        yield return new WaitForSecondsRealtime(jumpTime);
        if (jumpControl.x <= skillManager.charRB.transform.position.x)
        {
            skillManager.charRB.constraints = RigidbodyConstraints2D.FreezePosition;
        }

        skillManager.charRB.constraints = RigidbodyConstraints2D.FreezeRotation;

        yield return new WaitForSecondsRealtime(.5f);
        skillManager.playerController.enabled = true;
        attackController.canAttack = true;
        yield return new WaitForSecondsRealtime(.5f);

        skillManager.restrictAll = false;
    }


}
