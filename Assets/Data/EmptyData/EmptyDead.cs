using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "newUnitData", menuName = "Data/Dead Data/" + nameof(EmptyDead))]
public class EmptyDead : UnitDeadDataSo
{
    public override void DeadAction(UnitPlat user)
    {
        
    }
}
