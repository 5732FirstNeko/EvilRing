using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Item", menuName = "Data/Item/" + nameof(itemData_0))]
public class itemData_0 : ItemDataSO
{
    public override void Action()
    {
        BattleSystem.instance.OnUnitplatDequeue += OnAction;
    }

    private void OnAction(UnitPlat unitPlat)
    {
        if (unitPlat.unit.faction != Faction.Friendly) return;
        BattleSystem.instance.UnitReEnqueue(unitPlat);
        BattleSystem.instance.OnUnitplatDequeue -= OnAction;
    }
}