using UnityEngine;

[CreateAssetMenu(fileName = "Item", menuName = "Data/Item/" + nameof(itemData_9))]
public class itemData_9 : ItemDataSO
{
    public override void Action()
    {
        ItemDataSO item = InventoryManager.instance.lastUsedItem;
        if (item == null)
        {
            item = FactorySystem.instance.GetRandomItem();
        }
        InventoryManager.instance.AddInventoryToList(item);
    }
}