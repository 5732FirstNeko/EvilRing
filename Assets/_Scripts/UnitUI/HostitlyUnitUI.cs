using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class HostitlyUnitUI : MonoBehaviour, IPointerClickHandler
{
    public UnitDataSo unitData;
    public Image image;

    private void Start()
    {
        image = GetComponentsInChildren<Image>()[1];
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            if (UIManager.instance.hostitlyUnitDataUILeftRect.gameObject.activeSelf)
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
