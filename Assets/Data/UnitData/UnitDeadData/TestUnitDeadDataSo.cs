using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "newUnitData", menuName = "Data/Dead Data/" + nameof(TestUnitDeadDataSo))]
public class TestUnitDeadDataSo : UnitDeadDataSo
{
    public override void DeadAction(Unit user)
    {
        Debug.Log("Unit Dead!");
    }
}
