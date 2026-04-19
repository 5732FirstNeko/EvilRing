using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Item", menuName = "Data/Item/" + nameof(itemData_32))]
public class itemData_32 : ItemDataSO
{
    public override void Action()
    {
        BattleSystem.instance.OnRoundStart += OnAction;
    }

    private float OnAction(int round)
    {
        if (round == 0)
        {
            List<UnitPlat> unitplats = BattleSystem.instance.FriendlyUnitPlatsQueue.GetAllUnitPlat();
            for (int i = 0; i < unitplats.Count; i++)
            {
                BattleSystem.instance.UnitRemoveQueue(unitplats[i]);
            }

            BattleSystem.instance.OnRoundStart -= OnAction;
        }

        return 0;
    }
}