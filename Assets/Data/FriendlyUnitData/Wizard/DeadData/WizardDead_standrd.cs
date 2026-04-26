using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "newUnitData", menuName = "Data/Dead Data/" + nameof(WizardDead_standrd))]
public class WizardDead_standrd : UnitDeadDataSo
{
    public override void DeadAction(UnitPlat user)
    {
        StandrdDead(user);
    }
}
