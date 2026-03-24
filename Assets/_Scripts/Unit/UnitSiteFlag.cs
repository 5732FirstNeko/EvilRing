using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class UnitSiteFlag : MonoBehaviour, IDropHandler, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    public Faction faction;
    public UnitSite site;

    public void OnDrop(PointerEventData eventData)
    {
        
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left &&
            UnitCardSystem.instance.currentFriendlyUnitUI != null)
        {
            UnitCardSystem.instance.AddUnitToFriendlyList(UnitCardSystem.instance.currentFriendlyUnitUI.unitData,
                                    eventData.pointerEnter.GetComponent<UnitSiteFlag>().site);
        }
        else if(eventData.button == PointerEventData.InputButton.Right)
        {
            UnitCardSystem.instance.RemoveUnitFromFriendlyList(site);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (UnitCardSystem.instance.isHaveCardDrag)
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

                List<UnitPlat> hostitlyUnitPlats = UnitCardSystem.instance.GetCurrentHostitlyUnitPlats();

                if (hostitlyUnitPlats.Count == 4 && hostitlyUnitPlats[index].unitData != null &&
                    hostitlyUnitPlats[index].unitData.Skills.Count > 0)
                {
                    UIManager.instance.HostitlyUnitDataDisplay(hostitlyUnitPlats[index].unitData, site);
                }
                break;
        }
        
    }

    public void OnPointerExit(PointerEventData eventData)
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
