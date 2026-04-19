using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UnitSiteFlag : MonoBehaviour, IDropHandler, IPointerClickHandler
{
    public Faction faction;
    public UnitSite site;

    private bool isUIDataDisplay;

    public void OnDrop(PointerEventData eventData)
    {
        
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (faction == Faction.Friendly)
        {
            if (eventData.button == PointerEventData.InputButton.Left &&
            UnitCardSystem.instance.currentFriendlyUnitUI != null)
            {
                UnitCardSystem.instance.AddUnitToFriendlyList(UnitCardSystem.instance.currentFriendlyUnitUI.unitData,
                    eventData.pointerEnter.GetComponent<UnitSiteFlag>().site);
            }
            else if (eventData.button == PointerEventData.InputButton.Left &&
                UnitCardSystem.instance.currentFriendlyUnitUI == null)
            {
                if (!isUIDataDisplay)
                {
                    isUIDataDisplay = true;
                    OnUIDataDisplay(eventData);
                }
                else
                {
                    isUIDataDisplay = false;
                    OnUIDataUnDisplay(eventData);
                }
            }
            else if (eventData.button == PointerEventData.InputButton.Left &&
                InventoryManager.instance.currentSelectInventory != null &&
                InventoryManager.instance.currentSelectInventory.itemData.itemType == ItemBuffType.FriendlyUnit)
            {
                UnitPlat unitPlat = UnitCardSystem.instance.GetHostitlyUnitPlats()
                    [BattleSystem.GetIndexByUnitSite(site)];

                if (unitPlat != null) return;

                InventoryManager.instance.RemoveInventryFromList(InventoryManager.instance.currentSelectInventory);
                InventoryManager.instance.currentSelectInventory = null;

                InventoryManager.instance.currentSelectInventory.Action(unitPlat);
            }
            else if (eventData.button == PointerEventData.InputButton.Left &&
                InventoryManager.instance.currentSelectInventory != null &&
                InventoryManager.instance.currentSelectInventory.itemData.itemType == ItemBuffType.Global)
            {
                InventoryManager.instance.RemoveInventryFromList(InventoryManager.instance.currentSelectInventory);
                InventoryManager.instance.currentSelectInventory = null;

                InventoryManager.instance.currentSelectInventory.Action();
            }
            else if (eventData.button == PointerEventData.InputButton.Right)
            {
                UnitCardSystem.instance.RemoveUnitFromFriendlyList(site);
            }
        }
        else
        {
            if (InventoryManager.instance.currentSelectInventory != null)
            {
                if (InventoryManager.instance.currentSelectInventory.itemData.itemType == ItemBuffType.HostitlyUnit)
                {
                    UnitPlat unitPlat = UnitCardSystem.instance.GetHostitlyUnitPlats()
                        [BattleSystem.GetIndexByUnitSite(site)];

                    if (unitPlat != null) return;

                    InventoryManager.instance.RemoveInventryFromList(InventoryManager.instance.currentSelectInventory);
                    InventoryManager.instance.currentSelectInventory = null;

                    InventoryManager.instance.currentSelectInventory.Action(unitPlat);
                }
                else if(InventoryManager.instance.currentSelectInventory.itemData.itemType == ItemBuffType.Global)
                {
                    InventoryManager.instance.RemoveInventryFromList(InventoryManager.instance.currentSelectInventory);
                    InventoryManager.instance.currentSelectInventory = null;

                    InventoryManager.instance.currentSelectInventory.Action();
                }
            }
        }
    }

    public void OnUIDataDisplay(PointerEventData eventData)
    {
        if (GameManager.instance.isHaveDrag || 
            GameManager.instance.gameState == GameManager.GameState.Game)
        {
            return;
        }

        int index = BattleSystem.GetIndexByUnitSite(site);
        switch (faction)
        {
            case Faction.Friendly:
                List<UnitPlat> friendlyUnitPlats = UnitCardSystem.instance.GetCurrentFriendlyUnitPlats();

                if (friendlyUnitPlats.Count == 4 && friendlyUnitPlats[index].unitData != null && 
                    friendlyUnitPlats[index].unitData.Skills.Count > 0)
                {
                    UIManager.instance.FriendlyUnitDataDisplay(friendlyUnitPlats[index].unitData, site);
                }
                break;
            case Faction.Hostility:

                List<UnitPlat> hostitlyUnitPlats = UnitCardSystem.instance.GetHostitlyUnitPlats();

                if (hostitlyUnitPlats.Count == 4 && hostitlyUnitPlats[index].unitData != null &&
                    hostitlyUnitPlats[index].unitData.Skills.Count > 0)
                {
                    UIManager.instance.HostitlyUnitDataDisplay(hostitlyUnitPlats[index].unitData, site);
                }
                break;
        }
    }

    public void OnUIDataUnDisplay(PointerEventData eventData)
    {
        switch (faction)
        {
            case Faction.Friendly:
                UIManager.instance.FriendlyUnitDataUnDisplay();
                break;
            case Faction.Hostility:
                UIManager.instance.HostitlyUnitDataUnDisplay();
                break;
        }
    }
}
