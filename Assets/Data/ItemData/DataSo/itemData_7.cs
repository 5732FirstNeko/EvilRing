using UnityEngine;

[CreateAssetMenu(fileName = "Item", menuName = "Data/Item/" + nameof(itemData_7))]
public class itemData_7 : ItemDataSO
{
    public override void Action()
    {
        BattleSystem.instance.OnGameEnd += OnAction;
    }

    private void OnAction()
    {
        //TODO : Choice Different Card

        BattleSystem.instance.OnGameEnd -= OnAction;
    }
}