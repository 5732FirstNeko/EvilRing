using UnityEngine;

[CreateAssetMenu(fileName = "Item", menuName = "Data/Item/" + nameof(itemData_17))]
public class itemData_17 : ItemDataSO
{
    public override void Action()
    {
        BattleSystem.instance.OnUnitplatDequeue += OnAction;
    }

    private void OnAction(UnitPlat unitPlat)
    {
        if (BattleSystem.instance.currentRound < 0)
        {
            return;
        }
        else if (BattleSystem.instance.currentRound == 0 && unitPlat.unit.faction == Faction.Friendly)
        {
            BattleSystem.instance.UnitReEnqueue(unitPlat);
        }
        else if (BattleSystem.instance.currentRound >= 1 && unitPlat.unit.faction == Faction.Hostility)
        {
            BattleSystem.instance.UnitReEnqueue(unitPlat);
            BattleSystem.instance.OnUnitplatDequeue -= OnAction;
        }
    }
}