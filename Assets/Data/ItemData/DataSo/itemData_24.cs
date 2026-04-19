using UnityEngine;

[CreateAssetMenu(fileName = "Item", menuName = "Data/Item/" + nameof(itemData_24))]
public class itemData_24 : ItemDataSO
{
    public override void Action()
    {
        BattleSystem.instance.OnGameStart += () =>
        {
            foreach (var unit in
            BattleSystem.instance.FriendlyUnitPlatsQueue.GetAllUnitPlat())
            {
                if (unit != null && unit.unitData != null &&
                unit.unitData != FactorySystem.instance.EmptyFriendlyUnitData && unit.isDead)
                {
                    unit.unit.HP -= unit.unit.MaxHP;
                }
            }
            return 0.5f;
        };
        BattleSystem.instance.OnUnitplatDequeue += OnAction;
    }

    private void OnAction(UnitPlat unitPlat)
    {
        if (BattleSystem.instance.currentRound >= 1)
        {
            BattleSystem.instance.OnUnitplatDequeue -= OnAction;
            return;
        }

        if (unitPlat == null || unitPlat.unitData == null ||
                    unitPlat.unitData == FactorySystem.instance.EmptyFriendlyUnitData ||
                    unitPlat.unit.faction != Faction.Friendly)
        {
            return;
        }

        for (int i = 0; i < 3; i++)
        {
            BattleSystem.instance.UnitReEnqueue(unitPlat);
        }
    }
}