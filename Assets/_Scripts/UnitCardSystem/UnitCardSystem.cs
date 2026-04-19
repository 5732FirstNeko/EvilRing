using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Unity.VisualScripting;
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

    [SerializeField] private Sprite defaultCardSprite;
    [Header("Friendly")]
    public List<UnitPlat> friendlyUnitPlats = new List<UnitPlat>();
    public List<FriendlyUnitUI> friendlyUnitRefreshArea;

    public int friendlyCardCount { get => friendlyUnitRefreshArea.Count; }

    [Header("Hostitly")]
    public HostilityWaveDataSo hostilityWaveData;
    public List<UnitPlat> hostitlyUnitPlats;
    [SerializeField] private List<HostitlyUnitUI> hostitlyUnitRefreshArea;

    [SerializeField] private Text ghostCount;
    [HideInInspector] public FriendlyUnitUI currentFriendlyUnitUI 
    {
        get => _friendlyUnitUI;
        set 
        {
            if (_friendlyUnitUI != null)
            {
                _friendlyUnitUI.UnSelectAnimation();
            }
            _friendlyUnitUI = value;
            if (_friendlyUnitUI != null)
            {
                _friendlyUnitUI.SelectAnimation();
                InventoryManager.instance.currentSelectInventory = null;
            }
        }
    }
    private FriendlyUnitUI _friendlyUnitUI;

    public bool isHaveCardDrag;

    [SerializeField] private RectTransform contentRect;

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
        if (InventoryManager.Instance.gold > unitData.cost)
        {
            int index = BattleSystem.GetIndexByUnitSite(site);
            UnitPlat unitPlat = friendlyUnitPlats[index];
            unitPlat.UnitPlatInit(unitData, site);
            InventoryManager.Instance.gold -= unitData.cost;

            friendlyUnitPlats[index].unitData = unitData;

            unitPlat.iconSpriteRender.sprite = unitData.UnitSprite;
            Vector3 originScale = unitPlat.transform.localScale;
            unitPlat.transform.localScale = Vector3.zero;
            unitPlat.transform.DOScale(originScale, 1.5f);

            currentFriendlyUnitUI.unitData = null;
            currentFriendlyUnitUI.Image.sprite = defaultCardSprite;
            currentFriendlyUnitUI = null;
            FriendlyCardAreaAdjust();
        }
        else
        {
            UIManager.instance.UnitCardGoldNonEnougthTip();
        }
    }

    public void RemoveUnitFromFriendlyList(UnitSite site)
    {
        int index = BattleSystem.GetIndexByUnitSite(site);
        if (friendlyUnitPlats[index] == null || friendlyUnitPlats[index].unitData == null) return;
        
        InventoryManager.instance.gold += friendlyUnitPlats[index].unitData.cost;

        friendlyUnitPlats[index].unit = null;
        friendlyUnitPlats[index].unitData = null;
        friendlyUnitPlats[index].iconSpriteRender.sprite = defaultCardSprite;
    }

    public List<UnitPlat> GetCurrentFriendlyUnitPlats()
    {
        return friendlyUnitPlats;
    }

    public List<UnitPlat> GetFinalFriendlyUnitPlats()
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

    public List<UnitPlat> GetHostitlyUnitPlats()
    {
        return hostitlyUnitPlats;
    }

    public List<UnitPlat> GetFinalHostitlyUnitPlats()
    {
        for (int i = 0; i < hostitlyUnitPlats.Count; i++)
        {
            hostitlyUnitPlats[i].UnitPlatInit(
                hostitlyUnitPlats[i].unitData, hostitlyUnitPlats[i].site);
        }

        return hostitlyUnitPlats;
    }

    public void RefreshRandomFriendlyUnit()
    {
        int index = Random.Range(0, friendlyUnitRefreshArea.Count);

        if (friendlyUnitRefreshArea[index].unitData == null)
        {
            for (int i = 0; i < friendlyUnitRefreshArea.Count; i++)
            {
                if (friendlyUnitRefreshArea[i].unitData != null)
                {
                    index = i;
                    break;
                }
            }
        }

        friendlyUnitRefreshArea[index].unitData = null;

        RefreshFriendlyUnit();
        FriendlyCardAreaAdjust();
    }

    public void DestoryOneFriendlyUnit()
    {
        int index = Random.Range(0, friendlyUnitRefreshArea.Count);

        if (friendlyUnitRefreshArea[index].unitData == null)
        {
            for (int i = 0; i < friendlyUnitRefreshArea.Count; i++)
            {
                if (friendlyUnitRefreshArea[i].unitData != null)
                {
                    index = i;
                    break;
                }
            }
        }

        friendlyUnitRefreshArea[index].transform.SetAsLastSibling();

        friendlyUnitRefreshArea[index].unitData = null;
        Color color = friendlyUnitRefreshArea[index].Image.color;
        friendlyUnitRefreshArea[index].Image.color = new Color(color.r, color.g, color.b, 0);
        friendlyUnitRefreshArea[index].Image.sprite = defaultCardSprite;
        friendlyUnitRefreshArea[index].Image.DOColor(color, 0.5f);

        FriendlyCardAreaAdjust();
    }

    public void RefreshFriendlyUnit()
    {
        FriendlyUnitUI activeFriendlyUnitUI = null;
        foreach (var unit in friendlyUnitRefreshArea)
        {
            if (unit.unitData == null)
            {
                activeFriendlyUnitUI = unit;
                break;
            }
        }

        if (activeFriendlyUnitUI == null) return;

        activeFriendlyUnitUI.unitData = FactorySystem.instance.GetFriendlyUnitData();
        activeFriendlyUnitUI.Image.sprite = activeFriendlyUnitUI.unitData.UnitSprite;
        activeFriendlyUnitUI.isLock = false;

        activeFriendlyUnitUI.transform.localScale = new(0, 0, 0);
        activeFriendlyUnitUI.transform.rotation = new(0, 0, 0, 0);
        activeFriendlyUnitUI.Image.color = new Color(activeFriendlyUnitUI.Image.color.r,
            activeFriendlyUnitUI.Image.color.g, activeFriendlyUnitUI.Image.color.b, 0);

        activeFriendlyUnitUI.transform.DOScale(new Vector3(1, 1, 1), 1.5f).SetEase(Ease.Linear);
        activeFriendlyUnitUI.Image.DOFade(1,1.5f).SetEase(Ease.OutQuad);

        FriendlyCardAreaAdjust();
    }

    public void RefreshFriendlyUnit(UnitDataSo unitData, int index)
    {
        FriendlyUnitUI activeFriendlyUnitUI = friendlyUnitRefreshArea[index];

        if (activeFriendlyUnitUI == null || unitData == null) return;

        activeFriendlyUnitUI.unitData = FactorySystem.instance.GetFriendlyUnitData();
        activeFriendlyUnitUI.Image.sprite = activeFriendlyUnitUI.unitData.UnitSprite;
        activeFriendlyUnitUI.isLock = false;

        activeFriendlyUnitUI.transform.localScale = new(0, 0, 0);
        activeFriendlyUnitUI.transform.rotation = new(0, 0, 0, 0);
        activeFriendlyUnitUI.Image.color = new Color(activeFriendlyUnitUI.Image.color.r,
            activeFriendlyUnitUI.Image.color.g, activeFriendlyUnitUI.Image.color.b, 0);

        activeFriendlyUnitUI.transform.DOScale(new Vector3(1, 1, 1), 1.5f).SetEase(Ease.OutBounce);
        activeFriendlyUnitUI.Image.DOFade(1, 1.5f).SetEase(Ease.OutQuad);

        FriendlyCardAreaAdjust();
    }

    public void RefreshAllFriendlyUnit()
    {
        foreach (var friendlyUnitUI in friendlyUnitRefreshArea)
        {
            friendlyUnitUI.unitData = FactorySystem.instance.GetFriendlyUnitData();
            friendlyUnitUI.Image.sprite = friendlyUnitUI.unitData.UnitSprite;
            friendlyUnitUI.isLock = false;

            friendlyUnitUI.transform.localScale = new(0, 0, 0);
            friendlyUnitUI.transform.rotation = new(0, 0, 0, 0);
            friendlyUnitUI.Image.color = new Color(friendlyUnitUI.Image.color.r,
                friendlyUnitUI.Image.color.g, friendlyUnitUI.Image.color.b, 0);

            friendlyUnitUI.transform.DOScale(new Vector3(1, 1, 1), 1.5f).SetEase(Ease.OutBounce);
            friendlyUnitUI.Image.DOFade(1, 1.5f).SetEase(Ease.OutQuad);
        }

        FriendlyCardAreaAdjust();
    }

    public void RefreshHostitlyUnit()
    {
        hostilityWaveData = FactorySystem.instance.GetHostitlyWaveDataByGhost(InventoryManager.Instance.ghostTotal);

        ghostCount.text = "Áé»êÊý : " + hostilityWaveData.ghostCost;
        for (int i = 0; i < hostitlyUnitRefreshArea.Count; i++)
        {
            HostitlyUnitUI hostitlyUnitUI = hostitlyUnitRefreshArea[i];

            hostitlyUnitUI.unitData = hostilityWaveData.hostilityDataList[i];
            hostitlyUnitUI.image.sprite = hostilityWaveData.hostilityDataList[i].UnitSprite;
            hostitlyUnitUI.image.color = new Color(hostitlyUnitUI.image.color.r,
            hostitlyUnitUI.image.color.g, hostitlyUnitUI.image.color.b, 0);

            hostitlyUnitUI.image.DOFade(1, 1.5f).SetEase(Ease.OutQuad);

            UnitPlat hostitlyUnitplat = hostitlyUnitPlats[i];
            hostitlyUnitplat.UnitPlatInit(hostilityWaveData.hostilityDataList[i],
                hostitlyUnitplat.site);

            if (hostilityWaveData.hostilityDataList[i] == FactorySystem.instance.EmptyHostitlyUnitData)
            {
                hostitlyUnitRefreshArea[i].image.sprite = defaultCardSprite;
                continue;
            }

            hostitlyUnitRefreshArea[i].image.gameObject.SetActive(true);
            hostitlyUnitplat.iconSpriteRender.sprite = hostilityWaveData.hostilityDataList[i].UnitSprite;
            Vector3 originScale = hostitlyUnitplat.transform.localScale;
            hostitlyUnitplat.transform.localScale = Vector3.zero;
            hostitlyUnitplat.transform.DOScale(originScale, 1.5f);
        }
    }

    public void FriendlyCardAreaAdjust()
    {
        int displayCount = 0;
        foreach (var area in friendlyUnitRefreshArea)
        {
            if (area.unitData == null || area.Image.sprite == defaultCardSprite)
            {
                area.transform.SetAsLastSibling();
            }
            else
            {
                displayCount++;
            }
        }

        float targetRight = 750 - displayCount * 187.5f;

        Vector2 pos = contentRect.anchoredPosition;
        pos.x = contentRect.rect.width - targetRight;
        contentRect.anchoredPosition = pos;
    }

    public void RecoverDefaultHostitlyUnitSprite()
    {
        foreach (var unit in hostitlyUnitPlats)
        {
            unit.iconSpriteRender.sprite = defaultCardSprite;
        }
    }

    public ICollection<UnitPlat> GetActionPlats(Faction faction, ICollection<UnitSite> range)
    {
        List<UnitPlat> reslutes = new List<UnitPlat>();
        switch (faction)
        {
            case Faction.Friendly:
                foreach (var ran in range)
                {
                    int index = BattleSystem.GetIndexByUnitSite(ran);
                    reslutes.Add(friendlyUnitPlats[index]);
                }
                break;
            case Faction.Hostility:
                foreach (var ran in range)
                {
                    int index = BattleSystem.GetIndexByUnitSite(ran);
                    reslutes.Add(friendlyUnitPlats[index]);
                }
                break;
        }

        return reslutes;
    }
}
