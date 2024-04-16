using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class EndGameText : MonoBehaviour
{
    TextMeshProUGUI endText;
    void Start()
    {
        endText = GetComponent<TextMeshProUGUI>();
        if (RaceManager.Instance.playerWon)
        {
            endText.text = "You won!";
        }

        else endText.text = "You lost!";

        RaceManager.Instance.numberOfRace = 1;
        RaceManager.Instance.maxRaces = 3;
    }
}
