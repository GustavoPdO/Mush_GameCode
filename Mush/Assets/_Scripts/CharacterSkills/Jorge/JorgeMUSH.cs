using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JorgeMUSH : MonoBehaviour {

    public SkillManager skillManager;
    public AttackController_Ranged attackController;

    [Header("Definições Gerais")]
    [SerializeField]
    [Tooltip("0 para disparo instantaneo")]
    private int stanceDuration = 0;

    [Header("Definições do Disparo")]
    [SerializeField]
    private int numberOfShots = 1;
    [SerializeField]
    private int shotDamage = 80;
    [SerializeField]
    private int shotSpeed = 100;
    [SerializeField]
    private int shotLifeTime = 4;
    
    public GameObject missilePrefab;
    [SerializeField]
    private Transform missileSpawn;

    void Awake()
    {
        skillManager = GetComponentInParent<SkillManager>();
        attackController = skillManager.GetComponentInChildren<AttackController_Ranged>();
    }

    // Use this for initialization
    void Start () {

        skillManager.mushCD = skillManager.character.mushCooldown;
	}
	
	// Update is called once per frame
	void Update () {

        if (skillManager.healthController.actualNumberOfLives < 2)
        {
            if (skillManager.mushCD < skillManager.character.mushCooldown)
            {
                skillManager.mushUsable = false;
                skillManager.mushCD += Time.deltaTime;
            }
            else if (!skillManager.restrictAll)
            {
                skillManager.mushUsable = true;
            }

            if (Input.GetButtonDown("Mush" + skillManager.playerNumber) && skillManager.mushUsable)
            {
                StartCoroutine(Bazooka());
            }
        }

    }

    void BazookaShot(int direction)
    {
        GameObject missile = Instantiate(missilePrefab, missileSpawn.position, missileSpawn.rotation);
        missile.layer = LayerMask.NameToLayer("Damage" + skillManager.playerNumber);
        BulletController jorgeMissile = missile.GetComponent<BulletController>();
        BulletHealthController missileDamage = missile.GetComponent<BulletHealthController>();

        //missile.GetComponent<SpriteRenderer>().transform.
        jorgeMissile.bulletSpeed = shotSpeed;
        missileDamage.healthController = skillManager.healthController;
        missileDamage.bulletDamage = shotDamage;
        missileDamage.bulletLifeTime = shotLifeTime;
    }

    private IEnumerator Bazooka()
    {
        skillManager.restrictAll = true;
        attackController.canAttack = false;
        skillManager.playerController.canMove = false;

        if(skillManager.playerController.isFacingRight)
        {
            BazookaShot(1);
        }
        else
        {
            BazookaShot(-1);
        }

        yield return new WaitForSecondsRealtime(stanceDuration);

        skillManager.restrictAll = false;
        attackController.canAttack = true;
        skillManager.playerController.canMove = true;
        skillManager.mushCD = 0;
    }
}
