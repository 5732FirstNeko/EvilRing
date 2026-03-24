using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

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
    [SerializeField] private List<UnitPlat> friendlyUnitPlats = new List<UnitPlat>();
    [SerializeField] private List<FriendlyUnitUI> friendlyUnitRefreshArea;

    [Header("Hostitly")]
    public HostilityWaveDataSo hostilityWaveData;
    [SerializeField] private List<UnitPlat> hostitlyUnitPlats;
    [SerializeField] private List<GameObject> hostitlyUnitRefreshArea;


    [HideInInspector] public FriendlyUnitUI currentFriendlyUnitUI 
    {
        get => _friendlyUnitUI;
        set 
        {
            if (_friendlyUnitUI != null)
            {
                _friendlyUnitUI.transform.DOScale(new Vector3(1, 1, 1), 0.25f);
                _friendlyUnitUI.transform.DORotate(new Vector3(0, 0, 0), 0.25f);
            }
            _friendlyUnitUI = value;
            if (_friendlyUnitUI != null)
            {
                _friendlyUnitUI.transform.DOScale(new Vector3(1.1f, 1.1f, 1.1f), 0.25f);
                _friendlyUnitUI.transform.DORotate(new Vector3(0, 0, -15), 0.25f);
            }
        }
    }
    private FriendlyUnitUI _friendlyUnitUI;

    public bool isHaveCardDrag;

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

    public void AddUnitToFriendlyList(UnitDataSo unitData, UnitSite site)
    {
        RefreshFriendlyUnit();

        if (InventoryManager.Instance.gold > unitData.cost)
        {
            int index = BattleSystem.GetIndexByUnitSite(site);
            UnitPlat unitPlat = friendlyUnitPlats[index];
            unitPlat.UnitPlatInit(unitData, site);
            InventoryManager.Instance.gold -= unitData.cost;

            unitPlat.GetComponent<SpriteRenderer>().sprite = unitData.UnitSprite;
            Vector3 originScale = unitPlat.transform.localScale;
            unitPlat.transform.localScale = Vector3.zero;
            unitPlat.transform.DOScale(originScale, 1.5f);
        }
        else
        {
            //TODO : gold is not enougth Tip Logic
        }
    }

    public void RemoveUnitFromFriendlyList(UnitSite site)
    {
        int index = BattleSystem.GetIndexByUnitSite(site);
        InventoryManager.Instance.gold += friendlyUnitPlats[index].unitData.cost;

        friendlyUnitPlats[index].unit = null;
        friendlyUnitPlats[index].unitData = null;
    }

    public List<UnitPlat> GetCurrentFriendlyUnitPlats()
    {
        List<UnitPlat> reslutes = new List<UnitPlat>();
        foreach (var plat in friendlyUnitPlats)
        {
            if (plat.unitData == null)
            {
                plat.UnitPlatInit(FactorySystem.instance.EmptyFriendlyUnitData, plat.site);
                reslutes.Add(plat);
            }
            else
            {
                reslutes.Add(plat);
            }
        }

        return reslutes;
    }

    public List<UnitPlat> GetCurrentHostitlyUnitPlats()
    {
        return hostitlyUnitPlats;
    }

    private void RefreshFriendlyUnit()
    {
        //TODO : UnitRefresh Logic -- GetUnitDataSo from OtherSystem
        currentFriendlyUnitUI.unitData = null;
        //currentFriendlyUnitUI.GetComponent<Image>().sprite = currentFriendlyUnitUI.unitData.UnitSprite;

        currentFriendlyUnitUI.transform.localScale = new(0, 0, 0);
        currentFriendlyUnitUI.transform.rotation = new(0, 0, 0, 0);

        currentFriendlyUnitUI.transform.DOScale(new Vector3(1, 1, 1), 1.5f).SetEase(Ease.OutBounce).
            OnComplete(() => { currentFriendlyUnitUI = null; });
    }

    public void RefreshHostitlyUnit()
    {
        hostilityWaveData = FactorySystem.instance.GetHostitlyWaveDataByGhost(InventoryManager.Instance.ghost);

        for (int i = 0; i < 4; i++)
        {
            hostitlyUnitPlats[i].UnitPlatInit(hostilityWaveData.hostilityDataList[i], 
                hostitlyUnitPlats[i].site);
        }
    }
}
