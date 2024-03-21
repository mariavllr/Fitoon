using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GoalController : MonoBehaviour
{
    public TextMeshProUGUI currentPositionTextMesh;

    [SerializeField] private int finishedPlayers = 0;
    [SerializeField] private int playerPosition = 0;
    [SerializeField] private bool playerFinished = false;
    [SerializeField] private List<Transform> players;


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
    }

}
