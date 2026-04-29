using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public abstract class UnitDeadDataSo : ScriptableObject
{
    protected static Color DeadColor = new Color(0.5f, 0.5f, 0.5f, 1);

    public virtual void PrefabInit() { }

    public abstract void DeadAction(UnitPlat user);

    protected void StandrdDead(UnitPlat user)
    {
        user.iconSpriteRender.DOColor(DeadColor, 1f);

        if (user.unit.faction == Faction.Friendly)
        {
            return;
        }

        InventoryManager.instance.gold += Random.Range(3, 8);
        ItemDataSO item = FactorySystem.instance.GetRandomItem();
        if (item != null) 
        {
            InventoryManager.instance.AddInventoryToList(item);
        }
    }
}
