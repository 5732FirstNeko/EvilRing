using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Item", menuName = "Data/Item")]
public class ItemDataSO : ScriptableObject
{
    public string itemName;
    public Sprite itemIcon;
    [TextArea(3, 5)] public string itemDescription;

    public ItemBuffType itemType;
    public UnitBuffDataSo itemBuff;
}

public enum ItemBuffType
{
    Unit,
    Global
}
