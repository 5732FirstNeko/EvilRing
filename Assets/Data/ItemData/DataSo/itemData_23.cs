using UnityEngine;

[CreateAssetMenu(fileName = "Item", menuName = "Data/Item/" + nameof(itemData_23))]
public class itemData_23 : ItemDataSO
{
    public override void Action()
    {
        BattleSystem.instance.OnRoundEnd += OnAction;
    }

    private float OnAction(int round)
    {
        foreach (var unit in
            BattleSystem.instance.FriendlyUnitPlatsQueue.GetAllUnitPlat())
        {
            if (unit != null && unit.unitData != null &&
                unit.unitData != FactorySystem.instance.EmptyFriendlyUnitData && unit.isDead)
            {
                BattleSystem.instance.UnitResurrection(unit);
                BattleSystem.instance.OnRoundEnd -= OnAction;
                break;
            }
        }

        return 1;
    }
}