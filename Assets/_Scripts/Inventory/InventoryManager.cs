using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance;

    public int gold;
    // public UseItem useItem;
    public TextMeshProUGUI goldText;
    public Inventory[] itemSlots;



    private void Awake()
{
    if (Instance == null)
    {
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
    else
    {
        Destroy(gameObject);
    }
}

    private void Start()
    {
        foreach (var slot in itemSlots)
            {
                slot.UpdateUI();
            }
        goldText.text = "Gold : " + gold.ToString();
    }

    private void OnEnable()
    {
        Loot.OnItemLooted += AddItem;
    }

    private void OnDisable()
    {
        Loot.OnItemLooted -= AddItem;
    }

    public void AddItem(ItemSO itemSO)
    {
            foreach (var slot in itemSlots)
            {
                if (slot.itemSO == null)
                {
                    slot.itemSO = itemSO;
                    slot.UpdateUI();
                    return;
                }
            }

    }



    public void DropItem(Inventory slot)
    {
        if (slot.itemSO != null)
        {
            slot.itemSO = null;
            slot.UpdateUI();
        }
    }





    public void UseItem(Inventory slot)
    {
        if (slot.itemSO != null)
        {
            // useItem.ApplyItemEffects(slot.itemSO);
            slot.itemSO = null;
            slot.UpdateUI();
            Debug.Log("Use Item");

        }
        
    }

    // public bool HasItem(ItemSO itemSO)
    // {
    //     foreach (var slot in itemSlots)
    //     {
    //         if (slot.itemSO == itemSO)
    //         {
    //             return true;
    //         }
    //     }
    //     return false;
    // }

//     public int GetItemQuantity(ItemSO itemSO)
// {
//     int total = 0;

//     foreach (var slot in itemSlots)
//     {
//         if (slot.itemSO == itemSO)
//             total += slot.quantity;
//     }

//     return total;
// }
}
