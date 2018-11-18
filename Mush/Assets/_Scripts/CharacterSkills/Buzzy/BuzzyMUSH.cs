using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuzzyMUSH : MonoBehaviour
{

    public SkillManager skillManager;
    public AttackController_Ranged attackController;

    [Header("Definições Gerais")]
    [SerializeField]
    private int stanceShots = 6;
    [SerializeField]
    private int bulletDamage = 40;
    [SerializeField]
    private float shotSpeed = 120;
    [SerializeField]
    private float shotInterval = 0.3f;
    [SerializeField]
    [Range(0, 1)]
    private float recoveryTime = 0.5f;

    [SerializeField]
    private GameObject shotPrefab;
    [SerializeField]
    private Transform shotSpawn;

    private void Awake()
    {
        skillManager = GetComponentInParent<SkillManager>();
        attackController = skillManager.GetComponentInChildren<AttackController_Ranged>();
    }

    // Use this for initialization
    void Start()
    {
        skillManager.mushCD = skillManager.character.mushCooldown;
    }

    // Update is called once per frame
    void Update()
    {

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
                StartCoroutine(JusticeRain());
            }

        }

    }

    private void Shot()
    {
        //skillManager.charAnim.Play("Attack1");
        GameObject shot = Instantiate(shotPrefab, shotSpawn.position, Quaternion.identity);
        shot.layer = LayerMask.NameToLayer("Damage" + skillManager.playerNumber);

        HomingBulletController buzzyShot = shot.GetComponent<HomingBulletController>();
        BulletHealthController bulletHealthController = shot.GetComponent<BulletHealthController>();

        buzzyShot.enemy = skillManager.enemy.transform;
        buzzyShot.bulletSpeed = shotSpeed;
        bulletHealthController.bulletDamage = bulletDamage;
        bulletHealthController.bulletLifeTime = 5f;
        bulletHealthController.healthController = skillManager.healthController;

    }

    private IEnumerator JusticeRain()
    {

        skillManager.charRB.velocity = new Vector2(0, 0);
        skillManager.charRB.AddForce(new Vector2(0, 1000), ForceMode2D.Force);

        yield return new WaitForSecondsRealtime(.5f);
        skillManager.charRB.constraints = RigidbodyConstraints2D.FreezePosition;
        skillManager.restrictAll = true;
        attackController.canAttack = false;
        skillManager.playerController.canMove = false;

        if (skillManager.enemy.transform.position.x > skillManager.charRB.transform.position.x && !skillManager.playerController.isFacingRight)
        {
            this.gameObject.SendMessageUpwards("Flip", new Vector3(0, 0, 0));
        }

        if (skillManager.enemy.transform.position.x < skillManager.charRB.transform.position.x && skillManager.playerController.isFacingRight)
        {
            this.gameObject.SendMessageUpwards("Flip", new Vector3(0, 180, 0));
        }

        for (int i = 0; i < stanceShots; i++)
        {
            Shot();
            yield return new WaitForSecondsRealtime(shotInterval);
        }

        yield return new WaitForSecondsRealtime(recoveryTime);

        skillManager.charRB.constraints = RigidbodyConstraints2D.FreezeRotation;
        skillManager.restrictAll = false;
        attackController.canAttack = true;
        skillManager.playerController.canMove = true;
        skillManager.mushCD = 0;


    }


}
