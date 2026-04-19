using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Playables;

[CreateAssetMenu(fileName = "newUnitData", menuName = "Data/Skill Data/" + nameof(Shiled_recovery))]
public class Shiled_recovery : UnitSkillDataSo
{
    [SerializeField] private GameObject recoveryPrefab;

    [SerializeField] private int skillListIndex;

    private GameObject recoveryEffect;

    public override void GameStartInit()
    {
        base.GameStartInit();

        recoveryEffect = Instantiate(recoveryPrefab, Vector3.zero, Quaternion.identity);
        recoveryEffect.SetActive(false);
    }

    public override void Action(ICollection<UnitPlat> unitPlats, UnitPlat user)
    {
        if (unitPlats.Count <= 0)
        {
            user.unit.unitSkills[skillListIndex].SkillTime = 0.5f;
            return;
        }

        UnitPlat unitPlat = null;
        bool isfriCard = false;
        foreach (var unit in unitPlats)
        {
            if (FactorySystem.instance.shiledCards[0] == unit.unitData)
            {
                unitPlat = unit;
                isfriCard = true;
                break;
            }
        }

        if (unitPlat == null)
        {
            foreach (var unit in unitPlats)
            {
                if (FactorySystem.instance.shiledCards.Contains(unit.unitData))
                {
                    unitPlat = unit;
                    isfriCard = true;
                    break;
                }
            }
        }

        if (unitPlat == null)
        {
            foreach (var unit in unitPlats)
            {
                if (unit.unitData != FactorySystem.instance.EmptyFriendlyUnitData)
                {
                    unitPlat = unit;
                    break;
                }
            }
        }

        if (unitPlat == null) 
        {
            user.unit.unitSkills[skillListIndex].SkillTime = 0.5f;
            return;
        }

        GameManager.instance.GlobalLightControll(0.5f, 0.5f);
        user.transform.DOScale(UnitPlat.originScale * attackScale, 0.5f);

        PlayableDirector director = recoveryEffect.GetComponent<PlayableDirector>();
        TimerManager.instance.StartTimer(name + "RecoverEffet", 0.6f, 
            () => 
            {
                recoveryEffect.transform.position = unitPlat.transform.position + UnitPlat.bottomDistance;
                recoveryEffect.SetActive(true);
                unitPlat.unit.HP += isfriCard ? 5 * Damage : Damage;
                unitPlat.UnitPlatRecoveryAnimation();

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
