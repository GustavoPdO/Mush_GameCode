using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuzzySK2 : MonoBehaviour
{

    public SkillManager skillManager;

    [Header("Definições Gerais")]
    [SerializeField]
    private int droneLife;
    [SerializeField]
    private int droneDuration;
    [SerializeField]
    private float shotInterval;
    [SerializeField]
    private int shotDamage;
    [SerializeField]
    private int shotSpeed;

    public GameObject drone;
    public Transform droneSpawn;

    public bool isDroneAlive = false;

    private void Awake()
    {
        skillManager = GetComponentInParent<SkillManager>();

    }

    // Use this for initialization
    void Start()
    {

        skillManager.skill2CD = skillManager.character.skill2Cooldown;

    }

    // Update is called once per frame
    void Update()
    {

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

            if (isDroneAlive)
            {
                skillManager.skill2Usable = false;
                skillManager.skill2CD = 0;
            }

            if (Input.GetButtonDown("Offense" + skillManager.playerNumber) && skillManager.skill2Usable && !isDroneAlive)
            {
                Deploy();
            }

        }

    }

    private void Deploy()
    {
        isDroneAlive = true;

        GameObject buzzyDrone = Instantiate(drone, droneSpawn.position, droneSpawn.rotation);
        buzzyDrone.layer = LayerMask.NameToLayer("Player" + skillManager.playerNumber);
        DroneController droneSpecs = buzzyDrone.GetComponent<DroneController>();

        droneSpecs.skill = this;
        droneSpecs.droneLife = droneLife;
        droneSpecs.droneDuration = droneDuration;
        droneSpecs.shotInterval = shotInterval;
        droneSpecs.shotDamage = shotDamage;
        droneSpecs.shotSpeed = shotSpeed;
        droneSpecs.skillManager = skillManager;
        Destroy(buzzyDrone, droneDuration);
    }




}
