using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

[CreateAssetMenu(fileName = "newUnitData", menuName = "Data/Skill Data/" + nameof(Goblin_shield_sp))]
public class Goblin_shield_sp : UnitSkillDataSo
{
    [SerializeField] private int skillListIndex;

    public override void Action(ICollection<UnitPlat> unitPlats, UnitPlat user)
    {
        UnitPlat target = null;
        foreach (var unit in unitPlats)
        {
            if (unit.unitData != FactorySystem.instance.EmptyHostitlyUnitData && unit != user)
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

        TimerManager.instance.StartTimer(name + "ShiledEffect", 0.6f, 
            () => 
            {
                target.DamageTextJump("ÉËº¦×ªÒÆ", Color.white);

                target.unit.OnDefend += 
                (hpchange, u) => 
                {
                    if (hpchange < 0)
                    {
                        user.unit.HP += hpchange;
                    }

                    return OnDefendAction(hpchange, u);
                };
            });

        TimerManager.instance.StartTimer(name + "ShiledEffectClose",0.6f + 1f + 0.1f, 
            () => 
            {
                GameManager.instance.GlobalLightControll(1f, 0.5f);
                user.transform.DOScale(UnitPlat.originScale, 0.5f);
            });

        user.unit.unitSkills[skillListIndex].SkillTime = 0.6f + 1f + 0.1f + 0.5f + 0.1f;
    }

    private int OnDefendAction(int hpchange, UnitPlat user)
    {
        if (hpchange >= 0)
        {
            return hpchange;
        }

        user.DamageTextJump("·ÀÓù", Color.white);

        return -hpchange;
    }
}
