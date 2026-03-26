using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HierarchicalTreeNodeDataSo : ScriptableObject
{
    public int ghostCost;
    public int goldCost;
    public Sprite sprite;

    public TriggerTiming GlobalActionTriggerTiming;
    public Faction TargetFaction;
    public List<UnitSite> Range;

    [TextArea] public string descriptions;

    public virtual void GlobalAction(ICollection<UnitPlat> unitPlats) { }
    public virtual void UnLoackAction() { }
}
