using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Character", menuName = "Scriptable Objects/Character")] //permite a criacao de novos scriptable objects direto pela unity
public class CharacterScriptableObject : ScriptableObject
{ //dados gerais de um character (colocar animator junto depois tambem, etc)
    //public
    [Header("Character Data - Main")]
    public string characterName; //nome do character
    //public Animator anim; //animator do character

    [Space(5)]
    [Header("Movement")]
    public float movementSpeed; //velocidade do character
    [Range(0, 1)] /*[Tooltip("Help Text")]*/
    public float moveJumpModifier; //modificador de velocidade enquanto pula [0, 1] (para reduzir a velocidade enquanto pula, etc)
    public float jumpForce; //forca do pulo do character
    public float jumpDecay; //decaimento da doca de pulo //vai indicar o quao alto eh possivel ir segurando para pular
    public float maxJumpSpeed; //velocidade maxima do pulo
    [Range(0, 1)]
    public float moveAttackModifier; //modificador de velocidade enquanto ataca [0, 1] (para reduzir a velocidade enquanto ataca, etc)

    [Space(5)]
    [Header("Stats - General")]
    public int maxHealth; //vida maxima de uma vida do character
    public int maxNumberOfLives; //numero maximo de vida do character
    public float damageDelay; //delay para poder levar dano novamente

    [Space(5)]
    public int defense; //defesa do character
    public int maxQtdAttacks; //quantidade maxima de ataques (seguir quantidade de um combo)
    public int[] attackDamage; //ataque do character
    //public enum AttackType { Melee, Ranged }; //enum para o tipo de ataque
    //public AttackType attackType; //tipo de ataque

    public bool canStopMovement; //para parar ou nao movimento do player enquanto ataca //(parado ou nao, o modificador de movimento enquanto ataca ainda eh aplicado)

    [Space(5)]
    [TextArea/*(1,3)*/]
    public string weaponPath; //caminho de onde se encontra a arma/colisor (em qual child) partindo do player
    //ex: Ronnie -> Bones/Hip/Torso/R Upper Arm/R Arm/R Hand --> .GetChild(0) = Sword

    //[Space(2)]
    //[Header("Stats - Melee")]
    [Space(2)]
    [Header("Stats - Ranged")]
    public GameObject bulletPrefab; //referenca (/prefab) da bala que sera criada (/spawnada)

    public float bulletSpeed; //velocidade da bala
    public float bulletLifeTime; //tempo de vida/duracao do tiro
    public int bulletQuantity; // quantidade de balas que vai criar (/spawnar)
    [Range(0, 180)]
    public float bulletSpread; // quantidade de graus pra cada lado que pode dar spread no tiro

    [Space(5)]
    [Header("Stats - Skills")]
    public int skill1Cooldown; //tempo de recarga da skill 1 (ataque)
    public int skill2Cooldown; //tempo de recarga da skill 2 (defesa)
    public int mushCooldown; //tempo de recarga da skill 3 (mush)

    [Space(5)]
    public int[] skillsLifeUnlock; //em qual/quantas vidas desbloqueia essa skill //(colocar mesma na quantidade de skills e na ordem)

    [Space(2)]
    [Header("Sprites")]
    public Sprite[] skillsSprites; //sprites de cada skill para serem colocadas na interface (UIInGame) //(colocar mesma na quantidade de skills e na ordem)
    public Sprite playerInGameSprite; //sprite da interface dentro de jogo (/UIInGame) do jogador
    public Sprite playerSelectSceneSprite; //sprite na tela de selecao (/SelectScene) do jogador

    [Space(2)]
    public string[] skillsDescription; //descricao de cada skill para serem colocadas na interface (/SelectScene) //(colocar mesma na quantidade de skills e na ordem)

    //private


}
