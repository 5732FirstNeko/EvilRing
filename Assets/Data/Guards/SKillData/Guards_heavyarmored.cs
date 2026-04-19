using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Playables;

[CreateAssetMenu(fileName = "newUnitData", menuName = "Data/Skill Data/" + nameof(Guards_heavyarmored))]
public class Guards_heavyarmored : UnitSkillDataSo
{
    [SerializeField] private GameObject attackHitPrefab;

    [SerializeField] private int skillListIndex;
    [SerializeField] private UnitDataSo userDataSo;

    private GameObject attackHitEffect;
    //one is deafend State, zero is attack State
    public override void GameStartInit()
    {
        base.GameStartInit();

        attackHitEffect = Instantiate(attackHitPrefab, Vector3.zero, Quaternion.identity);
        attackHitEffect.SetActive(false);

        foreach (var unit in BattleSystem.instance.HostilityUnitPlatsQueue.GetAllUnitPlat())
        {
            if (unit.unitData == userDataSo)
            {
                unit.unit.OnHPChange += OnHPChangeAction;
            }
        }
    }

    public override void Action(ICollection<UnitPlat> unitPlats, UnitPlat user)
    {
        if (user.costumvalue_second == 0)
        {
            user.costumvalue_first++;
        }

        if ((user.costumvalue_second == 0 && user.costumvalue_first >= 1) || 
            user.unit.HP <= Mathf.RoundToInt(user.unit.MaxHP * 0.4f))
        {
            user.costumvalue_second = 1;
            user.costumvalue_first = 0;

            user.DamageTextJump("·ÀÓù", Color.white);
        }

        if (user.costumvalue_first == 1)
        {
            Defend(user);
        }
        else
        {
            Attack(user);
        }
    }

    #region Defend
    private void Defend(UnitPlat user)
    {
        GameManager.instance.GlobalLightControll(0.5f, 0.5f);
        user.transform.DOScale(UnitPlat.originScale * attackScale, 0.5f);

        int index = Random.Range(0, 101);

        if (index <= 75)
        {
            user.unit.OnDefend += DefendAction;
        }
        else
        {
            foreach (var unit in 
                BattleSystem.instance.HostilityUnitPlatsQueue.GetAllUnitPlat())
            {
                if (!unit.isDead && 
                    unit.unitData == FactorySystem.instance.EmptyHostitlyUnitData)
                {
                    unit.unit.OnDefend += AllUnitDefendAction;
                }
            }
        }

        TimerManager.instance.StartTimer(name + "ActionEnd",0.6f + 1.1f, 
            () => 
            {
                GameManager.instance.GlobalLightControll(1f, 0.5f);
                user.transform.DOScale(UnitPlat.originScale, 0.5f);
            });

        user.unit.unitSkills[skillListIndex].SkillTime = 0.6f + 1.1f + 0.6f;
    }

    private int DefendAction(int hpchange, UnitPlat user)
    {
        if (hpchange < 0)
        {
            user.DamageTextJump("¸ñµ²", Color.white);
            user.unit.OnDefend -= DefendAction;
            return Mathf.RoundToInt(hpchange * 0.2f);
        }

        return hpchange;
    }

    private int AllUnitDefendAction(int hpchange, UnitPlat user)
    {
        if (hpchange < 0)
        {
            user.DamageTextJump("ÕóµØ¹ÌÊØ", Color.white);
            user.unit.OnDefend -= DefendAction;
            return Mathf.RoundToInt(hpchange * 0.8f);
        }

        return hpchange;
    }
    #endregion

    #region Attack
    private void Attack(UnitPlat user)
    {
        int index = Random.Range(0, 101);

        if (index <= 75)
        {
            HeavyBlow(user);
        }
        else
        {
            ArmorPiercing(user);
        }
    }

    private void HeavyBlow(UnitPlat user)
    {
        UnitPlat target = null;
        foreach (var unit in BattleSystem.instance.FriendlyUnitPlatsQueue.GetAllUnitPlat())
        {
            if (!unit.isDead && unit.unitData != FactorySystem.instance.EmptyHostitlyUnitData)
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

        PlayableDirector director = attackHitEffect.GetComponent<PlayableDirector>();
        TimerManager.instance.StartTimer(name + "AttackEffect", 0.6f,
            () =>
            {
                attackHitEffect.transform.position = target.transform.position;
                attackHitEffect.SetActive(true);
                director.Play();

                target.UnitPlatHurtAnimation();
                target.unit.HP -= Damage;
            });

        TimerManager.instance.StartTimer(name + "EffectClose", 0.6f + (float)director.duration + 0.1f,
            () =>
            {
                attackHitEffect.SetActive(false);

                GameManager.instance.GlobalLightControll(1f, 0.5f);
                user.transform.DOScale(UnitPlat.originScale, 0.5f);
            });

        user.unit.unitSkills[skillListIndex].SkillTime = 0.6f + (float)director.duration + 0.1f + 0.6f;
    }

    private void ArmorPiercing(UnitPlat user)
    {
        UnitPlat target = null;
        foreach (var unit in 
            BattleSystem.instance.FriendlyUnitPlatsQueue.GetAllUnitPlat())
        {
            if (!unit.isDead && unit.unitData != FactorySystem.instance.EmptyHostitlyUnitData)
            {
                if (target == null)
                {
                    target = unit;
                    continue;
                }

                if (target.unit.HP < unit.unit.HP)
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

        TimerManager.instance.StartTimer(name + "ArmorPiercingText", 0.6f, 
            () => 
            {
                target.DamageTextJump("ÆÆ¼×", Color.blue);
                target.unit.OnDefend = null;
            });

        TimerManager.instance.StartTimer(name + "ArmorPiercingClose",0.6f + 1.1f, 
            () => 
            {
                GameManager.instance.GlobalLightControll(1f, 0.5f);
                user.transform.DOScale(UnitPlat.originScale, 0.5f);
            });

        user.unit.unitSkills[skillListIndex].SkillTime = 0.6f + 1.1f + 0.6f;
    }
    #endregion

    private void OnHPChangeAction(int hpchange, UnitPlat user)
    {
        if (user.costumvalue_second == 1)
        {
            user.costumvalue_first++;
        }

        if (user.costumvalue_second == 1 && user.costumvalue_first >= 2)
        {
            user.costumvalue_second = 0;
            user.costumvalue_first = 0;
            user.DamageTextJump("¹¥»÷", Color.white);
        }
    }
}
