using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Playables;

[CreateAssetMenu(fileName = "newUnitData", menuName = "Data/Skill Data/" + nameof(Goblin_anger))]
public class Goblin_anger : UnitSkillDataSo
{
    [SerializeField] private GameObject attackPrefab;
    [SerializeField] private GameObject hitPrefab;

    [SerializeField] private int skillListIndex;
    [SerializeField] private Vector3 attackSpwanPosition;

    private List<GameObject> attackEffects;
    private List<GameObject> hitEffects;

    public override void GameStartInit()
    {
        base.GameStartInit();

        attackEffects = new List<GameObject>();
        for (int i = 0; i < 4; i++)
        {
            GameObject effect = Instantiate(attackPrefab, Vector3.zero, Quaternion.identity);
            effect.SetActive(false);
            attackEffects.Add(effect);
        }
        
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
        int count = 1;
        foreach (var unit in BattleSystem.instance.HostilityUnitPlatsQueue.GetAllUnitPlat())
        {
            if (unit.isDead && FactorySystem.instance.GoblineCards.Contains(unit.unitData))
            {
                count++;
            }
        }

        GameManager.instance.GlobalLightControll(0.5f, 0.5f);
        user.transform.DOScale(UnitPlat.originScale * attackScale, 0.5f);

        TimerManager.instance.StartTimer(name + "AttackEffect", 0.6f, 
            () => 
            {
                for (int i = 0; i < count;)
                {
                    foreach (var unit in unitPlats)
                    {
                        int index = i;
                        attackEffects[index].transform.position = attackSpwanPosition;
                        attackEffects[index].SetActive(true);
                        attackEffects[index].transform.DOMove(unit.transform.position, 2f).SetEase(Ease.InQuart).
                            OnComplete(() =>
                            {
                                hitEffects[index].transform.position = unit.transform.position;
                                hitEffects[index].SetActive(false);
                                hitEffects[index].GetComponent<PlayableDirector>().Play();

                                unit.UnitPlatHurtAnimation();
                                unit.unit.HP -= Damage;
                            });

                        i++;
                        if (i >= count)
                        {
                            break;
                        }
                    }
                }
            });

        TimerManager.instance.StartTimer(name + "EffectClose",0.6f + 0.5f + 2f + 0.1f, 
            () => 
            {
                for (int i = 0; i < 4; i++)
                {
                    attackEffects[i].SetActive(false);
                    hitEffects[i].SetActive(false);
                }

                GameManager.instance.GlobalLightControll(1f, 0.5f);
                user.transform.DOScale(UnitPlat.originScale, 0.5f);
            });


        user.unit.unitSkills[skillListIndex].SkillTime = 0.6f + 0.5f + 2f + 0.1f + 0.5f + 0.1f;
    }
}
