using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class UnitBuffDataSo : ScriptableObject
{
    //TODO : UnitBuffDataSo is a base class,
    //TODO : must have The Reflection [CreateAssetMenu] when creat subClass
    //TODO : and the Implementation of Action must be in subClass to maintain Hierarchy structure

    public float BuffActionTime;

    public int ContinuousRound;
    public TriggerTiming TriggerTiming;
    public abstract void OnAction(ICollection<UnitPlat> unit,UnitPlat user);
    public abstract void BindBuff(UnitPlat unitPlat,UnitPlat user,bool isFirstBind = true);
    public abstract void UnbindBuff(UnitPlat unitPlat, UnitPlat user);
}
