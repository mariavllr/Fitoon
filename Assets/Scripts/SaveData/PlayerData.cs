using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerData
{
    public int normalCoins;
    public int diamondsCoins;
    public List<int> skinsCompradas;
    //public CharacterItem actualCharacter;
    public CharacterData playerCharacterData;
}
[System.Serializable]
public class CharacterData
{
    public string characterName;
    public string hairColor, skinColor, topColor, bottomColor; //hex color
    //public string shoes; //guid object
}

