using UnityEngine;

[CreateAssetMenu(fileName = "Item", menuName = "Data/Item/" + nameof(itemData_19))]
public class itemData_19 : ItemDataSO
{
    public override void Action()
    {
        BattleSystem.instance.OnUnitplatDequeue += OnAction;
    }

    private void OnAction(UnitPlat unitPlat)
    {
        if (BattleSystem.instance.currentRound == 1)
        {
            BattleSystem.instance.UnitReEnqueue(unitPlat);
        }
        else if (BattleSystem.instance.currentRound >= 2)
        {
            BattleSystem.instance.OnUnitplatDequeue -= OnAction;
        }
    }
}