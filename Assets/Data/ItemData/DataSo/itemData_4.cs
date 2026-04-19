using UnityEngine;

[CreateAssetMenu(fileName = "Item", menuName = "Data/Item/" + nameof(itemData_4))]
public class itemData_4 : ItemDataSO
{
    public override void Action()
    {
        UnitCardSystem.instance.DestoryOneFriendlyUnit();
        UnitCardSystem.instance.DestoryOneFriendlyUnit();
    }
}