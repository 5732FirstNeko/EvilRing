using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ExplodeDead", menuName = "Data/DeadData/Explode")]
public class ExplodeDeadDataSo : UnitDeadDataSo
{
    public GameObject ExplodeEffect; // 爆炸特效预制体
    public float EffectDuration = 1f; // 特效显示时长

    // 实现父类的抽象方法：死亡时播放爆炸
    public override void DeadAction(UnitPlat user)
    {
        // 生成爆炸特效
        GameObject effect = Instantiate(ExplodeEffect, user.transform.position, Quaternion.identity);
        // 1秒后销毁特效
        Destroy(effect, EffectDuration);
        // 标记单位死亡（后续BattleSystem会处理）
        //TODO : Unit Dead VFX
    }
}