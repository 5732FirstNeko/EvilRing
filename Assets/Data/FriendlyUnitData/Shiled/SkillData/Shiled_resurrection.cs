using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Playables;

[CreateAssetMenu(fileName = "newUnitData", menuName = "Data/Skill Data/" + nameof(Shiled_resurrection))]
public class Shiled_resurrection : UnitSkillDataSo
{
    [SerializeField] private GameObject recoveryPrefab;
    [SerializeField] private GameObject resurrectionPrefab;

    [SerializeField] private int skillListIndex;

    private GameObject recoveryEffect;
    private GameObject resurrectionEffect;

    public override void GameStartInit()
    {
        base.GameStartInit();

        recoveryEffect = Instantiate(recoveryPrefab, Vector3.zero, Quaternion.identity);
        recoveryEffect.SetActive(false);

        resurrectionEffect = Instantiate(resurrectionPrefab, Vector3.zero, Quaternion.identity);
        resurrectionEffect.SetActive(false);
    }

    public override void Action(ICollection<UnitPlat> unitPlats, UnitPlat user)
    {
        if (unitPlats.Count <= 0)
        {
            user.unit.unitSkills[skillListIndex].SkillTime = 0.5f;
            return;
        }

        UnitPlat target = null;
        bool isDeadCard = false;
        foreach (var unit in unitPlats)
        {
            if (unit.isDead && FactorySystem.instance.shiledCards.Contains(unit.unitData))
            {
                target = unit;
                isDeadCard = true;
            }
        }

        if (!isDeadCard && target == null)
        {
            foreach (var unit in unitPlats)
            {
                if (FactorySystem.instance.shiledCards[0] == unit.unitData)
                {
                    target = unit;
                }
            }
        }

        if (target == null)
        {
            foreach (var unit in unitPlats)
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

        if (isDeadCard)
        {
            PlayableDirector director = resurrectionEffect.GetComponent<PlayableDirector>();

            TimerManager.instance.StartTimer(name + "resurrectionEffect", 0.6f, 
                () => 
                {
                    resurrectionEffect.transform.position = target.transform.position + UnitPlat.bottomDistance;
                    resurrectionEffect.SetActive(true);

                    director.Play();
                    BattleSystem.instance.UnitResurrection(target);
                });

            TimerManager.instance.StartTimer(name + "resurectionEffectClose", 0.7f + (float)director.duration, 
                () => 
                {
                    resurrectionEffect.SetActive(false);
                    GameManager.instance.GlobalLightControll(1f, 0.5f);
                    user.transform.DOScale(UnitPlat.originScale, 0.5f);
                });

            user.unit.unitSkills[skillListIndex].SkillTime = 0.7f + (float)director.duration + 0.6f + 1f;
        }
        else
        {
            PlayableDirector director = recoveryEffect.GetComponent<PlayableDirector>();
            TimerManager.instance.StartTimer(name + "RecoverEffet", 0.6f,
                () =>
                {
                    recoveryEffect.transform.position = target.transform.position + UnitPlat.bottomDistance;
                    recoveryEffect.SetActive(true);
                    target.unit.HP += Damage;
                    target.UnitPlatRecoveryAnimation();

                    director.Play();
                });

            TimerManager.instance.StartTimer(name + "RecoveryEffectClose", 0.7f + (float)director.duration,
                () =>
                {
                    recoveryEffect.SetActive(false);
                    GameManager.instance.GlobalLightControll(1f, 0.5f);
                    user.transform.DOScale(UnitPlat.originScale, 0.5f);
                });

            user.unit.unitSkills[skillListIndex].SkillTime = 0.7f + (float)director.duration + 0.6f;
        }
    }
}
