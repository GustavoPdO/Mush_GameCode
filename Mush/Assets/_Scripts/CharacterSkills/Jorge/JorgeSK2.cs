using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JorgeSK2 : MonoBehaviour {

    public SkillManager skillManager;
    public AttackController_Ranged attackController;
    public Vector2 bombTarget;
    private GameObject bomb;

    [Header("Definições do Alvo")]
    [SerializeField]
    private float targetTime = 2;
    [SerializeField]
    private GameObject targetPrefab;
    [SerializeField]
    [Range(0,1)]
    private float targetSpeed;

    [Space(5)]
    [Header("Definições da Bomba")]
    [SerializeField]
    private GameObject bombPrefab;
    [SerializeField]
    private Transform bombSpawn;

    [Space(5)]
    [Header("Definições do Campo")]
    [SerializeField]
    private GameObject fieldPrefab;
    [SerializeField]
    private int fieldDPS = 2;
    [SerializeField]
    [Range(0.1f, 2f)]
    private float fieldSize = 1f;
    [SerializeField]
    [Range(1, 3)]
    private float fieldForce = 2.5f;
    [SerializeField]
    private float fieldDuration;


    private void Awake()
    {
        skillManager = GetComponentInParent<SkillManager>();
        attackController = skillManager.GetComponentInChildren<AttackController_Ranged>();
    }
    // Use this for initialization
    void Start () {

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
                StartCoroutine(Aim());
            }

            if (bomb != null && bomb.transform.position.y <= bombTarget.y)
            {
                Detonate();
            }
        }
    }

    public IEnumerator Aim()
    {
        RaycastHit2D hit = Physics2D.Raycast(skillManager.enemy.transform.position, Vector2.down, Mathf.Infinity, 1 << LayerMask.NameToLayer("Ground") | 1 << LayerMask.NameToLayer("GroundNotPlataform"));
        GameObject target = Instantiate(targetPrefab, new Vector2(skillManager.enemy.transform.position.x, hit.transform.position.y + 1.7f), targetPrefab.transform.rotation);
        target.layer = LayerMask.NameToLayer("Damage" + skillManager.playerNumber);
        TargetController jorgeTarget = target.GetComponent<TargetController>();

        jorgeTarget.skill = this;
        jorgeTarget.attackController = attackController;
        jorgeTarget.skillManager = skillManager;
        jorgeTarget.targetSpeed = targetSpeed;
        jorgeTarget.timer.value = targetTime;

        Debug.Log("foi");

        yield return new WaitForSecondsRealtime(targetTime);

    }

    private void BombDrop()
    {
        bomb = Instantiate(bombPrefab, new Vector2(bombTarget.x, 20), new Quaternion());
        BombController jorgeBomb = bomb.GetComponent<BombController>();

        jorgeBomb.skill = this;
        jorgeBomb.enemy = skillManager.enemy;
        
    }

    public void Detonate()
    {
        Destroy(bomb);
        GameObject gravityField = Instantiate(fieldPrefab, bomb.transform.position, new Quaternion());
        GravityField field = gravityField.GetComponent<GravityField>();

        field.skill = this;
        field.enemy = skillManager.enemy;
        field.fieldForce = fieldForce;
        field.fieldSize = fieldSize;
        field.fieldDamage = fieldDPS;
        Destroy(gravityField, fieldDuration);
    }
}
