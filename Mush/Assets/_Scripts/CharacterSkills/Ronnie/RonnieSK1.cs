using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RonnieSK1 : MonoBehaviour
{

    //Variaveis de controle interno
    public SkillManager skillManager;
    public Slider pressCount;
    public AttackController_Melee attackController;

    public bool isCharging = false;

    [Header("Definições Gerais")]

    [SerializeField]
    private int chargeTime = 2;

    [SerializeField]
    [Range(0, 1)]
    [Tooltip("Tempo para usar as demais habilidades")]
    private float globalCooldown;

    [Range(.1f, 2)]
    [SerializeField]
    [Tooltip("Multiplicador para o dano devolvido")]
    private float returnDamage; //Ainda não implementado
    [HideInInspector]
    public float timer = 0.0f;
    /*
    [SerializeField]
    [Tooltip("É possível acumular o dano recebido?")]
    private bool StackDamage;
    */

    /*
    [SerializeField]
    [Tooltip("O dano diminui com o tempo de recarga?")]
    private bool ReverseDamage;
    */

    /*
    [SerializeField]
    [Tooltip("É possível usar a skill no ar?")]
    private bool AllowInJumping;
    */

    private enum CounterType
    {
        Shot,
        CircleArea
    };

    [Space(10)]
    [SerializeField]
    private CounterType counterType;

    [Space(5)]
    [Header("Definições do Disparo")]
    [SerializeField]
    private float shotSpeed = 2200f;

    [SerializeField]
    private GameObject ShotPrefab;
    [SerializeField]
    private Transform ShotSpawn;

    [Space(5)]
    [Header("Definições do Campo")]
    [SerializeField]
    [Range(0.2f, 1)]
    private float fieldSpeed = 0.5f;
    [SerializeField]
    private int fieldScale = 10;

    [SerializeField]
    private GameObject fieldPrefab;
    [SerializeField]
    private Transform fieldSpawn;

    private void Awake()
    {
        skillManager = GetComponentInParent<SkillManager>();
        attackController = GameObject.Find("Bones/Hip/Torso/R Upper Arm/R Arm/R Hand/Sword").GetComponent<AttackController_Melee>();

        skillManager.charAnim = GetComponentInParent<Animator>();
        pressCount = skillManager.GetComponentInChildren<Slider>();
    }
    // Use this for initialization
    void Start()
    {

        skillManager.skill1CD = skillManager.character.skill1Cooldown;
        pressCount.GetComponent<CanvasGroup>().alpha = 0;
        pressCount.maxValue = chargeTime;

    }

    // Update is called once per frame
    void Update()
    {

        if (skillManager.healthController.actualNumberOfLives < 4)
        {
            //controle do character
            if (skillManager.skill1CD < skillManager.character.skill1Cooldown)
            {
                skillManager.skill1Usable = false;
                skillManager.skill1CD += Time.deltaTime;
            }
            else if (!skillManager.restrictAll)
            {
                skillManager.skill1Usable = true;
            }

            //ao pressionar o botao reseta o contador de dano
            if (Input.GetButtonDown("Defense" + skillManager.playerNumber) && skillManager.skill1Usable)
            {
                isCharging = true;
                skillManager.healthController.damageReceived = 0;
            }

            //enquanto o botao estiver pressionado
            if (Input.GetButton("Defense" + skillManager.playerNumber) && isCharging)
            {

                skillManager.charAnim.SetInteger("SkillNumber", 1);
                skillManager.charAnim.SetBool("Parry", true);
                chargeAttack();

                if (timer >= 2f)
                {
                    skillManager.charAnim.SetBool("Parry", false);
                    skillManager.charAnim.SetInteger("SkillNumber", 0);
                    StartCoroutine(counterAttack());
                }

            }

            //ao soltar o botao
            if (Input.GetButtonUp("Defense" + skillManager.playerNumber) && isCharging)
            {
                skillManager.charAnim.SetBool("Parry", false);
                skillManager.charAnim.SetInteger("SkillNumber", 0);
                StartCoroutine(counterAttack());
            }else

            if(!Input.GetButton("Defense" + skillManager.playerNumber) && isCharging)
            {
                skillManager.charAnim.SetBool("Parry", false);
                skillManager.charAnim.SetInteger("SkillNumber", 0);
                StartCoroutine(counterAttack());
            }
        }
    }

    //enquanto estiver pressionando o botao
    void chargeAttack()
    {
        isCharging = true;
        skillManager.healthController.canReceiveDamage = false;

        skillManager.restrictAll = true; // restringe o uso de todas as skills

        skillManager.playerController.canMove = false;
        attackController.canAttack = false;
        pressCount.GetComponent<CanvasGroup>().alpha = 1;
        timer += Time.deltaTime;
        pressCount.value = timer;

        if (skillManager.enemy.transform.position.x > skillManager.charRB.transform.position.x && !skillManager.playerController.isFacingRight)
        {
            this.gameObject.SendMessageUpwards("Flip", new Vector3(0, 0, 0));
        }

        if (skillManager.enemy.transform.position.x < skillManager.charRB.transform.position.x && skillManager.playerController.isFacingRight)
        {
            this.gameObject.SendMessageUpwards("Flip", new Vector3(0, 180, 0));
        }
    }

    void Shot(int zRotation)
    {
        GameObject shot = Instantiate(ShotPrefab, ShotSpawn.position, new Quaternion(0, 0, zRotation, 0));
        shot.layer = LayerMask.NameToLayer("Damage" + skillManager.playerNumber); //ajustar layer da bala

        HomingBulletController ronnieShot = shot.GetComponent<HomingBulletController>();
        BulletHealthController bulletHealthController = shot.GetComponent<BulletHealthController>(); //script bulletHealthController

        ronnieShot.enemy = skillManager.enemy.transform;
        ronnieShot.bulletSpeed = shotSpeed;
        bulletHealthController.bulletDamage = (int)(skillManager.healthController.damageReceived * returnDamage);
        bulletHealthController.bulletLifeTime = 20.0f;
        bulletHealthController.healthController = skillManager.healthController;
    }

    void CounterField()
    {
        GameObject field = Instantiate(fieldPrefab, fieldSpawn.position, new Quaternion(0, 0, 0, 0));
        CounterField ronnieField = field.GetComponent<CounterField>();

        ronnieField.enemy = skillManager.enemy;
        ronnieField.fieldSpeed = fieldSpeed;
        ronnieField.fieldScale = fieldScale;
        ronnieField.fieldDamage = (int)(skillManager.healthController.damageReceived * returnDamage);
    }

    private IEnumerator counterAttack()
    {

        isCharging = false;
        pressCount.GetComponent<CanvasGroup>().alpha = 0;

        skillManager.playerController.canMove = true;
        attackController.canAttack = true;
        skillManager.healthController.canReceiveDamage = true;

        //reseta os contadores
        timer = 0;
        pressCount.value = timer;

        if (counterType == CounterType.Shot)
        {
            //virar o personagem caso necessário
            if (skillManager.playerController.isFacingRight)
            {
                Shot(0);
            }
            else
            {
                Shot(180);
            }
        }

        if (counterType == CounterType.CircleArea)
        {
            CounterField();
        }


        yield return new WaitForSecondsRealtime(globalCooldown);
        skillManager.restrictAll = false; //libera o uso da skills
        skillManager.skill1CD = 0;

    }


}