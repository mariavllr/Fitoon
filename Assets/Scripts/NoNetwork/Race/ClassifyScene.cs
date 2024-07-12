using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ClassifyScene : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI countdownText;
    [SerializeField] int countdown;

    void Start()
    {
        countdownText.text = countdown.ToString();
        StartCoroutine(Countdown());

        //RaceManager.Instance.numberOfRace = 1;
        //RaceManager.Instance.maxRaces = 3;
    }

    IEnumerator Countdown()
    {
        while (countdown != 0)
        {
            yield return new WaitForSeconds(1f);
            countdown--;
            if (countdown == 0)
            {
                GetComponent<ButtonFunctions>().LoadScene("FindingScenario");
            }
            else
            {
                countdownText.text = countdown.ToString();
            }
        }

    }
}
