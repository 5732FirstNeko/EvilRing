using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "newUnitData", menuName = "Data/Skill Data/" + nameof(TestSkillDataSo))]
public class TestSkillDataSo : UnitSkillDataSo
{
    public override void Action(ICollection<Unit> units, Unit user)
    {
        UnitSite site = BattleSystem.instance.GetUnitSiteByUnit(user.faction, user);

        Debug.Log(user.faction + " " + user + " " + site + " " + UnitSite.second);

        BattleSystem.instance.UnitSiteChange(user.faction, site, UnitSite.second);
    }
}
