using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;

[CreateAssetMenu(fileName = "newUnitData", menuName = "Data/Skill Data/Goblin/" + nameof(Goblin_shield_sp))]
public class Goblin_shield_sp : UnitSkillDataSo
{
    [SerializeField] private int skillListIndex;

    private Func<int, UnitPlat, int> defendFunction;

    public override void GameEndAction()
    {
        
    }

    public override void Action(ICollection<UnitPlat> unitPlats, UnitPlat user)
    {
        UnitPlat target = null;
        foreach (var unit in unitPlats)
        {
            if (unit.unitData != FactorySystem.instance.EmptyHostitlyUnitData && unit != user && !unit.isDead)
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

                
                defendFunction = (hpchange, u) =>
                {
                    if (hpchange < 0)
                    {
                        user.unit.HP += hpchange;
                    }

                    u.DamageTextJump("·ÀÓù", Color.white);

                    target.unit.OnDefend -= defendFunction;
                    return hpchange >= 0 ? hpchange : -hpchange;
                };

                target.unit.OnDefend -= defendFunction;
                target.unit.OnDefend += defendFunction;
            });

        TimerManager.instance.StartTimer(name + "ShiledEffectClose",0.6f + 1f + 0.1f, 
            () => 
            {
                GameManager.instance.GlobalLightControll(1f, 0.5f);
                user.transform.DOScale(UnitPlat.originScale, 0.5f);
            });

        user.unit.unitSkills[skillListIndex].SkillTime = 0.6f + 1f + 0.1f + 0.5f + 0.1f;
    }
}
