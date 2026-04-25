using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Playables;

[CreateAssetMenu(fileName = "newUnitData", menuName = "Data/Skill Data/Guards/" + nameof(Guards_bladeWarden))]
public class Guards_bladeWarden : UnitSkillDataSo
{
    [SerializeField] private GameObject doubleEdgedPrefab;
    [SerializeField] private GameObject attackPrefab;
    [SerializeField] private GameObject hitPrefab;

    [SerializeField] private int skillListIndex;
    [SerializeField] private UnitDataSo userData;

    private GameObject doubleEdgedEffect;
    private GameObject attackEffect;
    private List<GameObject> hitEffects;

    //one is deafend State, zero is attack State
    //hurt in round is one, no hurt in round is zero
    public override void GameStartInit()
    {
        base.GameStartInit();

        doubleEdgedEffect = Instantiate(doubleEdgedPrefab, Vector3.zero, Quaternion.identity);
        doubleEdgedEffect.SetActive(false);

        attackEffect = Instantiate(attackPrefab, Vector3.zero, Quaternion.identity);
        attackEffect.SetActive(false);

        hitEffects = new List<GameObject>();
        for (int i = 0; i < 3; i++)
        {
            GameObject effect = Instantiate(hitPrefab, Vector3.zero, Quaternion.identity);
            effect.SetActive(false);
            hitEffects.Add(effect);
        }

        UnitPlat user = null;
        foreach (var unit in BattleSystem.instance.HostilityUnitPlatsQueue.GetAllUnitPlat())
        {
            if (unit.unitData == user)
            {
                user = unit;
            }
        }

        user.unit.OnHPChange += HPChangeAction;
        BattleSystem.instance.OnRoundStart += StateChange;
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
        int index = Random.Range(0, 101);

        if (index <= 75)
        {
            Vector3 spwanPos = Vector3.zero;
            List<UnitPlat> attackTarget = new List<UnitPlat>();
            foreach (var unit in unitPlats)
            {
                if (!unit.isDead &&
                    unit.unitData != FactorySystem.instance.EmptyFriendlyUnitData)
                {
                    attackTarget.Add(unit);
                    spwanPos += unit.transform.position;
                    if (attackTarget.Count > 2)
                    {
                        break;
                    }
                }
            }
            spwanPos /= attackTarget.Count;

            if (attackTarget.Count <= 0)
            {
                user.unit.unitSkills[skillListIndex].SkillTime = 0.5f;
                return;
            }

            GameManager.instance.GlobalLightControll(0.5f, 0.5f);
            user.transform.DOScale(UnitPlat.originScale * attackScale, 0.5f);

            PlayableDirector attackDirector = doubleEdgedPrefab.GetComponent<PlayableDirector>();
            TimerManager.instance.StartTimer(name + "AttackEffect", 0.6f,
                () =>
                {
                    doubleEdgedPrefab.transform.position = spwanPos;
                    doubleEdgedPrefab.SetActive(true);
                    attackDirector.Play();
                });

            PlayableDirector hitDirector = hitEffects[0].GetComponent<PlayableDirector>();
            TimerManager.instance.StartTimer(name + "HitEffect", 0.6f + (float)attackDirector.duration,
                () =>
                {
                    for (int i = 0; i < attackTarget.Count; i++)
                    {
                        int index = i;
                        hitEffects[index].transform.position = attackTarget[i].transform.position;
                        hitEffects[index].SetActive(false);
                        hitEffects[index].GetComponent<PlayableDirector>().Play();

                        attackTarget[index].unit.HP -= Damage;
                        attackTarget[index].UnitPlatHurtAnimation();
                    }
                });

            TimerManager.instance.StartTimer(name + "EffectClose",
                0.6f + (float)attackDirector.duration + (float)hitDirector.duration,
                () =>
                {
                    doubleEdgedPrefab.SetActive(false);
                    for (int i = 0; i < hitEffects.Count; i++)
                    {
                        hitEffects[i].SetActive(false);
                    }

                    GameManager.instance.GlobalLightControll(1f, 0.5f);
                    user.transform.DOScale(UnitPlat.originScale, 0.5f);
                });

            user.unit.unitSkills[skillListIndex].SkillTime =
                0.6f + (float)attackDirector.duration + (float)hitDirector.duration + 0.6f;
        }
        else
        {
            UnitPlat target = null;
            foreach (var unit in unitPlats)
            {
                if (!unit.isDead && unit.unitData != FactorySystem.instance.EmptyFriendlyUnitData)
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
            }

            if (target != null)
            {
                user.unit.unitSkills[skillListIndex].SkillTime = 0.5f;
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

            PlayableDirector hitDirector = hitEffects[0].GetComponent<PlayableDirector>();
            TimerManager.instance.StartTimer(name + "HitEffect", 0.6f + (float)attackDirector.duration, 
                () => 
                {
                    hitEffects[0].transform.position = target.transform.position;
                    hitEffects[0].SetActive(true);
                    hitDirector.Play();

                    target.unit.HP -= Damage;
                    target.UnitPlatHurtAnimation();
                });

            TimerManager.instance.StartTimer(name + "EffectClose",
                0.6f + (float)attackDirector.duration + (float)hitDirector.duration + 0.1f, 
                () => 
                {
                    attackEffect.SetActive(false);
                    hitEffects[0].SetActive(false);

                    GameManager.instance.GlobalLightControll(1f, 0.5f);
                    user.transform.DOScale(UnitPlat.originScale, 0.5f);
                });

            user.unit.unitSkills[skillListIndex].SkillTime =
                0.6f + (float)attackDirector.duration + (float)hitDirector.duration + 0.1f + 0.6f;
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
            if (unit.unitData == user)
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
        else if(user.costumvalue_second == 0)
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
                user.unit.HP <= Mathf.RoundToInt(user.unit.MaxHP * 0.6f))
            {
                user.costumvlue_third = 1;
            }
        }
    }
}
