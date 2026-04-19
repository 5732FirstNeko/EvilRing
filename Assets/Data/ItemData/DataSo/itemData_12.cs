using UnityEngine;

[CreateAssetMenu(fileName = "Item", menuName = "Data/Item/" + nameof(itemData_12))]
public class itemData_12 : ItemDataSO
{
    [SerializeField] private int HPAdditive;

    public override void Action()
    {
        BattleSystem.instance.OnRoundStart += OnAction;
    }

    private float OnAction(int round)
    {
        if (round == 0)
        {
            foreach (var unit in
                BattleSystem.instance.FriendlyUnitPlatsQueue.GetAllUnitPlat())
            {
                BattleSystem.instance.UnitRemoveQueue(unit);
                unit.unit.MaxHP += HPAdditive;
                unit.unit.HP += HPAdditive;
            }
        }
        BattleSystem.instance.OnRoundStart -= OnAction;

        return 0;
    }
}