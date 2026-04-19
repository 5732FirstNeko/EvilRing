using UnityEngine;

[CreateAssetMenu(fileName = "Item", menuName = "Data/Item/" + nameof(itemData_8))]
public class itemData_8 : ItemDataSO
{
    public override void Action()
    {
        InventoryManager.instance.AddInventoryToList(FactorySystem.instance.GetRandomItem());
    }
}