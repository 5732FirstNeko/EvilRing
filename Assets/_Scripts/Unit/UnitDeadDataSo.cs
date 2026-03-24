using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class UnitDeadDataSo : ScriptableObject
{
    public abstract void DeadAction(UnitPlat user);
}
