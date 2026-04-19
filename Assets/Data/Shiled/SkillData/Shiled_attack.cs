using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Playables;

[CreateAssetMenu(fileName = "newUnitData", menuName = "Data/Skill Data/" + nameof(Shiled_attack))]
public class Shiled_attack : UnitSkillDataSo
{
    [SerializeField] private GameObject attackprefab;
    [SerializeField] private GameObject hitprefab;

    [SerializeField] private int skillListIndex;

    private GameObject attackEffect;
    private List<GameObject> hiteffects;

    public override void GameStartInit()
    {
        base.GameStartInit();

        hiteffects = new List<GameObject>();
        for (int i = 0; i < 5; i++)
        {
            GameObject effect = Instantiate(hitprefab, Vector3.zero, Quaternion.identity);
            effect.SetActive(false);
            hiteffects.Add(effect);
        }

        attackEffect = Instantiate(attackprefab, Vector3.zero, Quaternion.identity);
        attackEffect.SetActive(false);
    }

    public override void Action(ICollection<UnitPlat> unitPlats, UnitPlat user)
    {
        if (unitPlats.Count <= 0)
        {
            user.unit.unitSkills[skillListIndex].SkillTime = 0.5f;
            return;
        }

        UnitPlat target = null;
        if (target == null)
        {
            foreach (var unit in 
                BattleSystem.instance.FriendlyUnitPlatsQueue.GetAllUnitPlat())
            {
                if (FactorySystem.instance.shiledCards[0] == unit.unitData)
                {
                    target = unit;
                }
            }
        }

        if (target == null)
        {
            foreach (var unit in 
                BattleSystem.instance.FriendlyUnitPlatsQueue.GetAllUnitPlat())
            {
                if (unit.unitData != null &&
                    unit.unitData != FactorySystem.instance.EmptyFriendlyUnitData)
                {
                    target = unit;
                }
            }
        }

        if (target == null)
        {
            user.unit.unitSkills[skillListIndex].SkillTime = 0.5f;
            return;
        }

        GameManager.instance.GlobalLightControll(0.5f, 0.5f);
        user.transform.DOScale(UnitPlat.originScale * attackScale, 0.5f);

        PlayableDirector director = hiteffects[0].GetComponent<PlayableDirector>();
        TimerManager.instance.StartTimer(name + "frihurtEffect", 0.6f, 
            () => 
            {
                hiteffects[0].transform.position = target.transform.position;
                hiteffects[0].SetActive(true);

                target.unit.HP -= Damage;
                target.UnitPlatHurtAnimation();
                director.Play();
            });

        UnitPlat enemy = null;
        foreach (var unit in unitPlats)
        {
            if (unit.unitData != null &&
                unit.unitData != FactorySystem.instance.EmptyHostitlyUnitData)
            {
                enemy = unit;
                break;
            }
        }

        TimerManager.instance.StartTimer(name + "hoshurtEffect", 0.7f + (float)director.duration, 
            () => 
            {
                attackEffect.transform.position = user.transform.position;
                attackEffect.SetActive(true);

                attackEffect.transform.DOMove(enemy.transform.position, 1.5f).SetEase(Ease.InQuart).
                OnComplete(
                    () => 
                    {
                        attackEffect.SetActive(false);

                        GameObject effect = null;
                        for (int i = 0; i < hiteffects.Count; i++)
                        {
                            if (!hiteffects[i].activeSelf)
                            {
                                effect = hiteffects[i];
                                break;
                            }
                        }

                        enemy.unit.HP -= Damage;
                        enemy.UnitPlatHurtAnimation();

                        effect.transform.position = enemy.transform.position;
                        effect.SetActive(true);
                        effect.GetComponent<PlayableDirector>().Play();
                    });
            });

        TimerManager.instance.StartTimer(name + "hitEffectClose",0.7f + 2 * (float)director.duration + 1.6f, 
            () =>
            {
                for (int i = 0; i < hiteffects.Count; i++)
                {
                    hiteffects[i].SetActive(false);
                }

                GameManager.instance.GlobalLightControll(1f, 0.5f);
                user.transform.DOScale(UnitPlat.originScale, 0.5f);
            });

        user.unit.unitSkills[skillListIndex].SkillTime = 0.7f + 2 * (float)director.duration + 1.6f + 0.6f;

    }
}
