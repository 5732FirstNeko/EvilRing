using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Playables;

[CreateAssetMenu(fileName = "newUnitData", menuName = "Data/Dead Data/" + nameof(NecroDead_SkeletalArc))]
public class NecroDead_SkeletalArc : UnitDeadDataSo
{
    [SerializeField] private GameObject arrowAttackPrefab;
    [SerializeField] private GameObject arrowHitPrefab;

    private GameObject arrowAttackEffect;
    private GameObject arrowHitEffect;

    public override void PrefabInit()
    {
        base.PrefabInit();

        arrowAttackEffect = Instantiate(arrowAttackPrefab, Vector3.zero, Quaternion.identity);
        arrowAttackEffect.SetActive(false);

        arrowHitEffect = Instantiate(arrowHitPrefab, Vector3.zero, Quaternion.identity);
        arrowHitEffect.SetActive(false);
    }

    public override void DeadAction(UnitPlat user)
    {
        UnitPlat target = null;

        foreach (var unit in
            BattleSystem.instance.FriendlyUnitPlatsQueue.GetAllUnitPlat())
        {
            if (!unit.isDead && unit.unitData != FactorySystem.instance.EmptyFriendlyUnitData)
            {
                if (target == null)
                {
                    target = unit;
                    continue;
                }
            }
        }

        if (target == null)
        {
            StandrdDead(user);
            user.unit.DeadAnimationTime = 1f;
            return;
        }

        TimerManager.instance.StartTimer(name + "ArrowAttackEffetc", 0.6f,
            () =>
            {
                Vector3 startDir_first = target.transform.position - arrowAttackEffect.transform.position;
                arrowAttackEffect.transform.rotation = Quaternion.FromToRotation(Vector3.up, startDir_first);

                arrowAttackEffect.transform.position = user.transform.position;
                arrowAttackEffect.SetActive(true);
                arrowAttackEffect.transform.DOMove(target.transform.position, 1f).
                    SetEase(Ease.InQuart).SetDelay(0.5f).OnComplete(
                    () =>
                    {
                        arrowAttackEffect.SetActive(false);
                        arrowHitEffect.transform.position = target.transform.position;
                        arrowHitEffect.SetActive(true);
                        arrowHitEffect.GetComponent<PlayableDirector>().Play();

                        target.unit.HP -= 6;
                        target.UnitPlatHurtAnimation();
                    });
            });

        TimerManager.instance.StartTimer(name + "EffectClose", 0.6f + 1.6f,
            () =>
            {
                arrowAttackEffect.SetActive(false);
                arrowHitEffect.SetActive(false);

                StandrdDead(user);
            });

        user.unit.DeadAnimationTime = 0.6f + 1.6f + 1.1f;
    }
}
