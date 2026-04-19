using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Item", menuName = "Data/Item/" + nameof(itemData_29))]
public class itemData_29 : ItemDataSO
{
    public override void Action()
    {
        BattleSystem.instance.OnGameStart += OnAction;
    }

    private float OnAction()
    {
        List<UnitPlat> unitPlats = BattleSystem.instance.FriendlyUnitPlatsQueue.GetAllUnitPlat();

        for (int i = 0; i < unitPlats.Count; i++)
        {
            UnitPlat unit = unitPlats[i];
            if (unit != null || unit.unitData != null ||
                unit.unitData != FactorySystem.instance.EmptyFriendlyUnitData)
            {
                unit.unit.OnHPChange += OnHPChange;
            }
        }

        return 0;
    }

    private void OnHPChange(int HPChange, UnitPlat unit)
    {
        if (HPChange <= 0) return;

        List<UnitPlat> unitplats = BattleSystem.instance.FriendlyUnitPlatsQueue.GetAllUnitPlat();

        for (int i = 0; i < unitplats.Count; i++)
        {
            if (unitplats[i] == unit) continue;

            unitplats[i].unit.HP += HPChange;
        }
    }
}