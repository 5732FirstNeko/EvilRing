using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "newUnitData", menuName = "Data/Dead Data/" + nameof(WarlockDead_standrd))]
public class WarlockDead_standrd : UnitDeadDataSo
{
    public override void DeadAction(UnitPlat user)
    {
        StandrdDead(user);
    }
}
