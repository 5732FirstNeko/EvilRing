using UnityEngine;

[CreateAssetMenu(fileName = "Item", menuName = "Data/Item/" + nameof(itemData_25))]
public class itemData_25 : ItemDataSO
{
    public override void Action()
    {
        BattleSystem.instance.OnRoundStart += OnAction;
    }

    private float OnAction(int round)
    {
        if (round == 0)
        {
            foreach (var unit in
            BattleSystem.instance.HostilityUnitPlatsQueue.GetAllUnitPlat())
            {
                BattleSystem.instance.UnitRemoveQueue(unit);
            }
        }
        else if (round >= 1)
        {
            foreach (var unit in
            BattleSystem.instance.FriendlyUnitPlatsQueue.GetAllUnitPlat())
            {
                BattleSystem.instance.UnitRemoveQueue(unit);
            }

            BattleSystem.instance.OnRoundStart -= OnAction;
        }


        return 0;
    }
}