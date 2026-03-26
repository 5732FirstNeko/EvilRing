using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class UnitSkillDataSo : ScriptableObject
{
    public float SkillTime;

    public int Damage;
    public TriggerTiming TriggerTiming;
    public Faction SkillTarget;
    public List<UnitSite> Range = new List<UnitSite>();
    public List<UnitBuffDataSo> UnitBuffs = new List<UnitBuffDataSo>();

    [TextArea] public string description;

    public abstract void Action(ICollection<UnitPlat> unitPlats, UnitPlat user);
}

public enum TriggerTiming
{
    OnRoundStart,
    OnRoundEnd,
    OnGameStart,
    OnRound,
    OnStrikeBack
}
