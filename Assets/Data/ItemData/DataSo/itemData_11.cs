using UnityEngine;

[CreateAssetMenu(fileName = "Item", menuName = "Data/Item/" + nameof(itemData_11))]
public class itemData_11 : ItemDataSO
{
    public override void Action()
    {
        FriendlyUnitUI unitUI = null;
        foreach (var unit in UnitCardSystem.instance.friendlyUnitRefreshArea)
        {
            if (unitUI == null)
            {
                unitUI = unit;
                continue;
            }

            if (unitUI.unitData.HP < unit.unitData.HP)
            {
                unitUI = unit;
            }
        }

        unitUI.isLock = true;
        BattleSystem.instance.OnGameStart += OnAction;
    }

    public float OnAction()
    {
        foreach (var unit in
            BattleSystem.instance.FriendlyUnitPlatsQueue.GetAllUnitPlat())
        {
            unit.unit.MaxHP *= 2;
            unit.unit.HP *= 2;
        }

        BattleSystem.instance.OnGameStart -= OnAction;

        return 0;
    }
}