using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Item", menuName = "Data/Item/" + nameof(itemData_31))]
public class itemData_31 : ItemDataSO
{
    public override void Action()
    {
        BattleSystem.instance.OnGameStart += OnAction;
    }

    private float OnAction()
    {
        List<UnitPlat> unitPlats_fri = BattleSystem.instance.FriendlyUnitPlatsQueue.GetAllUnitPlat();

        int index_fri = Random.Range(0, unitPlats_fri.Count);

        UnitPlat plat_fri = unitPlats_fri[index_fri];
        if (plat_fri == null || plat_fri.unitData == null ||
            plat_fri.unitData == FactorySystem.instance.EmptyFriendlyUnitData)
        {
            for (int i = 0; i < unitPlats_fri.Count; i++)
            {
                UnitPlat unit = unitPlats_fri[i];
                if (unit != null || unit.unitData != null ||
                    unit.unitData != FactorySystem.instance.EmptyFriendlyUnitData)
                {
                    plat_fri = unit;
                }
            }
        }

        List<UnitPlat> unitPlats_hos = BattleSystem.instance.HostilityUnitPlatsQueue.GetAllUnitPlat();

        int index_hos = Random.Range(0, unitPlats_hos.Count);

        UnitPlat plat_hos = unitPlats_hos[index_hos];
        if (plat_hos == null || plat_hos.unitData == null ||
            plat_hos.unitData == FactorySystem.instance.EmptyFriendlyUnitData)
        {
            for (int i = 0; i < unitPlats_hos.Count; i++)
            {
                UnitPlat unit = unitPlats_hos[i];
                if (unit != null || unit.unitData != null ||
                    unit.unitData != FactorySystem.instance.EmptyFriendlyUnitData)
                {
                    plat_hos = unit;
                }
            }
        }

        int temp_HP = plat_fri.unit.HP;
        int temp_MaxHP = plat_fri.unit.MaxHP;

        plat_fri.unit.MaxHP = plat_hos.unit.MaxHP;
        plat_fri.unit.HP = plat_hos.unit.HP;

        plat_hos.unit.MaxHP = temp_MaxHP;
        plat_hos.unit.HP = temp_HP;

        BattleSystem.instance.OnGameStart -= OnAction;

        return 1;
    }
}