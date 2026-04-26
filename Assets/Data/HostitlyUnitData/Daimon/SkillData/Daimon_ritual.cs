using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Playables;

[CreateAssetMenu(fileName = "newUnitData", menuName = "Data/Skill Data/Daimon/" + nameof(Daimon_ritual))]
public class Daimon_ritual : UnitSkillDataSo
{
    [SerializeField] private GameObject sacrificePrefab;
    [SerializeField] private GameObject hitPrefab;

    [SerializeField] private UnitDataSo tentacleCard;
    [SerializeField] private UnitDataSo assassinCard;

    private GameObject sacrificeEffect;
    private List<GameObject> hitEffects;

    public override void GameStartInit()
    {
        base.GameStartInit();
        sacrificeEffect = Instantiate(sacrificePrefab, Vector3.zero, Quaternion.identity);
        sacrificeEffect.SetActive(false);

        hitEffects = new List<GameObject>();
        for (int i = 0; i < 4; i++)
        {
            GameObject effect = Instantiate(hitPrefab, Vector3.zero, Quaternion.identity);
            effect.SetActive(false);
            hitEffects.Add(effect);
        }
    }

    public override void Action(ICollection<UnitPlat> unitPlats, UnitPlat user)
    {
        int deadCount = 0;
        foreach (var unit in BattleSystem.instance.HostilityUnitPlatsQueue.GetAllUnitPlat())
        {
            if (unit.unitData != FactorySystem.instance.EmptyHostitlyUnitData && !unit.isDead)
            {
                deadCount++;
            }
        }

        if (deadCount >= 2 && user.unit.spCount >= 6)
        {
            Summon(user);
            return;
        }

        int index = Random.Range(0, 101);

        if (index <= 75)
        {
            Attack(user);
        }
        else
        {
            Defend(user);
        }
    }

    private void Summon(UnitPlat user)
    {
        UnitPlat first = BattleSystem.instance.HostilityUnitPlatsQueue.
            GetUnitPlatByUnitSite(UnitSite.first).plat;
        if (first != null && first.unitData != FactorySystem.instance.EmptyHostitlyUnitData)
        {
            user.unit.unitSkills[0].SkillTime = 0.5f;
            return;
        }

        GameManager.instance.GlobalLightControll(0.5f, 0.5f);
        user.transform.DOScale(UnitPlat.originScale * attackScale, 0.5f);

        TimerManager.instance.StartTimer(name + "SummonEffect", 0.6f, 
            () => 
            {
                user.DamageTextJump("´¥ÊÖÕÙ»½", GameManager.purple);

                first.UnitPlatInit(tentacleCard, UnitSite.first);

                first.iconSpriteRender.sprite = tentacleCard.UnitSprite;
                Vector3 originScale = first.transform.localScale;
                first.transform.localScale = Vector3.zero;
                first.transform.DOScale(originScale, 1.5f);
            });

        TimerManager.instance.StartTimer(name + "EffectClose", 0.6f + 1.6f, 
            () => 
            {
                GameManager.instance.GlobalLightControll(1f, 0.5f);
                user.transform.DOScale(UnitPlat.originScale, 0.5f);
            });

        user.unit.unitSkills[0].SkillTime = 0.6f + 1.6f + 0.6f;
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

        PlayableDirector sacrificeDirector = sacrificeEffect.GetComponent<PlayableDirector>();
        TimerManager.instance.StartTimer(name + "SacrificeEffect", 0.6f, 
            () => 
            {
                sacrificeEffect.transform.position = target.transform.position;
                sacrificeEffect.SetActive(true);
                sacrificeDirector.Play();
            });

        TimerManager.instance.StartTimer(name + "SacrificeEndEffect", 0.6f + (float)sacrificeDirector.duration,
            () => 
            {
                target.DamageTextJump("Ï×¼À", Color.white);
                target.unit.HP = 0;
                target.UnitPlatHurtAnimation();

                foreach (var unit in BattleSystem.instance.HostilityUnitPlatsQueue.GetAllUnitPlat())
                {
                    if (unit.unitData == assassinCard)
                    {
                        BattleSystem.instance.UnitReEnqueue(unit);
                    }
                }

                BattleSystem.instance.UnitDead(target);
            });

        TimerManager.instance.StartTimer(name + "HitEffect",
            0.6f + (float)sacrificeDirector.duration + 1f, 
            () => 
            {
                int i = 0;
                foreach (var unit in BattleSystem.instance.FriendlyUnitPlatsQueue.GetAllUnitPlat())
                {
                    if (!unit.isDead && unit.unitData != FactorySystem.instance.EmptyFriendlyUnitData)
                    {
                        int index = i;
                        hitEffects[index].transform.position = unit.transform.position;
                        hitEffects[index].SetActive(true);
                        hitEffects[index].GetComponent<PlayableDirector>().Play();

                        unit.unit.HP -= Damage;
                        unit.UnitPlatHurtAnimation();

                        i++;
                    }
                }
            });

        TimerManager.instance.StartTimer(name + "EffectClose", 
            0.6f + (float)sacrificeDirector.duration + 1.1f + 0.6f, 
            () => 
            {
                sacrificeEffect.SetActive(false);
                for (int i = 0; i < hitEffects.Count; i++)
                {
                    hitEffects[i].SetActive(false);
                }

                GameManager.instance.GlobalLightControll(1f, 0.5f);
                user.transform.DOScale(UnitPlat.originScale, 0.5f);
            });

        user.unit.unitSkills[0].SkillTime = 0.6f + (float)sacrificeDirector.duration + 1.1f + 0.6f + 0.6f;
    }

    private void Defend(UnitPlat user)
    {
        GameManager.instance.GlobalLightControll(0.5f, 0.5f);
        user.transform.DOScale(UnitPlat.originScale * attackScale, 0.5f);

        TimerManager.instance.StartTimer(name + "SacrificeEffect", 0.6f,
            () =>
            {
                foreach (var unit in 
                    BattleSystem.instance.HostilityUnitPlatsQueue.GetAllUnitPlat())
                {
                    if (!unit.isDead &&
                        unit.unitData != FactorySystem.instance.EmptyHostitlyUnitData)
                    {
                        unit.unit.HP += 5;
                        unit.unit.spCount += 5;

                        unit.UnitPlatRecoveryAnimation();
                    }
                }
            });
    }
}
