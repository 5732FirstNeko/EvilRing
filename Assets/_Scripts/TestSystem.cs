#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestSystem : MonoBehaviour
{
    [SerializeField] private List<Unit> friendlyUnits = new List<Unit>();
    [SerializeField] private List<Unit> hostitlyUnits = new List<Unit>();

    private bool flag = false;
    void Update()
    {
        if (!flag)
        {
            flag = true;
            BattleSystem.instance.BattleInit(hostitlyUnits, friendlyUnits);
        }
    }
}
#endif