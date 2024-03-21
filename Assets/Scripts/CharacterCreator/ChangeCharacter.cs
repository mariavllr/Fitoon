using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEditor;

public class ChangeCharacter : MonoBehaviour
{
    public CharacterItem actualCharacter; //el que sale mostrado actualmente en el creador de personajes
    public CharacterItem playerSavedCharacter; //el que elige y tiene guardado


    [SerializeField] GameObject container;
    [SerializeField] List<CharacterItem> characters;
    int characterActive = 0;
    [SerializeField] TextMeshProUGUI nameText;

    SaveData saveData;

    private void Awake()
    {
        //Leer el personaje guardado
        /* saveData = GetComponent<SaveData>();
         saveData.ReadFromJson();
         CharacterDataToScriptableObject();



         //Actualizar el personaje en pantalla
         Destroy(container.transform.GetChild(0).gameObject);
         Instantiate(characters[characterActive].prefab, Vector3.zero, Quaternion.identity, container.transform);
         nameText.text = characters[characterActive].characterName;



         print(characterActive);*/
        actualCharacter = characters[0];
        
        //Para hacer reset de todos los personajes al darle al play
        foreach (CharacterItem character in characters)
        {
            character.hairColor = character.hair.color;
            character.skinColor = character.skin.color;
            character.topColor = character.top.color;
            character.bottomColor = character.bottom.color;
        }
    }

    /*public void CharacterDataToScriptableObject()
    {
        //Leer el nombre. Con esto puedo buscar el material y el prefab
        actualCharacter.characterName = saveData.player.playerCharacterData.characterName;

        //Buscar en qué índice de la lista de personajes está, segun el NOMBRE del personaje
        characterActive = characters.FindIndex(character => character.characterName == actualCharacter.characterName);

        //Asignar el prefab y los materiales
        actualCharacter.prefab = characters[characterActive].prefab;
        actualCharacter.hair = characters[characterActive].hair;
        actualCharacter.skin = characters[characterActive].skin;
        actualCharacter.top = characters[characterActive].top;
        actualCharacter.bottom = characters[characterActive].bottom;

        //Leer los colores
        Color color = Color.black; //si falla saldrá negro
        if(ColorUtility.TryParseHtmlString(saveData.player.playerCharacterData.hairColor, out color))
        {
            actualCharacter.hairColor = color;
        }
        if (ColorUtility.TryParseHtmlString(saveData.player.playerCharacterData.skinColor, out color))
        {
            actualCharacter.skinColor = color;
        }
        if (ColorUtility.TryParseHtmlString(saveData.player.playerCharacterData.bottomColor, out color))
        {
            actualCharacter.bottomColor = color;
        }
        if (ColorUtility.TryParseHtmlString(saveData.player.playerCharacterData.topColor, out color))
        {
            actualCharacter.topColor = color;
        }


    }*/

    public void OnArrowClicked(string direction)
    {
        Destroy(container.transform.GetChild(0).gameObject);

        if (direction == "RIGHT")
        {
            characterActive++;

            if (characterActive == characters.Count)
            {
                characterActive = 0;
            }
        }

        else if (direction == "LEFT")
        {
            characterActive--;

            if (characterActive < 0)
            {
                characterActive = characters.Count - 1;
            }
        }

        Instantiate(characters[characterActive].prefab, Vector3.zero, Quaternion.identity, container.transform);
        nameText.text = characters[characterActive].characterName;

        actualCharacter = characters[characterActive];
    }

    public void ResetCharacter()
    {
        characters[characterActive].hair.color = characters[characterActive].hairColor;
        characters[characterActive].skin.color = characters[characterActive].skinColor;
        characters[characterActive].top.color = characters[characterActive].topColor;
        characters[characterActive].bottom.color = characters[characterActive].bottomColor;

        GameObject[] shoes = GameObject.FindGameObjectsWithTag("Shoes");

        foreach (GameObject shoe in shoes)
        {
            SkinnedMeshRenderer renderer = shoe.GetComponent<SkinnedMeshRenderer>();
            renderer.sharedMesh = characters[characterActive].shoes.mesh;
            renderer.materials = characters[characterActive].shoes.materials;
        }

    }

    public void SaveCharacter()
    {
        //Se guarda el nombre
        playerSavedCharacter.characterName = characters[characterActive].characterName;
        //Se guarda la skin
        playerSavedCharacter.prefab = characters[characterActive].prefab;
        //Se guarda el mismo material que el original. De esta manera si se vuelve a entrar al editor, el material estará actualizado
        playerSavedCharacter.hair = characters[characterActive].hair;
        playerSavedCharacter.skin = characters[characterActive].skin;
        playerSavedCharacter.top = characters[characterActive].top;
        playerSavedCharacter.bottom = characters[characterActive].bottom;
        //Se guardan los nuevos colores
        playerSavedCharacter.hairColor = playerSavedCharacter.hair.color;
        playerSavedCharacter.skinColor = playerSavedCharacter.skin.color;
        playerSavedCharacter.topColor = playerSavedCharacter.top.color;
        playerSavedCharacter.bottomColor = playerSavedCharacter.bottom.color;
        //Se guardan las zapatillas
        //characterToSave.shoes = 


        saveData.player.playerCharacterData.characterName = characters[characterActive].characterName;
        saveData.player.playerCharacterData.hairColor = ColorToHex(playerSavedCharacter.hairColor);
        saveData.player.playerCharacterData.skinColor = ColorToHex(playerSavedCharacter.skinColor);
        saveData.player.playerCharacterData.topColor = ColorToHex(playerSavedCharacter.topColor);
        saveData.player.playerCharacterData.bottomColor = ColorToHex(playerSavedCharacter.bottomColor);
        saveData.SaveToJson();

    }

    public static string ColorToHex(Color color)
    {
        // Convert RGB values to hexadecimal format
        int r = (int)(color.r * 255f);
        int g = (int)(color.g * 255f);
        int b = (int)(color.b * 255f);

        // Format the hexadecimal string
        string hex = string.Format("#{0:X2}{1:X2}{2:X2}", r, g, b);

        return hex;
    }

}
