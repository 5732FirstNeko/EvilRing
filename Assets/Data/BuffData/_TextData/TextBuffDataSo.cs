using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "newUnitData", menuName = "Data/Buff Data/" + nameof(TextBuffDataSo))]
public class TextBuffDataSo : UnitBuffDataSo
{
    public override void BindBuff(UnitPlat unit, UnitPlat user, bool isFirstBind = true)
    {
        //Debug.Log("BindBuff Seccessed!");
    }

    public override void OnAction(ICollection<UnitPlat> unitPlats, UnitPlat user)
    {
        //Debug.Log("Buff Action!");
    }

    public override void UnbindBuff(UnitPlat unit, UnitPlat user)
    {
        //Debug.Log("UnBindBuff Seccessed!");
    }
}
