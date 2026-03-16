using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Unit : MonoBehaviour
{
    [SerializeField] private UnitDataSo unitData;

    public byte HP;
    public byte MaxHP;
    public byte speed;
    public Faction faction;
    public List<UnitSkill> unitSkills;
    public UnitSkill SPSkill;

    public float DeadAnimationTime;
    public Action<Unit> OnDead;

    private void Awake()
    {
        DataInit(unitData);
    }

    public UnitSkill UnitSkillChoice()
    {
        //TODO : OnRound UnitSkill Change Logic

        return unitSkills[0];
    }

    public void DeadAction(Unit user)
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
        OnDead += unitData.UnitDeadData.DeadAction;

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
                User = this,
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
                    User = this,
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
    public Unit User;

    public float SkillTime;
    public int Damage;
    public TriggerTiming TriggerTiming;
    public Faction SkillTarget;
    public List<UnitSite> Range = new List<UnitSite>();
    public List<UnitBuff> UnitBuffs = new List<UnitBuff>();
    public Action<ICollection<Unit>,Unit> OnAction;

    public void Action(ICollection<Unit> units)
    {
        OnAction?.Invoke(units, User);
    }
}

public class UnitBuff
{
    public Unit User;

    public float BuffActionTime;
    public int ContinuousRound;
    public TriggerTiming TriggerTiming;
    public Action<ICollection<Unit>, Unit> OnAction;
    public Action<Unit, Unit, bool> OnBindBuff;
    public Action<Unit, Unit> OnUnBindBuff;


    public void Action(ICollection<Unit> unit)
    {
        OnAction?.Invoke(unit, User);
    }

    public void BindBuff(Unit unit, bool isFirstBind = true)
    {
        OnBindBuff?.Invoke(unit, User, isFirstBind);
    }

    public void UnBindBuff(Unit unit)
    {
        OnUnBindBuff?.Invoke(unit, User);
    }
}
