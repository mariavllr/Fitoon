using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using static UnityEditor.Progress;
public class Tienda : MonoBehaviour
{
    public List<IconTienda> itemsTienda;
    public GameObject iconPrefab;
    public GameObject gameManager;
    SaveData save;
    public GameObject container;
    ScrollRect scrollRect;
    RectTransform item;
    void Start()
    {
        save = gameManager.GetComponent<SaveData>();
        CleanShop();
        CreateShop();
        scrollRect = FindObjectOfType<ScrollRect>();
        item = container.GetComponent<RectTransform>();
    }

    private void CleanShop()
    {
        for(int i = 0; i < container.transform.childCount; i++)
        {
            Destroy(container.transform.GetChild(i).gameObject);
        }
    }
    private void CreateShop()
    {
        //Create skins
        for(int i = 0; i < itemsTienda.Count; i++)
        {
            GameObject iconoCreado = Instantiate(iconPrefab, container.transform);
            //Primer hijo: Boton. Hijos (en orden): Texto monedas y Icono monedas
            iconoCreado.transform.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().text = itemsTienda[i].itemPrice.ToString();
            iconoCreado.transform.GetChild(0).GetChild(1).GetComponent<Image>().sprite = itemsTienda[i].coinType;
            //segundo hijo: icono 3D
            Transform iconTransform = iconoCreado.transform.GetChild(1).GetChild(0).transform;
            //destruir hijo que ya existe
            Destroy(iconoCreado.transform.GetChild(1).GetChild(0).gameObject);
            Instantiate(itemsTienda[i].objectIcon, iconTransform.position, iconTransform.rotation, iconoCreado.transform.GetChild(1));
            //tercer hijo: nombre
            iconoCreado.transform.GetChild(2).gameObject.GetComponentInChildren<TextMeshProUGUI>().text = itemsTienda[i].itemName;
            //añadir id
            itemsTienda[i].itemID = i;
        }
    }

    public void ScrollUntilItem()
    {
        StartCoroutine(ScrollViewFocusFunctions.FocusOnItemToLeftCoroutine(scrollRect, item, 0.5f));
    }

}
