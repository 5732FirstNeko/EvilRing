using UnityEngine;

[CreateAssetMenu(fileName = "Item", menuName = "Data/Item/" + nameof(itemData_3))]
public class itemData_3 : ItemDataSO
{
    public override void Action(UnitPlat unitPlat)
    {
        unitPlat.unit.speed = 1;
    }
}