using UnityEngine;

[CreateAssetMenu(fileName = "Item", menuName = "Data/Item/" + nameof(itemData_27))]
public class itemData_27 : ItemDataSO
{
    public override void Action()
    {
        UnitCardSystem.instance.RefreshAllFriendlyUnit();
    }
}