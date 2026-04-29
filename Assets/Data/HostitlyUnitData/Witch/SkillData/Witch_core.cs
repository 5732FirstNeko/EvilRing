using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Playables;

[CreateAssetMenu(fileName = "newUnitData", menuName = "Data/Skill Data/" + nameof(Witch_core))]
public class Witch_core : UnitSkillDataSo
{
    [SerializeField] private GameObject destoryPrefab;
    [SerializeField] private GameObject hitPrefab;

    [SerializeField] private UnitDataSo potCard;
    [SerializeField] private int skillListIndex;

    private GameObject destoryEffect;
    private GameObject hitEffect;

    public override void GameStartInit()
    {
        base.GameStartInit();

        destoryEffect = Instantiate(destoryPrefab, Vector3.zero, Quaternion.identity);
        destoryEffect.SetActive(false);

        hitEffect = Instantiate(hitPrefab, Vector3.zero, Quaternion.identity);
        hitEffect.SetActive(false);
    }

    public override void Action(ICollection<UnitPlat> unitPlats, UnitPlat user)
    {
        UnitPlat target = null;
        foreach (var unit in unitPlats)
        {
            if (target == null)
            {
                target = unit;
                continue;
            }

            if (target.unit.HP > unit.unit.HP)
            {
                target = unit;
            }
        }

        UnitPlat spwanUnit = BattleSystem.instance.HostilityUnitPlatsQueue.
            GetUnitPlatByUnitSite(UnitSite.first).plat;
        if (target == null || 
            spwanUnit.unitData != FactorySystem.instance.EmptyHostitlyUnitData || 
            user.costumvalue_second == 1)
        {
            user.unit.spCount += user.unit.spCost;
            user.unit.unitSkills[skillListIndex].SkillTime = 0.5f;
            return;
        }

        GameManager.instance.GlobalLightControll(0.5f, 0.5f);
        user.transform.DOScale(UnitPlat.originScale * attackScale, 0.5f);
        user.costumvalue_first = 3;

        PlayableDirector DestoryDirector = destoryEffect.GetComponent<PlayableDirector>();
        TimerManager.instance.StartTimer(name + "DestoryEffect", 0.6f, 
            () => 
            {
                destoryEffect.transform.position = target.transform.position;
                destoryEffect.SetActive(true);
                DestoryDirector.Play();

                target.unit.HP = 0;
            });

        PlayableDirector HitDirector = hitEffect.GetComponent<PlayableDirector>();
        TimerManager.instance.StartTimer(name + "HitEffect", 0.7f + (float)DestoryDirector.duration, 
            () => 
            {
                hitEffect.transform.position = target.transform.position;
                hitEffect.SetActive(true);
                HitDirector.Play();
            });

        TimerManager.instance.StartTimer(name + "UnitCardSpwan",
            0.7f + (float)DestoryDirector.duration + (float)HitDirector.duration, 
            () => 
            {
                destoryEffect.SetActive(false);
                hitEffect.SetActive(false);

                spwanUnit.UnitPlatInit(potCard, UnitSite.first);

                spwanUnit.iconSpriteRender.sprite = potCard.UnitSprite;
                Vector3 originScale = spwanUnit.transform.localScale;
                spwanUnit.transform.localScale = Vector3.zero;
                spwanUnit.transform.DOScale(originScale, 1.5f);

                spwanUnit.costumvalue_first = 3;
                user.costumvalue_second = 1;
            });

        TimerManager.instance.StartTimer(name + "ActionEnd",
            0.7f + (float)DestoryDirector.duration + (float)HitDirector.duration + 1.6f, 
            () =>
            {
                GameManager.instance.GlobalLightControll(1f, 0.5f);
                user.transform.DOScale(UnitPlat.originScale, 0.5f);
            });

        user.unit.unitSkills[skillListIndex].SkillTime =
            0.7f + (float)DestoryDirector.duration + (float)HitDirector.duration + 1.6f + 0.6f;
    }
}
