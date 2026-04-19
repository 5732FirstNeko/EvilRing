using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Item", menuName = "Data/Item/" + nameof(itemData_34))]
public class itemData_34 : ItemDataSO
{
    public override void Action()
    {
        BattleSystem.instance.OnUnitplatDequeue += OnAction;
    }

    private void OnAction(UnitPlat unitPlat)
    {
        int count = 0;
        List<UnitPlat> unitplats = BattleSystem.instance.FriendlyUnitPlatsQueue.GetAllUnitPlat();
        for (int i = 0; i < unitplats.Count; i++)
        {
            if (unitplats[i].unitData != null ||
                unitplats[i].unitData != FactorySystem.Instance.EmptyFriendlyUnitData)
            {
                count++;
            }
        }

        count = BattleSystem.unitPlatQueueCount - count;

        for (int i = 0; i < count; i++)
        {
            BattleSystem.instance.UnitReEnqueue(unitPlat);
        }
    }
}