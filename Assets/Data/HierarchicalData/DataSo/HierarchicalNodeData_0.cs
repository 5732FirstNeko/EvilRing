using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "newUnitData", menuName = "Data/HierarchicalNode Data/" + nameof(HierarchicalNodeData_0))]
public class HierarchicalNodeData_0 : HierarchicalTreeNodeDataSo
{
    public override void UnLoackAction()
    {
        base.UnLoackAction();

        Debug.Log("Node UnLock!");
    }
}
