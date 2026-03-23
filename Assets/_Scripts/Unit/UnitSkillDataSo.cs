using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class UnitSkillDataSo : ScriptableObject
{
    //TODO : UnitSkillDataSo is a base class,
    //TODO : must have The Reflection [CreateAssetMenu] when creat subClass
    //TODO : and the Implementation of Action must be in subClass to maintain Hierarchy structure

    public float SkillTime;

    public int Damage;
    public TriggerTiming TriggerTiming;
    public Faction SkillTarget;
    public List<UnitSite> Range = new List<UnitSite>();
    public List<UnitBuffDataSo> UnitBuffs = new List<UnitBuffDataSo>();

    public abstract void Action(ICollection<UnitPlat> unitPlats, UnitPlat user);
}

public enum TriggerTiming
{
    OnRoundStart,
    OnRoundEnd,
    OnLevelStart,
    OnRound,
    OnStrikeBack
}
