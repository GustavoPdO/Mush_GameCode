using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RonnieMUSH : MonoBehaviour {

    public SkillManager skillManager;
    public AttackController_Melee attackController;
    public ParticleSystem swordLight;

    [Header("Definições Gerais")]
    [SerializeField]
    private float skillDuration = 5;

    [SerializeField]
    [Tooltip("Quanto aumenta a velocidade de movimento?")]
    [Range(1, 2)]
    private float moveSpeedBuff = 1.5f;
    [SerializeField]
    [Tooltip("Quanto aumenta a defesa?")]
    [Range(1, 2)]
    private float defenseBuff = 1.5f;
    [SerializeField]
    [Tooltip("Quanto aumenta o dano de ataque?")]
    [Range(1, 2)]
    private float[] attackBuff = { 1.3f, 1.3f, 1.3f, 1.3f };

    //variaveis de controle interno
    public float originalMovementSpeed;
    public int originalDefense;
    public int[] originalAttackDamage;

    private bool auraOn = false;

    void Awake()
    {
        skillManager = GetComponentInParent<SkillManager>();
        attackController = skillManager.GetComponentInChildren<AttackController_Melee>();
        originalAttackDamage = new int[skillManager.character.maxQtdAttacks];
        swordLight = attackController.GetComponentInChildren<ParticleSystem>();
    }

    // Use this for initialization
    void Start ()
    {

        skillManager.mushCD = skillManager.character.mushCooldown;

        originalMovementSpeed = skillManager.character.movementSpeed;
        originalDefense = skillManager.character.defense;
        for(int i = 0; i < attackController.maxQtdAttacks; i++)
        {
            originalAttackDamage[i] = attackController.attackDamage[i];
        }
	}
	
	// Update is called once per frame
	void Update ()
    {
		if(skillManager.healthController.actualNumberOfLives < 2)
        {
            if (skillManager.mushCD < skillManager.character.mushCooldown)
            {
                skillManager.mushUsable = false;
                skillManager.mushCD += Time.deltaTime;
            }
            else if (!skillManager.restrictAll && auraOn == false)
            {
                skillManager.mushUsable = true;
            }

            if(Input.GetButtonDown("Mush" + skillManager.playerNumber) && skillManager.mushUsable)
            {
                StartCoroutine(Overdrive());
            }
        }


	}


    private IEnumerator Overdrive()
    {

        Debug.Log("OVERDRIVE ON");
        auraOn = true;
        skillManager.mushUsable = false;
        swordLight.gameObject.SetActive(true);
        swordLight.Play();
        skillManager.playerController.movementSpeed = originalMovementSpeed * (1 + moveSpeedBuff);
        skillManager.healthController.actualDefense = (int)(originalDefense * (1 + defenseBuff));
        for (int i = 0; i < attackController.maxQtdAttacks; i++)
        {
            attackController.attackDamage[i] = (int)(originalAttackDamage[i] * (1 + attackBuff[i]));
        }

        yield return new WaitForSecondsRealtime(skillDuration);
        Debug.Log("OVERDRIVE OFF");

        swordLight.Stop();
        swordLight.gameObject.SetActive(false);
        skillManager.playerController.movementSpeed = originalMovementSpeed;
        skillManager.healthController.actualDefense = originalDefense;

        skillManager.mushCD = 0;
        auraOn = false;
        
        for (int i = 0; i < attackController.maxQtdAttacks; i++)
        {
            attackController.attackDamage[i] = originalAttackDamage[i];
        }




    }

    
}
