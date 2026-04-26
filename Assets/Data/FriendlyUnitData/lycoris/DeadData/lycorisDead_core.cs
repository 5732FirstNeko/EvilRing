using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "newUnitData", menuName = "Data/Dead Data/" + nameof(lycorisDead_core))]
public class lycorisDead_core : UnitDeadDataSo
{
    [SerializeField] public GameObject ResurrectionEffect;

    private GameObject effect;
    public override void PrefabInit()
    {
        base.PrefabInit();

        effect = Instantiate(ResurrectionEffect, Vector3.zero, Quaternion.identity);
        effect.SetActive(false);
    }

    public override void DeadAction(UnitPlat user)
    {
        StandrdDead(user);

        UnitPlat recervePlat = null;
        foreach (var unit in
            BattleSystem.instance.FriendlyUnitPlatsQueue.GetAllUnitPlat())
        {
            if (unit != user && unit.isDead)
            {
                recervePlat = unit;
            }
        }

        if (recervePlat == null) return;

        TimerManager.instance.StartTimer(name + "REsurrection", 1.3f, 
            () => 
            {
                BattleSystem.instance.UnitResurrection(recervePlat);
                effect.transform.position = recervePlat.transform.position + UnitPlat.bottomDistance;
                effect.SetActive(true);
                effect.GetComponentInChildren<ParticleSystem>().Play(true);
            });

        TimerManager.instance.StartTimer(name + "EffectRecycling", 5f, 
            () => { effect.SetActive(false); });
    }
}
