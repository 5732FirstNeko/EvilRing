using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

public class FriendlyUnitUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IDragHandler,IBeginDragHandler,IEndDragHandler
{
    public UnitPlat unitPlat;

    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;

    private Transform originParent;
    private Vector2 originPosition;

    private void Start()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        UIManager.instance.UnitDataUnDisPlay();
        canvasGroup.blocksRaycasts = false;
        originPosition = rectTransform.anchoredPosition;
        originParent = transform.parent;
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = Input.mousePosition;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        Debug.Log(eventData.pointerEnter);
        if (transform.parent != originParent && eventData.pointerEnter == null)
        {
            rectTransform.anchoredPosition = originPosition;
        }
        else if (transform.parent == originParent && eventData.pointerEnter == null)
        {
            rectTransform.anchoredPosition = originPosition;
            transform.SetParent(originParent);
        }
        else
        {
            UnitCardSystem.instance.AddOneUnitToFriendlyList(unitPlat,
                    eventData.pointerEnter.GetComponent<UnitSiteFlag>().site);
        }

        canvasGroup.blocksRaycasts = true;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        //UIManager.instance.UnitDataDisplay(user.unit);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        //UIManager.instance.UnitDataUnDisPlay();
    }
}
