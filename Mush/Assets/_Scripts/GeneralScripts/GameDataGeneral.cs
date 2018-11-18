using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameDataGeneral : MonoBehaviour
{
    //public
    public GameObject[] playersPrefab; //prefab dos players //(na ordem de selecao)

    [HideInInspector]
    public int playersNumber; //numero de jogadores na cena
    [HideInInspector]
    public int[] characterSelected; //personagem escolhido por cada jogador
    [HideInInspector]
    public int scenarioSelected; //cenario escolhido
    [HideInInspector]
    public Color[] playerColor; //cor de cada player

    //private
    private static bool doGameData; //se os dados ja existem na cena //static para todos os criados terem a mesma variavel para checar

    // Use this for initialization
    void Start()
    {
        //teste para passar de cena sem sumir os dados/refazer 
        if (!doGameData)
        {
            doGameData = true;
            DontDestroyOnLoad(gameObject);
        }
        else { Destroy(gameObject); }
    }

    public void SetDoGameData(bool value) //setar um valor no doGameData
    {
        doGameData = value;
    }

    // Update is called once per frame
    void Update()
    {

    }

}
