using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "newUnitData", menuName = "Data/Skill Data/" + nameof(SkillData_2))]
public class SkillData_2 : UnitSkillDataSo
{
    public override void Action(ICollection<UnitPlat> unitPlats, UnitPlat user)
    {
        DamageHurt(unitPlats, Damage);
        int targetindex = BattleSystem.GetIndexByUnitSite(user.site);
        BattleSystem.instance.UnitSiteChange(user.unit.faction, user.site,
            BattleSystem.GetUnitSiteByIndex(
                (targetindex - 1 + BattleSystem.unitPlatQueueCount) % BattleSystem.unitPlatQueueCount));
    }
}
