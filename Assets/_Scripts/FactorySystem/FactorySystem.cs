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
    public List<UnitDataSo> lowLevelCards = new List<UnitDataSo>();
    public List<UnitDataSo> heighlevelCards = new List<UnitDataSo>();
    public List<UnitDataSo> friendlyUnitDataList = new List<UnitDataSo>();

    public int cardLevel = 0;
    private int freshlowCardCount;
    [SerializeField] private int freshCardCount = 20;

    public List<UnitDataSo> LycorisCards;
    public List<UnitDataSo> wizardCards;
    public List<UnitDataSo> swordmanCards;
    public List<UnitDataSo> warlockCards;
    public List<UnitDataSo> shiledCards;
    public List<UnitDataSo> knightCards;

    public List<UnitDataSo> WitchCards;
    public List<UnitDataSo> SlimeCards;
    public List<UnitDataSo> ThiefCards;
    public List<UnitDataSo> GoblineCards;
    public List<UnitDataSo> GuardsCards;
    public List<UnitDataSo> NecroCards;
    public List<UnitDataSo> ElementCards;
    public List<UnitDataSo> FinalBoss;

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

    public UnitDataSo GetFriendlyUnitData()
    {
        int midValue = 100;

        switch (cardLevel)
        {
            case 0:
                midValue = 97;
                break;
            case 1:
                midValue = 95;
                break;
            case 2:
                midValue = 90;
                break;
            case 3:
                midValue = 80;
                break;
            case 4:
                midValue = 85;
                break;
        }

        int value = Random.Range(0, 101);

        if (value < midValue)
        {
            freshlowCardCount += cardLevel >= 2 ? 1 : 0;

            if (freshlowCardCount >= freshCardCount && cardLevel >= 2)
            {
                int heightCardIndex = Random.Range(0, heighlevelCards.Count);
                freshlowCardCount = 0;

                return heighlevelCards[heightCardIndex];
            }

            int index = Random.Range(0, lowLevelCards.Count);
            return lowLevelCards[index];
        }
        else
        {
            int index = Random.Range(0, heighlevelCards.Count);

            freshlowCardCount = 0;
            return heighlevelCards[index];
        }
    }

    public ItemDataSO GetRandomItem()
    {
        int index = Random.Range(0, items.Count);

        return items[index];
    }

    public void UnLockCardToList(ICollection<UnitDataSo> cards, bool islowCard = false)
    {
        foreach (var unit in cards)
        {
            if (islowCard)
            {
                lowLevelCards.Add(unit);
            }
            else
            {
                heighlevelCards.Add(unit);
            }

            friendlyUnitDataList.Add(unit);
        }
    }
}
