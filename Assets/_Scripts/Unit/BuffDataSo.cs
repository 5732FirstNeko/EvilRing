using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CommonBuff", menuName = "Data/Buff/CommonBuff")]
public class BuffDataSo : UnitBuffDataSo
{
    [Header("Buff属性")]
    public int HealValue = 10; // 每回合回血值
    public int SpeedBonus = 0; // 速度加成
    public int MaxHPBonus = 0; // 最大血量加成
    public int DamageReduction = 0; // 减伤值

    // 实现Buff生效逻辑：每回合回血
    public override void OnAction(ICollection<Unit> unit, Unit user)
    {
        foreach (Unit target in unit)
        {
            if (HealValue > 0)
            {
                target.HP = (byte)Mathf.Min(target.MaxHP, target.HP + HealValue);
                Debug.Log($"{target.name} 回血{HealValue}，当前血量{target.HP}");
            }
        }
    }

    // 挂载Buff时的逻辑：应用属性加成
    public override void BindBuff(Unit unit, Unit user, bool isFirstBind = true)
    {
        if (SpeedBonus != 0)
        {
            unit.speed += (byte)SpeedBonus;
            Debug.Log($"{unit.name} 速度+{SpeedBonus}，当前速度{unit.speed}");
        }
        if (MaxHPBonus != 0)
        {
            unit.MaxHP += (byte)MaxHPBonus;
            Debug.Log($"{unit.name} 最大血量+{MaxHPBonus}，当前最大血量{unit.MaxHP}");
        }
        if (DamageReduction != 0)
        {
            unit.DamageReduction += DamageReduction;
            Debug.Log($"{unit.name} 减伤+{DamageReduction}，当前减伤{unit.DamageReduction}");
        }
    }

    // 移除Buff时的逻辑：移除属性加成
    public override void UnbindBuff(Unit unit, Unit user)
    {
        if (SpeedBonus != 0)
        {
            unit.speed -= (byte)SpeedBonus;
            Debug.Log($"{unit.name} 速度-{SpeedBonus}，当前速度{unit.speed}");
        }
        if (MaxHPBonus != 0)
        {
            unit.MaxHP -= (byte)MaxHPBonus;
            Debug.Log($"{unit.name} 最大血量-{MaxHPBonus}，当前最大血量{unit.MaxHP}");
        }
        if (DamageReduction != 0)
        {
            unit.DamageReduction -= DamageReduction;
            Debug.Log($"{unit.name} 减伤-{DamageReduction}，当前减伤{unit.DamageReduction}");
        }
    }
}