using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Playables;

[CreateAssetMenu(fileName = "newUnitData", menuName = "Data/Skill Data/" + nameof(Wizard_damage))]
public class Wizard_damage : UnitSkillDataSo
{
    [SerializeField] private GameObject laserPrefab;
    [SerializeField] private GameObject laserHitPrefab;

    [SerializeField] private int skillListIndex;

    private GameObject laserEffect;
    private List<GameObject> laserHitEffects;

    public override void GameStartInit()
    {
        base.GameStartInit();

        laserEffect = Instantiate(laserPrefab, Vector3.zero, Quaternion.identity);
        laserEffect.SetActive(false);
        BattleSystem.instance.destoryEffect.Add(laserEffect);

        laserHitEffects = new List<GameObject>();
        for (int i = 0; i < 4; i++)
        {
            GameObject effect = Instantiate(laserHitPrefab, Vector3.zero, Quaternion.identity);
            effect.SetActive(false);
            laserHitEffects.Add(effect);
            BattleSystem.instance.destoryEffect.Add(effect);
        }
    }

    public override void GameEndAction()
    {
        laserEffect = null;

        laserHitEffects = null;
    }

    public override void Action(ICollection<UnitPlat> unitPlats, UnitPlat user)
    {
        unitPlats = BattleSystem.instance.HostilityUnitPlatsQueue.GetAllUnitPlat();
        if (unitPlats.Count <= 0)
        {
            user.unit.unitSkills[skillListIndex].SkillTime = 0.5f;
            return;
        }

        GameManager.instance.GlobalLightControll(0.5f, 0.5f);
        user.transform.DOScale(UnitPlat.originScale * attackScale, 0.5f);

        PlayableDirector director = laserEffect.GetComponent<PlayableDirector>();

        TimerManager.instance.StartTimer(name + "DamageAnimation", 0.6f,
            () =>
            {
                laserEffect.transform.position = new Vector3(-5.25f, -0.5f, 0f);
                laserEffect.SetActive(true);
                director.Play();
            });

        TimerManager.instance.StartTimer(name + "LaserHitAnimation", (float)director.duration * 0.25f + 0.6f, 
            () => 
            {
                int i = 0;
                foreach (var unit in unitPlats)
                {
                    if (unit.isDead || 
                        unit.unitData == FactorySystem.instance.EmptyFriendlyUnitData)
                    {
                        continue;
                    }

                    unit.unit.HP -= 6;

                    int index = i;
                    unit.UnitPlatHurtAnimation(1, 0.1f,
                        () =>
                        {
                            laserHitEffects[index].transform.position = unit.transform.position;
                            laserHitEffects[index].SetActive(true);
                            laserHitEffects[index].GetComponent<PlayableDirector>().Play();
                        });
                    i++;
                }
            });

        TimerManager.instance.StartTimer(name + "DamageCloseAnimation",(float)director.duration + 0.6f,
            () => 
            {
                laserEffect.SetActive(false);
                foreach (var effect in laserHitEffects)
                {
                    effect.SetActive(false);
                }

                GameManager.instance.GlobalLightControll(0.5f, 0.5f);
                user.transform.DOScale(UnitPlat.originScale, 0.5f);
            });

        user.unit.unitSkills[skillListIndex].SkillTime = (float)director.duration + 1.1f;
    }
}
