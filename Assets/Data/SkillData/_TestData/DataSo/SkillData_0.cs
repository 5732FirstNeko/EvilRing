using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "newUnitData", menuName = "Data/Skill Data/" + nameof(SkillData_0))]
public class SkillData_0 : UnitSkillDataSo
{
    public override void Action(ICollection<UnitPlat> unitPlats, UnitPlat user)
    {
        DamageHurt(unitPlats, Damage);
    }
}
