using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class GameOver : MonoBehaviour
{
    //Debe de ejecutarse este script cuando el jugador acaba la partida por completo, ganando (1 puesto) o perdiendo

    //Primer puesto: Medalla de oro
    //Puesto 20% o superior: Medalla de plata
    //Puesto 50% o superior: Medalla de bronce 
    //Puesto menor al 50%: Sin medalla (loser points) -> Perder en la primera carrera


    [SerializeField] int goldPoints, silverPoints, bronzePoints, loserPoints;
    [SerializeField] TextMeshProUGUI rewardText;
    SaveData saveData;
    private void Awake()
    {
        saveData = GetComponent<SaveData>();
    }
    void Start()
    {
        CalculateReward();
        

        RaceManager.Instance.Reset();
        saveData.SaveToJson();
    }

    private void CalculateReward()
    {
        if (RaceManager.Instance.numberOfRace == 3)
        {
            if (RaceManager.Instance.playerWon)
            {
                //Primer puesto
                saveData.player.points += goldPoints;
                rewardText.text = $"#1 position!\nYou won {goldPoints} points!";
            }
            else
            {
                saveData.player.points += silverPoints;
                rewardText.text = $"You won {silverPoints} points!";
            }

        }
        else if (RaceManager.Instance.numberOfRace == 2)
        {
            saveData.player.points += bronzePoints;
            rewardText.text = $"You won {bronzePoints} points!";
        }
        else if (RaceManager.Instance.numberOfRace == 1)
        {
            saveData.player.points += loserPoints;
            rewardText.text = $"You won {loserPoints} points!";
        }
        else
        {
            Debug.Log("ERROR: No se pueden calcular los puntos finales");
        }
    }

}
