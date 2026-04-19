using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Item", menuName = "Data/Item/" + nameof(itemData_26))]
public class itemData_26 : ItemDataSO
{
    public override void Action()
    {
        BattleSystem.instance.OnGameStart += OnAction;
    }

    private float OnAction()
    {
        List<UnitPlat> unitPlats = BattleSystem.instance.FriendlyUnitPlatsQueue.GetAllUnitPlat();

        int index = Random.Range(0, unitPlats.Count);

        UnitPlat plat = BattleSystem.instance.FriendlyUnitPlatsQueue.GetAllUnitPlat()[index];
        if (plat == null || plat.unitData == null ||
            plat.unitData == FactorySystem.instance.EmptyFriendlyUnitData)
        {
            for (int i = 0; i < unitPlats.Count; i++)
            {
                UnitPlat unit = unitPlats[i];
                if (unit != null || unit.unitData != null ||
                    unit.unitData != FactorySystem.instance.EmptyFriendlyUnitData)
                {
                    plat = unit;
                }
            }
        }

        for (int i = 0; i < unitPlats.Count; i++)
        {
            if (unitPlats[i] != plat)
            {
                unitPlats[i].unit.speed -= 1;
            }
        }

        BattleSystem.instance.OnGameStart -= OnAction;

        return 0;
    }
}