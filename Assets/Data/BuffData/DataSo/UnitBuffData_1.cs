using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "newUnitData", menuName = "Data/UnitBuff Data/" + nameof(UnitBuffData_1))]
public class UnitBuffData_1 : UnitBuffDataSo
{
    [SerializeField] private int receive;
    public override void BindBuff(UnitPlat unitPlat, UnitPlat user, bool isFirstBind = true)
    {
        
    }

    public override void OnAction(ICollection<UnitPlat> unit, UnitPlat user)
    {
        Debug.Log("Buff_1 Action HPReceive!");
        UnitSkillDataSo.HPReceive(unit, receive);
    }

    public override void UnbindBuff(UnitPlat unitPlat, UnitPlat user)
    {
        
    }
}
