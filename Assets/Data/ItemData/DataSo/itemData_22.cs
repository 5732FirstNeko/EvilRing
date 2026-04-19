using UnityEngine;

[CreateAssetMenu(fileName = "Item", menuName = "Data/Item/" + nameof(itemData_22))]
public class itemData_22 : ItemDataSO
{
    public override void Action()
    {
        BattleSystem.instance.OnGameStart += () =>
        {
            foreach (var unit in
            BattleSystem.instance.HostilityUnitPlatsQueue.GetAllUnitPlat())
            {
                if (unit.unit.speed < 5)
                {
                    foreach (var skill in unit.unit.unitSkills)
                    {
                        skill.Damage /= 2;
                    }
                }
            }
            return 0;
        };
        BattleSystem.instance.OnRoundStart += OnAction;
    }

    private float OnAction(int idnex)
    {
        foreach (var unit in
            BattleSystem.instance.HostilityUnitPlatsQueue.GetAllUnitPlat())
        {
            if (unit.unit.speed > 5)
            {
                BattleSystem.instance.UnitRemoveQueue(unit);
            }
        }

        return 0;
    }
}