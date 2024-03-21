using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveData : MonoBehaviour
{
    public PlayerData player = new PlayerData();
    
    private void Awake()
    {
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
        string filePath = Application.persistentDataPath + "/PlayerData.json";
        System.IO.File.WriteAllText(filePath, playerData);
        Debug.Log("Datos guardados en " + filePath);
    }

    public void ReadFromJson()
    {
        string filePath = Application.persistentDataPath + "/PlayerData.json";
        string playerData = System.IO.File.ReadAllText(filePath);

        player = JsonUtility.FromJson<PlayerData>(playerData);
        Debug.Log("Datos leidos");

    }
}
