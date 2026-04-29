using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "HierarchicalNode", 
    menuName = "Data/HierarchicalNode/" + nameof(CardHirearchicalNode))]
public class CardHirearchicalNode : HierarchicalTreeNodeDataSo
{
    [SerializeField] private List<UnitDataSo> unlockCards;

    public override void UnLoackAction()
    {
        base.UnLoackAction();

        FactorySystem.instance.UnLockCardToList(unlockCards);
        FactorySystem.instance.cardLevel++;
    }
}
