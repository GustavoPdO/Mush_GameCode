using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DroneController : MonoBehaviour {

    public SkillManager skillManager;
    public BuzzySK2 skill;
    public Rigidbody2D droneRB;
    public Slider healthBar;
    public float droneHeight;

    public int droneLife;
    public int droneDuration;
    public float shotInterval;
    public int shotDamage;
    public int shotSpeed;

    public GameObject bulletPrefab;
    public Transform bulletSpawn;

    public float droneAngle;
    public float timer;
    private float lifeTimer;

    private void Awake()
    {
        droneRB = GetComponent<Rigidbody2D>();
        healthBar = GetComponentInChildren<Slider>();      

    }

    // Use this for initialization
    void Start () {

        droneHeight = this.transform.position.y;
        droneRB.velocity = new Vector2(0, .5f);
        timer = shotInterval;
        healthBar.maxValue = droneDuration;
        lifeTimer = droneDuration;
        
    }
	
	// Update is called once per frame
	void Update () {

        healthBar.value = lifeTimer;
        lifeTimer -= Time.deltaTime;

        Float();

        if(timer <= shotInterval)
        {
            timer += Time.deltaTime;
        }
        else
        {
            Shoot();
            timer = 0;
        }
		
	}

    private void OnDestroy()
    {
        skill.isDroneAlive = false;
    }

    private void Float()
    {
        if (this.transform.position.y >= (droneHeight + .2f))
        {
            droneRB.velocity = new Vector2(0, -.5f);
        }
        if (this.transform.position.y <= droneHeight)
        {
            droneRB.velocity = new Vector2(0, .5f);
        }
    }

    private void Shoot()
    {
        GameObject bullet = Instantiate(bulletPrefab, bulletSpawn.position, bulletSpawn.rotation);
        bullet.layer = LayerMask.NameToLayer("Damage" + skillManager.playerNumber);

        BulletController bulletController = bullet.GetComponent<BulletController>();
        BulletHealthController bulletHealth = bullet.GetComponent<BulletHealthController>();

        bulletController.bulletSpeed = shotSpeed;
        bulletHealth.bulletLifeTime = 5f;
        bulletHealth.healthController = skillManager.healthController;
    }
}
