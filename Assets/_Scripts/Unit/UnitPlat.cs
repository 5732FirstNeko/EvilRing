using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class UnitPlat : MonoBehaviour
{
    public Unit unit;
    public UnitDataSo unitData;
    public UnitSite site;
    public SpriteRenderer iconSpriteRender;

    public bool isDead 
    {
        get => _isDead; 
        set 
        { 
            _isDead = value;
            if (value)
            {
                unit.DeadAction(this);
            }
        }
    }

    public Dictionary<UnitBuff, int> buffList = new Dictionary<UnitBuff, int>();

    private bool _isDead;

    public int costumvlue_first;
    public int costumvlue_second;
    public int costumvlue_third;
    public int costumvlue_fourth;

    private void Start()
    {
        iconSpriteRender = GetComponentsInChildren<SpriteRenderer>()[1];
    }

    public void UnitPlatInit(UnitDataSo unitData, UnitSite site)
    {
        unit = new Unit(unitData, this);
        isDead = false;
        this.unitData = unitData;
        this.site = site;

        costumvlue_first = -1;
        costumvlue_second = -1;
        costumvlue_third = -1;
        costumvlue_fourth = -1;
    }

    public void UnitPlatClear()
    {
        isDead = false;

        foreach (var buff in buffList)
        {
            buff.Key.UnBindBuff(this);
        }
        buffList.Clear();

        costumvlue_first = -1;
        costumvlue_second = -1;
        costumvlue_third = -1;
        costumvlue_fourth = -1;
    }

    public void AddBuff(UnitBuff buff)
    {
        bool isFirstBindBuff = true;
        UnitBuff addbuff = null;
        foreach (var buf in buffList)
        {
            if (buf.Key.Equals(buff))
            {
                isFirstBindBuff = false;
                addbuff = buf.Key;
            }
        }

        if (!isFirstBindBuff)
        {
            buffList[addbuff] += buff.ContinuousRound;
        }
        else
        {
            buffList.Add(buff, buff.ContinuousRound);
        }

        buff.BindBuff(this, isFirstBindBuff);
    }

    public int BuffConsumption(UnitBuff buff)
    {
        if (!buffList.ContainsKey(buff)) return 0;

        buffList[buff]--;
        if (buffList[buff] <= 0)
        {
            buff.UnBindBuff(this);
            buffList.Remove(buff);
            return 0;
        }

        return buffList[buff];
    }
}
