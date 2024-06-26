using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateBots : MonoBehaviour
{

    public GameObject botPrefab;
    public Transform botsFolder;
   // public Transform goal;
    public List<SpawnpointData> spawnpointData;

    private string spawnedBotName = "";
    private const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

    public Countdown countdownTimer;

    public struct SpawnpointData
    {
        public Vector3 _spPosition;
        public bool _isOccupied;
        public string _playerId;

        public SpawnpointData(Vector3 spPosition, bool isOccupied, string playerId)
        {
            _spPosition = spPosition;
            _isOccupied = isOccupied;
            _playerId = playerId;
        }

    }

    void Awake()
    {
        spawnpointData = new List<SpawnpointData>();
        foreach (Transform child in botsFolder)
        {
            Destroy(child.gameObject);
        }
        GenerateSpawnpointsList();
    }

    private void Start()
    {
        
    }

    public void InitializeBots()
    {
        
        FillSpawnpointsWithBots();
        countdownTimer.StartCountdown();
    }

    public void GenerateSpawnpointsList()
    {
        spawnpointData.Clear();

            foreach (var gO in FindObjectsOfType(typeof(GameObject)) as GameObject[])
            {
                if (gO.tag == "SpawnPoint")
                {
                    spawnpointData.Add(new SpawnpointData(gO.transform.position, false, ""));
                }
            }
    }

    public void FillSpawnpointsWithBots()
    {

            for (int i = 0; i < spawnpointData.Count; i++)
            {
                if (!spawnpointData[i]._isOccupied)
                {
                    //Generate unique random bot name
                    GenerateUniqueRandomName(i + 1);

                    //Updates the spawnpoint NetworkList with this spawnpoint as isOccupied = true and the playerId on it
                    UpdateSpawnpointsList(i, spawnpointData[i]._spPosition, true, spawnedBotName);

                    SpawnBot(spawnpointData[i]._spPosition + new Vector3(0, 1, 0));
                }
            }
    }

    public void UpdateSpawnpointsList(int index, Vector3 spPosition, bool isOccupied, string playerId)
    {
        spawnpointData[index] = new SpawnpointData(spPosition, isOccupied, playerId);
    }

    private void GenerateUniqueRandomName(int botNum)
    {
        spawnedBotName = "Bot#" + botNum + "-"
            + chars[Random.Range(0, chars.Length)]
            + chars[Random.Range(0, chars.Length)]
            + chars[Random.Range(0, chars.Length)]
            + chars[Random.Range(0, chars.Length)]
            + chars[Random.Range(0, chars.Length)];
    }

    public void SpawnBot(Vector3 spawnpointPosition)
    {
        //Debug.Log("Bot Spawned");
        GameObject inst = Instantiate(botPrefab, spawnpointPosition, transform.rotation);
        inst.name = spawnedBotName;
        //inst.GetComponent<NetworkObject>().Spawn();
        inst.transform.parent = transform;
    }
}
