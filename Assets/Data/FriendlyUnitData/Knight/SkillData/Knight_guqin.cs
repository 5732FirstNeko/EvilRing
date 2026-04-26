using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Playables;

[CreateAssetMenu(fileName = "newUnitData", menuName = "Data/Skill Data/" + nameof(Knight_guqin))]
public class Knight_guqin : UnitSkillDataSo
{
    [SerializeField] private GameObject hosAttackPrefab;
    [SerializeField] private GameObject friAttackPrefab;
    [SerializeField] private GameObject hitPrefab;

    [SerializeField] private int skillListIndex;

    private GameObject hosAttackEffect;
    private GameObject friAttackEffect;
    private List<GameObject> hitEffects;

    public override void GameStartInit()
    {
        base.GameStartInit();

        hosAttackEffect = Instantiate(hosAttackPrefab, Vector3.zero, Quaternion.identity);
        hosAttackEffect.SetActive(false);

        friAttackEffect = Instantiate(friAttackPrefab, Vector3.zero, Quaternion.identity);
        friAttackEffect.SetActive(false);

        hitEffects = new List<GameObject>();
        for (int i = 0; i < 5; i++)
        {
            GameObject effect = Instantiate(hitPrefab, Vector3.zero, Quaternion.identity);
            effect.SetActive(false);
            hitEffects.Add(effect);
        }
    }

    public override void Action(ICollection<UnitPlat> unitPlats, UnitPlat user)
    {
        List<UnitPlat> friunits = BattleSystem.instance.FriendlyUnitPlatsQueue.GetAllUnitPlat();

        UnitPlat friTarget = null;
        for (int i = 0; i < friunits.Count; i++)
        {
            if (friunits[i].unitData != null &&
                friunits[i] != user && FactorySystem.instance.knightCards.Contains(friunits[i].unitData) &&
                friunits[i].costumvalue_first > 0)
            {
                friTarget = friunits[i];
                break;
            }
        }

        UnitPlat hosTarget = null;
        if (friTarget == null)
        {
            foreach (var unit in unitPlats)
            {
                if (unit.unitData != null && unit.unitData != FactorySystem.instance.EmptyHostitlyUnitData)
                {
                    hosTarget = unit;
                }
            }
        }

        if (friTarget == null && hosTarget == null)
        {
            user.unit.unitSkills[skillListIndex].SkillTime = 0.5f;
            return;
        }

        if (friTarget != null)
        {
            FriAction(friTarget, user);
            return;
        }

        if (hosTarget != null)
        {
            hosAction(hosTarget, user);
        }
    }

    private void FriAction(UnitPlat target, UnitPlat user)
    {
        GameManager.instance.GlobalLightControll(0.5f, 0.5f);
        user.transform.DOScale(UnitPlat.originScale * attackScale, 0.5f);

        PlayableDirector director = null;
        TimerManager.instance.StartTimer(name + "friAttackEffect", 0.6f, 
            () => 
            {
                friAttackEffect.transform.position = user.transform.position;
                friAttackEffect.SetActive(true);

                friAttackEffect.transform.DOMove(target.transform.position, 1.5f).
                OnComplete(
                    () =>
                    {
                        friAttackEffect.SetActive(false);
                        GameObject hitEffect = null;
                        for (int i = 0; i < hitEffects.Count; i++)
                        {
                            if (!hitEffects[i].activeSelf)
                            {
                                hitEffect = hitEffects[i];
                            }
                        }

                        hitEffect.transform.position = target.transform.position;
                        hitEffect.SetActive(true);
                        director = hitEffect.GetComponent<PlayableDirector>();

                        director.Play();

                        target.unit.HP = 1;
                        target.costumvalue_first += 2;
                        target.UnitPlatHurtAnimation();
                    });
            });

        TimerManager.instance.StartTimer(name + "friHitEffectClose", 0.6f + (float)director.duration, 
            () => 
            {
                for (int i = 0; i < hitEffects.Count; i++)
                {
                    hitEffects[i].SetActive(false);
                }

                GameManager.instance.GlobalLightControll(1f, 0.5f);
                user.transform.DOScale(UnitPlat.originScale, 0.5f);
            });

        user.unit.unitSkills[skillListIndex].SkillTime = 0.6f + (float)director.duration + 0.7f;
    }

    private void hosAction(UnitPlat target, UnitPlat user)
    {
        GameManager.instance.GlobalLightControll(0.5f, 0.5f);
        user.transform.DOScale(UnitPlat.originScale * attackScale, 0.5f);

        PlayableDirector director = null;
        TimerManager.instance.StartTimer(name + "friAttackEffect", 0.6f,
            () =>
            {
                hosAttackEffect.transform.position = user.transform.position;
                hosAttackEffect.SetActive(true);

                hosAttackEffect.transform.DOMove(target.transform.position, 1.5f).
                OnComplete(
                    () =>
                    {
                        hosAttackEffect.SetActive(false);
                        GameObject hitEffect = null;
                        for (int i = 0; i < hitEffects.Count; i++)
                        {
                            if (!hitEffects[i].activeSelf)
                            {
                                hitEffect = hitEffects[i];
                            }
                        }

                        hitEffect.transform.position = target.transform.position;
                        hitEffect.SetActive(true);
                        director = hitEffect.GetComponent<PlayableDirector>();

                        director.Play();

                        target.UnitPlatHurtAnimation();
                        target.unit.HP -= Mathf.RoundToInt(Damage * (1 + 0.5f * user.costumvalue_first));
                    });
            });

        TimerManager.instance.StartTimer(name + "friHitEffectClose", 0.6f + (float)director.duration,
            () =>
            {
                for (int i = 0; i < hitEffects.Count; i++)
                {
                    hitEffects[i].SetActive(false);
                }

                GameManager.instance.GlobalLightControll(1f, 0.5f);
                user.transform.DOScale(UnitPlat.originScale, 0.5f);
            });

        user.unit.unitSkills[skillListIndex].SkillTime = 0.6f + (float)director.duration + 0.7f;
    }
}
