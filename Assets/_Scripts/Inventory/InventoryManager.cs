using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance;
    public static InventoryManager instance
    {
        get
        {
            if (Instance == null)
            {
                GameObject Object = new GameObject(typeof(InventoryManager).Name);
                Instance = Object.AddComponent<InventoryManager>();
                DontDestroyOnLoad(Object);
            }
            return Instance;
        }
    }

    public int gold
    {
        get => _gold;
        set 
        {
            _gold = value;
            UIManager.instance.goldText.text = "»Æ½ð : " + _gold;
        }
    }
    public int ghost
    {
        get => _ghost;
        set
        {
            _ghost = value;
            UIManager.instance.ghostText.text = "Áé»ê : " + _ghost;
        }
    }
    public int ghostTotal
    {
        get => _ghostTotal;
        set
        {
            _ghostTotal = value;
            UIManager.instance.ghostTotalText.text = "Áé»ê×ÜÁ¿ : " + _ghost;
        }
    }

    public int _gold;
    public int _ghost;
    public int _ghostTotal;

    public List<Inventory> itemList;
    [SerializeField] private Transform InventoryParent;
    [SerializeField] private Transform usedInventoryParent;
    [SerializeField] private GameObject inventoryPrefab;

    public Inventory currentSelectInventory 
    {
        get => _selectInventory;
        set
        {
            _selectInventory = value;
            if (_selectInventory != null)
            {
                UnitCardSystem.instance.currentFriendlyUnitUI = null;
            }
        }
    }
    private Inventory _selectInventory;

    public bool isHaveDrag;
    public Image InventoryInstance;

    public List<Inventory> globalInventoryList;
    public Dictionary<Inventory, UnitPlat> InventoryTargetMap;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        globalInventoryList = new List<Inventory>();
        InventoryTargetMap = new Dictionary<Inventory, UnitPlat>();
    }

    public void AddInventoryToList(ItemDataSO itemData)
    {
        GameObject newInventory = Instantiate(inventoryPrefab, InventoryParent.position, Quaternion.identity);
        newInventory.transform.SetParent(InventoryParent);
        newInventory.GetComponent<Inventory>().DataInit(itemData);
    }

    public void RemoveInventryFromList(Inventory inventory)
    {
        itemList.Remove(inventory);
        inventory.transform.SetParent(usedInventoryParent);
        inventory.transform.localScale = Vector3.zero;
    }

    #region UnitInventory
    public void AddInventoryToUnit(Inventory inventory, UnitPlat target)
    {
        InventoryTargetMap.Add(inventory, target);
    }

    public void RemoveInventoryfromUnit(Inventory inventory)
    {
        inventory.UnBindBuff(InventoryTargetMap[inventory]);
        InventoryTargetMap.Remove(inventory);
        Destroy(inventory.gameObject);
    }

    public void UnitInventoryBind()
    {
        foreach (var (invent, tar) in InventoryTargetMap)
        {
            invent.BindBuff(tar);
        }
    }

    public void UnitInventoryUnBind()
    {
        foreach (var (invent, tar) in InventoryTargetMap)
        {
            invent.UnBindBuff(tar);
        }
    }
    #endregion

    #region GlobalInventory
    public void AddInventoryToGlobal(Inventory inventory)
    {
        globalInventoryList.Add(inventory);
    }

    public void RemoveInventoryfromGlobal(Inventory inventory)
    {
        foreach (var unit in BattleSystem.instance.HostilityUnitPlatsQueue.unitPlatQueue)
        {
            inventory.UnBindBuff(unit);
        }
        globalInventoryList.Remove(inventory);
        Destroy(inventory.gameObject);
    }

    public void GlobalInventoryBind()
    {
        foreach (var inventory in globalInventoryList)
        {
            foreach (var unit in BattleSystem.instance.HostilityUnitPlatsQueue.unitPlatQueue)
            {
                inventory.BindBuff(unit);
            }
        }
    }

    public void GlobalInventoryUnbind()
    {
        foreach (var inventory in globalInventoryList)
        {
            foreach (var unit in BattleSystem.instance.HostilityUnitPlatsQueue.unitPlatQueue)
            {
                inventory.UnBindBuff(unit);
            }
        }
    }
    #endregion
}
