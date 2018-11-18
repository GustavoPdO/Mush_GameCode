using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuzzySK1 : MonoBehaviour
{

    public SkillManager skillManager;
    public Slider flightCount;

    [Header("Definições Gerais")]
    [SerializeField]
    private int flightDuration = 3;
    [SerializeField]
    private int launchDistance = 1800;
    [SerializeField]
    private int launchHorizontal = 600;
    [SerializeField]
    private int flightSpeed = 1250;
    [SerializeField]
    [Range(0, 1)]
    private float launchWait = 0.35f;

    public bool isFlying = false;
    public float flightTimer = 0;

    void Awake()
    {
        skillManager = GetComponentInParent<SkillManager>();
        flightCount = skillManager.GetComponentInChildren<Slider>();
        flightCount.maxValue = flightDuration;
    }

    // Use this for initialization
    void Start()
    {

        skillManager.skill1CD = skillManager.character.skill1Cooldown;
        flightCount.GetComponent<CanvasGroup>().alpha = 0;

    }

    // Update is called once per frame
    void Update()
    {
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

            if (Input.GetButtonDown("Defense" + skillManager.playerNumber) && skillManager.skill1Usable && !isFlying)
            {
                StartCoroutine(Launch());
            }

            if (flightTimer > 0)
            {
                flightTimer -= Time.deltaTime;
                flightCount.value = flightTimer;
                if (flightTimer <= 0)
                {
                    isFlying = false;
                    flightCount.GetComponent<CanvasGroup>().alpha = 0;
                    skillManager.skill1CD = 0;
                }
            }

            if (Input.GetButtonDown("Jump" + skillManager.playerNumber) && isFlying)
            {
                StartCoroutine(multipleJump());
            }
        }
    }

    private IEnumerator Launch()
    {
        isFlying = true;
        flightCount.GetComponent<CanvasGroup>().alpha = 1;
        flightTimer = flightDuration;
        skillManager.playerController.enabled = false;
        skillManager.skill1Usable = false;

        skillManager.charRB.velocity = new Vector2(0, 0);
        if (skillManager.playerController.isFacingRight)
        {
            skillManager.charRB.AddForce(new Vector2(-launchHorizontal, launchDistance), ForceMode2D.Force);
            //skillManager.charRB.AddForce(new Vector2(-launchHorizontal, launchDistance), ForceMode2D.Force);
            //skillManager.charRB.velocity = new Vector2(0, launchDistance);
            //skillManager.charRB.velocity = Vector2.right * (-launchHorizontal);
        }
        else
        {
            skillManager.charRB.AddForce(new Vector2(launchHorizontal, launchDistance), ForceMode2D.Force);
            //skillManager.charRB.velocity = new Vector2(0, launchDistance);
            //skillManager.charRB.velocity = Vector2.right * (launchHorizontal);
            //skillManager.charRB.AddForce(Vector2.up * launchDistance, ForceMode2D.Force);
           // skillManager.charRB.AddForce(Vector2.right * launchHorizontal, ForceMode2D.Force);

        }

        yield return new WaitForSecondsRealtime(launchWait);

        skillManager.playerController.enabled = true;

    }

    private IEnumerator multipleJump()
    {

        float currentJumpForce = skillManager.playerController.jumpForce;

        do
        {
            skillManager.charRB.AddForce(skillManager.playerController.jumpForce * skillManager.charRB.gravityScale * Vector3.up * Time.deltaTime, ForceMode2D.Impulse); //pular v2
            skillManager.charRB.velocity = new Vector2(skillManager.charRB.velocity.x, Mathf.Clamp(skillManager.charRB.velocity.y, float.NegativeInfinity, skillManager.playerController.maxJumpSpeed)); //limitar velocidade maxima de pulo (para evitar/limitar bugs de pulo atravessando plataformas seguidas , etc)

            currentJumpForce -= skillManager.playerController.jumpDecay; //diminuir forca com o tempo
            yield return null; //passar frame
        } while (Input.GetButton("Jump" + skillManager.playerNumber) && currentJumpForce > 0); //enquanto estiver segurando o pulo e a forca ainda nao zerou

    }

}
