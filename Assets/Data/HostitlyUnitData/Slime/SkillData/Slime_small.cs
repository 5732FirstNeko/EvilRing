using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Playables;

[CreateAssetMenu(fileName = "newUnitData", menuName = "Data/Skill Data/" + nameof(Slime_small))]
public class Slime_small : UnitSkillDataSo
{
    [SerializeField] private GameObject impactHitPrefab;

    [SerializeField] private int skillListIndex;
    [SerializeField] private Vector3 impactPosition;

    private List<GameObject> impactHiteffects;

    public override void GameStartInit()
    {
        base.GameStartInit();

        impactHiteffects = new List<GameObject>();
        for (int i = 0; i < 4; i++)
        {
            GameObject effect = Instantiate(impactHitPrefab, Vector3.zero, Quaternion.identity);
            effect.SetActive(false);
            impactHiteffects.Add(effect);
        }
    }

    public override void Action(ICollection<UnitPlat> unitPlats, UnitPlat user)
    {
        if (unitPlats.Count <= 0)
        {
            user.unit.unitSkills[skillListIndex].SkillTime = 0.5f;
            return;
        }

        GameManager.instance.GlobalLightControll(0.5f, 0.5f);
        user.transform.DOScale(UnitPlat.originScale * attackScale, 0.5f);

        ImpactAttack(unitPlats, user);
    }

    private void ImpactAttack(ICollection<UnitPlat> unitPlats, UnitPlat user)
    {
        TimerManager.instance.StartTimer(name + "Impact", 0.6f,
                    () =>
                    {
                        Vector3 originPosition = user.transform.position;

                        Tweener move = user.transform.DOMove(impactPosition, 1.5f).SetEase(Ease.Linear).OnComplete(
                            () =>
                            {
                                user.transform.DOMove(originPosition, 0.5f);
                            });

                        UnitPlat first = null;
                        UnitPlat second = null;
                        UnitPlat third = null;
                        UnitPlat fourth = null;
                        foreach (var unit in unitPlats)
                        {
                            if (unit.unitData != FactorySystem.instance.EmptyHostitlyUnitData &&
                                unit.site == UnitSite.first)
                            {
                                first = unit;
                                break;
                            }
                        }
                        foreach (var unit in unitPlats)
                        {
                            if (unit.unitData != FactorySystem.instance.EmptyHostitlyUnitData &&
                                unit.site == UnitSite.second)
                            {
                                second = unit;
                                break;
                            }
                        }
                        foreach (var unit in unitPlats)
                        {
                            if (unit.unitData != FactorySystem.instance.EmptyHostitlyUnitData &&
                                unit.site == UnitSite.third)
                            {
                                third = unit;
                                break;
                            }
                        }
                        foreach (var unit in unitPlats)
                        {
                            if (unit.unitData != FactorySystem.instance.EmptyHostitlyUnitData &&
                                unit.site == UnitSite.fourth)
                            {
                                fourth = unit;
                                break;
                            }
                        }

                        int index = 0;
                        move.OnUpdate(
                            () =>
                            {
                                switch (index)
                                {
                                    case 0:
                                        if (first == null)
                                        {
                                            index++;
                                            break;
                                        }

                                        if (Vector3.Distance(user.transform.position, first.transform.position) < 0.1f)
                                        {
                                            int i = index;
                                            impactHiteffects[i].transform.position = first.transform.position;
                                            impactHiteffects[i].SetActive(true);
                                            impactHiteffects[i].GetComponent<PlayableDirector>().Play();

                                            first.unit.HP -= Damage;
                                            first.UnitPlatHurtAnimation();
                                            index++;
                                        }
                                        break;
                                    case 1:
                                        if (second == null)
                                        {
                                            index++;
                                            break;
                                        }

                                        if (Vector3.Distance(user.transform.position, second.transform.position) < 0.1f)
                                        {
                                            int i = index;
                                            impactHiteffects[i].transform.position = second.transform.position;
                                            impactHiteffects[i].SetActive(true);
                                            impactHiteffects[i].GetComponent<PlayableDirector>().Play();

                                            second.unit.HP -= Damage;
                                            second.UnitPlatHurtAnimation();
                                            index++;
                                        }
                                        break;
                                    case 2:
                                        if (third == null)
                                        {
                                            index++;
                                            break;
                                        }

                                        if (Vector3.Distance(user.transform.position, third.transform.position) < 0.1f)
                                        {
                                            int i = index;
                                            impactHiteffects[i].transform.position = third.transform.position;
                                            impactHiteffects[i].SetActive(true);
                                            impactHiteffects[i].GetComponent<PlayableDirector>().Play();

                                            third.unit.HP -= Damage;
                                            third.UnitPlatHurtAnimation();
                                            index++;
                                        }
                                        break;
                                    case 3:
                                        if (fourth == null)
                                        {
                                            index++;
                                            break;
                                        }

                                        if (Vector3.Distance(user.transform.position, fourth.transform.position) < 0.1f)
                                        {
                                            int i = index;
                                            impactHiteffects[i].transform.position = fourth.transform.position;
                                            impactHiteffects[i].SetActive(true);
                                            impactHiteffects[i].GetComponent<PlayableDirector>().Play();

                                            fourth.unit.HP -= Damage;
                                            fourth.UnitPlatHurtAnimation();
                                            index++;
                                        }
                                        break;
                                }
                            });
                    });

        TimerManager.instance.StartTimer(name + "impactEffectClose", 0.6f + 2.1f,
            () =>
            {
                for (int i = 0; i < impactHiteffects.Count; i++)
                {
                    impactHiteffects[i].SetActive(false);
                }

                GameManager.instance.GlobalLightControll(1f, 0.5f);
                user.transform.DOScale(UnitPlat.originScale, 0.5f);
            });

        user.unit.unitSkills[skillListIndex].SkillTime = 0.6f + 2.1f + 0.5f + 0.1f;
    }
}
