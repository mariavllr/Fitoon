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
    [SerializeField] private TextMeshProUGUI roundText;
    [SerializeField] private TextMeshProUGUI infoText;
    int maxPlayers = 32;

    //Evento cuando acaba la carrera. Se manda a todos los participantes (bots incluidos)
    public delegate void OnRaceFinish();
    public static event OnRaceFinish onRaceFinishEvent;

    private void Awake()
    {
        Reset();
    }

    private void LateUpdate()
    {
        //Order player list by distance to the goal. This only works for straight courses
       /* if (!playerFinished)
            if (players != null && players.Count > 1)
                players.Sort((a, b) => Vector3.Distance(a.transform.position, transform.position).CompareTo(Vector3.Distance(b.transform.position, transform.position)));*/
    }

    public void UpdatePosition(Transform player)
    {
        //if (players.Contains(player)) playerPosition = finishedPlayers + (players.IndexOf(player) + 1);

        //currentPositionTextMesh.text = playerPosition + "/" + (players.Count + finishedPlayers);

        //En lugar de la posicion mostramos los clasificados
        currentPositionTextMesh.text = finishedPlayers + "/" + (maxPlayers);
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
        

        if (player.GetChild(0).gameObject.tag == "Character")
        {
            //Ha entrado el jugador
            PlayerFinish();
            FindObjectOfType<PlayerControl>().StopCharacterOnFinish();
            FindObjectOfType<FinishController>().Finish(); //Animacion de acabar. To do: que diga You win!
        }

        else
        {
            //Ha entrado otro
            //Hacer que desaparezca el bot
            player.gameObject.SetActive(false);
            //Añadirlo a una lista en race manager para la siguiente ronda

        }

        if (finishedPlayers == maxPlayers)
        {
            //Fin de carrera
            EndRace();
        }
       
    }

    private void EndRace()
    {
        FindObjectOfType<FinishController>().Finish(); //Animacion de acabar

        //Lanzar evento fin de carrera a los que queden
        if (onRaceFinishEvent != null)
        {
            onRaceFinishEvent();
        }


        RaceManager.Instance.numberOfRace++;
        if (RaceManager.Instance.numberOfRace > RaceManager.Instance.maxRaces)
        {
            EndGame();
        }
        else
        {
            if (playerFinished)
            {
                //Ha ganado, puede pasar a siguiente nivel aleatorio
                StartCoroutine(NextLevel());
            }
            else
            {
                //Ha perdido, vuelve al inicio
                infoText.text = "You lost... Press exit to go back";
                RaceManager.Instance.playerWon = false;
                exitButton.gameObject.SetActive(true);
                exitButton.onClick.AddListener(delegate { FindObjectOfType<ButtonFunctions>().LoadScene("FinPartida"); });
            }
        }
    }

    private void EndGame()
    {
        Debug.LogWarning("fin de PARTIDA");
        if (playerFinished)
        {
            //Ganó el jugador
            infoText.text = "You win!!! Press exit to go back";
            RaceManager.Instance.playerWon = true;
            exitButton.gameObject.SetActive(true);
            exitButton.onClick.AddListener(delegate { FindObjectOfType<ButtonFunctions>().LoadScene("FinPartida"); });
        }
        else
        {
            //Perdió
            infoText.text = "You lost!!! Press exit to go back";
            RaceManager.Instance.playerWon = false;
            exitButton.gameObject.SetActive(true);
            exitButton.onClick.AddListener(delegate { FindObjectOfType<ButtonFunctions>().LoadScene("FinPartida"); });
        }
    }

    IEnumerator NextLevel()
    {
        infoText.text = "You win! Next level...";
        yield return new WaitForSeconds(5f);
        FindObjectOfType<ButtonFunctions>().LoadScene("FindingScenario");
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
            int ronda = RaceManager.Instance.numberOfRace;
            switch (ronda)
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
            roundText.text = "RONDA " + ronda;
        }

        else Debug.LogError("Error: No race manager");
    }
}
