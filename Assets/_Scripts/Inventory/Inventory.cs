using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Inventory : MonoBehaviour, IPointerClickHandler,
    IBeginDragHandler, IEndDragHandler, IDragHandler
{
    public ItemDataSO itemData;
    public Image itemImage;

    public int roundContinue;
    public Action OnAction;
    public Action<UnitPlat> OnUnitplatAction;

    private void Start()
    {

    }

    public void OnPointerClick(PointerEventData eventData)
    {
        InventorySelectChange();
    }

    public void DataInit(ItemDataSO itemData)
    {
        this.itemData = itemData;
        itemImage.sprite = itemData.itemIcon;
        OnAction += itemData.Action;
        OnUnitplatAction += itemData.Action;
    }

    public void DataClear()
    {
        itemData = null;
        itemImage.sprite = null;
    }

    public void Action()
    {
        OnAction?.Invoke();
        InventoryManager.instance.lastUsedItem = itemData;
    }

    public void Action(UnitPlat unitPlat)
    {
        OnUnitplatAction?.Invoke(unitPlat);
        InventoryManager.instance.lastUsedItem = itemData;
    }

    private void InventorySelectChange()
    {
        if (InventoryManager.instance.currentSelectInventory == this)
        {
            InventoryManager.instance.currentSelectInventory = null;
            UIManager.instance.InventoryDataUIUnDisplay();
            transform.DOScale(Vector3.one, 0.25f);
        }
        else
        {
            InventoryManager.instance.currentSelectInventory = this;
            UIManager.instance.InventoryDataUIDisplay(itemData);
            transform.DOScale(new Vector3(1.1f, 1.1f, 1.1f), 0.25f);
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        UIManager.instance.InventoryDataUIUnDisplay();
        InventoryManager.instance.currentSelectInventory = this;
        InventoryManager.instance.isHaveDrag = true;

        itemImage.color = new Color(itemImage.color.r, itemImage.color.g, itemImage.color.b, 0.75f);
        InventoryManager.instance.InventoryInstance.sprite = itemData.itemIcon;
        InventoryManager.instance.InventoryInstance.raycastTarget = false;
        InventoryManager.instance.InventoryInstance.gameObject.SetActive(true);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        InventoryManager.instance.InventoryInstance.gameObject.SetActive(false);
        InventoryManager.instance.isHaveDrag = false;

        InventorySelectChange();

        itemImage.color = new Color(itemImage.color.r, itemImage.color.g, itemImage.color.b, 1f);

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction);
        UnitSiteFlag unitSiteFlag = hit.collider.GetComponent<UnitSiteFlag>();

        if (itemData.itemType == ItemBuffType.HostitlyUnit &&
            unitSiteFlag != null && unitSiteFlag.faction == Faction.Hostility)
        {
            UnitPlat unitPlat = UnitCardSystem.instance.GetHostitlyUnitPlats()
                [BattleSystem.GetIndexByUnitSite(unitSiteFlag.site)];

            if (unitPlat.unitData == null) return;

            transform.DOKill();
            InventoryManager.instance.RemoveInventryFromList(this);
            InventoryManager.instance.currentSelectInventory = null;

            Action(unitPlat);
        }
        else if (itemData.itemType == ItemBuffType.FriendlyUnit &&
            unitSiteFlag != null && unitSiteFlag.faction == Faction.Friendly)
        {
            UnitPlat unitPlat = UnitCardSystem.instance.GetCurrentFriendlyUnitPlats()
                [BattleSystem.GetIndexByUnitSite(unitSiteFlag.site)];
            if (unitPlat.unitData == null) return;

            transform.DOKill();
            InventoryManager.instance.RemoveInventryFromList(this);
            InventoryManager.instance.currentSelectInventory = null;

            Action(unitPlat);
        }
        else if (itemData.itemType == ItemBuffType.Global && 
            (eventData.pointerEnter == null || unitSiteFlag != null))
        {
            transform.DOKill();
            InventoryManager.instance.RemoveInventryFromList(this);
            InventoryManager.instance.currentSelectInventory = null;

            Action();
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        InventoryManager.instance.InventoryInstance.transform.position = Input.mousePosition;
    }

    public bool Equals(Inventory other)
    {
        return itemData == other.itemData;
    }
}
