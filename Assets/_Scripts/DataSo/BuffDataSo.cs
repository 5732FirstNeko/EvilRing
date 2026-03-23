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
    public override void OnAction(ICollection<UnitPlat> unitPlats, UnitPlat user)
    {
        foreach (UnitPlat target in unitPlats)
        {
            if (HealValue > 0)
            {
                target.unit.HP = (byte)Mathf.Min(target.unit.MaxHP, target.unit.HP + HealValue);
                Debug.Log($"{nameof(target)} 回血{HealValue}，当前血量{target.unit.HP}");
            }
        }
    }

    // 挂载Buff时的逻辑：应用属性加成
    public override void BindBuff(UnitPlat unitPlats, UnitPlat user, bool isFirstBind = true)
    {
        if (SpeedBonus != 0)
        {
            unitPlats.unit.speed += (byte)SpeedBonus;
            Debug.Log($"{nameof(unitPlats)} 速度+{SpeedBonus}，当前速度{unitPlats.unit.speed}");
        }
        if (MaxHPBonus != 0)
        {
            unitPlats.unit.MaxHP += (byte)MaxHPBonus;
            Debug.Log($"{nameof(unitPlats)} 最大血量+{MaxHPBonus}，当前最大血量{unitPlats.unit.MaxHP}");
        }
        if (DamageReduction != 0)
        {
            unitPlats.unit.DamageReduction += DamageReduction;
            Debug.Log($"{nameof(unitPlats)} 减伤+{DamageReduction}，当前减伤{unitPlats.unit.DamageReduction}");
        }
    }

    // 移除Buff时的逻辑：移除属性加成
    public override void UnbindBuff(UnitPlat unitPlats, UnitPlat user)
    {
        if (SpeedBonus != 0)
        {
            unitPlats.unit.speed -= (byte)SpeedBonus;
            Debug.Log($"{nameof(unitPlats)} 速度-{SpeedBonus}，当前速度{unitPlats.unit.speed}");
        }
        if (MaxHPBonus != 0)
        {
            unitPlats.unit.MaxHP -= (byte)MaxHPBonus;
            Debug.Log($"{nameof(unitPlats)} 最大血量-{MaxHPBonus}，当前最大血量{unitPlats.unit.MaxHP}");
        }
        if (DamageReduction != 0)
        {
            unitPlats.unit.DamageReduction -= DamageReduction;
            Debug.Log($"{nameof(unitPlats)} 减伤-{DamageReduction}，当前减伤{unitPlats.unit.DamageReduction}");
        }
    }
}