#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestSystem : MonoBehaviour
{
    [SerializeField] private List<UnitPlat> friendlyUnits = new List<UnitPlat>();
    [SerializeField] private List<UnitPlat> hostitlyUnits = new List<UnitPlat>();

    [SerializeField] private UnitSkillDataSo unitskill;

    [SerializeField] private ItemDataSO ItemDataSO;

    

    private void Start()
    {
        InventoryManager.instance.AddInventoryToList(ItemDataSO);

        //foreach (var friendly in friendlyUnits)
        //{
        //    friendly.UnitPlatInit(friendly.unitData, friendly.site);
        //    friendly.unit.HP -= Mathf.RoundToInt(0.75f * friendly.unit.MaxHP);
        //}

        //foreach (var friendly in hostitlyUnits)
        //{
        //    friendly.UnitPlatInit(friendly.unitData, friendly.site);
        //    friendly.unit.HP -= Mathf.RoundToInt(0.75f * friendly.unit.MaxHP);
        //}

        //unitskill.GameStartInit();
        //friendlyUnits[0].costumvalue_first = 8;
    }

    public void UnitSkillTest()
    {
        unitskill.Action(hostitlyUnits, friendlyUnits[0]);
    }
}
#endif