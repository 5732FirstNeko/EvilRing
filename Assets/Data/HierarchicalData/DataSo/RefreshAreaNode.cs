using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "HierarchicalNode", 
    menuName = "Data/HierarchicalNode/" + nameof(RefreshAreaNode))]
public class RefreshAreaNode : HierarchicalTreeNodeDataSo
{
    public override void UnLoackAction()
    {
        base.UnLoackAction();

        foreach (var area in UnitCardSystem.instance.friendlyUnitRefreshArea)
        {
            if (!area.gameObject.activeSelf)
            {
                area.gameObject.SetActive(true);
                return;
            }
        }
    }
}
