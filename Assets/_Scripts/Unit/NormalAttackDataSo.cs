using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NormalAttack", menuName = "Data/Skill/NormalAttack")]
public class NormalAttackDataSo : UnitSkillDataSo
{
    // 实现普攻逻辑：给目标扣血
    public override void Action(ICollection<Unit> units, Unit user)
    {
        foreach (Unit target in units)
        {
            // 扣血（防止血量为负）
            target.HP = (byte)Mathf.Max(0, target.HP - Damage);
            Debug.Log($"{user.name} 普攻{target.name}，造成{Damage}伤害，{target.name}剩余血量{target.HP}");
        }
    }
}