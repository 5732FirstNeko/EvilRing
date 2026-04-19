using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Item", menuName = "Data/Item/" + nameof(itemData_33))]
public class itemData_33 : ItemDataSO
{
    public override void Action()
    {
        BattleSystem.instance.OnGameStart += OnAction;
    }

    private float OnAction()
    {
        int count = 0;
        List<UnitPlat> unitplats = BattleSystem.instance.FriendlyUnitPlatsQueue.GetAllUnitPlat();
        for (int i = 0; i < unitplats.Count; i++)
        {
            if (unitplats[i].unitData != null &&
                unitplats[i].unitData != FactorySystem.Instance.EmptyFriendlyUnitData)
            {
                count++;
            }
        }

        for (int i = 0; i < unitplats.Count; i++)
        {
            if (unitplats[i].unitData != null &&
                unitplats[i].unitData != FactorySystem.Instance.EmptyFriendlyUnitData)
            {
                unitplats[i].unit.MaxHP += Mathf.RoundToInt(unitplats[i].unit.MaxHP * 0.1f * count);
                unitplats[i].unit.HP = unitplats[i].unit.MaxHP;
            }
        }

        return 0;
    }
}