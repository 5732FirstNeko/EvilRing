using System;
using System.Collections;
using System.Collections.Generic;

public class Unit
{
    private UnitPlat user;

    public byte HP;
    public byte MaxHP;
    public byte speed;
    public int DamageReduction;
    public Faction faction;
    public List<UnitSkill> unitSkills;
    public UnitSkill SPSkill;

    public float DeadAnimationTime;
    public Action<UnitPlat> OnDead;

    public Unit(UnitDataSo unitData, UnitPlat user)
    {
        this.user = user;
        DataInit(unitData);
    }

    public UnitSkill UnitSkillChoice()
    {
        //TODO : OnRound UnitSkill Change Logic

        return unitSkills[0];
    }

    public void DeadAction(UnitPlat user)
    {
        OnDead?.Invoke(user);
    }

    public void DataInit(UnitDataSo unitData)
    {
        unitSkills = null;
        SPSkill = null;
        OnDead = null;

        MaxHP = unitData.HP;
        HP = MaxHP;
        speed = unitData.Speed;
        faction = unitData.Faction;
        DeadAnimationTime = unitData.DeadAnimationTime;
        OnDead += unitData.UnitDeadData != null ? unitData.UnitDeadData.DeadAction : null;

        if (faction == Faction.Hostility)
        {
            //TODO : After Creat HostilityData ScriptsObject, give SPSkill value,
            //TODO : and HostilityData ScriptsObject must be have a UnitSkill as SPSkill !
        }

        unitSkills = new List<UnitSkill>();
        foreach (var skillData in unitData.Skills)
        {
            UnitSkill initSkill = new UnitSkill
            {
                User = user,
                SkillTime = skillData.SkillTime,
                Damage = skillData.Damage,
                TriggerTiming = skillData.TriggerTiming,
                SkillTarget = skillData.SkillTarget,
                Range = skillData.Range,
                UnitBuffs = new List<UnitBuff>(),
            };
            initSkill.OnAction += skillData.Action;

            foreach (var buffData in skillData.UnitBuffs)
            {
                UnitBuff initBuff = new UnitBuff
                {
                    User = user,
                    BuffActionTime = buffData.BuffActionTime,
                    TriggerTiming = buffData.TriggerTiming,
                };
                initBuff.OnAction += buffData.OnAction;
                initBuff.OnBindBuff += buffData.BindBuff;
                initBuff.OnUnBindBuff += buffData.UnbindBuff;

                initSkill.UnitBuffs.Add(initBuff);
            }

            unitSkills.Add(initSkill);
        }
    }
}

public enum Faction
{
    Friendly,
    Hostility
}

public class UnitSkill
{
    public UnitPlat User;

    public float SkillTime;
    public int Damage;
    public TriggerTiming TriggerTiming;
    public Faction SkillTarget;
    public List<UnitSite> Range = new List<UnitSite>();
    public List<UnitBuff> UnitBuffs = new List<UnitBuff>();
    public Action<ICollection<UnitPlat>, UnitPlat> OnAction;

    public void Action(ICollection<UnitPlat> units)
    {
        OnAction?.Invoke(units, User);
    }
}

public class UnitBuff
{
    public UnitPlat User;

    public float BuffActionTime;
    public int ContinuousRound;
    public TriggerTiming TriggerTiming;
    public Action<ICollection<UnitPlat>, UnitPlat> OnAction;
    public Action<UnitPlat, UnitPlat, bool> OnBindBuff;
    public Action<UnitPlat, UnitPlat> OnUnBindBuff;


    public void Action(ICollection<UnitPlat> unit)
    {
        OnAction?.Invoke(unit, User);
    }

    public void BindBuff(UnitPlat unit, bool isFirstBind = true)
    {
        OnBindBuff?.Invoke(unit, User, isFirstBind);
    }

    public void UnBindBuff(UnitPlat unit)
    {
        OnUnBindBuff?.Invoke(unit, User);
    }
}
