using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public abstract class ItemDataSO : ScriptableObject
{
    public string itemName;
    public Sprite itemIcon;
    [TextArea(3, 5)] public string itemDescription;

    public ItemBuffType itemType;

    public virtual void Action() { }

    public virtual void Action(UnitPlat unitPlat) { }
}

public enum ItemBuffType
{
    FriendlyUnit,
    HostitlyUnit,
    Global
}
