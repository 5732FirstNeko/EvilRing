using UnityEngine;

[CreateAssetMenu(fileName = "Item", menuName = "Data/Item/" + nameof(itemData_5))]
public class itemData_5 : ItemDataSO
{
    public override void Action()
    {
        BattleSystem.instance.OnRoundStart += OnAction;
    }

    private float OnAction(int round)
    {
        foreach (var unit in
            BattleSystem.instance.FriendlyUnitPlatsQueue.GetAllUnitPlat())
        {
            BattleSystem.instance.UnitRemoveQueue(unit);
        }
        BattleSystem.instance.OnRoundStart -= OnAction;
        return 0;
    }
}