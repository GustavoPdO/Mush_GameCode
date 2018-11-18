using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundsGeneral : MonoBehaviour
{
    //public

    //private
    private static bool doSoundsManager; //se os sons ja existem na cena //static para todos os criados terem a mesma variavel para checar

    private Transform[] children; //componentes transform de todas as childs do objeto

    // Use this for initialization
    void Start()
    {
        //teste para passar de cena sem sumir os sons/refazer 
        if (!doSoundsManager)
        {
            doSoundsManager = true;
            DontDestroyOnLoad(gameObject);
        }
        else { Destroy(gameObject); }

        Transform[] children = gameObject.GetComponentsInChildren<Transform>(); //inicializar valores
    }

    public void FindAndPlay(string soundName) //encontra e comeca a tocar um som
    {
        if (children == null)
            children = gameObject.GetComponentsInChildren<Transform>(); //inicializar valores

        foreach (Transform child in children)
        {
            if (child.name.Equals(soundName))
            {
                child.GetComponent<AudioSource>().Play();
            }
        }
    }

    public void FindAndStop(string soundName) //encontra e parar um som
    {
        if (children == null)
            children = gameObject.GetComponentsInChildren<Transform>(); //inicializar valores

        foreach (Transform child in children)
        {
            if (child.name.Equals(soundName))
            {
                child.GetComponent<AudioSource>().Stop();
            }
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
