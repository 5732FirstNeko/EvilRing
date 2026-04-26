using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Playables;

[CreateAssetMenu(fileName = "newUnitData", menuName = "Data/Skill Data/" + nameof(Swordman_core_OnStrikeBack))]
public class Swordman_core_OnStrikeBack : UnitSkillDataSo
{
    [SerializeField] private GameObject flySwordPrefab;
    [SerializeField] private GameObject swordhitPrefab;

    [SerializeField] private int skillListIndex;
    [SerializeField] private float flySwordSpeed;

    private List<GameObject> flySwordEffects;
    private List<GameObject> swordhitEffects;

    public override void GameStartInit()
    {
        base.GameStartInit();

        flySwordEffects = new List<GameObject>();
        for (int i = 0; i < 8; i++)
        {
            GameObject effect = Instantiate(flySwordPrefab, Vector3.zero, Quaternion.identity);
            effect.transform.localScale = Vector3.zero;
            effect.SetActive(false);
            flySwordEffects.Add(effect);
        }

        swordhitEffects = new List<GameObject>();
        for (int i = 0; i < 4; i++)
        {
            GameObject effect = Instantiate(swordhitPrefab, Vector3.zero, Quaternion.identity);
            effect.SetActive(false);
            swordhitEffects.Add(effect);
        }
    }

    public override void Action(ICollection<UnitPlat> unitPlats, UnitPlat user)
    {
        if (user.costumvalue_first <= 0 || unitPlats.Count <= 0)
        {
            user.unit.unitSkills[skillListIndex].SkillTime = 0.5f;
            return;
        }

        GameManager.instance.GlobalLightControll(0.5f, 0.5f);
        user.transform.DOScale(UnitPlat.originScale * attackScale, 0.5f);

        if (user.costumvalue_first > 8)
        {
            for (int i = 0; i < user.costumvalue_first - 8; i++)
            {
                GameObject effect = Instantiate(flySwordPrefab, Vector3.zero, Quaternion.identity);
                effect.transform.localScale = Vector3.zero;
                effect.SetActive(false);
                flySwordEffects.Add(effect);
            }
        }

        for (int i = 0; i < user.costumvalue_first; i++)
        {
            flySwordEffects[i].SetActive(true);

            Vector3 spawnPos = user.transform.position + new Vector3(Random.Range(0f, 3f), 0, 0);
            Vector3 spwanheight = spawnPos + new Vector3(0, Random.Range(-2f, 2f), 0);

            flySwordEffects[i].transform.DOScale(Vector3.one, 0.75f);
            flySwordEffects[i].transform.DOMove(spawnPos, 1f);
            flySwordEffects[i].transform.DOMove(spwanheight, 2f).SetDelay(1f);
        }

        int count = user.costumvalue_first;
        TimerManager.instance.StartTimer(name + "strikeBackAttack", 2.6f,
            () =>
            {
                for (int i = 0; i < count;)
                {
                    foreach (var unit in unitPlats)
                    {
                        int index = i;
                        Vector3 dir = unit.transform.position -
                        flySwordEffects[i].transform.position;
                        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg + 180;

                        flySwordEffects[index].transform.DOKill();

                        flySwordEffects[index].transform.DORotate(new Vector3(0, 0, angle), 0.2f);
                        flySwordEffects[index].transform.DOMove(unit.transform.position +
                            (dir.normalized * 3f), 0.5f).SetEase(Ease.OutQuart).SetDelay(0.2f).
                            OnComplete(
                            () =>
                            {
                                flySwordEffects[index].transform.DOScale(Vector3.zero, 0.1f).
                                OnComplete(() => 
                                {
                                    flySwordEffects[index].transform.position = Vector3.zero;
                                    flySwordEffects[index].transform.localRotation = Quaternion.identity;
                                    flySwordEffects[index].SetActive(false);
                                });
                            });

                        i++;
                        if (i >= user.costumvalue_first)
                        {
                            break;
                        }
                    }
                }
            });


        Dictionary<UnitPlat, int> damageCountMap = new Dictionary<UnitPlat, int>();
        for (int i = 0; i < user.costumvalue_first;)
        {
            foreach (var unit in unitPlats)
            {
                if (damageCountMap.ContainsKey(unit))
                {
                    damageCountMap[unit]++;
                }
                else
                {
                    damageCountMap.Add(unit, 1);
                }
                i++;

                if (i >= user.costumvalue_first)
                {
                    break;
                }
            }
        }

        TimerManager.instance.StartTimer(name + "swordhitanimation", 2.6f + 0.2f + 0.5f * 0.25f,
            () =>
            {
                int index = 0;
                foreach (var unit in unitPlats)
                {
                    unit.unit.HP -= Damage * damageCountMap[unit];
                    unit.UnitPlatHurtAnimation();

                    swordhitEffects[index].transform.position = unit.transform.position;
                    swordhitEffects[index].SetActive(true);
                    swordhitEffects[index].GetComponent<PlayableDirector>().Play();
                    index++;
                }
            });

        TimerManager.instance.StartTimer(name + "swordEffectRecover", 2.8f + 0.5f * 0.25f + 0.5f,
            () =>
            {
                foreach (var hitEffect in swordhitEffects)
                {
                    hitEffect.SetActive(false);
                }
                user.transform.DOScale(UnitPlat.originScale, 0.5f);
                GameManager.instance.GlobalLightControll(1f, 0.5f);
            });

        user.costumvalue_first = 0;
        user.unit.unitSkills[skillListIndex].SkillTime = 2.7f + 0.5f * 0.25f + 1f;
    }
}
