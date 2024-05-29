using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InitialScreen : MonoBehaviour
{
    [SerializeField] GameObject characterContainer;
    SaveData saveData;
    [SerializeField] List<CharacterItem> characters;
    [SerializeField] GameObject treadmillPrefab;
    private void Start()
    {
        saveData = GetComponent<SaveData>();
        ReadCharacter();
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
        GameObject treadmill = Instantiate(treadmillPrefab, characterContainer.transform);

        //Instanciar el personaje como hijo de la cinta
        Destroy(characterContainer.transform.GetChild(0).gameObject);
        GameObject characterInstance = Instantiate(actualCharacter.prefab, Vector3.zero, Quaternion.identity, treadmill.transform);
        characterInstance.GetComponent<Animator>().SetBool("isRunning", true);

        //Para alejarlo un poco de la camara
        characterContainer.transform.position = new Vector3(0, 0, -2.91f);
    }
}
