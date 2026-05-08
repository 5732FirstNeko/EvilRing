using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "newUnitData", menuName = "Data/Dead Data/" + nameof(lycorisDead_standrd))]
public class lycorisDead_standrd : UnitDeadDataSo
{
    [SerializeField] private GameObject effectprefab;

    private GameObject effect;

    public override void PrefabInit()
    {
        base.PrefabInit();

        effect = Instantiate(effectprefab, Vector3.zero, Quaternion.identity);
        effect.SetActive(false);
        BattleSystem.instance.destoryEffect.Add(effect);
    }

    public override void PrefabDestory()
    {
        base.PrefabDestory();

        effect = null;
    }

    public override void DeadAction(UnitPlat user)
    {
        StandrdDead(user);
        user.isDead = false;

        if (user.costumvalue_first >= 1)
        {
            return;
        }

        user.costumvalue_first++;

        TimerManager.instance.StartTimer(name + user.name + "ResurrectionEffect", 1f, 
            () => 
            {
                effect.transform.position = user.transform.position + UnitPlat.bottomDistance;
                effect.SetActive(true);
                effect.GetComponentInChildren<ParticleSystem>().Play(true);
            });

        TimerManager.instance.StartTimer(name + user.name + "Resurrection", 3f, 
            () => 
            {
                effect.SetActive(false);
                BattleSystem.instance.UnitResurrection(user);
            });
    }
}
