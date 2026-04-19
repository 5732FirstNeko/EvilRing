using UnityEngine;

[CreateAssetMenu(fileName = "Item", menuName = "Data/Item/" + nameof(itemData_15))]
public class itemData_15 : ItemDataSO
{
    public override void Action()
    {
        BattleSystem.instance.OnRoundStart += OnAction;
    }

    private float OnAction(int round)
    {
        if (round > 0)
        {
            BattleSystem.instance.OnRoundStart -= OnAction;
            return 0;
        }

        int index1 = Random.Range(0, BattleSystem.instance.FriendlyUnitPlatsQueue.GetAllUnitPlat().Count);
        UnitPlat friUnit = BattleSystem.instance.FriendlyUnitPlatsQueue.GetAllUnitPlat()[index1];
        if (friUnit == null || friUnit.unitData == null ||
            friUnit.unitData == FactorySystem.instance.EmptyFriendlyUnitData)
        {
            for (int i = 0; i < BattleSystem.instance.FriendlyUnitPlatsQueue.GetAllUnitPlat().Count; i++)
            {
                UnitPlat plat = BattleSystem.instance.FriendlyUnitPlatsQueue.GetAllUnitPlat()[i];
                if (plat != null && plat.unitData != null &&
                    plat.unitData != FactorySystem.instance.EmptyFriendlyUnitData)
                {
                    friUnit = plat;
                    break;
                }
            }
        }

        int index2 = Random.Range(0, BattleSystem.instance.HostilityUnitPlatsQueue.GetAllUnitPlat().Count);
        UnitPlat hosUnit = BattleSystem.instance.FriendlyUnitPlatsQueue.GetAllUnitPlat()[index2];
        if (hosUnit == null || hosUnit.unitData == null ||
            hosUnit.unitData == FactorySystem.instance.EmptyFriendlyUnitData)
        {
            for (int i = 0; i < BattleSystem.instance.FriendlyUnitPlatsQueue.GetAllUnitPlat().Count; i++)
            {
                UnitPlat plat = BattleSystem.instance.FriendlyUnitPlatsQueue.GetAllUnitPlat()[i];
                if (plat != null && plat.unitData != null &&
                    plat.unitData != FactorySystem.instance.EmptyFriendlyUnitData)
                {
                    hosUnit = plat;
                    break;
                }
            }
        }

        if (friUnit != null)
        {
            friUnit.unit.HP = 0;
            BattleSystem.instance.UnitDead(friUnit);
        }

        if (hosUnit != null)
        {
            hosUnit.unit.HP = 0;
            BattleSystem.instance.UnitDead(hosUnit);
        }

        BattleSystem.instance.OnRoundStart -= OnAction;
        return 0;
    }
}