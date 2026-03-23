using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitCardSystem : MonoBehaviour
{
    public static UnitCardSystem Instance;
    public static UnitCardSystem instance
    {
        get
        {
            if (Instance == null)
            {
                GameObject Object = new GameObject(typeof(UnitCardSystem).Name);
                Instance = Object.AddComponent<UnitCardSystem>();
                DontDestroyOnLoad(Object);
            }
            return Instance;
        }
    }

    [Header("Friendly")]
    //to Creat 4 Plat instance, the value will be optimized in future
    [SerializeField] private List<UnitPlat> friendlyUnitPlats;

    [SerializeField] private List<FriendlyUnitUI> friendlyUnitReFreshArea;

    [Header("Hostitly")]
    //to Creat 4 Plat instance, the value will be optimized in future
    [SerializeField] private List<UnitPlat> hostitlyUnitPlats;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }

    public void AddOneUnitToFriendlyList(UnitPlat unitPlat,UnitSite site)
    {
        unitPlat.site = site;
        friendlyUnitPlats.Add(unitPlat);
    }

    public List<UnitDataSo> RefreshFriendlyUnit()
    {
        List<UnitDataSo> unitPlats = new List<UnitDataSo>(friendlyUnitPlats.Count);

        return unitPlats;
    }
}
