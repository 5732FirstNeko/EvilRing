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

        laserHitEffects = new List<GameObject>();
        for (int i = 0; i < 4; i++)
        {
            GameObject effect = Instantiate(laserHitPrefab, Vector3.zero, Quaternion.identity);
            effect.SetActive(false);
            laserHitEffects.Add(effect);
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

        PlayableDirector director = laserEffect.GetComponent<PlayableDirector>();

        TimerManager.instance.StartTimer(name + "DamageAnimation", 0.6f,
            () =>
            {
                float initpos = 0f;
                foreach (var unit in unitPlats)
                {
                    initpos += unit.transform.position.x;
                }
                initpos /= unitPlats.Count;
                laserEffect.transform.position = new Vector3(initpos, user.transform.position.y, 0f);
                laserEffect.SetActive(true);
                director.Play();
            });

        TimerManager.instance.StartTimer(name + "LaserHitAnimation", (float)director.duration * 0.25f + 0.6f, 
            () => 
            {
                int i = 0;
                foreach (var unit in unitPlats)
                {
                    unit.unit.HP -= Damage;

                    int index = i;
                    unit.UnitPlatHurtAnimation(6, 0.1f,
                        () =>
                        {
                            laserHitEffects[index].transform.position = unit.transform.position;
                            if (!laserHitEffects[index].activeSelf)
                            {
                                laserHitEffects[index].SetActive(true);
                                laserHitEffects[index].GetComponent<PlayableDirector>().Play();
                            }
                            else
                            {
                                laserHitEffects[index].SetActive(false);
                            }
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
