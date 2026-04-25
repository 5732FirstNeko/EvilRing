using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Playables;

[CreateAssetMenu(fileName = "newUnitData", menuName = "Data/Skill Data/Daimon/" + nameof(Daimon_assassin))]
public class Daimon_assassin : UnitSkillDataSo
{
    [SerializeField] private GameObject attackPrefab;
    [SerializeField] private GameObject hitPrefab;

    [SerializeField] private float hitEffectRate;

    private GameObject attackEffect;
    private GameObject hitEffect;

    public override void GameStartInit()
    {
        base.GameStartInit();

        attackEffect = Instantiate(attackPrefab, Vector3.zero, Quaternion.identity);
        attackEffect.SetActive(false);

        hitEffect = Instantiate(hitPrefab, Vector3.zero, Quaternion.identity);
        hitEffect.SetActive(false);
    }

    public override void Action(ICollection<UnitPlat> unitPlats, UnitPlat user)
    {
        int index = UnityEngine.Random.Range(0, 101);

        if (index <= 75)
        {
            Attack(user);
        }
        else
        {
            Stealth(user);
        }
    }

    private void Attack(UnitPlat user)
    {
        UnitPlat target = null;
        foreach (var unit in BattleSystem.instance.HostilityUnitPlatsQueue.GetAllUnitPlat())
        {
            if (target == null && !unit.isDead &&
                unit.unitData != FactorySystem.instance.EmptyHostitlyUnitData)
            {
                target = unit;
                continue;
            }

            if (target.unit.HP > unit.unit.HP && !unit.isDead &&
                unit.unitData != FactorySystem.instance.EmptyHostitlyUnitData)
            {
                target = unit;
            }
        }

        if (target == null)
        {
            user.unit.unitSkills[0].SkillTime = 0.5f;
            return;
        }

        GameManager.instance.GlobalLightControll(0.5f, 0.5f);
        user.transform.DOScale(UnitPlat.originScale * attackScale, 0.5f);

        PlayableDirector attackDirector = attackEffect.GetComponent<PlayableDirector>();
        TimerManager.instance.StartTimer(name + "AttackEffect", 0.6f,
            () =>
            {
                attackEffect.transform.position = target.transform.position;
                attackEffect.SetActive(true);
                attackDirector.Play();
            });

        PlayableDirector hitDirector = hitEffect.GetComponent<PlayableDirector>();
        TimerManager.instance.StartTimer(name + "HitEffect", 
            0.6f + (float)attackDirector.duration * hitEffectRate, 
            () => 
            {
                hitEffect.transform.position = target.transform.position;
                hitEffect.SetActive(true);
                hitDirector.Play();
            });

        TimerManager.instance.StartTimer(name + "EffectClose",
            0.6f + (float)attackDirector.duration + (float)hitDirector.duration,
            () => 
            {
                attackEffect.SetActive(false);
                hitEffect.SetActive(false);

                GameManager.instance.GlobalLightControll(1f, 0.5f);
                user.transform.DOScale(UnitPlat.originScale, 0.5f);
            });

        user.unit.unitSkills[0].SkillTime =
            0.6f + (float)attackDirector.duration + (float)hitDirector.duration + 0.6f;
    }

    private void Stealth(UnitPlat user)
    {
        GameManager.instance.GlobalLightControll(0.5f, 0.5f);
        user.transform.DOScale(UnitPlat.originScale * attackScale, 0.5f);

        TimerManager.instance.StartTimer(name + "Effect", 0.6f, 
            () =>
            {
                UnitDataSo originData = user.unitData;
                user.unitData = FactorySystem.instance.EmptyHostitlyUnitData;

                user.iconSpriteRender.DOColor(new Color(0.5f, 0.5f, 0.5f), 1f);

                Func<int, float> func = null;

                func = (round) =>
                {
                    user.unitData = originData;
                    user.iconSpriteRender.DOColor(new Color(1f, 1f, 1f), 1f);

                    BattleSystem.instance.OnRoundEnd -= func;
                    return 0;
                };

                BattleSystem.instance.OnRoundEnd += func;
            });

        TimerManager.instance.StartTimer(name + "EffectClose", 0.6f + 1.1f,
            () => 
            {
                GameManager.instance.GlobalLightControll(1f, 0.5f);
                user.transform.DOScale(UnitPlat.originScale, 0.5f);
            });

        user.unit.unitSkills[0].SkillTime = 0.6f + 1.1f + 0.6f;
    }
}
