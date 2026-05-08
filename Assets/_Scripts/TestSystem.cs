#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestSystem : MonoBehaviour
{
    [SerializeField] private List<UnitPlat> friendlyUnits = new List<UnitPlat>();
    [SerializeField] private List<UnitPlat> hostitlyUnits = new List<UnitPlat>();

    [SerializeField] private ItemDataSO ItemDataSO;

    [SerializeField] private HierarchicalTreeNodeDataSo HierarchicalTreeNodeDataSO;

    [SerializeField] private UnitDataSo dataSo;

    private void Start()
    {
        InventoryManager.instance.AddInventoryToList(ItemDataSO);

    }

    public void UnitSkillTest()
    {
        UnitCardSystem.instance.friendlyUnitRefreshArea[0].unitData = dataSo;
        UnitCardSystem.instance.friendlyUnitRefreshArea[0].Image.sprite = dataSo.UnitSprite;
        UnitCardSystem.instance.friendlyUnitRefreshArea[0].isLock = false;
    }
}
#endif