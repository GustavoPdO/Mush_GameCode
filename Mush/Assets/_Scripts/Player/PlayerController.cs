using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{ //movimentar e pular
    //public
    public CharacterScriptableObject characterSO; //referencia para os dados do personagem
    public bool isFacingRight; //informar oricentacao atual do objeto no eixo x (virado ou nao para a direita)

    [HideInInspector]
    public bool canMove; //se pode ou nao se mover (pelo input)

    //scriptable public - private
    public float movementSpeed; //velocidade atual do player
    [Range(0, 1)] /*[Tooltip("Help Text")]*/
    public float moveJumpModifier; //modificador de velocidade enquanto pula [0, 1] (para reduzir a velocidade enquanto pula, etc)
    public float jumpForce; //forca de pulo do player
    public float jumpDecay; //decaimento da doca de pulo //vai indicar o quao alto eh possivel ir segurando para pular
    public float maxJumpSpeed; //velocidade maxima do pulo

    [Range(0, 1)]
    public float moveAttackModifier; //modificador de velocidade enquanto ataca [0, 1] (para reduzir a velocidade enquanto ataca, etc)

    [HideInInspector]
    public bool isAttacking; //se esta atacando

    //private
    private Rigidbody2D rb2d; //rigdbody2D
    private Collider2D col2d; //collider2D do objeto (generico)
    private Animator anim; //animator
    private HealthController healthController; //script HealthController

    private Vector2 playerPosition; //posicao atual do player
    //private Transform groundCheck; //objeto que vai servir de parametro para verificar se o player esta ou nao no chao
    private Transform[] groundCheck; //objetos que vao servir de parametro para verificar se o player esta ou nao no chao //(para aumentar a precisao do groundCheck e nao ficar deslizando na beirada das plataformas, etc.)
    private int groundCheckQuantity; //quantidade de objetos que vao servir de parametro para verificar se o player esta ou nao no chao

    private bool isGrounded; //se o player esta no chao
    private bool canJump; //se o player pode/vai pular (relacionado com o update)
    private bool canJumpOff; //se o player pode "pular"/vai para baixo (relacionado com o update) //(para descer plataformas)

    private bool isJumping; //para verificar se esta pulando (relacionado a coroutine Jump)
    private bool isJumpingOff; //para verificar se esta "pulando" para baixo (relacionado a coroutine JumpOff) //(para descer plataformas)

    private HashSet<PlatformEffector2D> allCollisions; //PlataformEffector2D de "todos" os objetos que estao em contato com este player (relacionado ao JumpOff e a "desabilitar" colisao com plataformas rapidamente)

    private int playerNumber; //numero do player relacionado a este script (para a selecao de controles, etc)

    //inicializar antes do start
    private void Awake()
    {
        rb2d = gameObject.GetComponent<Rigidbody2D>(); //inicializar valores
        col2d = gameObject.GetComponent<Collider2D>();
        anim = gameObject.GetComponent<Animator>();
        healthController = gameObject.GetComponent<HealthController>();

        Transform groundCheckParent = gameObject.transform.Find("GroundCheck"); //encontrar filho "Player/GroundCheck" 
        groundCheckQuantity = groundCheckParent.childCount; //pegar quantidade de transforms (filhos (/childs))

        groundCheck = new Transform[groundCheckQuantity]; //inicializar array
        for (int i = 0; i < groundCheckQuantity; i++) //para cada um dos transforms no groundCheckParent (pegar todos os filhos (/childs))
        {
            groundCheck[i] = groundCheckParent.GetChild(i); //encontrar filhos "Player/GroundCheck/GroundCheck_" 
        }

        Initialize(); //inicializar variaveis
    }

    public void Initialize() //inicializa variaveis do player
    {
        movementSpeed = characterSO.movementSpeed;
        jumpForce = characterSO.jumpForce;
        jumpDecay = characterSO.jumpDecay;
        maxJumpSpeed = characterSO.maxJumpSpeed;
        moveJumpModifier = characterSO.moveJumpModifier;
        moveAttackModifier = characterSO.moveAttackModifier;
    }

    // Use this for initialization
    void Start()
    {
        allCollisions = new HashSet<PlatformEffector2D>(); //inicalizar valores

        if (gameObject.CompareTag("Player1")) playerNumber = 1; //encontrar numero do player
        else /*if(gameObject.CompareTag("Player2"))*/ playerNumber = 2;

        canMove = true;
        //isFacingRight = true;
        isGrounded = true;
        isJumping = false;
        canJumpOff = false;
        isAttacking = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (healthController.actualNumberOfLives < 1) return; //se o jogador nao tiver mais "vidas"

        bool isGroundedAux = false; //para auxiliar na verificacao se um player 
        for (int i = 0; i < groundCheckQuantity; i++) //para cada um dos transforms no groundCheck
        {
            isGroundedAux = Physics2D.Linecast(gameObject.transform.position, groundCheck[i].position, 1 << LayerMask.NameToLayer("Ground") | 1 << LayerMask.NameToLayer("GroundNotPlataform")); //verificar se o player "esta ou nao" no chao //(talvez criar/usar em um atibuto dps)
            if (isGroundedAux == true) break; //se encontrar um deles diferente do inicial, parar
        }
        isGrounded = isGroundedAux;

        if (canMove && isGrounded) //se estiver no chao e puder se mover
        {
            if (Input.GetButtonDown("Jump" + playerNumber) || Input.GetAxis("JumpC" + playerNumber) > 0) //se for pular
            {
                canJump = true; //habilitar um pulo
            }
            else if (Input.GetButtonDown("JumpDown" + playerNumber) || Input.GetAxis("JumpC" + playerNumber) < 0) //se for "pular" para baixo //(descer plataforma)
            {
                canJumpOff = true; //habilitar "pulo" para baixo //(para descer plataforma)
            }
        }
    }

    //fisica
    private void FixedUpdate()
    {
        if (healthController.actualNumberOfLives < 1) return; //se o jogador nao tiver mais "vidas"

        //float moveVertical = 0; //inicializar o movimento
        float moveHorizontal = 0; //inicializar o movimento

        if (canMove) //se puder se mover
        {
            //moveVertical = Input.GetAxis("Vertical" + playerNumber); //pegar inputs manuais
            moveHorizontal = Input.GetAxis("Horizontal" + playerNumber);
        }

        playerPosition.Set(gameObject.transform.position.x, gameObject.transform.position.y); //pegar posicao atual do player

        Move(/*moveVertical*/0, moveHorizontal); //mover o player

        if (canMove && canJump) //caso o player va pular
        {
            if (!isJumping) StartCoroutine(Jump()); //pular (se nao estiver pulando)
            canJump = false; //"ja pulou"
        }

        if (canMove && canJumpOff) //caso o player va "pular" para baixo //(descer plataforma)
        {
            if (!isJumpingOff) StartCoroutine(JumpOff()); //"pular" para baixo (se "nao estiver" "pulando" para baixo)
            canJumpOff = false; //"ja 'pulou' para baixo"
        }
    }

    public void Move(float moveVertical, float moveHorizontal) //mover o player
    {
        Vector3 movement = new Vector3(moveHorizontal, moveVertical, 0.0f);

        if (movement.magnitude > 1)
        { //normalizar movimento
            movement = movement.normalized;
        }

        movement = movement * movementSpeed * Time.deltaTime; //ajustar movimento por tempo/frame 
        if (!isGrounded) movement *= moveJumpModifier; // * modificador para regular a velocidade enquanto "pula" (se nao estiver no chao)
        if (isAttacking) movement *= moveAttackModifier; // * modificador para regular a velocidade enquanto ataca (se estiver atacando)

        Move_Animating(movement); //animar movimentacao

        //rb2d.MovePosition(transform.position + movement); //mover para a nova posicao = atual + movimento
        rb2d.velocity = new Vector3(movement.x, rb2d.velocity.y, 0.0f); //setar movimentacao soh no eixo x
    }

    public IEnumerator Jump() //pular para cima (player)
    {
        isJumping = true;

        //Vector3 jump = new Vector3(0.0f, jumpForce, 0.0f);
        //jump = jump * Time.deltaTime;

        //rb2d.velocity = new Vector2(rb2d.velocity.x, jumpHeight); //pular v1
        float currentJumpForce = jumpForce;

        do
        {
            //Debug.Log("Jump:" + currentJumpForce);
            rb2d.AddForce(/*jump*/jumpForce * rb2d.gravityScale * Vector3.up * Time.deltaTime, ForceMode2D.Impulse); //pular v2
            rb2d.velocity = new Vector2(rb2d.velocity.x, Mathf.Clamp(rb2d.velocity.y, float.NegativeInfinity, maxJumpSpeed)); //limitar velocidade maxima de pulo (para evitar/limitar bugs de pulo atravessando plataformas seguidas , etc)

            currentJumpForce -= jumpDecay; //diminuir forca com o tempo
            yield return null; //passar frame
        } while ((Input.GetButton("Jump" + playerNumber) || Input.GetAxis("JumpC" + playerNumber) > 0) && currentJumpForce > 0); //enquanto estiver segurando o pulo e a forca ainda nao zerou

        isJumping = false;
    }

    public IEnumerator JumpOff() //"pular" para baixo (player) //(descer plataformas)
    {// OBS: Da maneira como esta feito eh possivel pular/voltar a plataforma assim que estiver descendo, de modo a nao descer
        isJumpingOff = true;

        Debug.Log(LayerMask.LayerToName(gameObject.layer) + " - Going Down");

        Collider2D[] colliders = new Collider2D[20]; //criar array de colliders 2D //(+valor maximo que o vetor de colliders2D pode ter)
        /*int numberOfContacts = */
        col2d.GetContacts(colliders); //pegar tudo que esta colidindo
        //Debug.Log("colliders quantity: " + colliders.Length);

        foreach (Collider2D c2d in colliders) //para cada collider no array obtido verificar se eh uma plataforma/ground e guardar referencia ao PlataformEffector2D
        {
            if (c2d != null && c2d.gameObject.CompareTag("Ground")) //se for uma plataforma/ground
            {
                allCollisions.Add(c2d.gameObject.GetComponent<PlatformEffector2D>()); //guardar no HashSet (referencia unica pra cada plataforma/ground em contato)
            }
        }

        foreach (PlatformEffector2D pe2D in allCollisions) //para cada plataforma/ground selecionado
        {
            //Debug.Log("Plataform colliderMask before:" + pe2D.colliderMask);
            pe2D.colliderMask = pe2D.colliderMask ^ (1 << gameObject.layer); //desabilitar layer do player
        }

        //Physics2D.IgnoreLayerCollision(gameObject.layer, LayerMask.NameToLayer("Ground"), true); //ignorar colisao com plataformas/chao
        yield return new WaitForSeconds(0.5f); //esperar um tempo
        //yield return null;
        //Physics2D.IgnoreLayerCollision(gameObject.layer, LayerMask.NameToLayer("Ground"), false); //voltar a considerar colisao com plataformas/chao

        foreach (PlatformEffector2D pe2D in allCollisions) //para cada plataforma/ground selecionado
        {
            pe2D.colliderMask = pe2D.colliderMask ^ (1 << gameObject.layer); //reabilitar layer do player
        }

        allCollisions.Clear(); //limpar HashSet
        Debug.Log(LayerMask.LayerToName(gameObject.layer) + " - Going Down End");

        isJumpingOff = false;
    }

    public void Move_Animating(Vector3 movement) //animar player
    {
        bool walking = !(movement == Vector3.zero); //se esta parado ou se movendo
        anim.SetBool("isWalking", walking); //seta flag bool isWalking
        anim.SetBool("isGrounded", isGrounded); //seta flag bool isGrounded

        int inputX = MoveAux_ChooseNormalization(movement.normalized.x); //pega o valor do movimento no eixo x normalizado

        if (walking) //se estiver se movendo
        {
            //anim.SetFloat("input_y", MoveAux_ChooseNormalization(movement.normalized.y)); //setar posicoes de movimento (para blend tree ou n)
            anim.SetInteger/*Float*/("input_x", inputX);
        }

        //4.2f por causa dos elevators
        anim.SetInteger/*Float*/("input_y", !(isGrounded && rb2d.velocity.y > 4.2f) ? 0 : MoveAux_ChooseNormalization(rb2d.velocity.y)); //setar posicoes de movimento (para blend tree ou n) // setar y para saber se esta caindo ou pulando

        if (inputX > 0 && !isFacingRight) Flip(new Vector3(0, 0, 0)); //vira a animacao do player caso necessario
        else if (inputX < 0 && isFacingRight) Flip(new Vector3(0, 180, 0));
    }

    public int MoveAux_ChooseNormalization(float norm) //verifica normalizacao para a animacao (para a diagonal)
    {
        if (norm != 0 && norm != -1 && norm != 1) return norm > -0.5f && norm < 0.5f ? 0 : norm > 0 ? 1 : -1; //para movimentacao diagonal, etc
        else return (int)norm; //se ja estiver normalizado soh retorna
    }

    public void Flip(Vector3 flipRotation) //vira a animacao do player
    {
        isFacingRight = !isFacingRight; //re informar oricentacao atual do objeto no eixo x

        transform.localRotation = Quaternion.Euler(flipRotation); //rotacionar o objeto //para flipar o player (nao estava funcionando bones + flip por -1*scale.x)

        //Vector3 actualScale = transform.localScale; //virar a animacao tornando o scale da sprite ao contrario
        //actualScale.x *= -1;
        //transform.localScale = actualScale;
    }
}
