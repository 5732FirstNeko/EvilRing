#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestSystem : MonoBehaviour
{
    [SerializeField] private List<Unit> friendlyUnits = new List<Unit>();
    [SerializeField] private List<Unit> hostitlyUnits = new List<Unit>();

    [SerializeField] private ItemDataSO ItemDataSO;

    private void Start()
    {
        InventoryManager.instance.AddInventoryToList(ItemDataSO);
    }

}
#endif