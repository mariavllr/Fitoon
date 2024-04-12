using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Android;

public class SaveData : MonoBehaviour
{
    public PlayerData player = new PlayerData();

    
    private void Awake()
    {
        // Create directory if not exists
        if (!Directory.Exists(Application.persistentDataPath))
        {
            Directory.CreateDirectory(Application.persistentDataPath);
            Debug.Log("DIRECTORY DOES NOT EXIST: CREATING DIRECTORY");
        }
        Debug.Log("--FIRST CHECK--");
        CheckStoragePermission();
        RequestStoragePermission();
        Debug.Log("--SECOND CHECK--");
        CheckStoragePermission();
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
        string filePath = Application.persistentDataPath + Path.DirectorySeparatorChar + "PlayerData.json";
        System.IO.File.WriteAllText(filePath, playerData);
        Debug.Log("Datos guardados en " + filePath);
    }

    public void ReadFromJson()
    {
        string filePath = Application.persistentDataPath + Path.DirectorySeparatorChar + "PlayerData.json";
        string playerData = System.IO.File.ReadAllText(filePath);

        player = JsonUtility.FromJson<PlayerData>(playerData);
        Debug.Log("Datos leidos");

    }

    // Request external storage permissions
    private void RequestStoragePermission()
    {
        if (!Permission.HasUserAuthorizedPermission(Permission.ExternalStorageRead) ||
            !Permission.HasUserAuthorizedPermission(Permission.ExternalStorageWrite))
        {
            Debug.Log("------------------PERMISION ASKED-----------------");
            Permission.RequestUserPermission(Permission.ExternalStorageRead);
            Permission.RequestUserPermission(Permission.ExternalStorageWrite);
        }
    }

    private void CheckStoragePermission()
    {
        if (!Permission.HasUserAuthorizedPermission(Permission.ExternalStorageRead) ||
            !Permission.HasUserAuthorizedPermission(Permission.ExternalStorageWrite))
        {
            Debug.Log("CHECK PERMISIONS: DOES NOT HAVE PERMISSION!!!!!!!!!!!!!!!!");
        }

        else
        {
            Debug.Log("CHECK PERMISIONS: HAVE PERMISSIONS!!!!!!!!!!!!!!!!");
        }
    }
}
