using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class HierarchicalTreeSystem : MonoBehaviour
{
    public static HierarchicalTreeSystem instance { get; private set; }

    public List<HierarchicalTreeNode> HierarchicalTrees = new List<HierarchicalTreeNode>();

    public HierarchicalTreeNode currentSelectNode 
    {
        get => _SelectNode;
        set 
        {
            if (_SelectNode != null)
            {
                _SelectNode.SelectStateChange();
            }
            _SelectNode = value;
            if (_SelectNode != null)
            {
                _SelectNode.SelectStateChange();
            }
        }
    }
    private HierarchicalTreeNode _SelectNode;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        
    }

    public void HierarchicalTreeLockStateUpdate()
    {
        foreach (var node in HierarchicalTrees)
        {
            if (!node.isLocked) continue;

            bool isprecondDitionCompleted = true;
            foreach (var precondition in node.preconditionNodes)
            {
                if (precondition.isLocked)
                {
                    isprecondDitionCompleted = false;
                    break;
                }
            }

            if (isprecondDitionCompleted)
            {
                node.lockImage.DOColor(new Color(1, 1, 1, 0), 1.5f).SetEase(Ease.OutQuart);
            }
        }
    }
}
