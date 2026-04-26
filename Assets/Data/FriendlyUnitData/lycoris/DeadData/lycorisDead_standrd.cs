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
    }

    public override void DeadAction(UnitPlat user)
    {
        StandrdDead(user);

        if (!user.isDead && user.costumvalue_first >= 1) return;

        BattleSystem.instance.UnitResurrection(user);

        effect.transform.position = user.transform.position + UnitPlat.bottomDistance;
        effect.SetActive(true);
        effect.GetComponentInChildren<ParticleSystem>().Play(true);
        user.costumvalue_first++;
    }
}
