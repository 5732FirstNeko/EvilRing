using System.Collections;
using System.Collections.Generic;
using Coffee.UIEffects;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Playables;

[CreateAssetMenu(fileName = "newUnitData", menuName = "Data/Skill Data/" + nameof(Goblin_dagger))]
public class Goblin_dagger : UnitSkillDataSo
{
    [SerializeField] private GameObject daggerPrefab;
    [SerializeField] private GameObject hitPrefab;

    [SerializeField] private int skillListIndex;

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
        DaggerAttack(unitPlats, user);
    }

    private void DaggerAttack(ICollection<UnitPlat> unitPlats, UnitPlat user)
    {
        UnitPlat target = null;

        foreach (var unit in unitPlats)
        {
            if (target == null &&
                unit.unitData != FactorySystem.instance.EmptyFriendlyUnitData)
            {
                target = unit;
                continue;
            }

            if (target.unit.HP > target.unit.HP &&
                unit.unitData != FactorySystem.instance.EmptyFriendlyUnitData)
            {
                target = unit;
            }
        }

        if (target == null)
        {
            user.unit.unitSkills[skillListIndex].SkillTime = 0.5f;
            return;
        }

        GameManager.instance.GlobalLightControll(0.5f, 0.5f);
        user.transform.DOScale(UnitPlat.originScale * attackScale, 0.5f);

        TimerManager.instance.StartTimer(name + "ArrowEffect", 0.6f,
            () =>
            {
                daggerEffect.transform.position = user.transform.position;
                daggerEffect.SetActive(true);

                daggerEffect.transform.DOMove(target.transform.position, 1f).
                OnComplete(() => { daggerEffect.SetActive(false); });
            });

        TimerManager.instance.StartTimer(name + "ArrowEffect", 0.6f + 0.5f,
            () =>
            {
                hitEffect.transform.position = target.transform.position;
                hitEffect.SetActive(true);
                hitEffect.GetComponent<PlayableDirector>().Play();

                target.unit.HP -= Damage;
                target.UnitPlatHurtAnimation();
            });

        TimerManager.instance.StartTimer(name + "EffectClose", 0.6f + 1f + 0.1f,
            () =>
            {
                user.unit.spCount -= 2;
                hitEffect.SetActive(false);

                GameManager.instance.GlobalLightControll(1f, 0.5f);
                user.transform.DOScale(UnitPlat.originScale, 0.5f);
            });

        user.unit.unitSkills[skillListIndex].SkillTime = 0.6f + 1f + 0.2f;
    }
}
