using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CallFunctionMainScene : MonoBehaviour
{
    //public

    //private
    private GameEndManager gameEndManager; //script GameEndManager

    // Use this for initialization
    void Start()
    {
        gameEndManager = GameObject.FindObjectOfType<GameEndManager>(); //inicializar valores
    }

    public void CallActionFunctionRoot() // chama funcao de acao no root
    {
        gameEndManager.CallActionFunction();
    }

    // Update is called once per frame
    void Update()
    {

    }
}
