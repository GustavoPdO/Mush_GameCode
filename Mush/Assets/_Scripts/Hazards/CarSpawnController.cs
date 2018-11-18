using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarSpawnController : MonoBehaviour
{
    //public
    public GameObject spawnCarPrefab; //prefab do carro
    public Transform spawnPoint; //ponto de spawn
    public Transform endPoint; //ponto de termino

    public float spawnTime; //de quanto em quanto tempo spawna um carro

    public float removeCarAfter; //depois de quanto tempo remove o carro do cenario

    [Header("Arrows")]
    public SpriteRenderer arrowsSpriteObject;
    public Color arrowsOriginalColor; //cor das arrows antes de spawnar
    public Color arrowsSpawningColor;

    public float delaySpawnTime; //delay entre o spawn do carro

    public ParticleSystem[] sparksParticleSystem; //efeitos de faiscas (para exetucar no final da animacao das setas)
    public int numberOfSparks; //numero de faiscas //(menor que o tamanho do array) 

    //private
    private bool isWaiting; //se esta esperando um tempo
    private bool isWaitingDelay; //se esta esperando um tempo de delay

    private GameEndManager gameEndManager; //script/objeto gameEndManaged

    private GameObject newCar;

    // Use this for initialization
    void Start()
    {
        gameEndManager = GameObject.FindObjectOfType<GameEndManager>(); //inicializar valores

        isWaiting = false;
        isWaitingDelay = false;

        arrowsSpriteObject.color = arrowsOriginalColor;
    }

    // Update is called once per frame
    void Update()
    {
        if (!gameEndManager.isGameEnd && !isWaiting && !isWaitingDelay) //se nao estiver esperando
        {
            StartCoroutine(SpawnSecondsDelay());
        }
        //else if (newCar != null && gameEndManager.isGameEnd) //se o jogo ja tiver acabado
        //{
        //    Destroy(newCar);
        //}
    }

    public GameObject CreateNewCar() //instanciar um novo carro na cena
    {
        GameObject newCar = Instantiate(spawnCarPrefab, spawnPoint.position, spawnPoint.rotation);
        MoveController moveController = newCar.GetComponent<MoveController>();
        moveController.movePoints = new Transform[1];
        moveController.movePoints[0] = endPoint;

        return newCar;
    }

    public IEnumerator WaitSeconds() //pra esperar um tempo e voltar a andar entre os pontos
    {
        isWaiting = true;

        yield return new WaitForSeconds(spawnTime); //esperar um tempo

        isWaiting = false;
    }

    public IEnumerator SpawnSecondsDelay() //pra esperar um tempo para spawnar
    {
        isWaitingDelay = true;

        arrowsSpriteObject.color = arrowsSpawningColor;
        yield return new WaitForSeconds(delaySpawnTime); //esperar um tempo
        arrowsSpriteObject.color = arrowsOriginalColor;

        HashSet<int> randomAux = new HashSet<int>(); //dar play em sparks aleatorios
        while (randomAux.Count < numberOfSparks)
        {
            randomAux.Add(Random.Range(0, numberOfSparks));
        }

        foreach (int i in randomAux)
            sparksParticleSystem[i].Play();

        newCar = CreateNewCar(); //instanciar um novo carro na cena
        Destroy(newCar, removeCarAfter);

        StartCoroutine(WaitSeconds());

        isWaitingDelay = false;
    }
}
