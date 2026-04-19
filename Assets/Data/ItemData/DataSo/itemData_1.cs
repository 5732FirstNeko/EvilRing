using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Item", menuName = "Data/Item/" + nameof(itemData_1))]
public class itemData_1 : ItemDataSO
{
    public override void Action()
    {
        UnitCardSystem.instance.RefreshRandomFriendlyUnit();
    }
}