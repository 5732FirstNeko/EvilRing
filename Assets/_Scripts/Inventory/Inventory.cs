using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;



public class Inventory : MonoBehaviour, IPointerClickHandler
{
    public ItemSO itemSO;
    public Image itemImage;
    private InventoryManager inventoryManager;


    private void Start()
    {
        inventoryManager = GetComponentInParent<InventoryManager>();
        UpdateUI();
    }



    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {

            inventoryManager.UseItem(this);
            Debug.Log("Use Item");
        }
        // else if (eventData.button == PointerEventData.InputButton.Right)
        // {
        //     inventoryManager.DropItem(this);
        // }
        
    }

    public void UpdateUI()
    {

        if (itemSO != null)
        {
            itemImage.sprite = itemSO.itemIcon;
            itemImage.gameObject.SetActive(true);

        }
        else
        {
            itemImage.gameObject.SetActive(false);

        }
    }
}
