using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

[CreateAssetMenu(fileName = "newUnitData", menuName = "Data/Dead Data/" + nameof(DaimonDead_believer))]
public class DaimonDead_believer : UnitDeadDataSo
{
    [SerializeField] private GameObject hitPrefab;

    private List<GameObject> hitEffects;

    public override void PrefabInit()
    {
        base.PrefabInit();

        hitEffects = new List<GameObject>();
        for (int i = 0; i < 4; i++)
        {
            GameObject effect = Instantiate(hitPrefab, Vector3.zero, Quaternion.identity);
            effect.SetActive(false);
            hitEffects.Add(effect);
        }
    }

    public override void DeadAction(UnitPlat user)
    {
        int i = 0;
        foreach (var unit in
            BattleSystem.instance.FriendlyUnitPlatsQueue.GetAllUnitPlat())
        {
            if (!unit.isDead && unit.unitData != FactorySystem.instance.EmptyFriendlyUnitData)
            {
                int index = i;
                hitEffects[index].transform.position = unit.transform.position;
                hitEffects[index].SetActive(true);
                hitEffects[index].GetComponent<PlayableDirector>().Play();

                unit.unit.HP -= 10;
                unit.UnitPlatHurtAnimation();

                i++;
            }
        }

        TimerManager.instance.StartTimer(name + "EffectClose", 0.5f,
            () =>
            {
                for (int i = 0; i < hitEffects.Count; i++)
                {
                    hitEffects[i].SetActive(false);
                }
            });

        StandrdDead(user);
        user.unit.DeadAnimationTime = 1f;
    }
}
