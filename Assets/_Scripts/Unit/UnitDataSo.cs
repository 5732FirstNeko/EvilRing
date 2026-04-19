using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "newUnitData", menuName = "Data/Unit Data")]
public class UnitDataSo : ScriptableObject
{
    public string cardName;

    public byte HP;
    public byte Speed;
    public int cost;
    public Faction Faction;
    public Sprite UnitSprite;
    public List<UnitSkillDataSo> Skills = new List<UnitSkillDataSo>();
    public int spCost;
    public UnitSkillDataSo SpKillData;

    public float DeadAnimationTime;
    public UnitDeadDataSo UnitDeadData;
}
