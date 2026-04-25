using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Playables;

[CreateAssetMenu(fileName = "newUnitData", menuName = "Data/Skill Data/Element/" + nameof(Element_ice))]
public class Element_ice : UnitSkillDataSo
{
    [SerializeField] private GameObject iceAttackPrefab;
    [SerializeField] private GameObject iceHitPrefab;

    [SerializeField] private Vector3 spwanPos;

    private List<GameObject> iceAttackEffect;
    private List<GameObject> iceHitEffect;

    public override void GameStartInit()
    {
        base.GameStartInit();

        iceAttackEffect = new List<GameObject>();
        iceHitEffect = new List<GameObject>();

        for (int i = 0; i < 4; i++)
        {
            GameObject attack = Instantiate(iceHitPrefab, Vector3.zero, Quaternion.identity);
            GameObject hit = Instantiate(iceHitPrefab, Vector3.zero, Quaternion.identity);

            attack.SetActive(false);
            hit.SetActive(false);

            iceAttackEffect.Add(attack);
            iceHitEffect.Add(hit);
        }
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
            Defend(user);
        }
    }

    private void Attack(UnitPlat user)
    {
        GameManager.instance.GlobalLightControll(0.5f, 0.5f);
        user.transform.DOScale(UnitPlat.originScale * attackScale, 0.5f);

        TimerManager.instance.StartTimer(name + "AttackEfect", 0.6f,
            () =>
            {
                int i = 0;
                foreach (var unit in
                    BattleSystem.instance.FriendlyUnitPlatsQueue.GetAllUnitPlat())
                {
                    if (!unit.isDead && unit.unitData != FactorySystem.instance.EmptyFriendlyUnitData)
                    {
                        int index = i;

                        iceAttackEffect[index].transform.position = user.transform.position +
                            UnitPlat.topDistance + spwanPos;

                        Vector3 direction = (unit.transform.position -
                            iceAttackEffect[index].transform.position).normalized;
                        Quaternion targetRot = Quaternion.LookRotation(direction, Vector3.up);
                        Quaternion axisCorrection = Quaternion.FromToRotation(Vector3.forward, Vector3.up);
                        iceAttackEffect[index].transform.rotation = targetRot * axisCorrection;

                        iceAttackEffect[index].SetActive(true);
                        iceAttackEffect[index].transform.DOMove(unit.transform.position, 1.5f).
                            OnComplete(() =>
                            {
                                iceHitEffect[index].transform.position = unit.transform.position;
                                iceHitEffect[index].SetActive(true);
                                iceHitEffect[index].GetComponent<PlayableDirector>().Play();


                                unit.UnitPlatHurtAnimation();
                                unit.unit.HP -= Damage;
                                if (user.costumvalue_first == 1)
                                {
                                    unit.DamageTextJump("减速", Color.white);
                                    unit.unit.speed -= 2;

                                    Func<int, float> func = null;

                                    func = (round) =>
                                    {
                                        unit.unit.speed += 2;
                                        BattleSystem.instance.OnRoundEnd -= func;
                                        return 0;
                                    };

                                    BattleSystem.instance.OnRoundEnd += func;
                                }
                            });

                        i++;

                        if (i >= 2)
                        {
                            break;
                        }
                    }

                }
            });

        TimerManager.instance.StartTimer(name + "EffectClose", 0.6f + 1.5f + 0.5f + 0.1f,
            () =>
            {
                for (int i = 0; i < 4; i++)
                {
                    iceAttackEffect[i].transform.position = spwanPos;
                    iceAttackEffect[i].SetActive(false);
                    iceHitEffect[i].SetActive(false);
                }

                GameManager.instance.GlobalLightControll(1f, 0.5f);
                user.transform.DOScale(UnitPlat.originScale, 0.5f);
            });

        user.unit.spCount -= 2;
        user.unit.unitSkills[0].SkillTime = 0.6f + 1.5f + 0.5f + 0.1f + 0.6f;
    }

    private void Defend(UnitPlat user)
    {
        GameManager.instance.GlobalLightControll(0.5f, 0.5f);
        user.transform.DOScale(UnitPlat.originScale * attackScale, 0.5f);

        TimerManager.instance.StartTimer(name + "DefendEffect", 0.6f,
            () =>
            {
                user.DamageTextJump("冰霜保护", Color.white);
                user.unit.OnDefend += DefendAction;
            });

        TimerManager.instance.StartTimer(name + "EffectClose", 0.6f + 1f,
            () =>
            {
                GameManager.instance.GlobalLightControll(1f, 0.5f);
                user.transform.DOScale(UnitPlat.originScale, 0.5f);
            });

        user.unit.unitSkills[0].SkillTime = 0.6f + 1f + 0.6f;
    }

    private int DefendAction(int hpcahnge, UnitPlat user)
    {
        if (hpcahnge < 0)
        {
            user.DamageTextJump("冰霜保护", Color.white);
            user.unit.OnDefend -= DefendAction;
            return hpcahnge - 15 > 0 ? 0 : hpcahnge - 15;
        }

        return hpcahnge;
    }
}
