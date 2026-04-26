using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "newUnitData", menuName = "Data/Dead Data/" + nameof(KnightDead))]
public class KnightDead : UnitDeadDataSo
{
    public override void DeadAction(UnitPlat user)
    {
        StandrdDead(user);

        if (user.costumvalue_first <= 0)
        {
            return;
        }

        UnitPlat target = null;
        List<UnitPlat> unitPlats = BattleSystem.instance.FriendlyUnitPlatsQueue.GetAllUnitPlat();
        for (int i = 0; i < unitPlats.Count; i++)
        {
            if (unitPlats[i].unitData != null &&
                FactorySystem.instance.knightCards.Contains(unitPlats[i].unitData))
            {
                target = unitPlats[i];
                break;
            }
        }

        if (target == null) return;

        target.costumvalue_first = user.costumvalue_first + 1;
        user.costumvalue_first = 0;
    }
}
