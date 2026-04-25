using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Playables;

[CreateAssetMenu(fileName = "newUnitData", menuName = "Data/Skill Data/Necro/" + nameof(Necromancer))]
public class Necromancer : UnitSkillDataSo
{
    [SerializeField] private GameObject resurrectionPrefab;
    [SerializeField] private GameObject curseAttackPrefab;
    [SerializeField] private GameObject hitPrefab;
    [SerializeField] private GameObject shiledPrefab;

    [SerializeField] private UnitDataSo userData;
    [SerializeField] private int skillListInedx;
    [SerializeField] private int spCount;

    private GameObject resurrectionEffect;
    private GameObject curseAttackEffect;
    private GameObject hitEffect;
    private GameObject shiledEffect;

    public override void GameStartInit()
    {
        base.GameStartInit();

        resurrectionEffect = Instantiate(resurrectionPrefab, Vector3.zero, Quaternion.identity);
        resurrectionEffect.SetActive(false);

        curseAttackEffect = Instantiate(curseAttackPrefab, Vector3.zero, Quaternion.identity);
        curseAttackEffect.SetActive(false);

        hitEffect = Instantiate(hitPrefab, Vector3.zero, Quaternion.identity);
        hitEffect.SetActive(false);

        shiledEffect = Instantiate(shiledPrefab, Vector3.zero, Quaternion.identity);
        shiledEffect.SetActive(false);
    }

    public override void Action(ICollection<UnitPlat> unitPlats, UnitPlat user)
    {
        if (user.unit.spCount >= spCount)
        {
            UnitPlat recurrectionTarget = null;
            foreach (var unit in
                BattleSystem.instance.HostilityUnitPlatsQueue.GetAllUnitPlat())
            {
                if (unit.isDead &&
                    unit.unitData != FactorySystem.instance.EmptyHostitlyUnitData)
                {
                    recurrectionTarget = unit;
                }
            }

            if (recurrectionTarget != null)
            {
                ResurrectionUnit(recurrectionTarget, user);
                return;
            }
        }

        if (user.unit.spCount >= 2)
        {
            int index = Random.Range(0, 101);
            if (index <= 75)
            {
                NecroCurse(user);
            }
            else
            {
                Shiled(user);
            }
        }
        else
        {
            NecroCurse(user);
        }
    }

    private void ResurrectionUnit(UnitPlat target, UnitPlat user)
    {
        GameManager.instance.GlobalLightControll(0.5f, 0.5f);
        user.transform.DOScale(UnitPlat.originScale * attackScale, 0.5f);

        PlayableDirector director = resurrectionEffect.GetComponent<PlayableDirector>();
        TimerManager.instance.StartTimer(name + "ResurrectionEffect", 0.6f, 
            () =>
            {
                resurrectionEffect.transform.position = target.transform.position;
                resurrectionEffect.SetActive(true);
                director.Play();
            });

        TimerManager.instance.StartTimer(name + "UnitResurrection",0.6f + (float)director.duration, 
            () => 
            {
                BattleSystem.instance.UnitResurrection(target);
            });

        TimerManager.instance.StartTimer(name + "EffectClose", 0.6f + (float)director.duration + 1.1f, 
            () =>
            {
                resurrectionEffect.SetActive(false);
                user.unit.spCount -= 3;

                GameManager.instance.GlobalLightControll(1f, 0.5f);
                user.transform.DOScale(UnitPlat.originScale, 0.5f);
            });

        user.unit.unitSkills[skillListInedx].SkillTime =
            0.6f + (float)director.duration + 1.1f + 0.6f;
    }

    private void NecroCurse(UnitPlat user)
    {
        List<UnitPlat> unitplats = BattleSystem.instance.FriendlyUnitPlatsQueue.GetAllUnitPlat();

        if (unitplats.Count <= 0)
        {
            user.unit.unitSkills[skillListInedx].SkillTime = 0.5f;
            return;
        }

        UnitPlat target = null;
        foreach (var unit in unitplats)
        {
            if (!unit.isDead && unit.unitData != FactorySystem.instance.EmptyFriendlyUnitData)
            {
                target = unit;
                break;
            }
        }

        if (target == null)
        {
            user.unit.unitSkills[skillListInedx].SkillTime = 0.5f;
            return;
        }

        GameManager.instance.GlobalLightControll(0.5f, 0.5f);
        user.transform.DOScale(UnitPlat.originScale * attackScale, 0.5f);

        PlayableDirector director = curseAttackEffect.GetComponent<PlayableDirector>();
        TimerManager.instance.StartTimer(name + "HtAttack", 0.6f, 
            () => 
            {
                curseAttackEffect.transform.position = target.transform.position;
                curseAttackEffect.SetActive(true);
                director.Play();
            });

        PlayableDirector hitDirector = hitEffect.GetComponent<PlayableDirector>();
        TimerManager.instance.StartTimer(name + "HitEffect", 0.6f + (float)director.duration,
            () => 
            {
                hitEffect.transform.position = target.transform.position;
                hitEffect.SetActive(true);
                hitDirector.Play();

                user.DamageTextJump("×çÖä", new Color(0.5f, 0, 0.5f));

                target.unit.HP -= Damage;
                target.UnitPlatHurtAnimation();

                target.unit.OnHPChange += CurseAction;
            });

        TimerManager.instance.StartTimer(name + "EffectClose",
            0.6f + (float)director.duration + (float)hitDirector.duration + 0.1f,
            () =>
            {
                curseAttackEffect.SetActive(false);
                hitEffect.SetActive(false);

                GameManager.instance.GlobalLightControll(1f, 0.5f);
                user.transform.DOScale(UnitPlat.originScale, 0.5f);
            });

        user.unit.unitSkills[skillListInedx].SkillTime =
            0.6f + (float)director.duration + (float)hitDirector.duration + 0.1f + 0.6f;
    }

    private void CurseAction(int hpChange, UnitPlat user)
    {
        if (hpChange < 0)
        {
            foreach (var unit in
                BattleSystem.instance.HostilityUnitPlatsQueue.GetAllUnitPlat())
            {
                if (!unit.isDead && unit.unitData == userData)
                {
                    unit.unit.spCount += 2;
                    unit.DamageTextJump("sp +2", Color.white);
                }
            }

            user.unit.OnHPChange -= CurseAction;
            user.DamageTextJump("×çÖä½â³ý", new Color(0.5f, 0, 0.5f));
        }
    }

    private void Shiled(UnitPlat user)
    {
        GameManager.instance.GlobalLightControll(0.5f, 0.5f);
        user.transform.DOScale(UnitPlat.originScale * attackScale, 0.5f);

        PlayableDirector director = shiledEffect.GetComponent<PlayableDirector>();
        TimerManager.instance.StartTimer(name + "ShiledEffect", 0.6f, 
            () => 
            {
                shiledEffect.transform.position = user.transform.position;
                shiledEffect.SetActive(false);
                director.Play();
            });

        TimerManager.instance.StartTimer(name + "Shiled", 0.6f + (float)director.duration, 
            () => 
            {
                foreach (var unit in 
                    BattleSystem.instance.HostilityUnitPlatsQueue.GetAllUnitPlat())
                {
                    unit.DamageTextJump("±Ó»¤", Color.white);
                    unit.unit.OnDefend += OnShiledAction;
                }

                BattleSystem.instance.OnRoundEnd += OnRoundEndAction;
            });

        TimerManager.instance.StartTimer(name + "EffectClose", 0.6f + (float)director.duration + 1.1f,
            () => 
            {
                shiledEffect.SetActive(false);

                GameManager.instance.GlobalLightControll(1f, 0.5f);
                user.transform.DOScale(UnitPlat.originScale, 0.5f);
            });

        user.unit.unitSkills[skillListInedx].SkillTime = 0.6f + (float)director.duration + 1.1f + 0.6f;
    }

    private int OnShiledAction(int hpcahnge, UnitPlat user)
    {
        if (hpcahnge < 0)
        {
            user.unit.OnDefend -= OnShiledAction;
            user.DamageTextJump("ÍöÁé±Ó»¤", new Color(0.5f, 0, 0.5f));
            return hpcahnge + 10 > 0 ? 0 : hpcahnge + 10;
        }

        return hpcahnge;
    }

    private float OnRoundEndAction(int round)
    {
        BattleSystem.instance.OnRoundEnd += OnRoundEndAction;
        return 0;
    }
}
