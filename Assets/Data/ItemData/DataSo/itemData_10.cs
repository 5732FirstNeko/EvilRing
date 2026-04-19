using UnityEngine;

[CreateAssetMenu(fileName = "Item", menuName = "Data/Item/" + nameof(itemData_10))]
public class itemData_10 : ItemDataSO
{
    public override void Action()
    {
        BattleSystem.instance.OnGameStart += OnAction;
    }

    public float OnAction()
    {
        foreach (var unit in BattleSystem.Instance.
            HostilityUnitPlatsQueue.GetAllUnitPlat())
        {
            foreach (var skill in unit.unit.unitSkills)
            {
                if (skill.SkillTarget == Faction.Hostility)
                {
                    skill.SkillTarget = Faction.Friendly;
                }
            }
        }

        foreach (var unit in BattleSystem.Instance.
            FriendlyUnitPlatsQueue.GetAllUnitPlat())
        {
            foreach (var skill in unit.unit.unitSkills)
            {
                if (skill.SkillTarget == Faction.Hostility)
                {
                    skill.SkillTarget = Faction.Friendly;
                }
            }
        }

        BattleSystem.instance.OnGameStart -= OnAction;
        return 0;
    }
}