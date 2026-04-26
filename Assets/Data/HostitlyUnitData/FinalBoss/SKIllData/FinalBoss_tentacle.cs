using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Playables;

[CreateAssetMenu(fileName = "newUnitData", menuName = "Data/Skill Data/FinalBoss/" + nameof(FinalBoss_tentacle))]
public class FinalBoss_tentacle : UnitSkillDataSo
{
    [SerializeField] private GameObject tentaclePrefab;
    [SerializeField] private GameObject hitPrefab;

    [SerializeField] private Vector3 offest;

    private GameObject tentacleEffect;
    private GameObject hitEffect;

    public override void GameStartInit()
    {
        base.GameStartInit();

        tentacleEffect = Instantiate(tentaclePrefab, Vector3.zero, Quaternion.identity);
        tentacleEffect.SetActive(false);

        hitEffect = Instantiate(hitPrefab, Vector3.zero, Quaternion.identity);
        hitEffect.SetActive(false);
    }

    public override void Action(ICollection<UnitPlat> unitPlats, UnitPlat user)
    {
        UnitPlat target = null;
        foreach (var tar in unitPlats)
        {
            if (!tar.isDead && tar.unitData != FactorySystem.instance.EmptyFriendlyUnitData)
            {
                target = tar;
                break;
            }
        }

        if (target == null)
        {
            user.unit.unitSkills[0].SkillTime = 0.5f;
            return;
        }

        GameManager.instance.GlobalLightControll(0.5f, 0.5f);
        user.transform.DOScale(UnitPlat.originScale * attackScale, 0.5f);

        TimerManager.instance.StartTimer(name + "AttackEffect", 0.6f, 
            () => 
            {
                tentacleEffect.transform.position = user.transform.position;
                tentacleEffect.SetActive(true);

                Vector3[] path = new Vector3[]
                {
                    user.transform.position,
                    user.transform.position +=
                        new Vector3(-offest.x, offest.y, offest.z),
                    target.transform.position,
                };

                tentacleEffect.transform.DOPath(path, 1f, PathType.CatmullRom).
                    OnComplete(() =>
                    {
                        tentacleEffect.SetActive(false);

                        hitEffect.transform.position = target.transform.position;
                        hitEffect.SetActive(true);
                        hitEffect.GetComponent<PlayableDirector>().Play();

                        target.unit.HP -= Damage;
                        target.UnitPlatHurtAnimation();
                    });
            });

        TimerManager.instance.StartTimer(name + "EffectClose", 0.6f + 1.6f, 
            () => 
            {
                hitEffect.SetActive(false);

                GameManager.instance.GlobalLightControll(1f, 0.5f);
                user.transform.DOScale(UnitPlat.originScale, 0.5f);
            });

        user.unit.unitSkills[0].SkillTime = 0.6f + 1.6f + 0.6f;
    }
}
