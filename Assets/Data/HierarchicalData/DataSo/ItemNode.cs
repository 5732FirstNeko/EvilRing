using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "HierarchicalNode",
    menuName = "Data/HierarchicalNode/" + nameof(ItemNode))]
public class ItemNode : HierarchicalTreeNodeDataSo
{
    public override void UnLoackAction()
    {
        FactorySystem.instance.itemLevel++;
    }
}
