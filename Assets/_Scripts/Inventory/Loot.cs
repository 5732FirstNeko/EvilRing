using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Loot : MonoBehaviour
{
    public ItemSO itemSO;
    public static event Action<ItemSO> OnItemLooted;
    public SpriteRenderer sr;

    private void OnValidate()
    {
        if (itemSO == null)
        return;
        UpdateAppearance();
        OnDestroy();
    }

    
    private void UpdateAppearance()
    {
        sr.sprite = itemSO.itemIcon;
        this.name = itemSO.itemName;
    }

    // private void OnTriggerEnter2D(Collider2D collision)
    // {

    //         OnItemLooted?.Invoke(itemSO);
    //         Destroy(gameObject,0.5f);

    // }

    private void OnDestroy()
    {
        OnItemLooted?.Invoke(itemSO);
        Destroy(gameObject,0.5f);
        Debug.Log("Loot Destroyed");
    }

}
