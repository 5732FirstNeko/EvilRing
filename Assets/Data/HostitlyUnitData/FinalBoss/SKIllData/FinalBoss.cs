using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Playables;

[CreateAssetMenu(fileName = "newUnitData", menuName = "Data/Skill Data/FinalBoss/" + nameof(FinalBoss))]
public class FinalBoss : UnitSkillDataSo
{
    [SerializeField] private GameObject absorbPrefab;
    [SerializeField] private GameObject hitPrefab;
    [SerializeField] private GameObject flashPrefab;

    [SerializeField] private Vector3 offest;
    [SerializeField] private UnitDataSo tentacleData;
    [SerializeField] private UnitDataSo finalBossData;

    private List<GameObject> absorbEfffect;
    private List<GameObject> hitEffects;
    private List<GameObject> flashEffects;

    public override void GameStartInit()
    {
        base.GameStartInit();

        absorbEfffect = new List<GameObject>();
        for (int i = 0; i < 3; i++)
        {
            GameObject effect = Instantiate(absorbPrefab, Vector3.zero, Quaternion.identity);
            effect.SetActive(false);
            absorbEfffect.Add(effect);
            BattleSystem.instance.destoryEffect.Add(effect);
        }

        hitEffects = new List<GameObject>();
        for (int i = 0; i < 4; i++)
        {
            GameObject effect = Instantiate(hitPrefab, Vector3.zero, Quaternion.identity);
            effect.SetActive(false);
            hitEffects.Add(effect);
            BattleSystem.instance.destoryEffect.Add(effect);
        }

        flashEffects = new List<GameObject>();
        for (int i = 0; i < 4; i++)
        {
            GameObject effect = Instantiate(flashPrefab, Vector3.zero, Quaternion.identity);
            effect.SetActive(false);
            flashEffects.Add(effect);
            BattleSystem.instance.destoryEffect.Add(effect);
        }

        BattleSystem.instance.OnGameStart += 
            () => 
            {
                foreach (var unit in BattleSystem.instance.HostilityUnitPlatsQueue.GetAllUnitPlat())
                {
                    if (unit.unitData == finalBossData && unit.unit.OnDefend == null)
                    {
                        unit.unit.OnDefend += HPAction;
                    }
                }
                return 0f; 
            };
    }

    public override void GameEndAction()
    {
        absorbEfffect = null;

        hitEffects = null;

        flashEffects = null;
    }

    private int HPAction(int hpchange, UnitPlat user)
    {
        if (user == null || user.unit == null) return hpchange;

        if (hpchange >= 0) return hpchange;

        int currentHP = user.unit.HP;
        int finalHP = currentHP + hpchange;

        if (user.costumvalue_second == 1)
        {
            if (finalHP < 40)
            {
                return 40 - currentHP;
            }
        }
        else if (user.costumvalue_second == 0)
        {
            if (finalHP < 80)
            {
                return 80 - currentHP;
            }
        }

        return hpchange;
    }

    public override void Action(ICollection<UnitPlat> unitPlats, UnitPlat user)
    {
        float ActionTime = 0f;

        GameManager.instance.GlobalLightControll(0.5f, 0.5f);
        user.transform.DOScale(UnitPlat.originScale * attackScale, 0.5f);
        ActionTime += 0.5f;

        if (user.costumvalue_second == 0 && user.unit.HP <= 80)
        {
            DOVirtual.DelayedCall(ActionTime, () =>
            {
                Skill_sp_0(user);
            });

            ActionTime += 1.1f + 1.1f;
        }
        else if (user.costumvalue_second == 1 && user.unit.HP <= 40)
        {
            DOVirtual.DelayedCall(ActionTime, () =>
            {
                SKill_sp_1(user);
            });
            ActionTime += 1.1f;

            DOVirtual.DelayedCall(ActionTime, () =>
            {
                Skill_sp_3(user);
            });
            ActionTime += 1.1f;

            Skill_sp_4(user);
        }

        if (user.costumvalue_second == 0)
        {
            if (BattleSystem.instance.currentRound % 3 == 0)
            {
                DOVirtual.DelayedCall(ActionTime, () =>
                {
                    SKill_sp_2(user);
                });

                ActionTime += 2.6f;
            }

            DOVirtual.DelayedCall(ActionTime, () =>
            {
                Skill_sp_5(user);
            });

            int count = 0;
            foreach (var unit in
                BattleSystem.instance.FriendlyUnitPlatsQueue.GetAllUnitPlat())
            {
                if (!unit.isDead && unit.unitData != FactorySystem.instance.EmptyFriendlyUnitData &&
                    unit.costumvlue_fourth >= 3)
                {
                    count++;
                }
            }
            ActionTime += count == 0 ? 0 : 2.1f;

            int index = UnityEngine.Random.Range(0, 101);
            if (index <= 75)
            {
                DOVirtual.DelayedCall(ActionTime, () =>
                {
                    Skill_0(user);
                });

                count = 0;
                foreach (var unit in
                    BattleSystem.instance.FriendlyUnitPlatsQueue.GetAllUnitPlat())
                {
                    if (!unit.isDead && unit != user)
                    {
                        count++;
                    }
                }
                ActionTime += count != 0 ? 0.6f : 0f;
            }
            else
            {
                DOVirtual.DelayedCall(ActionTime, () =>
                {
                    Skill_1(user);
                });

                UnitPlat target = null;
                foreach (var unit in BattleSystem.instance.FriendlyUnitPlatsQueue.GetAllUnitPlat())
                {
                    if (target == null && !unit.isDead &&
                        unit.unitData != FactorySystem.instance.EmptyFriendlyUnitData)
                    {
                        target = unit;
                        continue;
                    }

                    if (target.unit.HP > unit.unit.HP && !unit.isDead &&
                        unit.unitData != FactorySystem.instance.EmptyFriendlyUnitData)
                    {
                        target = unit;
                    }
                }
                ActionTime += target == null ? 0f : 1.7f;
            }
        }
        else if (user.costumvalue_second == 1)
        {
            DOVirtual.DelayedCall(ActionTime, () =>
            {
                Skill_sp_6(user);
            });
            int count = 0;
            foreach (var unit in BattleSystem.instance.FriendlyUnitPlatsQueue.GetAllUnitPlat())
            {
                if (!unit.isDead && unit.unitData != FactorySystem.instance.EmptyFriendlyUnitData &&
                            unit.costumvlue_fourth >= 4)
                {
                    count++;
                }
            }
            ActionTime += count == 0 ? 0 : 1.1f + 2f;

            int index = UnityEngine.Random.Range(0, 101);

            if (index <= 75)
            {
                DOVirtual.DelayedCall(ActionTime, () =>
                {
                    Skill_2(user);
                });

                count = 0;
                foreach (var unit in
                    BattleSystem.instance.FriendlyUnitPlatsQueue.GetAllUnitPlat())
                {
                    if (!unit.isDead && unit != user)
                    {
                        count++;
                    }
                }
                ActionTime += count == 0 ? 0 : 0.6f;
            }
            else
            {
                DOVirtual.DelayedCall(ActionTime, () =>
                {
                    Skill_3(user);
                });

                UnitPlat target = null;
                foreach (var unit in BattleSystem.instance.FriendlyUnitPlatsQueue.GetAllUnitPlat())
                {
                    if (!unit.isDead && unit.unitData != FactorySystem.instance.EmptyFriendlyUnitData)
                    {
                        target = unit;
                        break;
                    }
                }
                ActionTime += target == null ? 0 : 1.1f;
            }
        }
        else if (user.costumvalue_second == 2)
        {
            DOVirtual.DelayedCall(ActionTime, () =>
            {
                Skill_sp_7(user);
            });
            int count = 0;
            foreach (var unit in BattleSystem.instance.FriendlyUnitPlatsQueue.GetAllUnitPlat())
            {
                if (!unit.isDead && unit.unitData != FactorySystem.instance.EmptyFriendlyUnitData &&
                            unit.costumvlue_fourth >= 5)
                {
                    count++;
                }
            }
            ActionTime += count == 0 ? 0f : 2.6f;

            DOVirtual.DelayedCall(ActionTime, () =>
            {
                Skill_4(user);
            });
            count = 0;
            foreach (var unit in
                BattleSystem.instance.FriendlyUnitPlatsQueue.GetAllUnitPlat())
            {
                if (!unit.isDead && unit != user)
                {
                    count++;
                }
            }
            ActionTime += count != 0 ? 0.6f : 0f;
        }

        GameManager.instance.GlobalLightControll(1f, 0.5f);
        user.transform.DOScale(UnitPlat.originScale, 0.5f);

        user.unit.unitSkills[0].SkillTime = ActionTime;
    }

    private float Skill_sp_0(UnitPlat user)
    {
        Debug.LogError("[FinalBoss Skill] skill_sp_0");

        Vector3 userPosition = user.transform.position;

        int count = 0;
        foreach (var unit in
            BattleSystem.instance.HostilityUnitPlatsQueue.GetAllUnitPlat())
        {
            if (unit.unitData == tentacleData)
            {
                int index = count;
                absorbEfffect[index].transform.position = unit.transform.position;
                absorbEfffect[index].SetActive(true);

                Vector3[] path = new Vector3[]
                {
                      unit.transform.position,
                      userPosition +=
                      unit.site == UnitSite.first || unit.site == UnitSite.second ?
                        new Vector3(-offest.x, offest.y,offest.z) :
                        new Vector3(offest.x, offest.y,offest.z),
                      user.transform.position,
                };

                absorbEfffect[index].transform.DOPath(path, 1f, PathType.CatmullRom).
                OnComplete(() =>
                {
                    unit.DamageTextJump("吸收", Color.white);
                    absorbEfffect[index].SetActive(false);
                });

                unit.UnitPlatInit(FactorySystem.instance.EmptyHostitlyUnitData, unit.site);

                unit.iconSpriteRender.sprite = tentacleData.BattleSprite;
                Vector3 originScale = unit.transform.localScale;
                unit.transform.localScale = Vector3.zero;
                unit.transform.DOScale(originScale, 1.5f);

                count++;
            }
        }

        TimerManager.instance.StartTimer(name + "RecoveryEffect", 1.1f,
        () =>
        {
            user.unit.HP += 10 * count;
            user.UnitPlatRecoveryAnimation();

            user.DamageTextJump("深渊回响", GameManager.purple);
            user.costumvalue_second = 1;
        });

        return 1.1f + 1.1f;
    }

    private float SKill_sp_1(UnitPlat user)
    {
        Debug.LogError("[FinalBoss Skill] skill_sp_1");
        foreach (var unit in
                    BattleSystem.instance.HostilityUnitPlatsQueue.GetAllUnitPlat())
        {
            if (unit.unitData != FactorySystem.instance.EmptyFriendlyUnitData)
            {
                unit.costumvlue_fourth = 0;
            }
        }

        user.DamageTextJump("吞噬者", GameManager.purple);
        user.costumvalue_second = 2;

        return 1.1f;
    }

    private float SKill_sp_2(UnitPlat user)
    {
        Debug.LogError("[FinalBoss Skill] skill_sp_2");
        user.DamageTextJump("召唤", GameManager.purple);

        UnitPlat first = BattleSystem.instance.HostilityUnitPlatsQueue.
            GetUnitPlatByUnitSite(UnitSite.first).plat;
        UnitPlat fourth = BattleSystem.instance.HostilityUnitPlatsQueue.
            GetUnitPlatByUnitSite(UnitSite.fourth).plat;

        absorbEfffect[0].transform.position = first.transform.position;
        absorbEfffect[0].SetActive(true);

        Vector3 userPosition = user.transform.position;

        Vector3[] path = new Vector3[]
        {
            user.transform.position,
            userPosition +=
                new Vector3(-offest.x, offest.y, offest.z),
            first.transform.position,
        };

        absorbEfffect[0].transform.DOPath(path, 1f, PathType.CatmullRom).
                OnComplete(() =>
                {
                    absorbEfffect[0].SetActive(false);

                    first.UnitPlatInit(tentacleData, UnitSite.first);
                    first.unitData.Skills[0].GameStartInit();

                    first.iconSpriteRender.sprite = tentacleData.BattleSprite;
                    Vector3 originScale = first.transform.localScale;
                    first.transform.localScale = Vector3.zero;
                    first.transform.DOScale(originScale, 1.5f);
                });

        absorbEfffect[1].transform.position = first.transform.position;
        absorbEfffect[1].SetActive(true);

        Vector3[] path_second = new Vector3[]
        {
                    user.transform.position,
                    userPosition +=
                        new Vector3(offest.x, offest.y, offest.z),
                    fourth.transform.position,
        };

        absorbEfffect[1].transform.DOPath(path_second, 1f, PathType.CatmullRom).
                OnComplete(() =>
                {
                    absorbEfffect[1].SetActive(false);

                    fourth.UnitPlatInit(tentacleData, UnitSite.fourth);
                    fourth.unitData.Skills[0].GameStartInit();

                    fourth.iconSpriteRender.sprite = tentacleData.BattleSprite;
                    Vector3 scale = fourth.transform.localScale;
                    fourth.transform.localScale = Vector3.zero;
                    fourth.transform.DOScale(scale, 1.5f);
                });

        user.unit.unitSkills[0].SkillTime = 2.6f;

        return 2.6f;
    }

    private Func<int, float> skill_sp_3_func = null;
    private float Skill_sp_3(UnitPlat user)
    {
        Debug.LogError("[FinalBoss Skill] skill_sp_3");
        int lastRound = BattleSystem.instance.currentRound + 8;
        skill_sp_3_func = (round) => 
        {
            if (round < lastRound) return 0;

            user.DamageTextJump("终焉时刻", Color.black);

            TimerManager.instance.StartTimer(name + "GameEnd", 1.1f, 
                () => 
                {
                    int i = 0;
                    foreach (var unit in
                        BattleSystem.instance.FriendlyUnitPlatsQueue.GetAllUnitPlat())
                    {
                        if (!unit.isDead)
                        {
                            int index = i;

                            hitEffects[index].transform.position = unit.transform.position;
                            hitEffects[index].GetComponent<PlayableDirector>().Play();

                            unit.unit.HP = 0;
                            unit.UnitPlatHurtAnimation();
                            i++;
                        }
                    }
                });
            

            return 0.35f + 1.1f;
        };

        BattleSystem.instance.OnRoundStart += skill_sp_3_func;

        user.DamageTextJump("终焉时刻开启", GameManager.purple);

        return 1.1f;
    }

    private Func<int, float> sp_skill_4_func = null;
    private void Skill_sp_4(UnitPlat user)
    {
        Debug.LogError("[FinalBoss Skill] skill_sp_4");
        if (user.costumvalue_second >= 2)
        {
            int currentRound = BattleSystem.instance.currentRound;

            sp_skill_4_func = (round) => 
            {
                user.unit.OnDefend += (hpcahnge, user) =>
                {
                    if (hpcahnge < 0)
                    {
                        return Mathf.RoundToInt
                        (Mathf.Pow(2, BattleSystem.instance.currentRound - currentRound)) * hpcahnge;
                    }

                    return hpcahnge;
                };

                BattleSystem.instance.OnRoundStart -= sp_skill_4_func;
                return 0;
            };

            BattleSystem.instance.OnRoundStart += sp_skill_4_func;
        }
    }

    private float Skill_sp_5(UnitPlat user)
    {
        Debug.LogError("[FinalBoss Skill] skill_sp_5");
        int count = 0;
        foreach (var unit in
            BattleSystem.instance.FriendlyUnitPlatsQueue.GetAllUnitPlat())
        {
            if (!unit.isDead && unit.unitData != FactorySystem.instance.EmptyFriendlyUnitData &&
                unit.costumvlue_fourth >= 3)
            {
                int index = count;
                unit.unit.HP -= 20;
                unit.UnitPlatHurtAnimation();

                hitEffects[index].transform.position = unit.transform.position;
                hitEffects[index].SetActive(true);
                hitEffects[index].GetComponent<PlayableDirector>().Play();

                count++;
            }
        }

        user.unit.HP += 20;
        user.UnitPlatRecoveryAnimation(); ;

        TimerManager.instance.StartTimer(name + "EffectClose", 2f,
            () =>
            {
                for (int i = 0; i < hitEffects.Count; i++)
                {
                    hitEffects[i].SetActive(false);
                }
            });

        return count == 0 ? 0 : 2.1f;
    }

    private float Skill_sp_6(UnitPlat user)
    {
        Debug.LogError("[FinalBoss Skill] skill_sp_6");

        Vector3 userPosition = user.transform.position;

        int count = 0;
        foreach (var unit in BattleSystem.instance.FriendlyUnitPlatsQueue.GetAllUnitPlat())
        {
            if (!unit.isDead && unit.unitData != FactorySystem.instance.EmptyFriendlyUnitData &&
                        unit.costumvlue_fourth >= 4)
            {
                int index = count;
                unit.unit.HP -= 0;
                unit.UnitPlatHurtAnimation();

                hitEffects[index].transform.position = unit.transform.position;
                hitEffects[index].SetActive(true);
                hitEffects[index].GetComponent<PlayableDirector>().Play();

                absorbEfffect[index].transform.position = unit.transform.position;
                absorbEfffect[index].SetActive(true);

                Vector3[] path = new Vector3[]
                {
                    unit.transform.position,
                    userPosition +=
                        new Vector3(-offest.x, offest.y, offest.z),
                    user.transform.position,
                };

                absorbEfffect[index].transform.DOPath(path, 1f, PathType.CatmullRom).
                        OnComplete(() =>
                        {
                            absorbEfffect[index].SetActive(false);

                            unit.unit.HP = 0;
                        });
                count++;
            }
        }

        TimerManager.instance.StartTimer(name + "EffectClose", 1.1f,
            () => 
            {
                user.unit.HP += 20;
                user.UnitPlatRecoveryAnimation();
                user.unit.spCount -= 5;

                for (int i = 0; i < hitEffects.Count; i++)
                {
                    hitEffects[i].SetActive(false);
                }
            });

        return count == 0 ? 0 : 1.1f + 2f;
    }

    private float Skill_sp_7(UnitPlat user)
    {
        Debug.LogError("[FinalBoss Skill] skill_sp_7");

        Vector3 userPosition = user.transform.position;

        int count = 0;
        foreach (var unit in BattleSystem.instance.FriendlyUnitPlatsQueue.GetAllUnitPlat())
        {
            if (!unit.isDead && unit.unitData != FactorySystem.instance.EmptyFriendlyUnitData &&
                        unit.costumvlue_fourth >= 5)
            {
                int index = count;
                unit.unit.HP -= Damage;
                unit.UnitPlatHurtAnimation();

                hitEffects[index].transform.position = unit.transform.position;
                hitEffects[index].SetActive(true);
                hitEffects[index].GetComponent<PlayableDirector>().Play();

                absorbEfffect[index].transform.position = unit.transform.position;
                absorbEfffect[index].SetActive(true);

                Vector3[] path = new Vector3[]
                {
                    unit.transform.position,
                    userPosition +=
                        new Vector3(-offest.x, offest.y, offest.z),
                    user.transform.position,
                };

                absorbEfffect[index].transform.DOPath(path, 1f, PathType.CatmullRom).
                        OnComplete(() =>
                        {
                            absorbEfffect[index].SetActive(false);

                            unit.UnitPlatInit(FactorySystem.instance.EmptyFriendlyUnitData, unit.site);

                            unit.iconSpriteRender.sprite = 
                                FactorySystem.instance.EmptyFriendlyUnitData.BattleSprite;
                            Vector3 originScale = unit.transform.localScale;
                            unit.transform.localScale = Vector3.zero;
                            unit.transform.DOScale(originScale, 1.5f);

                            unit.unit.HP = 0;
                        });
                count++;
            }
        }

        TimerManager.instance.StartTimer(name + "EffectClose", 2.6f, 
            () => 
            {
                for (int i = 0; i < hitEffects.Count; i++)
                {
                    hitEffects[i].SetActive(false);
                }
            });

        return count == 0 ? 0f : 2.6f;
    }

    private float Skill_0(UnitPlat user)
    {
        Debug.LogError("[FinalBoss Skill] skill_0");
        int count = 0;
        foreach (var unit in
            BattleSystem.instance.FriendlyUnitPlatsQueue.GetAllUnitPlat())
        {
            if (!unit.isDead && unit != user)
            {
                int index = count;
                hitEffects[index].transform.position = unit.transform.position;
                hitEffects[index].SetActive(true);
                hitEffects[index].GetComponent<PlayableDirector>().Play();

                unit.unit.HP -= 10;
                unit.UnitPlatHurtAnimation();

                unit.costumvlue_fourth++;
                unit.unit.spCount--;

                count++;
            }
        }

        TimerManager.instance.StartTimer(name + "EffectClose", 0.6f,
           () =>
           {
               for (int i = 0; i < hitEffects.Count; i++)
               {
                   hitEffects[i].SetActive(false);
               }
           });

        return count != 0 ? 0.6f : 0f;
    }

    private float Skill_1(UnitPlat user)
    {
        Debug.LogError("[FinalBoss Skill] skill_1");

        Vector3 userPosition = user.transform.position;

        UnitPlat target = null;
        foreach (var unit in BattleSystem.instance.FriendlyUnitPlatsQueue.GetAllUnitPlat())
        {
            if(target == null && !unit.isDead && 
                unit.unitData != FactorySystem.instance.EmptyFriendlyUnitData)
            {
                target = unit;
                continue;
            }

            if (target.unit.HP > unit.unit.HP && !unit.isDead &&
                unit.unitData != FactorySystem.instance.EmptyFriendlyUnitData)
            {
                target = unit;
            }
        }

        if (target == null)
        {
            return 0f;
        }

        absorbEfffect[0].transform.position = target.transform.position;
        absorbEfffect[0].SetActive(true);

        Vector3[] path = new Vector3[]
        {
            target.transform.position,
            userPosition +=
                new Vector3(-offest.x, offest.y, offest.z),
            user.transform.position,
        };

        absorbEfffect[0].transform.DOPath(path, 1f, PathType.CatmullRom).
            OnComplete(() =>
            {
                absorbEfffect[0].SetActive(false);

                hitEffects[0].transform.position = target.transform.position;
                hitEffects[0].SetActive(true);
                hitEffects[0].GetComponent<PlayableDirector>().Play();

                target.unit.HP -= 15;
                target.UnitPlatHurtAnimation();

                target.costumvlue_fourth++;
            });

        TimerManager.instance.StartTimer(name + "EffectClose", 1.7f, 
            () => 
            {
                for (int i = 0; i < hitEffects.Count; i++)
                {
                    hitEffects[i].SetActive(false);
                }
            });

        return 1.7f;
    }

    private float Skill_2(UnitPlat user)
    {
        Debug.LogError("[FinalBoss Skill] skill_2");
        int count = 0;
        foreach (var unit in
            BattleSystem.instance.FriendlyUnitPlatsQueue.GetAllUnitPlat())
        {
            if (!unit.isDead && unit != user)
            {
                int index = count;

                flashEffects[index].transform.position = unit.transform.position;
                flashEffects[index].SetActive(true);
                flashEffects[index].GetComponent<PlayableDirector>().Play();

                hitEffects[index].transform.position = unit.transform.position;
                hitEffects[index].SetActive(true);
                hitEffects[index].GetComponent<PlayableDirector>().Play();

                unit.unit.HP -= 15;
                unit.UnitPlatHurtAnimation();

                unit.costumvlue_fourth++;
                unit.unit.spCount -= 2;

                count++;
            }
        }

        TimerManager.instance.StartTimer(name + "EffectClose", 0.6f,
            () =>
            {
                for (int i = 0; i < hitEffects.Count; i++)
                {
                    hitEffects[i].SetActive(false);
                    flashEffects[i].SetActive(false);
                }
            });

        return count == 0 ? 0 : 0.6f;
    }

    private float Skill_3(UnitPlat user)
    {
        Debug.LogError("[FinalBoss Skill] skill_3");
        UnitPlat target = null;
        foreach (var unit in BattleSystem.instance.FriendlyUnitPlatsQueue.GetAllUnitPlat())
        {
            if (!unit.isDead && unit.unitData != FactorySystem.instance.EmptyFriendlyUnitData)
            {
                target = unit; 
                break;
            }
        }

        if(target == null)
        {
            return 0;
        }

        target.DamageTextJump("精神控制", GameManager.purple);

        int currentRound = BattleSystem.instance.currentRound;

        foreach (var skill in target.unit.unitSkills)
        {
            skill.SkillTarget = skill.SkillTarget == Faction.Friendly ? 
                Faction.Hostility : Faction.Friendly;
        }

        Func<int, float> func = null;

        func = (round) => 
        {
            if (round < currentRound + 1)
            {
                return 0f;
            }

            target.DamageTextJump("精神控制解除", Color.white);

            foreach (var skill in target.unit.unitSkills)
            {
                skill.SkillTarget = skill.SkillTarget == Faction.Friendly ?
                    Faction.Hostility : Faction.Friendly;
            }

            BattleSystem.instance.OnRoundEnd -= func;

            return 1.1f;
        };

        BattleSystem.instance.OnRoundEnd += func;

        return 1.1f;
    }

    private float Skill_4(UnitPlat user)
    {
        Debug.LogError("[FinalBoss Skill] skill_4");
        int count = 0;
        foreach (var unit in
            BattleSystem.instance.FriendlyUnitPlatsQueue.GetAllUnitPlat())
        {
            if (!unit.isDead && unit != user)
            {
                int index = count;
                hitEffects[index].transform.position = unit.transform.position;
                hitEffects[index].SetActive(true);
                hitEffects[index].GetComponent<PlayableDirector>().Play();

                unit.unit.HP -= 20;
                unit.UnitPlatHurtAnimation();

                unit.costumvlue_fourth++;
                unit.unit.spCount -= 3;

                count++;
            }
        }

        TimerManager.instance.StartTimer(name + "EffectClose", 0.6f,
           () =>
           {
               for (int i = 0; i < hitEffects.Count; i++)
               {
                   hitEffects[i].SetActive(false);
               }
           });

        return count != 0 ? 0.6f : 0f;
    }
}
