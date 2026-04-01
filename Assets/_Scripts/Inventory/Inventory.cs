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

    public Action<ICollection<UnitPlat>, UnitPlat> OnAction;
    public Action<UnitPlat, UnitPlat, bool> OnBindBuff;
    public Action<UnitPlat, UnitPlat> OnUnBindBuff;

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

        roundContinue = itemData.itemBuff.ContinuousRound;
        OnAction += itemData.itemBuff.OnAction;
        OnBindBuff += itemData.itemBuff.BindBuff;
        OnUnBindBuff += itemData.itemBuff.UnbindBuff;
    }

    public void DataClear()
    {
        itemData = null;
        itemImage.sprite = null;

        roundContinue = -1;
        OnAction = null;
        OnBindBuff = null;
        OnUnBindBuff = null;
    }

    public void Action(ICollection<UnitPlat> unit)
    {
        OnAction?.Invoke(unit, null);
    }

    public void BindBuff(UnitPlat unit, bool isFirstBind = true)
    {
        OnBindBuff?.Invoke(unit, null, isFirstBind);
    }

    public void UnBindBuff(UnitPlat unit)
    {
        OnUnBindBuff?.Invoke(unit, null);
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
        GlobalArea globalArea = hit.collider.GetComponent<GlobalArea>();

        if (itemData.itemType == ItemBuffType.Unit &&
            unitSiteFlag != null && unitSiteFlag.faction == Faction.Hostility)
        {
            InventoryManager.instance.AddInventoryToUnit(this,
                UnitCardSystem.instance.GetHostitlyUnitPlats()
                [BattleSystem.GetIndexByUnitSite(unitSiteFlag.site)]);
            transform.DOKill();
            InventoryManager.instance.RemoveInventryFromList(this);
            InventoryManager.instance.currentSelectInventory = null;
        }
        else if (itemData.itemType == ItemBuffType.Global && (globalArea != null ||
            (unitSiteFlag != null && unitSiteFlag.faction == Faction.Friendly)))
        {
            InventoryManager.instance.AddInventoryToGlobal(this);
            transform.DOKill();
            InventoryManager.instance.RemoveInventryFromList(this);
            InventoryManager.instance.currentSelectInventory = null;
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
