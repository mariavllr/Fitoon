using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UpdateMoney : MonoBehaviour
{
    public TextMeshProUGUI coinsText;
    //public TextMeshProUGUI diamondsText;
    SaveData save;

    private void Start()
    {
        save = GetComponent<SaveData>();
        UpdateMoneyText();
    }

    public void UpdateMoneyText()
    {
        coinsText.text = save.player.normalCoins.ToString();
        //diamondsText.text = save.player.diamondsCoins.ToString();
    }

}
