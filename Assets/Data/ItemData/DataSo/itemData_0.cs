using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Item", menuName = "Data/Item/" + nameof(itemData_0))]
public class itemData_0 : ItemDataSO
{
    public override void Action()
    {
        BattleSystem.instance.OnRoundStart += OnAction;
    }

    private float OnAction(int round)
    {
        if (round > 0)
        {
            BattleSystem.instance.OnRoundStart -= OnAction;
            return 0;
        }

        UnitPlat target = null;
        foreach (var unit in BattleSystem.instance.FriendlyUnitPlatsQueue.GetAllUnitPlat())
        {
            if (!target.isDead && target.unitData != FactorySystem.instance.EmptyFriendlyUnitData)
            {
                target = unit;
                break;
            }
        }

        BattleSystem.instance.UnitReEnqueue(target);
        BattleSystem.instance.OnRoundStart -= OnAction;
        return 0;
    }
}