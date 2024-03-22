using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InitialScreen : MonoBehaviour
{
    [SerializeField] GameObject characterContainer;
    SaveData saveData;
    [SerializeField] List<CharacterItem> characters;
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

        //Actualizar el personaje en pantalla
        Destroy(characterContainer.transform.GetChild(0).gameObject);
        Instantiate(actualCharacter.prefab, Vector3.zero, Quaternion.identity, characterContainer.transform);
    }
}
