using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaceManager : MonoBehaviour
{
    public static RaceManager Instance;

    public int numberOfRace = 1;
    public int maxRaces = 3;
    public bool playerWon;

    private void Awake()
    {
        Screen.sleepTimeout = SleepTimeout.NeverSleep; //Para que no se apague el telefono

        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
}
