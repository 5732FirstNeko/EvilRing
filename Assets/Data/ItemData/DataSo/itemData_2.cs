using UnityEngine;

[CreateAssetMenu(fileName = "Item", menuName = "Data/Item/" + nameof(itemData_2))]
public class itemData_2 : ItemDataSO
{
    public override void Action()
    {
        UnitCardSystem.instance.DestoryOneFriendlyUnit();
    }
}