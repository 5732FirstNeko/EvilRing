using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Item", menuName = "Data/Item/" + nameof(itemData_28))]
public class itemData_28 : ItemDataSO
{
    public override void Action()
    {
        BattleSystem.instance.OnRoundStart += OnAction;
    }

    private float OnAction(int round)
    {
        if (round <= -1) return 0;

        List<UnitPlat> plats = BattleSystem.instance.HostilityUnitPlatsQueue.GetAllUnitPlat();
        for (int i = 0; i < plats.Count; i++)
        {
            for (int j = 0; j < plats[i].unit.unitSkills.Count; j++)
            {
                switch (plats[i].unit.unitSkills[j].SkillTarget)
                {
                    case Faction.Friendly:
                        plats[i].unit.unitSkills[j].SkillTarget = Faction.Hostility;
                        break;
                    case Faction.Hostility:
                        plats[i].unit.unitSkills[j].SkillTarget = Faction.Friendly;
                        break;
                }
            }
        }

        return 0;
    }
}