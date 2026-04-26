using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "newUnitData", menuName = "Data/Dead Data/" + nameof(NecroDead_skeletal))]
public class NecroDead_skeletal : UnitDeadDataSo
{
    public override void DeadAction(UnitPlat user)
    {
        StandrdDead(user);
        user.unit.DeadAnimationTime = 1f;
    }
}
