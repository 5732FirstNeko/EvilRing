using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "newUnitData", menuName = "Data/UnitBuff Data/" + nameof(UnitBuffData_0))]
public class UnitBuffData_0 : UnitBuffDataSo
{
    [SerializeField] private int damage;

    public override void BindBuff(UnitPlat unitPlat, UnitPlat user, bool isFirstBind = true)
    {
        
    }

    public override void OnAction(ICollection<UnitPlat> unit, UnitPlat user)
    {
        UnitSkillDataSo.DamageHurt(unit, damage);
    }

    public override void UnbindBuff(UnitPlat unitPlat, UnitPlat user)
    {
        
    }
}
