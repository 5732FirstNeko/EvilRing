using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class HierarchicalTreeNode : MonoBehaviour, IPointerClickHandler
{
    public HierarchicalTreeNodeDataSo hierarchicalTreeNodeData;

    public bool isLocked;

    public int ghostCost;
    public int goldCost;
    public List<HierarchicalTreeNode> preconditionNodes = new List<HierarchicalTreeNode>();

    public TriggerTiming GlobalActionTriggerTiming;
    public Faction TargetFaction;
    public List<UnitSite> Range;

    public Image lockImage;
    [SerializeField] private Image SelectBorder;
    [SerializeField] private Image imageMask;

    public Action<ICollection<UnitPlat>> OnGlobalAction;
    public Action OnUnLocalAction;

    private void Start()
    {
        //TODO : HierarchicalNode Data Init Code 
        //NodeInit();
    }

    public void NodeInit()
    {
        isLocked = true;
        ghostCost = hierarchicalTreeNodeData.ghostCost;
        goldCost = hierarchicalTreeNodeData.goldCost;
        GetComponent<Image>().sprite = hierarchicalTreeNodeData.sprite;

        GlobalActionTriggerTiming = hierarchicalTreeNodeData.GlobalActionTriggerTiming;
        TargetFaction = hierarchicalTreeNodeData.TargetFaction;
        Range = hierarchicalTreeNodeData.Range;

        OnGlobalAction += hierarchicalTreeNodeData.GlobalAction;
        OnUnLocalAction += hierarchicalTreeNodeData.UnLoackAction;

        HierarchicalTreeSystem.instance.HierarchicalTrees.Add(this);
    }

    public void GlobalAction(ICollection<UnitPlat> unitPlats)
    {
        OnGlobalAction?.Invoke(unitPlats);
    }

    public void UnLockAction()
    {
        imageMask.DOKill(true);
        imageMask.DOColor(new Color(imageMask.color.r, imageMask.color.g,imageMask.color.b, 0),1.5f).SetEase(Ease.InQuart).
            OnComplete(HierarchicalTreeSystem.instance.HierarchicalTreeLockStateUpdate);
        OnUnLocalAction?.Invoke();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        HierarchicalTreeSystem.instance.currentSelectNode = HierarchicalTreeSystem.instance.currentSelectNode 
            == this ? null : this;
    }

    public void SelectStateChange()
    {
        if (SelectBorder.gameObject.activeSelf)
        {
            SelectBorder.gameObject.SetActive(false);
            UIManager.instance.HierarchicalTreeNodeUIUnDisplay();
        }
        else
        {
            SelectBorder.gameObject.SetActive(true);
            UIManager.instance.HierarchicalTreeNodeUIDisplay(this);
        }
    }
}
