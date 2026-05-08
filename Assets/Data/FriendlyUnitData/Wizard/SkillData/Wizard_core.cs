using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Playables;

[CreateAssetMenu(fileName = "newUnitData", menuName = "Data/Skill Data/" + nameof(Wizard_core))]
public class Wizard_core : UnitSkillDataSo
{
    [SerializeField] private GameObject explsionPrefab;
    [SerializeField] private GameObject standrdMagicPrefab;
    [SerializeField] private GameObject standrdhitPrefab;

    [SerializeField] private int explsionEnergy;
    [SerializeField] private float standrdAttackSpeed;
    [SerializeField] private int skillListIndex;
    [SerializeField] private Vector2 explsionPosition;

    private GameObject explsionEffect;
    private GameObject standrdMagicAttackEffect;
    private GameObject standrdHitEffect;
    private List<GameObject> explsionHitList;

    public override void GameStartInit()
    {
        base.GameStartInit();

        explsionEffect = Instantiate(explsionPrefab, Vector3.zero, Quaternion.identity);
        explsionEffect.SetActive(false);
        BattleSystem.instance.destoryEffect.Add(explsionEffect);

        standrdMagicAttackEffect = Instantiate(standrdMagicPrefab, Vector3.zero, Quaternion.identity);
        standrdMagicAttackEffect.SetActive(false);
        BattleSystem.instance.destoryEffect.Add(standrdMagicAttackEffect);

        standrdHitEffect = Instantiate(standrdhitPrefab, Vector3.zero, Quaternion.identity);
        standrdHitEffect.SetActive(false);
        BattleSystem.instance.destoryEffect.Add(standrdHitEffect);

        explsionHitList = new List<GameObject>();
        for (int i = 0; i < 4; i++)
        {
            GameObject effect = Instantiate(standrdhitPrefab, Vector3.zero, Quaternion.identity);
            effect.SetActive(false);
            explsionHitList.Add(effect);
            BattleSystem.instance.destoryEffect.Add(explsionEffect);
        }
    }

    public override void GameEndAction()
    {
        explsionEffect = null;
        standrdMagicAttackEffect = null;
        standrdHitEffect = null;

        explsionHitList = null;
    }

    public override void Action(ICollection<UnitPlat> unitPlats, UnitPlat user)
    {
        if (unitPlats.Count <= 0)
        {
            user.unit.unitSkills[skillListIndex].SkillTime = 0;
            return;
        }

        GameManager.instance.GlobalLightControll(0.5f, 0.5f);
        user.transform.DOScale(UnitPlat.originScale * attackScale, 0.5f);

        if (user.costumvalue_first >= explsionEnergy)
        {
            Explsion(unitPlats, user);
            user.costumvalue_first = 0;
            return;
        }
        else
        {
            int index = UnityEngine.Random.Range(0, unitPlats.Count);
            UnitPlat unitPlat = null;
            int i = 0;
            foreach (var tar in unitPlats)
            {
                if (i >= index && !tar.isDead && tar.unitData == FactorySystem.instance.EmptyHostitlyUnitData)
                {
                    unitPlat = tar;
                    break;
                }
                i++;
            }
            StandrdAttack(unitPlat, user);

            foreach (var unit in BattleSystem.instance.FriendlyUnitPlatsQueue.GetAllUnitPlat())
            {
                if (FactorySystem.instance.wizardCards.Contains(unit.unitData))
                {
                    user.costumvalue_first++;
                }
            }
            user.costumvalue_first++;
        }
    }

    private void StandrdAttack(UnitPlat unitPlat,UnitPlat user)
    {
        if (unitPlat == null)
        {
            TimerManager.instance.StartTimer(name + "UnitScale", 0.6f, 
                () =>
                {
                    GameManager.instance.GlobalLightControll(1f, 0.5f);
                    user.transform.DOScale(UnitPlat.originScale, 0.5f);
                    user.unit.unitSkills[skillListIndex].SkillTime = 1.2f;
                });
        }

        standrdMagicAttackEffect.transform.position = user.transform.position;
        standrdMagicAttackEffect.SetActive(true);

        float movedis = Mathf.Abs(unitPlat.transform.position.x -
            standrdMagicAttackEffect.transform.position.x);
        float movetime = movedis / standrdAttackSpeed;

        standrdMagicAttackEffect.transform.DOMoveX(unitPlat.transform.position.x, movetime).
            OnComplete(() => 
            {
                unitPlat.UnitPlatHurtAnimation();
                unitPlat.unit.HP -= 12;

                standrdHitEffect.transform.position = unitPlat.transform.position;
                standrdHitEffect.SetActive(true);
                standrdHitEffect.GetComponent<PlayableDirector>().Play();
            });

        TimerManager.instance.StartTimer(name + "StandrdHitEffectClose",movetime + 1f,
            () => 
            {
                standrdMagicAttackEffect.SetActive(false);
                standrdHitEffect.SetActive(false);
                standrdMagicAttackEffect.transform.position = user.transform.position;
            });

        TimerManager.instance.StartTimer(name + "UnitScale", movetime + 1f, 
            () => 
            {
                GameManager.instance.GlobalLightControll(1f, 0.5f);
                user.transform.DOScale(UnitPlat.originScale, 0.5f);
            });

        user.unit.unitSkills[skillListIndex].SkillTime = movetime + 1f + 0.6f;
    }

    private void Explsion(ICollection<UnitPlat> unitPlats,UnitPlat user)
    {
        GameManager.instance.GlobalLightControll(0.5f, 0.5f);

        PlayableDirector director = explsionEffect.GetComponent<PlayableDirector>();

        TimerManager.instance.StartTimer(name + "LightDark", 0.5f, 
            () => 
            {
                explsionEffect.transform.position = explsionPosition;
                explsionEffect.SetActive(true);

                director.Play();
            });

        TimerManager.instance.StartTimer(name + "explsion",(float)director.duration * 0.5f + 0.5f, 
            () => 
            {
                int i = 0;
                foreach (var unit in unitPlats)
                {
                    if (unit.isDead || unit.unitData == FactorySystem.instance.EmptyHostitlyUnitData)
                    {
                        continue;
                    }

                    int index = i;
                    unit.unit.HP -= 20;
                    unit.UnitPlatHurtAnimation(6, 0.1f, () => 
                    {
                        explsionHitList[index].transform.position = unit.transform.position;
                        if (!explsionHitList[index].activeSelf)
                        {
                            explsionHitList[index].SetActive(true);
                            standrdHitEffect.GetComponent<PlayableDirector>().Play();
                        }
                        else
                        {
                            explsionHitList[index].SetActive(false);
                        }
                    });
                    i++;
                }
            });

        TimerManager.instance.StartTimer(name + "explsionClose", (float)director.duration + 0.5f + 0.5f,
            () => 
            {
                explsionEffect.SetActive(false);

                foreach (var effect in explsionHitList)
                {
                    effect.SetActive(false);
                }
                GameManager.instance.GlobalLightControll(1f, 0.5f);
            });

        user.costumvalue_first -= 9;
        int wizardCount = 0;
        foreach (var unit in
            BattleSystem.instance.FriendlyUnitPlatsQueue.GetAllUnitPlat())
        {
            foreach (var data in FactorySystem.instance.wizardCards)
            {
                if (unit.unitData == data)
                {
                    wizardCount++;
                    break;
                }
            }
        }
        user.costumvalue_first += wizardCount;

        user.unit.unitSkills[skillListIndex].SkillTime = (float)director.duration + 0.5f;

        user.transform.DOScale(UnitPlat.originScale, 0.5f).SetDelay((float)director.duration + 0.5f);
    }
}
