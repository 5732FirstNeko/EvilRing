using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "newUnitData", menuName = "Data/Skill Data/" + nameof(TestSkillDataSo))]
public class TestSkillDataSo : UnitSkillDataSo
{
    public override void Action(ICollection<UnitPlat> units, UnitPlat user)
    {
        UnitSite site = BattleSystem.instance.GetUnitSiteByUnit(user.unit.faction, user.unit);

        BattleSystem.instance.UnitSiteChange(user.unit.faction, site, UnitSite.second);
    }
}
