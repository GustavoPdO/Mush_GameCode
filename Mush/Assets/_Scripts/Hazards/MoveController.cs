using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveController : MonoBehaviour
{
    //public
    public float movementSpeed; //velocidade do hazard

    public float minStopTime; //tempos minimo e maximo para ficar parado
    public float maxStopTime; //tempos minimo e maximo para ficar parado

    public Transform[] movePoints; //"pontos de controle"(objetos) e falar pro hazard andar entre eles

    public bool canLoop; //se os pontos entram em loop

    //private
    private Rigidbody2D rb2d; //rigdbody2d

    private Vector2 hazardPosition; //posicao atual do inimigo

    private bool isWaitingTime; //se esta aguardando um tempo

    private int walkPointsAuxIndex; //pra indexar o vetor walkPointsAux
    private bool isWaitingBetweenPoints; //pra n entrar na funcao mais de uma vez

    // Use this for initialization
    void Start()
    {
        rb2d = gameObject.GetComponent<Rigidbody2D>(); //inicializar valores

        walkPointsAuxIndex = 0;

        isWaitingTime = false;
        isWaitingBetweenPoints = false;
    }

    // Update is called once per frame
    void Update()
    {
        rb2d.velocity = Vector2.zero; //"segura" o npc no lugar da um efeito de drag pelo menos
        rb2d.angularVelocity = 0;
    }

    //fisica
    private void FixedUpdate()
    {
        hazardPosition.Set(gameObject.transform.position.x, gameObject.transform.position.y); //pegar posicao atual do inimigo

        if (!isWaitingBetweenPoints) Move_BetweenPoints(); //se mover entre os pontos (caso n esteja parado esperando pra mover dnovo)
    }

    //move o npc, caso ele se nao mova aleatoriamente, entre os pontos selecionados (partindo da posicao atual do npc, que nao precisa ser adicionada (ja vai ser adicionada no comeco))
    public void Move_BetweenPoints() //a coroutine que esta dentro (eh soh pra ficar parado um tempo)
    {
        Vector3 movement = Vector3.zero;

        if (Vector3.Distance(transform.position, movePoints[walkPointsAuxIndex].position) > 0.1) //para nao animar no final da movimentacao //e para ir ate "chegar" no ponto designado 
            movement = new Vector3(movePoints[walkPointsAuxIndex].position.x - transform.position.x, movePoints[walkPointsAuxIndex].position.y - transform.position.y); //ir para o proximo ponto

        if (movement.magnitude > 1) //normalizar movimento caso passe a magnitude maxima
        {
            movement = movement.normalized;
        }

        movement = movement * movementSpeed * Time.deltaTime; //ajustar movimento por tempo/frame

        rb2d.velocity = new Vector3(movement.x, movement.y, 0.0f); //setar movimentacao 

        if (canLoop && Vector3.Distance(transform.position, movePoints[walkPointsAuxIndex].position) <= 0.1) //para nao animar no final da movimentacao //e para ir ate "chegar" no ponto designado 
        { //ta aqui embaixo pra n buga a movimentacao/animacao
            walkPointsAuxIndex = (walkPointsAuxIndex + 1) % movePoints.Length; //ir para o proximo ponto
            if (!isWaitingBetweenPoints) StartCoroutine(Move_BetweenPointsWaitSeconds(Random.Range(minStopTime, maxStopTime))); //pra esperar um tempo parado e voltar a andar entre os pontos
        }

    }

    public IEnumerator Move_BetweenPointsWaitSeconds(float seconds) //pra esperar um tempo e voltar a andar entre os pontos
    {
        isWaitingBetweenPoints = true;

        yield return new WaitForSeconds(seconds); //apos se movimentar, ficar um tempo parado

        isWaitingBetweenPoints = false;
    }
}
