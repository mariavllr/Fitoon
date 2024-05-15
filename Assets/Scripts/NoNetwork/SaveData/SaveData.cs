using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Android;

public class SaveData : MonoBehaviour
{
    public PlayerData player;

    
    private void Awake()
    {
        player  = new PlayerData();
        // Create directory if not exists
        if (!Directory.Exists(Application.persistentDataPath))
        {
            Directory.CreateDirectory(Application.persistentDataPath);
            Debug.Log("DIRECTORY DOES NOT EXIST: CREATING DIRECTORY");
        }
        ReadFromJson();
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.S))
        {
            SaveToJson();
        }

        if (Input.GetKeyDown(KeyCode.L))
        {
            ReadFromJson();
        }
    }
    public void SaveToJson()
    {
        string playerData = JsonUtility.ToJson(player);
        string filePath = System.IO.Path.Combine(Application.persistentDataPath,"PlayerData.json");
        System.IO.File.WriteAllText(filePath, playerData);
        Debug.Log("Datos guardados en " + filePath);
    }

    public void ReadFromJson()
    {
        string filePath = System.IO.Path.Combine(Application.persistentDataPath, "PlayerData.json");
        string playerData = System.IO.File.ReadAllText(filePath);
        Debug.Log(filePath);

        player = JsonUtility.FromJson<PlayerData>(playerData);
        Debug.Log("Datos leidos");

    }
}
