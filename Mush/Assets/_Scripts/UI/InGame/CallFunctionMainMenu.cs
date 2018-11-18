using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CallFunctionMainMenu : MonoBehaviour
{
    //public

    //private
    private MainMenuManager mainMenuManager; //script MainMenuManager

    // Use this for initialization
    void Start()
    {
        mainMenuManager = GameObject.FindObjectOfType<MainMenuManager>(); //inicializar valores
    }

    public void CallActionFunctionRoot() // chama funcao de acao no root
    {
        mainMenuManager.CallActionFunction();
    }

    // Update is called once per frame
    void Update()
    {

    }
}
