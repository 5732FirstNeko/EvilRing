using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "newUnitData", menuName = "Data/Dead Data/" + nameof(DaimonDead))]
public class DaimonDead : UnitDeadDataSo
{
    public override void DeadAction(UnitPlat user)
    {
        StandrdDead(user);
    }
}
