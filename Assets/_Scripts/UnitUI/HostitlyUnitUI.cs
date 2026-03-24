using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class HostitlyUnitUI : MonoBehaviour, IPointerClickHandler
{
    public UnitDataSo unitData;

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            if (UIManager.instance.hostitlyUnitDataUIObjectLeft.activeSelf)
            {
                UIManager.instance.HostitlyUnitDataUnDisplay();
            }
            else
            {
                UIManager.instance.HostitlyUnitDataDisplay(unitData);
            }
        }
    }
}
