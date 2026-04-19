using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FactorySystem : MonoBehaviour
{
    public static FactorySystem Instance;
    public static FactorySystem instance
    {
        get
        {
            if (Instance == null)
            {
                GameObject Object = new GameObject(typeof(FactorySystem).Name);
                Instance = Object.AddComponent<FactorySystem>();
                DontDestroyOnLoad(Object);
            }
            return Instance;
        }
    }

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

    public UnitDataSo EmptyFriendlyUnitData;
    public UnitDataSo EmptyHostitlyUnitData;

    public List<HostilityWaveDataSo> hostilityWaveDataList = new List<HostilityWaveDataSo>();
    public List<UnitDataSo> friendlyUnitDataList = new List<UnitDataSo>();

    public List<UnitDataSo> LycorisCards;
    public List<UnitDataSo> wizardCards;
    public List<UnitDataSo> swordmanCards;
    public List<UnitDataSo> warlockCards;
    public List<UnitDataSo> shiledCards;
    public List<UnitDataSo> knightCards;

    public List<UnitDataSo> GoblineCards;
    public List<UnitDataSo> GuardsCards;

    public List<ItemDataSO> items;

    public HostilityWaveDataSo GetHostitlyWaveDataByGhost(int ghostCount)
    {
        foreach (var waveData in hostilityWaveDataList)
        {
            if (ghostCount * 0.9f <= waveData.ghostCost && waveData.ghostCost <= ghostCount * 1.1f)
            {
                return waveData;
            }
        }
        return null;
    }

    private void Start()
    {
        
    }

    public UnitDataSo GetFriendlyUnitData()
    {
        int index = Random.Range(0, friendlyUnitDataList.Count);

        return friendlyUnitDataList[index];
    }

    public ItemDataSO GetRandomItem()
    {
        int index = Random.Range(0, items.Count);

        return items[index];
    }
}
