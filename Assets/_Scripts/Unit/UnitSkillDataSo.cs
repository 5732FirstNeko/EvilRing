using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class UnitSkillDataSo : ScriptableObject
{
    protected static float attackScale = 1.1f;

    public float SkillTime;

    public int Damage;
    public TriggerTiming TriggerTiming;
    public Faction SkillTarget;
    public List<UnitSite> Range = new List<UnitSite>();
    public List<UnitBuffDataSo> UnitBuffs = new List<UnitBuffDataSo>();

    [TextArea] public string description;

    public virtual void GameStartInit() { }

    public abstract void Action(ICollection<UnitPlat> unitPlats, UnitPlat user);

    public static void DamageHurt(ICollection<UnitPlat> unitPlats, int hurt)
    {
        foreach (var plat in unitPlats)
        {
            plat.unit.HP -= (byte)hurt;
        }
    }

    public static void HPReceive(ICollection<UnitPlat> unitPlats, int receive)
    {
        foreach (var plat in unitPlats)
        {
            plat.unit.HP += (byte)receive;
        }
    }
}

public enum TriggerTiming
{
    OnRoundStart,
    OnRoundEnd,
    OnGameStart,
    OnRound,
    OnStrikeBack,
    Manual
}
