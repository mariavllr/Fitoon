using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Ranking : MonoBehaviour
{
    [SerializeField] GameObject rankingPosPrefab;
    GameObject container;
    TextMeshProUGUI userText;
    TextMeshProUGUI positionText;

    int players = 33;

    private void Start()
    {
        CalculatePosition();
        UpdateRanking();
    }

    void CalculatePosition()
    {
        //Como calculamos la posicion si han perdido? Por lo lejos que quedaron de la meta?

        //Si es en la primera carrera, 
    }

    void UpdateRanking()
    {
        for (int i = 0; i < players; i++)
        {

        }
    }

}
