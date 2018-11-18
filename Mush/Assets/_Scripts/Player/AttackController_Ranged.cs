using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackController_Ranged : AttackController
{//para controlar o ataque ranged de um personagem (falta testar com as animacoes e as chamadas de funcao nas animations)
    //public

    //scriptable public - private
    private GameObject bulletPrefab; //referenca (/prefab) da bala que sera criada (/spawnada)

    public float bulletSpeed; //velocidade da bala
    public float bulletLifeTime; //tempo de vida/duracao do tiro
    public int bulletQuantity; // quantidade de balas que vai criar (/spawnar)
    [Range(0, 180)]
    public float bulletSpread; // quantidade de graus pra cada lado que pode dar spread no tiro

    //private
    //private Collider2D col2d; //collider2D
    private Transform bulletSpawn; //transform do bulletSpawn para instanciar balas

    //inicializar antes do start
    private void Awake()
    {
        //col2d = gameObject.GetComponent<Collider2D>(); //inicializar valores 
        player = gameObject.transform.root.gameObject;//gameObject.transform.parent.gameObject; //pegar referencia do parent (player)
        bulletSpawn = gameObject.transform.Find("BulletSpawn"); //pegar referencia do child (bulletSpawn)
        //Debug.Log(gameObject.transform.root);

        anim = player.GetComponent<Animator>(); //pegar animator do player
        healthController = player.GetComponent<HealthController>(); //pegar script health controller do parent (player)
        playerController = player.GetComponent<PlayerController>(); //pegar script player controller do parent (player)

        Initialize(); //inicializar variaveis
    }

    public void Initialize() //inicializa variaveis do player
    {
        attackDamage = characterSO.attackDamage;
        maxQtdAttacks = characterSO.maxQtdAttacks;

        canStopMovement = characterSO.canStopMovement;

        bulletPrefab = characterSO.bulletPrefab;

        bulletSpeed = characterSO.bulletSpeed;
        bulletLifeTime = characterSO.bulletLifeTime;
        bulletQuantity = characterSO.bulletQuantity;
        bulletSpread = characterSO.bulletSpread;
    }

    // Use this for initialization
    void Start()
    {
        if (player.CompareTag("Player1")) playerNumber = 1; //encontrar numero do player
        else /*if(gameObject.CompareTag("Player2"))*/ playerNumber = 2;

        attackCount = 0; //inicializar valores

        canAttack = true; //inicializar valores
        isAttacking = false;
    }

    //Update esta no AttackController

    // Update is called once per frame
    //void Update()
    //{
    //    //para testes sem a animacao
    //    for (int i = 0; i < bulletQuantity; i++) //atirar/instanciar bala(s)
    //    {
    //        Fire(); //atirar/instanciar bala(s)
    //    }
    //}

    public override void StartAttack(string name = "", int attackNumber = 0) //seta preparacoes para iniciar o ataque/dar dano (chamar pela animacao [comeco]) //(argumento string opcional, para testes) //(argumento int obrigatorio para ataques basicos)
    {
        //Debug.Log(player.tag + " : " + name); //para testes
        this.attackNumber = attackNumber;

        //isAttacking = true;
        anim.SetBool("doAttack", false); //resetar bool de atacar

        for (int i = 0; i < bulletQuantity; i++) //atirar/instanciar bala(s)
        {
            Fire(); //atirar/instanciar bala(s)
        }
    }

    //OBS: talvez para casos de cast de skill no meio do ataque seja necessario resetar as variaveis do ataque (talvez precise chamar EndAttack quando entrar na animacao de uma skill e desabilitar ataques)
    public override void EndAttack(string name = "") //seta preparacoes para finalizar o ataque/dar dano (chamar pela animacao [final]) //(para cancelar/finalizar ataques tambem) //(argumento opcional, para testes)
    {
        Debug.Log(player.tag + " : " + name); //para testes

        //ativar para conectar ultimo ataque com primeiro
        //if (/*!anim.GetBool("doAttack")*/attackCount > maxQtdAttacks) isAttacking = false; //se nao for atacar novamente/nao estiver atacando //(obs: antes era trigger, mas nao dava pra fazer essa verificacao, ou alterar para soh chavear no final da animacao de ataque)
    }

    public void Fire() //atirar/instanciar bala
    {
        Quaternion newSpread = DefineFireSpread(); //pegar spread dos tiros

        GameObject newBullet = Instantiate(bulletPrefab, bulletSpawn.position, bulletSpawn.rotation * newSpread); //instanciar bala // + nova rotacao/spread
        newBullet.layer = LayerMask.NameToLayer("Damage" + playerNumber); //ajustar layer da bala

        BulletController bulletController = newBullet.GetComponent<BulletController>(); //script bulletController
        BulletHealthController bulletHealthController = newBullet.GetComponent<BulletHealthController>(); //script bulletHealthController

        bulletController.bulletSpeed = bulletSpeed; //inicializar variaveis da bala

        if (isAttacking) bulletHealthController.bulletDamage = attackDamage[attackNumber]; //se estiver atacando, dano da bala para ataques
        else bulletHealthController.bulletDamage = skillDamage; //enviar dano caso nao esteja atacando (no caso, vai estar usando skill)

        bulletHealthController.bulletLifeTime = bulletLifeTime;
        bulletHealthController.healthController = healthController;

        //Destroy(newBullet, bulletLifeTime); //destruir bala apos tempo de vida acabar
    }

    public Quaternion DefineFireSpread() //define o espalhamento/margem de erro dos tiros
    {
        float spreadZ = Random.Range(-bulletSpread, bulletSpread); //pegar um valor aleatorio na dispersao
        Vector3 spreadVector = new Vector3(0f, 0f, spreadZ);
        return Quaternion.Euler(spreadVector); //retorna rotacao
    }
}
