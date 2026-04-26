using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Playables;

[CreateAssetMenu(fileName = "newUnitData", menuName = "Data/Skill Data/" + nameof(Thief_boss))]
public class Thief_boss : UnitSkillDataSo
{
    [SerializeField] private GameObject daggerPrefab;
    [SerializeField] private GameObject hitPrefab;

    [SerializeField] private int skilllistIndex;

    private GameObject daggerEffect;
    private List<GameObject> hitEffects;

    public override void GameStartInit()
    {
        base.GameStartInit();

        daggerEffect = Instantiate(daggerPrefab, Vector3.zero, Quaternion.identity);
        daggerEffect.SetActive(false);

        hitEffects = new List<GameObject>();
        for (int i = 0; i < 4; i++)
        {
            GameObject effect = Instantiate(hitPrefab, Vector3.zero, Quaternion.identity);
            effect.SetActive(false);
            hitEffects.Add(effect);
        }
    }
    public override void Action(ICollection<UnitPlat> unitPlats, UnitPlat user)
    {
        if (unitPlats.Count <= 0)
        {
            user.unit.unitSkills[skilllistIndex].SkillTime = 0.5f;
            return;
        }

        GameManager.instance.GlobalLightControll(0.5f, 0.5f);
        user.transform.DOScale(UnitPlat.originScale * attackScale, 0.5f);

        int index = Random.Range(0, 101);
        if (index <= 25)
        {
            Hypnosis(unitPlats, user);
        }
        else
        {
            DaggerAttack(unitPlats, user);
        }
    }

    private void DaggerAttack(ICollection<UnitPlat> unitPlats, UnitPlat user)
    {
        Vector3 effectSpwanPos = Vector3.zero;

        int count = 0;
        foreach (var unit in unitPlats)
        {
            if (!unit.isDead && unit.unitData != FactorySystem.instance.EmptyFriendlyUnitData)
            {
                effectSpwanPos += unit.transform.position + UnitPlat.bottomDistance;
                count++;
            }
        }

        effectSpwanPos /= count;

        PlayableDirector director = daggerEffect.GetComponent<PlayableDirector>();
        TimerManager.instance.StartTimer(name + "DaggerEffect", 0.6f, 
            () => 
            {
                daggerEffect.transform.position = effectSpwanPos;
                daggerEffect.SetActive(true);
                director.Play();

                int i = 0;
                foreach (var unit in unitPlats)
                {
                    if (!unit.isDead && unit.unitData != FactorySystem.instance.EmptyFriendlyUnitData)
                    {
                        int index = i;
                        unit.UnitPlatHurtAnimation(Mathf.FloorToInt((float)director.duration / 0.25f), 0, 
                            () => 
                            {
                                unit.unit.HP -= Damage / count;
                                if (hitEffects[index].activeSelf)
                                {
                                    hitEffects[index].SetActive(false);
                                }
                                else
                                {
                                    hitEffects[index].transform.position = unit.transform.position;
                                    hitEffects[index].SetActive(true);
                                    hitEffects[index].GetComponent<PlayableDirector>().Play();
                                }
                            });
                        i++;
                    }
                }
            });

        TimerManager.instance.StartTimer(name + "DaggerEffectClose", 0.6f + (float)director.duration + 0.1f, 
            () => 
            {
                daggerEffect.SetActive(false);

                for (int i = 0; i < hitEffects.Count; i++)
                {
                    hitEffects[i].SetActive(false);
                }

                GameManager.instance.GlobalLightControll(1f, 0.5f);
                user.transform.DOScale(UnitPlat.originScale, 0.5f);
            });

        user.unit.unitSkills[skilllistIndex].SkillTime = 0.6f + (float)director.duration + 0.1f + 0.6f;
    }

    private void Hypnosis(ICollection<UnitPlat> unitPlats, UnitPlat user)
    {
        TimerManager.instance.StartTimer(name + "hypnosis", 0.6f, 
            () => 
            {
                user.DamageTextJump("´ßÃß", new Color(0.5f, 0f, 0.5f));
                BattleSystem.instance.OnRoundStart += OnRoundStart;
            });

        TimerManager.instance.StartTimer(name + "EffectClose", 0.6f + 1.1f, 
            () => 
            {
                GameManager.instance.GlobalLightControll(1, 0.5f);
                user.transform.DOScale(UnitPlat.originScale, 0.5f);
            });

        user.unit.unitSkills[skilllistIndex].SkillTime = 0.6f + 1.1f + 0.5f + 0.1f;
    }

    private float OnRoundStart(int round)
    {
        List<UnitPlat> unitplats = BattleSystem.instance.HostilityUnitPlatsQueue.GetAllUnitPlat();

        for (int i = 0; i < unitplats.Count; i++)
        {
            if (!unitplats[i].isDead && unitplats[i].unitData != FactorySystem.instance.EmptyFriendlyUnitData)
            {
                BattleSystem.instance.UnitRemoveQueue(unitplats[i]);
                unitplats[i].DamageTextJump("¹¥»÷Ä¿±ê¶ªÊ§", Color.red);
            }
        }

        BattleSystem.instance.OnRoundStart -= OnRoundStart;
        return 1.1f;
    }
}
