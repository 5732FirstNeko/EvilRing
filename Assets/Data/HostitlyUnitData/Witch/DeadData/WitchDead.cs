using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "newUnitData", menuName = "Data/Dead Data/" + nameof(WitchDead))]
public class WitchDead : UnitDeadDataSo
{
    public override void DeadAction(UnitPlat user)
    {
        StandrdDead(user);
    }
}
