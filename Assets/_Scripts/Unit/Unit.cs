using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit
{
    private UnitPlat user;

    public int HP 
    {
        get => _hp;
        set
        {
            int hpchange = value - _hp;
            value = OnDefend?.Invoke(hpchange, user) ?? 0 + value;
            hpchange = value - _hp;
            if (value >= MaxHP)
            {
                _hp = MaxHP;
            }
            else if (value <= 0)
            {
                _hp = 0;
            }
            else
            {
                _hp = value;
            }
            OnHPChange?.Invoke(hpchange, user);
        }
    }
    public int MaxHP;
    public int speed;
    public int DamageReduction;
    public Faction faction;
    public List<UnitSkill> unitSkills;

    public int spCost;
    public UnitSkill SPSkill;

    public float DeadAnimationTime;
    public Action<UnitPlat> OnDead;

    public Action<int,UnitPlat> OnHPChange;
    public Func<int,UnitPlat,int> OnDefend;

    public int spCount;

    private int _hp;

    public Unit(UnitDataSo unitData, UnitPlat user)
    {
        this.user = user;
        spCount = 0;
        DataInit(unitData);
    }

    public UnitSkill UnitSkillChoice()
    {
        if (faction == Faction.Hostility && spCount >= spCost)
        {
            spCost = 0;
            return SPSkill;
        }
        spCost++;

        List<UnitSkill> usedSkills = new List<UnitSkill>();
        foreach (var skill in unitSkills)
        {
            if (skill.TriggerTiming == TriggerTiming.OnRound)
            {
                usedSkills.Add(skill);
            }
        }

        if (usedSkills.Count <= 0)
        {
            return null;
        }

        int index = UnityEngine.Random.Range(0, usedSkills.Count);

        return usedSkills[index];
    }

    public void DeadAction()
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

        if (faction == Faction.Hostility && unitData.SpKillData != null)
        {
            spCost = unitData.spCost;
            UnitSkill initSkill = new UnitSkill
            {
                User = user,
                SkillTime = unitData.SpKillData.SkillTime,
                Damage = unitData.SpKillData.Damage,
                TriggerTiming = unitData.SpKillData.TriggerTiming,
                SkillTarget = unitData.SpKillData.SkillTarget,
                Range = unitData.SpKillData.Range,
                UnitBuffs = new List<UnitBuff>(),
            };
            initSkill.OnAction += unitData.SpKillData.Action;

            foreach (var buffData in unitData.SpKillData.UnitBuffs)
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

            SPSkill = initSkill;
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

    public bool Equals(UnitBuff obj)
    {
        return OnAction.Equals(obj.OnAction) && 
            OnBindBuff.Equals(obj.OnBindBuff) && OnUnBindBuff.Equals(obj.OnUnBindBuff);
    }
}
