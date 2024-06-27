using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BotSkins : MonoBehaviour
{
    GameObject skin;
    int hair = 0;
    int shirt = 1;
    int pants = 2;
    int shoes = 3;
    private void Awake()
    {
        RandomizeSkin();
        RandomizeObject(hair);
        RandomizeObject(shirt);
        RandomizeObject(pants);
        RandomizeObject(shoes);
    }

 

    void RandomizeSkin()
    {
        int rand = Random.Range(0, transform.childCount);
        skin = transform.GetChild(rand).gameObject;
        skin.SetActive(true);
    }

    void RandomizeObject(int type)
    {
        //Obtiene el gameobject que contiene los peinados y elige uno aleatorio de sus hijos
        Transform container = skin.transform.GetChild(type);
        int rand = Random.Range(0, container.childCount);

        if(container.childCount != 0) container.GetChild(rand).gameObject.SetActive(true);
    }
}
