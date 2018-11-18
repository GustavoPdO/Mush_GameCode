using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TargetController : MonoBehaviour
{

    public SkillManager skillManager;
    public AttackController_Ranged attackController;
    public JorgeSK2 skill;
    [HideInInspector]
    public float targetSpeed = 0.6f;

    public Slider timer;
    [SerializeField]
    public Transform targetTR;

    public LayerMask layerMask;

    // Use this for initialization
    void Start()
    {

        targetTR = GetComponent<Transform>();
        timer = GetComponentInChildren<Slider>();
    }

    // Update is called once per frame
    void Update()
    {

        skillManager.playerController.canMove = false;
        skillManager.restrictAll = true;
        attackController.canAttack = false;
        skillManager.skill2CD = 0;

        timer.value -= Time.deltaTime;
        targetTR.Translate(0, Input.GetAxis("Horizontal" + skillManager.playerNumber) * targetSpeed, 0);

        if (Input.GetButtonDown("JumpDown" + skillManager.playerNumber))
        {
            RaycastHit2D hit = Physics2D.Raycast(targetTR.position - new Vector3(0, 3f, 0), Vector2.down, Mathf.Infinity, 1 << LayerMask.NameToLayer("Ground") | 1 << LayerMask.NameToLayer("GroundNotPlataform"));
            if (hit.transform != null)
            {
                targetTR.position = new Vector3(targetTR.position.x, hit.transform.position.y + 1.7f, targetTR.position.z);
            }
        }
        if (Input.GetButtonDown("Jump" + skillManager.playerNumber))
        {
            RaycastHit2D hit = Physics2D.Raycast(targetTR.position, Vector2.up, Mathf.Infinity, LayerMask.GetMask("Ground") | 1 << LayerMask.NameToLayer("GroundNotPlataform"));
            if (hit.transform != null)
            {
                targetTR.position = new Vector3(targetTR.position.x, hit.transform.position.y + 1.7f, targetTR.position.z);
            }
        }

        if (timer.value <= 0 || Input.GetButtonDown("Offense" + skillManager.playerNumber))
        {
            skill.bombTarget = targetTR.position;
            skillManager.playerController.canMove = true;
            skillManager.restrictAll = false;
            attackController.canAttack = true;
            skill.SendMessageUpwards("BombDrop");
            Destroy(this.gameObject);
        }


        // targetTR.localPosition = new Vector3(0, Input.GetButtonDown("Vertical" + skillManager.playerNumber), 0);

    }
}
