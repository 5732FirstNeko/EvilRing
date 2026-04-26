using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Playables;

[CreateAssetMenu(fileName = "newUnitData", menuName = "Data/Skill Data/Goblin/" + nameof(Goblin_shield))]
public class Goblin_shield : UnitSkillDataSo
{
    [SerializeField] private GameObject shiledAttackPrefab;
    [SerializeField] private GameObject hitPrefab;

    [SerializeField] private int skillListIndex;

    private GameObject shiledAttackEffect;
    private GameObject hitEffect;

    public override void GameStartInit()
    {
        base.GameStartInit();

        shiledAttackEffect = Instantiate(shiledAttackPrefab, Vector3.zero, Quaternion.identity);
        shiledAttackEffect.SetActive(false);

        hitEffect = Instantiate(hitPrefab, Vector3.zero, Quaternion.identity);
        hitEffect.SetActive(false);
    }

    public override void Action(ICollection<UnitPlat> unitPlats, UnitPlat user)
    {
        UnitPlat target = null;
        foreach (var unit in unitPlats)
        {
            if (unit.unitData != FactorySystem.instance.EmptyFriendlyUnitData)
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

        PlayableDirector attackDirector = shiledAttackEffect.GetComponent<PlayableDirector>();
        TimerManager.instance.StartTimer(name + "ShiledAttackHit", 0.6f, 
            () => 
            {
                shiledAttackEffect.transform.position = target.transform.position;
                shiledAttackEffect.SetActive(true);
                attackDirector.Play();
            });

        PlayableDirector hitDirector = hitEffect.GetComponent<PlayableDirector>();
        TimerManager.instance.StartTimer(name + "hitEffects",0.6f + (float)attackDirector.duration, 
            () => 
            {
                hitEffect.transform.position = target.transform.position;
                hitEffect.SetActive(true);
                attackDirector.Play();

                target.unit.HP -= Damage;
                target.UnitPlatHurtAnimation();
            });

        TimerManager.instance.StartTimer(name + "EffectClose", 
            0.6f + (float)attackDirector.duration + (float)hitDirector.duration, 
            () => 
            {
                shiledAttackEffect.SetActive(false);
                hitEffect.SetActive(false);

                GameManager.instance.GlobalLightControll(1f, 0.5f);
                user.transform.DOScale(UnitPlat.originScale, 0.5f);
            });

        user.unit.unitSkills[skillListIndex].SkillTime =
            0.6f + (float)attackDirector.duration + (float)hitDirector.duration + 0.5f + 0.1f;
    }
}
