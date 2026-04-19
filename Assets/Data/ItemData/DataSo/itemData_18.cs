using UnityEngine;

[CreateAssetMenu(fileName = "Item", menuName = "Data/Item/" + nameof(itemData_18))]
public class itemData_18 : ItemDataSO
{
    public override void Action()
    {
        BattleSystem.instance.OnGameStart += OnAction;
    }

    private float OnAction()
    {
        int first = -1;
        int second = -1;

        for (int i = 0; i < BattleSystem.instance.FriendlyUnitPlatsQueue.GetAllUnitPlat().Count; i++)
        {
            UnitPlat plat = BattleSystem.instance.FriendlyUnitPlatsQueue.GetAllUnitPlat()[i];
            if (plat != null && plat.unitData != null &&
                plat.unitData != FactorySystem.instance.EmptyFriendlyUnitData)
            {
                if (first != -1)
                {
                    second = i;
                    break;
                }
                else
                {
                    first = i;
                }
            }
        }

        UnitPlat firstplat = BattleSystem.instance.FriendlyUnitPlatsQueue.GetAllUnitPlat()[first];
        UnitPlat secondplat = BattleSystem.instance.FriendlyUnitPlatsQueue.GetAllUnitPlat()[second];

        if (firstplat == null || secondplat == null)
        {
            return 0;
        }

        secondplat.unit.MaxHP += firstplat.unit.MaxHP;
        secondplat.unit.HP += firstplat.unit.MaxHP;
        firstplat.unit.HP = 0;
        BattleSystem.instance.UnitDead(firstplat);

        return 1;
    }
}