using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Playables;

[CreateAssetMenu(fileName = "newUnitData", menuName = "Data/Skill Data/" + nameof(Thief_standrd))]
public class Thief_standrd : UnitSkillDataSo
{
    [SerializeField] private GameObject daggerPrefab;
    [SerializeField] private GameObject hitPrefab;

    [SerializeField] private int skillListIndex;
    [SerializeField] private float daggerMoveSpeed;

    private GameObject daggerEffect;
    private GameObject hitEffect;

    public override void GameStartInit()
    {
        base.GameStartInit();

        daggerEffect = Instantiate(daggerPrefab, Vector3.zero, Quaternion.identity);
        daggerEffect.SetActive(false);

        hitEffect = Instantiate(hitPrefab, Vector3.zero, Quaternion.identity);
        hitEffect.SetActive(false);
    }

    public override void Action(ICollection<UnitPlat> unitPlats, UnitPlat user)
    {
        if (unitPlats.Count <= 0)
        {
            user.unit.unitSkills[skillListIndex].SkillTime = 0.5f;
            return;
        }

        UnitPlat target = null;
        foreach (var unit in unitPlats)
        {
            if (!unit.isDead && unit.unitData != FactorySystem.instance.EmptyFriendlyUnitData)
            {
                target = unit; 
                break;
            }
        }

        if (target == null)
        {
            user.unit.unitSkills[skillListIndex].SkillTime = 0.5f;
            return;
        }

        GameManager.instance.GlobalLightControll(0.5f, 0.5f);
        user.transform.DOScale(UnitPlat.originScale * attackScale, 0.5f);

        PlayableDirector director = hitEffect.GetComponent<PlayableDirector>();
        float moveTime = Vector3.Distance(user.transform.position, target.transform.position) / daggerMoveSpeed;
        TimerManager.instance.StartTimer(name + "DaggerMove", 0.6f, 
            () => 
            {
                daggerEffect.transform.position = user.transform.position + 
                    UnitPlat.topDistance + new Vector3(0, 1.5f);
                daggerEffect.SetActive(false);

                daggerEffect.transform.DOMove(target.transform.position, moveTime).OnComplete(
                    () =>
                    {
                        hitEffect.transform.position = target.transform.position;
                        hitEffect.SetActive(true);
                        director.Play();

                        target.UnitPlatHurtAnimation();
                        target.unit.HP -= Damage;
                    });
            });

        TimerManager.instance.StartTimer(name + "EffectClose", 0.6f + moveTime + (float)director.duration + 0.1f,
            () => 
            {
                hitEffect.SetActive(false);

                GameManager.instance.GlobalLightControll(1f, 0.5f);
                user.transform.DOScale(UnitPlat.originScale, 0.5f);
            });

        user.unit.unitSkills[skillListIndex].SkillTime = 0.6f + moveTime + (float)director.duration + 0.1f + 0.6f;
    }
}
