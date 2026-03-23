using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitPlat : MonoBehaviour
{
    public Unit unit;
    public UnitSite site;
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

    public Dictionary<UnitBuff, int> buffList;

    private bool _isDead;

    public void UnitPlatInit(UnitDataSo unitdata, UnitSite site)
    {
        unit = new Unit(unitdata, this);
        this.site = site;
    }

    private void Start()
    {
        buffList = new Dictionary<UnitBuff, int>();
        isDead = false;
    }

    public void AddBuff(UnitBuff buff)
    {
        bool isFirstBindBuff = buffList.ContainsKey(buff);
        if (isFirstBindBuff)
        {
            buffList[buff] += buff.ContinuousRound;
        }
        else
        {
            buffList.Add(buff, buff.ContinuousRound);
        }

        buff.BindBuff(this, isFirstBindBuff);
    }

    public int BuffActiuon(UnitBuff buff)
    {
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
