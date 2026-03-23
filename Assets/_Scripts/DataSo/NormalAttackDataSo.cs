using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NormalAttack", menuName = "Data/Skill/NormalAttack")]
public class NormalAttackDataSo : UnitSkillDataSo
{
    // 实现普攻逻辑：给目标扣血
    public override void Action(ICollection<UnitPlat> unitPlats, UnitPlat user)
    {
        foreach (UnitPlat target in unitPlats)
        {
            // 扣血（防止血量为负）
            target.unit.HP = (byte)Mathf.Max(0, target.unit.HP - Damage);
            Debug.Log($"{user.name} 普攻{target.name}，造成{Damage}伤害，{target.name}剩余血量{target.unit.HP}");
        }
    }
}