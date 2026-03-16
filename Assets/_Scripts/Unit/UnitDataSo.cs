using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "newUnitData", menuName = "Data/Unit Data")]
public class UnitDataSo : ScriptableObject
{
    public byte HP;
    public byte Speed;
    public Faction Faction;
    public List<UnitSkillDataSo> Skills = new List<UnitSkillDataSo>();


    public float DeadAnimationTime;
    public UnitDeadDataSo UnitDeadData;
}
