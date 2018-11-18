using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CallFunctionSelectScene : MonoBehaviour
{
    //public

    //private
    private SelectSceneManager selectSceneManager; //script SelectSceneManager

    // Use this for initialization
    void Start()
    {
        selectSceneManager = GameObject.FindObjectOfType<SelectSceneManager>(); //inicializar valores
    }

    public void CallActionFunctionRoot() // chama funcao de acao no root
    {
        selectSceneManager.CallActionFunction();
    }

    // Update is called once per frame
    void Update()
    {

    }
}
