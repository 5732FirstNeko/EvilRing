using UnityEngine;

[CreateAssetMenu(fileName = "Item", menuName = "Data/Item/" + nameof(itemData_21))]
public class itemData_21 : ItemDataSO
{
    public override void Action()
    {
        BattleSystem.instance.OnGameEnd += OnAction;
    }

    private void OnAction()
    {
        if (BattleSystem.instance.friendlyDeadCount <= 4) return;

        for (int i = 0; i < 3; i++)
        {
            if (FactorySystem.instance.friendlyUnitDataList.Count <= 0) break;

            int idnex = Random.Range(0, FactorySystem.instance.friendlyUnitDataList.Count);
            FactorySystem.instance.friendlyUnitDataList.RemoveAt(idnex);
        }

        foreach (var unit in
            BattleSystem.instance.FriendlyUnitPlatsQueue.GetAllUnitPlat())
        {
            if (unit != null && unit.unitData != null &&
                unit.unitData != FactorySystem.instance.EmptyFriendlyUnitData)
            {
                BattleSystem.instance.UnitResurrection(unit);
            }
        }
    }
}