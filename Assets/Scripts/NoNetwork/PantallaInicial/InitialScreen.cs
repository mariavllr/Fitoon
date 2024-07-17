using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class InitialScreen : MonoBehaviour
{
    SaveData saveData;
    [SerializeField] GameObject characterContainer;
    [SerializeField] List<CharacterItem> characters;
    [SerializeField] GameObject treadmillPrefab;
    [SerializeField] TMP_InputField inputName;
    private void Start()
    {
        saveData = GetComponent<SaveData>();
        ReadUsername();
        ReadCharacter();
    }

    void ReadUsername()
    {
        if (saveData.player.username == "Username") return;
        else
        {
            inputName.text = saveData.player.username;
        }
    }
    void ReadCharacter()
    {
        //Leer la skin
        string savedSkin = saveData.player.playerCharacterData.characterName;
        if (savedSkin == null)
        {
            print("Error: No hay personaje guardado");
            //instanciar a juan
            Instantiate(characters[0].prefab, Vector3.zero, Quaternion.identity, characterContainer.transform);
            return;
        }
        //Buscar personaje
        CharacterItem actualCharacter = characters.Find(character => character.characterName == savedSkin);

        //Instanciar la cinta de correr
        GameObject treadmill = Instantiate(treadmillPrefab, Vector3.zero, Quaternion.identity, characterContainer.transform);

        //Instanciar el personaje como hijo de la cinta
        Destroy(characterContainer.transform.GetChild(0).gameObject);
        GameObject characterInstance = Instantiate(actualCharacter.prefab, Vector3.zero, Quaternion.identity, treadmill.transform);
        characterInstance.GetComponent<Animator>().SetBool("isRunning", true);
        //Colocar a personaje adecuadamente en la cinta
        characterInstance.transform.Rotate(transform.up, 180f);
        characterInstance.transform.position = new Vector3(0, 0.54f, 1.6f);

        //Para alejarlo un poco de la camara
        characterContainer.transform.position = new Vector3(0, 0, -2.91f);
        characterContainer.transform.Rotate(transform.up, 120f);
    }

    public void SaveUsername(string value)
    {
        saveData.player.username = value;
        saveData.SaveToJson();
    }
}
