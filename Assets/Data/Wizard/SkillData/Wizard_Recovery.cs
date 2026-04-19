using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Playables;

[CreateAssetMenu(fileName = "newUnitData", menuName = "Data/Skill Data/" + nameof(Wizard_Recovery))]
public class Wizard_Recovery : UnitSkillDataSo
{
    [SerializeField] private GameObject recoveryPrefab;

    [SerializeField] private Vector2 recoverAreaPosition;
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
            user.unit.unitSkills[skillListIndex].SkillTime = 0;
            return;
        }

        GameManager.instance.GlobalLightControll(0.5f, 0.5f);

        user.transform.DOScale(UnitPlat.originScale * attackScale, 0.5f);

        recoveryEffect.transform.position = recoverAreaPosition;
        recoveryEffect.SetActive(true);

        PlayableDirector director = recoveryEffect.GetComponent<PlayableDirector>();
        director.Play();

        TimerManager.instance.StartTimer(name + "RecoveryUnitAnimation",(float)director.duration * 0.5f,
            () => 
            {
                foreach (var unit in unitPlats)
                {
                    unit.unit.HP += Damage;
                    unit.UnitPlatRecoveryAnimation();
                }
            });

        TimerManager.instance.StartTimer(name + "RecoveryEffectClose", (float)director.duration,
            () => 
            {
                recoveryEffect.SetActive(false);
                GameManager.instance.GlobalLightControll(1f, 0.5f);
                user.transform.DOScale(UnitPlat.originScale, 0.5f);
            });
    }
}
