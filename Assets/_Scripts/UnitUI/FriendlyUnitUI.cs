using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class FriendlyUnitUI : MonoBehaviour, IPointerClickHandler,
    IDragHandler,IBeginDragHandler,IEndDragHandler
{
    public UnitDataSo unitData;
    public Image Image;

    private void Start()
    {
        Image = GetComponent<Image>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        UIManager.instance.FriendlyUnitDataUnDisplay();
        UnitCardSystem.instance.currentFriendlyUnitUI = this;
        UnitCardSystem.instance.isHaveCardDrag = true;

        Image.color = new Color(Image.color.r, Image.color.g, Image.color.b, 0.75f);
        UIManager.instance.friendlyUnitInstance.GetComponent<Image>().sprite = unitData.UnitSprite;
        UIManager.instance.friendlyUnitInstance.gameObject.SetActive(true);
    }

    public void OnDrag(PointerEventData eventData)
    {
        UIManager.instance.friendlyUnitInstance.transform.position = Input.mousePosition;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        UIManager.instance.friendlyUnitInstance.gameObject.SetActive(false);
        UnitCardSystem.instance.isHaveCardDrag = false;

        Image.color = new Color(Image.color.r, Image.color.g, Image.color.b, 1f);

        if (eventData.pointerEnter != null && eventData.pointerEnter.GetComponent<UnitSiteFlag>() != null)
        {
            UnitCardSystem.instance.AddUnitToFriendlyList(unitData,
                   eventData.pointerEnter.GetComponent<UnitSiteFlag>().site);
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            if (UIManager.instance.friendlyUnitDataUIRightRect.gameObject.activeSelf)
            {
                UIManager.instance.FriendlyUnitDataUnDisplay();
            }
            else
            {
                UIManager.instance.FriendlyUnitDataDisplay(unitData);
            }

            return;
        }
        else if (eventData.button == PointerEventData.InputButton.Left)
        {
            UIManager.instance.FriendlyUnitDataUnDisplay();
            if (UnitCardSystem.instance.currentFriendlyUnitUI == this)
            {
                UnitCardSystem.instance.currentFriendlyUnitUI = null;
                return;
            }

            UnitCardSystem.instance.currentFriendlyUnitUI = this;
        }
    }

    public void UnSelectAnimation()
    {
        transform.DOScale(new Vector3(1, 1, 1), 0.25f);
        transform.DORotate(new Vector3(0, 0, 0), 0.25f);
    }

    public void SelectAnimation()
    {
        transform.DOScale(new Vector3(1.1f, 1.1f, 1.1f), 0.25f);
        transform.DORotate(new Vector3(0, 0, -15), 0.25f);
    }
}
