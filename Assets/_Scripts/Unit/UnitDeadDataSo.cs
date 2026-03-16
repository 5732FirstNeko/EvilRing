using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class UnitDeadDataSo : ScriptableObject
{
    //TODO : UnitDeadDataSo is a base class,
    //TODO : must have The Reflection [CreateAssetMenu] when creat subClass
    //TODO : and the Implementation of Action must be in subClass to maintain Hierarchy structure

    public abstract void DeadAction(Unit user);
}
