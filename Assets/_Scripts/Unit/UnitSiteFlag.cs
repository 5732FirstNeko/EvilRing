using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UnitSiteFlag : MonoBehaviour, IDropHandler
{
    public Faction faction;
    public UnitSite site;

    public void OnDrop(PointerEventData eventData)
    {
        eventData.pointerDrag.transform.SetParent(transform.parent);
    }
}
