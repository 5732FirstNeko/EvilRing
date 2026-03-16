using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "newUnitData", menuName = "Data/Buff Data/" + nameof(TextBuffDataSo))]
public class TextBuffDataSo : UnitBuffDataSo
{
    public override void BindBuff(Unit unit, Unit user, bool isFirstBind = true)
    {
        //Debug.Log("BindBuff Seccessed!");
    }

    public override void OnAction(ICollection<Unit> unit, Unit user)
    {
        //Debug.Log("Buff Action!");
    }

    public override void UnbindBuff(Unit unit, Unit user)
    {
        //Debug.Log("UnBindBuff Seccessed!");
    }
}
