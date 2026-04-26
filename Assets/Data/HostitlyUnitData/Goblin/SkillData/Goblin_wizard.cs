using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Playables;

[CreateAssetMenu(fileName = "newUnitData", menuName = "Data/Skill Data/Goblin/" + nameof(Goblin_wizard))]
public class Goblin_wizard : UnitSkillDataSo
{
    [SerializeField] private GameObject wizardPrefab;
    [SerializeField] private GameObject wizardHitPrefab;
    [SerializeField] private GameObject flashAttackPrefab;
    [SerializeField] private GameObject flahsHitPrefab;

    [SerializeField] private int skillListIndex;
    [SerializeField] private UnitDataSo unitData;
    [SerializeField] private float hitStartRate;

    private GameObject wizardEffect;
    private List<GameObject> wizardHitEffects;
    private GameObject flashEffect;
    private GameObject flashHiteffect;
    private UnitPlat m_user;

    public override void GameStartInit()
    {
        base.GameStartInit();

        wizardEffect = Instantiate(wizardPrefab, Vector3.zero, Quaternion.identity);
        wizardEffect.SetActive(false);

        wizardHitEffects = new List<GameObject>();
        for (int i = 0; i < 4; i++)
        {
            GameObject effect = Instantiate(wizardHitPrefab, Vector3.zero, Quaternion.identity);
            effect.SetActive(false);
            wizardHitEffects.Add(effect);
        }

        flashEffect = Instantiate(flashHiteffect, Vector3.zero, Quaternion.identity);
        flashEffect.SetActive(false);

        flashHiteffect = Instantiate(flahsHitPrefab, Vector3.zero, Quaternion.identity);
        flashHiteffect.SetActive(false);

        foreach (var unit in 
            BattleSystem.instance.HostilityUnitPlatsQueue.GetAllUnitPlat())
        {
            if (unit.unitData == FactorySystem.Instance.GoblineCards.Contains(unit.unitData) && 
                unit.unitData != m_user.unitData && unit.unitData != FactorySystem.instance.EmptyHostitlyUnitData)
            {
                unit.unit.OnDead += UnitDeadAction;
            }
            if (unit.unitData == this.unitData)
            {
                m_user = unit;
            }
        }

        BattleSystem.instance.OnRoundStart += (round) => { m_user.costumvalue_first = 1; return 0; };
    }

    public override void Action(ICollection<UnitPlat> unitPlats, UnitPlat user)
    {
        UnitPlat target = null;
        foreach (var unit in unitPlats)
        {
            if (unit.unitData != FactorySystem.instance.EmptyFriendlyUnitData && !unit.isDead)
            {
                target = unit;
                return;
            }
        }

        if (target == null)
        {
            user.unit.unitSkills[skillListIndex].SkillTime = 0.5f;
            return;
        }

        GameManager.instance.GlobalLightControll(0.5f, 0.5f);
        user.transform.DOScale(UnitPlat.originScale * attackScale, 0.5f);

        PlayableDirector attackDirector = flashEffect.GetComponent<PlayableDirector>();
        TimerManager.instance.StartTimer(name + "FlashEffect", 0.6f, 
            () =>
            {
                flashEffect.transform.position = target.transform.position;
                flashEffect.SetActive(false);
                attackDirector.Play();
            });

        PlayableDirector hitDirector = flashHiteffect.GetComponent<PlayableDirector>();
        TimerManager.instance.StartTimer(name + "HitEffect", 0.6f + (float)attackDirector.duration, 
            () => 
            {
                flashHiteffect.transform.position = target.transform.position;
                flashHiteffect.SetActive(false);
                hitDirector.Play();

                target.UnitPlatHurtAnimation();
                target.unit.HP -= Damage;
            });

        TimerManager.instance.StartTimer(name + "FlashEffectClose", 
            0.6f + (float)attackDirector.duration + (float)hitDirector.duration, 
            () => 
            {
                flashEffect.SetActive(false);
                flashHiteffect.SetActive(false);

                GameManager.instance.GlobalLightControll(1f, 0.5f);
                user.transform.DOScale(UnitPlat.originScale, 0.5f);
            });

        user.unit.unitSkills[skillListIndex].SkillTime =
            0.6f + (float)attackDirector.duration + (float)hitDirector.duration + 0.6f;
    }

    public void UnitDeadAction(UnitPlat user)
    {
        UnitPlat wizard = null;
        foreach (var unit in BattleSystem.instance.HostilityUnitPlatsQueue.GetAllUnitPlat())
        {
            if (unit.unitData == this.unitData)
            {
                wizard = unit;
                break;
            }
        }

        if (wizard == null || this.m_user.costumvalue_first <= 0)
        {
            user.unit.DeadAnimationTime = 1;
            return;
        }

        GameManager.instance.GlobalLightControll(0.5f, 0.5f);
        wizard.transform.DOScale(UnitPlat.originScale * attackScale, 0.5f);

        Vector3 spawnPostion = Vector3.zero;
        foreach (var unit in BattleSystem.instance.FriendlyUnitPlatsQueue.GetAllUnitPlat())
        {
            if (unit.unitData != FactorySystem.instance.EmptyFriendlyUnitData)
            {
                spawnPostion += unit.transform.position;
            }
        }

        PlayableDirector director = wizardEffect.GetComponent<PlayableDirector>();
        TimerManager.instance.StartTimer(name + "ExplsionEffect", 0.6f, 
            () => 
            {
                wizard.DamageTextJump("Ä§·¨¹¥»÷", Color.white);
                wizardEffect.transform.position = spawnPostion;
                wizardEffect.SetActive(true);

                director.Play();
            });

        TimerManager.instance.StartTimer(name + "hITEffect", 0.6f + (float)director.duration * hitStartRate, 
            () => 
            {
                int frenquency = Mathf.RoundToInt((float)director.duration * (1 - hitStartRate) / 0.25f);
                int i = 0;
                foreach (var unit in BattleSystem.instance.FriendlyUnitPlatsQueue.GetAllUnitPlat())
                {
                    int index = i;
                    if (unit.unitData != FactorySystem.instance.EmptyFriendlyUnitData)
                    {
                        unit.UnitPlatHurtAnimation(frenquency, 0, 
                            () => 
                            {
                                unit.unit.HP -= Damage;
                                if (wizardHitEffects[index].activeSelf)
                                {
                                    wizardHitEffects[index].SetActive(false);
                                }
                                else
                                {
                                    wizardHitEffects[index].transform.position = unit.transform.position;
                                    wizardHitEffects[index].SetActive(true);
                                    wizardHitEffects[index].GetComponent<PlayableDirector>().Play();
                                }
                            });
                    }
                    i++;
                }
            });

        TimerManager.instance.StartTimer(name + "EffectClose",0.6f + (float)director.duration, 
            () => 
            {
                wizardEffect.SetActive(false);
                for (int i = 0; i < wizardHitEffects.Count; i++)
                {
                    wizardHitEffects[i].SetActive(true);
                }

                GameManager.instance.GlobalLightControll(1f, 0.5f);
                wizard.transform.DOScale(UnitPlat.originScale, 0.5f);
            });

        this.m_user.costumvalue_first = 0;
        user.unit.DeadAnimationTime = 1f + 0.6f + (float)director.duration + 0.5f + 0.1f;
    }
}
