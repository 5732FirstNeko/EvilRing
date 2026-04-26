using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Playables;

[CreateAssetMenu(fileName = "newUnitData", menuName = "Data/Skill Data/" + nameof(Swordman_swordArea))]
public class Swordman_swordArea : UnitSkillDataSo
{
    [SerializeField] private GameObject swordAreaPrefab;
    [SerializeField] private GameObject swordHitPrefab;
    [SerializeField] private GameObject magicStaffPrefab;

    [SerializeField] private int skillListIndex;

    private GameObject swordAreaEffect;
    private GameObject magicStaffEffect;
    private List<GameObject> swordHitEffects;

    public override void GameStartInit()
    {
        base.GameStartInit();

        swordAreaEffect = Instantiate(swordAreaPrefab, Vector3.zero, Quaternion.identity);
        swordAreaEffect.SetActive(false);

        magicStaffEffect = Instantiate(magicStaffPrefab, Vector3.zero, Quaternion.identity);
        magicStaffEffect.SetActive(false);

        swordHitEffects = new List<GameObject>();
        for (int i = 0; i < 4; i++)
        {
            GameObject effect = Instantiate(swordHitPrefab, Vector3.zero, Quaternion.identity);
            effect.SetActive(false);
            swordHitEffects.Add(effect);
        }
    }

    public override void Action(ICollection<UnitPlat> unitPlats, UnitPlat user)
    {
        if (unitPlats.Count <= 0)
        {
            user.unit.unitSkills[skillListIndex].SkillTime = 0.5f;
            return;
        }

        UnitPlat friTarget = null;
        foreach (var unit in BattleSystem.instance.FriendlyUnitPlatsQueue.GetAllUnitPlat())
        {
            if (friTarget == null)
            {
                friTarget = unit;
                continue;
            }

            if (friTarget.unit.MaxHP < unit.unit.MaxHP)
            {
                friTarget = unit;
            }
        }

        GameManager.instance.GlobalLightControll(0.5f, 0.5f);
        user.transform.DOScale(UnitPlat.originScale * attackScale, 0.5f);

        TimerManager.instance.StartTimer(name + "friTargetHurt", 0.5f,
            ()=>
            {
                friTarget.unit.HP -= 15;
                friTarget.UnitPlatHurtAnimation();

                magicStaffEffect.transform.position = friTarget.transform.position;
                magicStaffEffect.SetActive(true);
                magicStaffEffect.transform.DOMove(user.transform.position, 0.4f);
            });

        PlayableDirector director = swordAreaEffect.GetComponent<PlayableDirector>();
        TimerManager.instance.StartTimer(name + "AreaAnimation", 0.6f + 0.5f, 
            () => 
            {
                magicStaffEffect.SetActive(false);

                float spwanPosX = 0;
                foreach (var unit in unitPlats)
                {
                    spwanPosX += unit.transform.position.x;
                }
                swordAreaEffect.transform.position =
                    new Vector3(spwanPosX / unitPlats.Count, user.transform.position.y, 0)
                    + UnitPlat.bottomDistance;
                swordAreaEffect.SetActive(true);
                
                director.Play();
            });

        TimerManager.instance.StartTimer(name + "AreaHitAnimation",0.6f + (float)director.duration * 0.2f + 0.5f, 
            () => 
            {
                int i = 0;
                foreach (var unit in unitPlats)
                {
                    int index = i;
                    unit.UnitPlatHurtAnimation(7, 0,
                        () =>
                        {
                            if (!swordHitEffects[index].activeSelf)
                            {
                                swordHitEffects[index].transform.position = unit.transform.position;
                                swordHitEffects[index].SetActive(true);
                                swordHitEffects[index].GetComponent<PlayableDirector>().Play();
                            }
                            else
                            {
                                swordHitEffects[index].SetActive(false);
                            }
                            unit.unit.HP -= Damage * 6;
                        });
                    i++;
                }
            });


        TimerManager.instance.StartTimer(name + "EffectRecover", 0.6f + (float)director.duration + 0.6f, 
            () => 
            {
                swordAreaEffect.SetActive(false);
                foreach (var effect in swordHitEffects)
                {
                    effect.SetActive(false);
                }

                GameManager.instance.GlobalLightControll(1f, 0.5f);
                user.transform.DOScale(UnitPlat.originScale, 0.5f);
            });

        user.unit.unitSkills[skillListIndex].SkillTime = 0.6f + (float)director.duration + 0.7f;
    }
}
