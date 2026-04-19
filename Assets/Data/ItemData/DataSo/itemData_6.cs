using UnityEngine;

[CreateAssetMenu(fileName = "Item", menuName = "Data/Item/" + nameof(itemData_6))]
public class itemData_6 : ItemDataSO
{
    public override void Action()
    {
        BattleSystem.instance.OnUnitplatDequeue += OnAction;
    }

    private void OnAction(UnitPlat unit)
    {
        if (BattleSystem.instance.currentRound >= 1)
        {
            BattleSystem.instance.OnUnitplatDequeue -= OnAction;
            return;
        }

        if (unit == null || unit.unitData == null ||
            unit.unitData == FactorySystem.instance.EmptyFriendlyUnitData ||
            unit.unit.faction != Faction.Friendly)
        {
            return;
        }

        for (int i = 0; i < 3; i++)
        {
            BattleSystem.instance.UnitReEnqueue(unit);
        }
    }
}