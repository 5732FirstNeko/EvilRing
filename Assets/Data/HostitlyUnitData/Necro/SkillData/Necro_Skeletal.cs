using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Playables;

[CreateAssetMenu(fileName = "newUnitData", menuName = "Data/Skill Data/Necro/" + nameof(Necro_Skeletal))]
public class Necro_Skeletal : UnitSkillDataSo
{
    [SerializeField] private GameObject attackPrefab;
    [SerializeField] private GameObject hitprefab;

    [SerializeField] private int skillListIndex;
    [SerializeField] private float hitEffectRate;

    private GameObject attackEffect;
    private GameObject hitEffect;

    public override void GameStartInit()
    {
        base.GameStartInit();

        attackEffect = Instantiate(attackPrefab, Vector3.zero, Quaternion.identity);
        attackEffect.SetActive(false);

        hitEffect = Instantiate(hitprefab, Vector3.zero, Quaternion.identity);
        hitEffect.SetActive(false);
    }

    public override void Action(ICollection<UnitPlat> unitPlats, UnitPlat user)
    {
        int index = Random.Range(0, 101);

        if (index <= 75)
        {
            Attack(user);
        }
        else
        {
            Defend(user);
        }
    }

    private void Attack(UnitPlat user)
    { 
        UnitPlat target = null;

        foreach (var unit in
            BattleSystem.instance.FriendlyUnitPlatsQueue.GetAllUnitPlat())
        {
            if (target == null && !unit.isDead &&
                unit.unitData != FactorySystem.instance.EmptyFriendlyUnitData)
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

        PlayableDirector attackDirector = attackEffect.GetComponent<PlayableDirector>();
        TimerManager.instance.StartTimer(name + "AttackEffect", 0.6f, 
            () => 
            {
                attackEffect.transform.position = target.transform.position;
                attackEffect.SetActive(false);
                attackDirector.Play();
            });

        PlayableDirector hitDirector = hitEffect.GetComponent<PlayableDirector>();
        TimerManager.instance.StartTimer(name + "HitEffect", 
            0.6f + (float)hitDirector.duration * hitEffectRate, 
            () => 
            {
                hitEffect.transform.position = target.transform.position;
                hitEffect.SetActive(false);
                hitDirector.Play();

                target.unit.HP -= Damage;
                target.UnitPlatHurtAnimation();
            });

        TimerManager.instance.StartTimer(name + "EffectCLose", 
            0.6f + (float)attackDirector.duration + (float)hitDirector.duration, 
            () => 
            {
                attackEffect.SetActive(false);
                hitEffect.SetActive(false);

                GameManager.instance.GlobalLightControll(1f, 0.5f);
                user.transform.DOScale(UnitPlat.originScale, 0.5f);
            });

        user.unit.unitSkills[skillListIndex].SkillTime =
            0.6f + (float)attackDirector.duration + (float)hitDirector.duration + 0.6f;
    }

    private void Defend(UnitPlat user)
    {
        GameManager.instance.GlobalLightControll(0.5f, 0.5f);
        user.transform.DOScale(UnitPlat.originScale * attackScale, 0.5f);

        TimerManager.instance.StartTimer(name + "DefendEffect", 0.6f, 
            () => 
            {
                user.DamageTextJump("º¡¹Ç¸ñµ²", Color.white);
                user.unit.OnDefend += OnDefendAction;
            });

        TimerManager.instance.StartTimer(name + "EffectClose", 0.6f + 1f, 
            () => 
            {
                GameManager.instance.GlobalLightControll(1f, 0.5f);
                user.transform.DOScale(UnitPlat.originScale, 0.5f);
            });

        user.unit.unitSkills[skillListIndex].SkillTime = 0.6f + 1f + 0.6f;
    }

    private int OnDefendAction(int hpcahnge, UnitPlat user)
    {
        if (hpcahnge < 0)
        {
            user.DamageTextJump("¸ñµ²", Color.white);
            user.unit.OnDefend -= OnDefendAction;
            return Mathf.RoundToInt(hpcahnge * 0.5f);
        }

        return hpcahnge;
    }
}
