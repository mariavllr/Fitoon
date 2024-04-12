using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GoalController : MonoBehaviour
{
    public TextMeshProUGUI currentPositionTextMesh;

    [SerializeField] private int finishedPlayers = 0;
    [SerializeField] private int playerPosition = 0;
    [SerializeField] private bool playerFinished = false;
    [SerializeField] private List<Transform> players;
    [SerializeField] private Button exitButton;

    int maxPlayers = 32;

    private void Awake()
    {
        Reset();
    }

    private void LateUpdate()
    {
        //Order player list by distance to the goal. This only works for straight courses
        if (!playerFinished) if (players != null && players.Count > 1) players.Sort((a, b) => Vector3.Distance(a.transform.position, transform.position).CompareTo(Vector3.Distance(b.transform.position, transform.position)));
    }

    public void UpdatePosition(Transform player)
    {
        if (players.Contains(player)) playerPosition = finishedPlayers + (players.IndexOf(player) + 1);

        currentPositionTextMesh.text = playerPosition + "/" + (players.Count + finishedPlayers);
    }


    public void AddPlayerToList(Transform player)
    {
        if (!players.Exists(p => p == player))
        {
            players.Add(player);
            Debug.Log("AddPlayer");
        }
    }


    public void RemovePlayerFromList(Transform player)
    {
        //players.Remove(player);
        finishedPlayers++;
        players.RemoveAll(p => p == null || p == player);
        //Hacer que desaparezca el bot
        //Añadirlo a una lista en race manager para la siguiente ronda

        if (finishedPlayers == maxPlayers)
        {
            Debug.LogWarning("fin de ronda");

            PlayerFinish();
            FindObjectOfType<FinishController>().Finish();
            FindObjectOfType<PlayerControl>().StopCharacterOnFinish();


            RaceManager.Instance.numberOfRace++;
            if(RaceManager.Instance.numberOfRace > RaceManager.Instance.maxRaces)
            {
                Debug.LogWarning("fin de PARTIDA");
            }
            else
            {
                if (playerFinished)
                {
                    //Ha ganado, puede
                    //Pasar a siguiente nivel aleatorio
                    FindObjectOfType<ButtonFunctions>().LoadScene("FindingScenario");
                }
                else
                {
                    //Ha perdido, vuelve al inicio
                    exitButton.onClick.AddListener(delegate { FindObjectOfType<ButtonFunctions>().LoadScene("FinPartida"); });
                }
            }
            
        }

        else
        {
            if (playerFinished)
            {
                //Ha ganado pero tiene que esperar a los demas
                Debug.LogWarning("Has ganado la ronda");
            }
        }
        
    }


    public void PlayerFinish()
    {
        playerFinished = true;
    }


    public void Reset()
    {
        players.Clear();
        finishedPlayers = 0;
        playerPosition = 0;
        playerFinished = false;
        currentPositionTextMesh.text = playerPosition + "/" + players.Count;

        if (RaceManager.Instance != null)
        {

            switch (RaceManager.Instance.numberOfRace)
            {
                case 1:
                    maxPlayers = 16;
                    break;
                case 2:
                    maxPlayers = 8;
                    break;
                case 3:
                    maxPlayers = 1;
                    break;
            }
            Debug.Log("MAX PLAYERS" + maxPlayers);
        }

        else Debug.LogError("Error: No race manager");
    }
}
