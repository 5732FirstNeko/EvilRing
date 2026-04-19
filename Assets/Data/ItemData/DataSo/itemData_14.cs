using UnityEngine;

[CreateAssetMenu(fileName = "Item", menuName = "Data/Item/" + nameof(itemData_14))]
public class itemData_14 : ItemDataSO
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

        foreach (var unit in
            BattleSystem.instance.FriendlyUnitPlatsQueue.GetAllUnitPlat())
        {
            BattleSystem.instance.UnitRemoveQueue(unit);
        }
        BattleSystem.instance.OnRoundStart -= OnAction;

        return 0;
    }
}