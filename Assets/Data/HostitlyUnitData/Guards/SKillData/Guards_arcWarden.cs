using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Playables;

[CreateAssetMenu(fileName = "newUnitData", menuName = "Data/Skill Data/Guards/" + nameof(Guards_arcWarden))]
public class Guards_arcWarden : UnitSkillDataSo
{
    [SerializeField] private GameObject arrowPrefab;
    [SerializeField] private GameObject hitPrefab;

    [SerializeField] private int skillListIndex;
    [SerializeField] private float attackEffectSpeed;
    [SerializeField] private UnitDataSo userData;

    private GameObject attackEffect;
    private List<GameObject> hitEffects;
    public override void GameStartInit()
    {
        base.GameStartInit();

        attackEffect = Instantiate(arrowPrefab, Vector3.zero, Quaternion.identity);
        attackEffect.SetActive(false);
        BattleSystem.instance.destoryEffect.Add(attackEffect);

        hitEffects = new List<GameObject>();
        for (int i = 0; i < 3; i++)
        {
            GameObject effect = Instantiate(hitPrefab, Vector3.zero, Quaternion.identity);
            effect.SetActive(false);
            hitEffects.Add(effect);
            BattleSystem.instance.destoryEffect.Add(effect);
        }

        BattleSystem.instance.OnGameStart += () => 
        {
            UnitPlat user = null;
            foreach (var unit in BattleSystem.instance.HostilityUnitPlatsQueue.GetAllUnitPlat())
            {
                if (unit.unitData == userData)
                {
                    user = unit;
                }
            }

            if (user == null)
            {
                throw new Exception("[GuardsError] user is no Find, Check Card is in Factory");
            }
            user.unit.OnHPChange += HPChangeAction;
            BattleSystem.instance.OnRoundStart += StateChange;
            return 0;
        };
    }

    public override void GameEndAction()
    {
        attackEffect = null;

        hitEffects = null;
    }

    public override void Action(ICollection<UnitPlat> unitPlats, UnitPlat user)
    {
        if (user.costumvalue_second == 1)
        {
            user.unit.unitSkills[skillListIndex].SkillTime = 0.5f;
            return;
        }

        Attack(unitPlats, user);
    }

    #region Attack
    private void Attack(ICollection<UnitPlat> unitPlats, UnitPlat user)
    {
        if (unitPlats.Count <= 0)
        {
            user.unit.unitSkills[skillListIndex].SkillTime = 0.5f;
            return;
        }

        GameManager.instance.GlobalLightControll(0.5f, 0.5f);
        user.transform.DOScale(UnitPlat.originScale * attackScale, 0.5f);

        int random = UnityEngine.Random.Range(0, 101);
        if (random <= 80)
        {
            float skilltime = 0;

            UnitPlat lastUnit = null;
            for (int i = 0; i < Range.Count; i++)
            {
                UnitSite site = BattleSystem.GetUnitSiteByIndex(i);
                UnitPlat target = null;
                foreach (var unit in unitPlats)
                {
                    if (unit.site == site)
                    {
                        target = unit;
                        break;
                    }
                }
                if (target == null || target.isDead ||
                    target.unitData == FactorySystem.instance.EmptyFriendlyUnitData)
                {
                    continue;
                }

                lastUnit = target;

                float movetime = Mathf.Abs(target.transform.position.x -
                    attackEffect.transform.position.x) / attackEffectSpeed;

                skilltime += movetime;

                TimerManager.instance.StartTimer(name + string.Empty + i, movetime + 0.6f,
                    () =>
                    {
                        target.UnitPlatHurtAnimation();
                        target.unit.HP -= 9;
                        hitEffects[0].transform.position = target.transform.position;
                        hitEffects[0].SetActive(true);
                        hitEffects[0].GetComponent<PlayableDirector>().Play();
                    });
            }

            TimerManager.instance.StartTimer(name + "flowerInstantite", 0.6f, () =>
            {
                UnitPlat target = lastUnit;
                attackEffect.transform.position = user.transform.position;

                GameManager.LookAtTarget(attackEffect.transform, target.transform.position, Vector2.up);

                attackEffect.SetActive(true);
                attackEffect.transform.DOMoveX(target.transform.position.x - 2, 0.3f);
            });

            TimerManager.instance.StartTimer(name + "closeEffect", skilltime + 0.6f,
                () =>
                {
                    attackEffect.SetActive(false);
                    attackEffect.transform.position = user.transform.position;

                    foreach (var effect in hitEffects)
                    {
                        effect.SetActive(false);
                    }
                    GameManager.instance.GlobalLightControll(1f, 0.5f);
                    user.transform.DOScale(UnitPlat.originScale, 0.5f);
                });

            user.unit.unitSkills[skillListIndex].SkillTime = skilltime + 0.6f + 0.6f;
        }
        else
        {
            Sequence sequence = DOTween.Sequence();

            UnitPlat target = null;
            foreach (var unit in unitPlats)
            {
                if (target != null && !unit.isDead && 
                    unit.unitData != FactorySystem.instance.EmptyFriendlyUnitData)
                {
                    target = unit;
                    continue;
                }

                if(target != null && target.unit.HP > unit.unit.HP)
                {
                    target = unit;
                }
            }

            if (target == null)
            {
                user.unit.unitSkills[skillListIndex].SkillTime = 0.5f;
                return;
            }

            float movetime = (target.transform.position.x - user.transform.position.x) / attackEffectSpeed;

            PlayableDirector hitdirector = hitEffects[0].GetComponent<PlayableDirector>();
            int frenquecy = user.unit.HP <= Mathf.RoundToInt(user.unit.MaxHP * 0.3f) ? 2 : 1;
            TimerManager.instance.StartTimer(name + "arrowAttack", 0.6f, () =>
            {
                attackEffect.transform.position = user.transform.position;

                GameManager.LookAtTarget(attackEffect.transform, target.transform.position, Vector2.up);

                attackEffect.SetActive(true);
                attackEffect.transform.DOMoveX(target.transform.position.x, movetime).OnComplete(
                    () => 
                    {
                        GameObject hitEffect = null;
                        for (int i = 0; i < hitEffects.Count; i++)
                        {
                            if (!hitEffects[i].activeSelf)
                            {
                                hitEffect = hitEffects[i];
                                break;
                            }
                        }

                        hitEffect.transform.position = target.transform.position;
                        hitEffect.SetActive(true);
                        hitEffect.GetComponent<PlayableDirector>().Play();

                        target.unit.HP -= 18;
                        target.UnitPlatHurtAnimation(frenquecy);
                    });
            });

            TimerManager.instance.StartTimer(name + "closeEffect", 
                movetime + (float)hitdirector.duration * frenquecy + 0.1f,
                () =>
                {
                    attackEffect.SetActive(false);
                    attackEffect.transform.position = user.transform.position;
                    foreach (var effect in hitEffects)
                    {
                        effect.SetActive(false);
                    }

                    GameManager.instance.GlobalLightControll(1f, 0.5f);
                    user.transform.DOScale(UnitPlat.originScale, 0.5f);
                });

            user.unit.unitSkills[skillListIndex].SkillTime = 
                movetime + (float)hitdirector.duration * frenquecy + 0.1f + 0.6f;
        }
    }
    #endregion

    #region Defend
    private int DefendAction(int hpChange, UnitPlat user)
    {
        if (hpChange < 0)
        {
            user.DamageTextJump("ÉËº¦ÎüÊÕ", Color.white);
            return 0;
        }

        return hpChange;
    }
    #endregion

    private float StateChange(int round)
    {
        UnitPlat user = null;
        foreach (var unit in BattleSystem.instance.HostilityUnitPlatsQueue.GetAllUnitPlat())
        {
            if (unit.unitData == userData)
            {
                user = unit;
            }
        }

        if (user == null) return 0;

        //Defend and hurt
        if (user.costumvalue_second == 1 && user.costumvalue_first == 1)
        {
            user.costumvalue_second = 0;
        }
        user.costumvalue_first = 0;

        if (user.costumvlue_third == 1)
        {
            user.DamageTextJump("·ÀÓù", Color.blue);
            user.costumvlue_third = 0;
            user.unit.OnDefend += DefendAction;
        }
        else if (user.costumvalue_second == 0)
        {
            user.DamageTextJump("¹¥»÷", Color.red);
            user.unit.OnDefend -= DefendAction;
        }

        return 1.1f;
    }

    private void HPChangeAction(int hpchange, UnitPlat user)
    {
        if (hpchange < 0)
        {
            user.costumvalue_first = 1;

            if (user.costumvalue_second == 0 &&
                user.unit.HP <= Mathf.RoundToInt(user.unit.MaxHP * 0.3f))
            {
                user.costumvlue_third = 1;
            }
        }
    }
}
