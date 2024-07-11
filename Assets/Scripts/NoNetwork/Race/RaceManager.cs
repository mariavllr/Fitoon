using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaceManager : MonoBehaviour
{
    public struct RaceBotsData
    {
        string botID, hairIndex, topIndex, bottomIndex, shoeIndex;
    }

    public static RaceManager Instance;

    public int numberOfRace = 1;
    public int maxRaces = 3;
    public bool playerWon;

    public List<RaceBotsData> raceBots;

    private void Awake()
    {

        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
}
