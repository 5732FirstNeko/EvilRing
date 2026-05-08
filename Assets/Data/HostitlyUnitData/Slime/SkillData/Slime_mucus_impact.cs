using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Playables;

[CreateAssetMenu(fileName = "newUnitData", menuName = "Data/Skill Data/" + nameof(Slime_mucus_impact))]
public class Slime_mucus_impact : UnitSkillDataSo
{
    [SerializeField] private GameObject mucusAttackPrefab;
    [SerializeField] private GameObject mucusHitPrefab;
    [SerializeField] private GameObject impactHitPrefab;

    [SerializeField] private float mucusSpeed;
    [SerializeField] private int skillListIndex;
    [SerializeField] private Vector3 impactPosition;
    [SerializeField] private UnitDataSo slimeCard;

    private GameObject mucusAttackEffect;
    private GameObject mucusHitEffect;
    private List<GameObject> impactHiteffects;
    public override void GameStartInit()
    {
        base.GameStartInit();

        mucusAttackEffect = Instantiate(mucusAttackPrefab, Vector3.zero, Quaternion.identity);
        mucusAttackEffect.SetActive(false);
        BattleSystem.instance.destoryEffect.Add(mucusAttackEffect);

        mucusHitEffect = Instantiate(mucusHitPrefab, Vector3.zero, Quaternion.identity);
        mucusHitEffect.SetActive(false);
        BattleSystem.instance.destoryEffect.Add(mucusHitEffect);

        impactHiteffects = new List<GameObject>();
        for (int i = 0; i < 4; i++)
        {
            GameObject effect = Instantiate(impactHitPrefab, Vector3.zero, Quaternion.identity);
            effect.SetActive(false);
            impactHiteffects.Add(effect);
            BattleSystem.instance.destoryEffect.Add(effect);
        }
    }

    public override void GameEndAction()
    {
        mucusAttackEffect = null;
        mucusHitEffect = null;
        impactHiteffects = null;
    }

    public override void Action(ICollection<UnitPlat> unitPlats, UnitPlat user)
    {
        if (user.unit.HP <= user.unit.MaxHP * 0.5f)
        {
            SPAttack(unitPlats, user);
        }
        else
        {
            StandrdAttack(unitPlats, user);
        }
    }

    private void SPAttack(ICollection<UnitPlat> unitPlats, UnitPlat user)
    {
        if (user.unit.HP >= user.unit.MaxHP * 0.5f)
        {
            user.unit.unitSkills[skillListIndex].SkillTime = 0.5f;
            BattleSystem.instance.UnitReEnqueue(user);
            return;
        }

        UnitPlat nearPlat = null;
        int userIndex = BattleSystem.GetIndexByUnitSite(user.site);
        UnitSite rightSite = BattleSystem.GetUnitSiteByIndex(userIndex + 1);
        UnitSite leftSite = BattleSystem.GetUnitSiteByIndex(userIndex - 1);
        UnitPlat rightPlat = BattleSystem.instance.HostilityUnitPlatsQueue.GetUnitPlatByUnitSite(rightSite).plat;
        UnitPlat leftPlat = BattleSystem.instance.HostilityUnitPlatsQueue.GetUnitPlatByUnitSite(leftSite).plat;
        if (rightPlat != null && rightPlat.unitData == FactorySystem.instance.EmptyHostitlyUnitData)
        {
            nearPlat = rightPlat;
        }

        if (nearPlat == null && leftPlat != null && leftPlat.unitData == FactorySystem.instance.EmptyHostitlyUnitData)
        {
            nearPlat = leftPlat;
        }

        GameManager.instance.GlobalLightControll(0.5f, 0.5f);
        user.transform.DOScale(UnitPlat.originScale * attackScale, 0.5f);

        TimerManager.instance.StartTimer(name + "SlimeDivision", 0.6f,
            () =>
            {
                user.UnitPlatInit(slimeCard, user.site);
                slimeCard.Skills[skillListIndex].GameStartInit();
                BattleSystem.instance.UnitReEnqueue(user);
                Debug.LogError(user.isDead);

                user.iconSpriteRender.sprite = slimeCard.UnitSprite;
                user.iconSpriteRender.material = GameManager.litMaterial;
                Vector3 originScale = user.transform.localScale;
                user.transform.localScale = Vector3.zero;
                user.transform.DOScale(originScale, 1.5f).OnComplete(
                    () =>
                    {
                        GameManager.instance.GlobalLightControll(1f, 0.5f);
                        user.transform.DOScale(UnitPlat.originScale, 0.5f);
                    });

                if (nearPlat == null) return;

                nearPlat.iconSpriteRender.material = GameManager.litMaterial;
                nearPlat.UnitPlatInit(slimeCard, nearPlat.site);
                slimeCard.Skills[skillListIndex].GameStartInit();
                BattleSystem.instance.UnitReEnqueue(nearPlat);
                Debug.LogError(nearPlat.isDead);

                nearPlat.iconSpriteRender.sprite = slimeCard.UnitSprite;
                Vector3 nearoriginScale = nearPlat.transform.localScale;
                nearPlat.transform.localScale = Vector3.zero;
                nearPlat.transform.DOScale(nearoriginScale, 1.5f);
            });

        if (user.unit != null)
        {
            user.unit.unitSkills[skillListIndex].SkillTime = 0.6f + 1.5f + 0.5f + 0.1f;
        }
    }

    private void StandrdAttack(ICollection<UnitPlat> unitPlats, UnitPlat user)
    {
        unitPlats = BattleSystem.instance.FriendlyUnitPlatsQueue.GetAllUnitPlat();

        if (unitPlats.Count <= 0)
        {
            Debug.LogError("Standrd Attack Exit !");
            user.unit.unitSkills[skillListIndex].SkillTime = 0.5f;
            return;
        }

        UnitPlat target = null;
        foreach (var unit in unitPlats)
        {
            if (target == null && unit.unitData != FactorySystem.instance.EmptyHostitlyUnitData)
            {
                target = unit;
                break;
            }
        }

        GameManager.instance.GlobalLightControll(0.5f, 0.5f);
        user.transform.DOScale(UnitPlat.originScale * attackScale, 0.5f);

        int index = Random.Range(0, 101);
        if (index <= 25)
        {
            mucusAttack(user, target);
        }
        else
        {
            ImpactAttack(unitPlats, user);
        }
    }

    private void mucusAttack(UnitPlat user, UnitPlat target)
    {
        float mucusMoveTime = Vector3.Distance(target.transform.position, user.transform.position) / mucusSpeed;
        TimerManager.instance.StartTimer(name + "mucusAttackEffect", 0.6f,
            () =>
            {
                mucusAttackEffect.transform.position = user.transform.position;
                mucusAttackEffect.SetActive(true);

                mucusAttackEffect.transform.DOMove(target.transform.position, mucusMoveTime).
                OnComplete(
                    () =>
                    {
                        mucusAttackEffect.SetActive(false);
                        mucusHitEffect.transform.position = target.transform.position;
                        mucusHitEffect.SetActive(true);
                        mucusHitEffect.GetComponent<PlayableDirector>().Play();

                        target.UnitPlatHurtAnimation();
                        target.unit.HP -= Damage;
                    });

            });

        TimerManager.instance.StartTimer(name + "hitEffectClose", 0.6f + mucusMoveTime + 0.1f,
            () =>
            {
                mucusHitEffect.SetActive(false);

                GameManager.instance.GlobalLightControll(1f, 0.5f);
                user.transform.DOScale(UnitPlat.originScale, 0.5f);
            });

        user.unit.unitSkills[skillListIndex].SkillTime = 0.6f + mucusMoveTime + 0.1f + 0.5f + 0.1f;
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
                            if (unit.unitData != FactorySystem.instance.EmptyFriendlyUnitData && !unit.isDead &&
                                unit.site == UnitSite.first)
                            {
                                first = unit;
                                break;
                            }
                        }
                        foreach (var unit in unitPlats)
                        {
                            if (unit.unitData != FactorySystem.instance.EmptyFriendlyUnitData && !unit.isDead &&
                                unit.site == UnitSite.second)
                            {
                                second = unit;
                                break;
                            }
                        }
                        foreach (var unit in unitPlats)
                        {
                            if (unit.unitData != FactorySystem.instance.EmptyFriendlyUnitData && !unit.isDead &&
                                unit.site == UnitSite.third)
                            {
                                third = unit;
                                break;
                            }
                        }
                        foreach (var unit in unitPlats)
                        {
                            if (unit.unitData != FactorySystem.instance.EmptyFriendlyUnitData && !unit.isDead &&
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

                                            first.unit.HP -= Damage + 2;
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
