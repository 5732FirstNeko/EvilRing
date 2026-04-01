using System.Collections;
using System.Collections.Generic;
using System.Text;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;
    public static UIManager instance
    {
        get
        {
            if (Instance == null)
            {
                GameObject Object = new GameObject(typeof(UIManager).Name);
                Instance = Object.AddComponent<UIManager>();
                DontDestroyOnLoad(Object);
            }
            return Instance;
        }
    }

    public Button battleButton;
    #region UnitCardUIValue
    [Header("UnitCardSystem")]
    public RectTransform friendlyUnitDataUIRightRect;
    public RectTransform friendlyUnitDataUILeftRect;

    [SerializeField] private Text friendlyCostTextRight;
    [SerializeField] private Text friendlyCostTextLeft;
    [SerializeField] private Text friendlySpeedTextRight;
    [SerializeField] private Text friendlySpeedTextLeft;
    [SerializeField] private List<Text> friendlySkillTextsRight;
    [SerializeField] private List<Text> friendlySkillTextsLeft;

    public RectTransform hostitlyUnitDataUILeftRect;
    public RectTransform hostitlyUnitDataUIRightRect;
    [SerializeField] private Text hostitlySpeedTextLeft;
    [SerializeField] private Text hostitlySpeedTextRight;
    [SerializeField] private List<Text> hostitlySkillTextsLeft;
    [SerializeField] private List<Text> hostitlySkillTextsRight;

    [SerializeField] private float UnitDataUIHeight;

    public RectTransform goldnonenougthRect;
    public RectTransform cardnonenougthRect;

    public Image friendlyUnitInstance;
    [SerializeField] private RectMask2D mask;
    #endregion

    #region HierarchTreeValue
    [Header("HierarchicalTree")]
    [SerializeField] private GameObject hierarchicalTreeObject;
    [SerializeField] private Button hierarchicalTreeButton;

    [SerializeField] private RectTransform hierarchicalTreeNodeUIRect;

    [SerializeField] private Image hierarchicalTreeNodeIcon;
    [SerializeField] private Text goldCost;
    [SerializeField] private Text ghostCost;
    [SerializeField] private Text hierarchicalTreeNodeDescriptions;
    [SerializeField] private Button hierarchicalUnLockButton;

    [SerializeField] private RectTransform ghostnonenougth;
    private float HierarchTreeNodeUIHeight;
    #endregion

    #region InventorySystemValue
    [Header("Inventory")]
    [SerializeField] private RectTransform InventoryDataUIRect;
    [SerializeField] private Image InventoryIcon;
    [SerializeField] private Text InventoryText;
    [SerializeField] private Button InventoryUseButton;

    public Text goldText;
    public Text ghostText;
    public Text ghostTotalText;
    private float InventoryRightDistance;
    #endregion

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

        friendlyUnitInstance.gameObject.SetActive(false);
    }

    private void Start()
    {
        battleButton.transform.localScale = Vector3.zero;
        battleButton.gameObject.SetActive(false);

        hierarchicalTreeButton.onClick.AddListener(HierarchicalTreeUIChange);

        UnitDataUIHeight = friendlyUnitDataUIRightRect.anchoredPosition.y;
        friendlyUnitDataUIRightRect.anchoredPosition 
            = new Vector2(friendlyUnitDataUIRightRect.anchoredPosition.x,0);
        friendlyUnitDataUIRightRect.transform.localScale = Vector3.zero;
        friendlyUnitDataUILeftRect.anchoredPosition 
            = new Vector2(friendlyUnitDataUILeftRect.anchoredPosition.x, 0);
        friendlyUnitDataUILeftRect.transform.localScale = Vector3.zero;
        hostitlyUnitDataUILeftRect.anchoredPosition
            = new Vector2(hostitlyUnitDataUILeftRect.anchoredPosition.x, 0);
        hostitlyUnitDataUILeftRect.transform.localScale = Vector3.zero;
        hostitlyUnitDataUIRightRect.anchoredPosition
            = new Vector2(hostitlyUnitDataUIRightRect.anchoredPosition.x, 0);
        hostitlyUnitDataUIRightRect.transform.localScale = Vector3.zero;

        HierarchTreeNodeUIHeight = hierarchicalTreeNodeUIRect.anchoredPosition.y;
        hierarchicalTreeNodeUIRect.anchoredPosition
            = new Vector2(hierarchicalTreeNodeUIRect.anchoredPosition.x, 0);
        hierarchicalTreeNodeUIRect.transform.localScale = Vector3.zero;

        InventoryRightDistance = InventoryDataUIRect.anchoredPosition.x;
        InventoryDataUIRect.anchoredPosition = 
            new Vector2(0, InventoryDataUIRect.anchoredPosition.y);
        InventoryDataUIRect.transform.localScale = Vector3.zero;
    }

    #region UnitDataDisPlayFunction
    public void FriendlyUnitDataDisplay(UnitDataSo unitData, UnitSite site)
    {
        if (site == UnitSite.first || site == UnitSite.second)
        {
            friendlyUnitDataUIRightRect.DOKill();
            friendlyUnitDataUIRightRect.gameObject.SetActive(true);

            UIDisPlayFunction(friendlyUnitDataUIRightRect, UnitDataUIHeight);

            friendlyCostTextRight.text = InventoryManager.Instance.gold >= unitData.cost ?
                new StringBuilder( "GoldCost : " + "<color=green>" + unitData.cost + "</color>").ToString() : 
                new StringBuilder( "GoldCost : " + "<color=red>" + unitData.cost + "</color>").ToString();
            friendlySpeedTextRight.text = "Speed : " + unitData.Speed;
            for (int i = 0; i < friendlySkillTextsRight.Count; i++)
            {
                friendlySkillTextsRight[i].gameObject.SetActive(true);
                friendlySkillTextsRight[i].text = unitData.Skills[i].description;
            }
        }
        else if(site == UnitSite.third || site == UnitSite.fourth)
        {
            friendlyUnitDataUILeftRect.DOKill();
            friendlyUnitDataUILeftRect.gameObject.SetActive(true);

            UIDisPlayFunction(friendlyUnitDataUILeftRect, UnitDataUIHeight);

            friendlyCostTextLeft.text = InventoryManager.Instance.gold >= unitData.cost ?
                new StringBuilder("GoldCost : " + "<color=green>" + unitData.cost + "</color>").ToString() :
                new StringBuilder("GoldCost : " + "<color=red>" + unitData.cost + "</color>").ToString();
            friendlySpeedTextLeft.text = "Speed : " + unitData.Speed;
            for (int i = 0; i < unitData.Skills.Count; i++)
            {
                friendlySkillTextsLeft[i].gameObject.SetActive(true);
                friendlySkillTextsLeft[i].text = unitData.Skills[i].description;
            }
        }
    }

    public void FriendlyUnitDataDisplay(UnitDataSo unitData)
    {
        friendlyUnitDataUIRightRect.gameObject.SetActive(true);

        UIDisPlayFunction(friendlyUnitDataUIRightRect, UnitDataUIHeight);

        friendlyCostTextRight.text = InventoryManager.Instance.gold >= unitData.cost ?
                new StringBuilder("GoldCost : " + "<color=green>" + unitData.cost + "</color>").ToString() :
                new StringBuilder("GoldCost : " + "<color=red>" + unitData.cost + "</color>").ToString();
        friendlySpeedTextRight.text = "Speed : " + unitData.Speed;
        Debug.Log(unitData.Skills.Count);
        for (int i = 0; i < unitData.Skills.Count; i++)
        {
            friendlySkillTextsRight[i].gameObject.SetActive(true);
            friendlySkillTextsRight[i].text = unitData.Skills[i].description;
        }
    }

    public void FriendlyUnitDataUnDisplay()
    {
        UIUnDisPlayFunction(friendlyUnitDataUIRightRect);

        for (int i = 0; i < friendlySkillTextsRight.Count; i++)
        {
            friendlySkillTextsRight[i].gameObject.SetActive(false);
        }

        UIUnDisPlayFunction(friendlyUnitDataUILeftRect);

        for (int i = 0; i < friendlySkillTextsLeft.Count; i++)
        {
            friendlySkillTextsLeft[i].gameObject.SetActive(false);
        }
    }

    public void HostitlyUnitDataDisplay(UnitDataSo unitData, UnitSite site)
    {
        if (site == UnitSite.first || site == UnitSite.second)
        {
            hostitlyUnitDataUILeftRect.DOKill();
            hostitlyUnitDataUILeftRect.gameObject.SetActive(true);

            UIDisPlayFunction(hostitlyUnitDataUILeftRect, UnitDataUIHeight);
            hostitlySpeedTextLeft.text = "Speed : " + unitData.Speed;
            for (int i = 0; i < unitData.Skills.Count; i++)
            {
                hostitlySkillTextsLeft[i].gameObject.SetActive(true);
                hostitlySkillTextsLeft[i].text = unitData.Skills[i].description;
            }
        }
        else if (site == UnitSite.third || site == UnitSite.fourth)
        {
            hostitlyUnitDataUIRightRect.DOKill();
            hostitlyUnitDataUIRightRect.gameObject.SetActive(true);

            UIDisPlayFunction(hostitlyUnitDataUIRightRect, UnitDataUIHeight);
            hostitlySpeedTextRight.text = "Speed : " + unitData.Speed;
            for (int i = 0; i < unitData.Skills.Count; i++)
            {
                hostitlySkillTextsRight[i].gameObject.SetActive(true);
                hostitlySkillTextsRight[i].text = unitData.Skills[i].description;
            }
        }
    }

    public void HostitlyUnitDataDisplay(UnitDataSo unitData)
    {
        hostitlyUnitDataUILeftRect.gameObject.SetActive(true);

        UIDisPlayFunction(hostitlyUnitDataUILeftRect, UnitDataUIHeight);
        hostitlySpeedTextLeft.text = "Speed : " + unitData.Speed;
        for (int i = 0; i < unitData.Skills.Count; i++)
        {
            hostitlySkillTextsLeft[i].gameObject.SetActive(true);
            hostitlySkillTextsLeft[i].text = unitData.Skills[i].description;
        }
    }

    public void HostitlyUnitDataUnDisplay()
    {
        UIUnDisPlayFunction(hostitlyUnitDataUILeftRect);
        for (int i = 0; i < hostitlySkillTextsLeft.Count; i++)
        {
            hostitlySkillTextsLeft[i].gameObject.SetActive(false);
        }

        UIUnDisPlayFunction(hostitlyUnitDataUIRightRect);
        for (int i = 0; i < hostitlySkillTextsRight.Count; i++)
        {
            hostitlySkillTextsRight[i].gameObject.SetActive(false);
        }
    }

    public void HostitlyUIRefreshAnimation()
    {
        DOTween.To(
            () => (float)mask.softness.x,
            (x) => mask.softness = new Vector2Int((int)x, mask.softness.y),
            150,
            1f
        ).SetEase(Ease.OutQuad).OnComplete(() =>
        {
            DOTween.To(
            () => (float)mask.padding.x,
            (x) => mask.padding = new Vector4((int)x, 0, 0, 0),
            700,
            3f
        ).OnComplete(HostitlyUIOpenAnimation);
        });
    }

    public void HostitlyUIOpenAnimation()
    {
        DOTween.To(
            () => (float)mask.padding.x,
            (x) => mask.padding = new Vector4((int)x, 0, 0, 0),
            0,
            2f
        ).SetEase(Ease.OutQuart);

        DOTween.To(
           () => (float)mask.softness.x,
           (x) => mask.softness = new Vector2Int((int)x, mask.softness.y),
           0,
           1f
        ).SetEase(Ease.OutQuad).SetDelay(2f);
    }

    public void UnitCardGoldNonEnougthTip()
    {
        goldnonenougthRect.gameObject.SetActive(true);
        goldnonenougthRect.DOKill();
        goldnonenougthRect.DOScale(Vector3.one, 0.25f).OnComplete(
            () =>
            {
                goldnonenougthRect.DOScale(Vector3.one, 1.5f).OnComplete(() =>
            {
                goldnonenougthRect.DOScale(Vector3.zero, 0.75f).OnComplete(() =>
            { goldnonenougthRect.gameObject.SetActive(false); });
            });
            }
            );
    }

    public void UnitCardNonEnougthTip()
    {
        cardnonenougthRect.gameObject.SetActive(true);
        cardnonenougthRect.DOKill();
        cardnonenougthRect.DOScale(Vector3.one, 0.25f).OnComplete(
            () =>
            {
                cardnonenougthRect.DOScale(Vector3.one, 1.5f).OnComplete(() =>
                {
                    cardnonenougthRect.DOScale(Vector3.zero, 0.75f).OnComplete(() =>
                    { cardnonenougthRect.gameObject.SetActive(false); });
                });
            }
            );
    }
    #endregion

    #region HierarchTreeFunction
    public void HierarchicalTreeUIChange()
    {
        if (!hierarchicalTreeObject.activeSelf)
        {
            HierarchicalTreeSystem.instance.HierarchicalTreeLockStateUpdate();
        }

        hierarchicalTreeObject.SetActive(!hierarchicalTreeObject.activeSelf);
        battleButton.gameObject.SetActive(!battleButton.gameObject.activeSelf);
    }

    public void HierarchicalTreeNodeUIDisplay(HierarchicalTreeNode node)
    {
        hierarchicalTreeNodeUIRect.gameObject.SetActive(true);

        if (node.isLocked)
        {
            hierarchicalUnLockButton.onClick.AddListener(node.UnLockAction);
            hierarchicalUnLockButton.gameObject.SetActive(true);
        }

        goldCost.text = InventoryManager.Instance.gold >= node.goldCost ? 
            new StringBuilder("gold : " + "<color=green>" + node.goldCost + "</color>").ToString() :
            new StringBuilder("gold : " + "<color=red>" + node.goldCost + "</color>").ToString();
        ghostCost.text = InventoryManager.Instance.ghost >= node.ghostCost ?
            new StringBuilder("ghost : " + "<color=green>" + node.ghostCost + "</color>").ToString() :
            new StringBuilder("ghost : " + "<color=red>" + node.ghostCost + "</color>").ToString();
        hierarchicalTreeNodeIcon.sprite = node.hierarchicalTreeNodeData.sprite;
        hierarchicalTreeNodeDescriptions.text = node.hierarchicalTreeNodeData.descriptions;

        UIDisPlayFunction(hierarchicalTreeNodeUIRect, HierarchTreeNodeUIHeight);
    }

    public void HierarchicalTreeNodeUIUnDisplay()
    {
        hierarchicalUnLockButton.gameObject.SetActive(false);
        UIUnDisPlayFunction(hierarchicalTreeNodeUIRect);
        hierarchicalUnLockButton.onClick.RemoveAllListeners();
    }

    public void HierarchicalTreeNodeGhostNonEnougthTip()
    {
        ghostnonenougth.DOScale(Vector3.one, 0.25f).OnComplete(
            () => {
                ghostnonenougth.DOScale(Vector3.one, 1.5f).OnComplete(() =>
                { ghostnonenougth.DOScale(Vector3.zero, 0.75f); });
            }
            );
    }
    #endregion

    #region InventorySystemFunction
    public void InventoryDataUIDisplay(ItemDataSO itemData)
    {
        InventoryIcon.sprite = itemData.itemIcon;
        InventoryText.text = itemData.itemDescription;

        if (itemData.itemType == ItemBuffType.Global)
        {
            InventoryUseButton.gameObject.SetActive(true);
        }

        InventoryDataUIRect.gameObject.SetActive(true);
        InventoryDataUIRect.DOScale(Vector3.one, 0.25f);
        InventoryDataUIRect.DOAnchorPosX(InventoryRightDistance,0.25f);
    }

    public void InventoryDataUIUnDisplay()
    {
        InventoryUseButton.gameObject.SetActive(false);

        InventoryDataUIRect.DOScale(Vector3.zero, 0.25f);
        InventoryDataUIRect.DOAnchorPosX(0, 0.25f).
            OnComplete(() => InventoryDataUIRect.gameObject.SetActive(true));
    }
    #endregion

    #region otherFunction
    private void UIDisPlayFunction(RectTransform UIRectTransfrom, float height)
    {
        UIRectTransfrom.DOScale(Vector3.one, 0.25f);
        UIRectTransfrom.DOAnchorPosY(height, 0.25f);
    }

    private void UIUnDisPlayFunction(RectTransform UIRectTransfrom)
    {
        UIRectTransfrom.DOScale(Vector3.zero, 0.25f);
        UIRectTransfrom.DOAnchorPosY(0, 0.25f).OnComplete(
            () =>{ UIRectTransfrom.gameObject.SetActive(false); });
    }

    public void BattleButtonDisPlaty()
    {
        battleButton.DOKill();
        battleButton.gameObject.SetActive(true);
        battleButton.transform.DOScale(Vector3.one,0.25f);
    }

    public void BattleButtonUnDisPlay()
    {
        battleButton.DOKill();
        battleButton.transform.DOScale(Vector3.zero, 0.25f);
        battleButton.gameObject.SetActive(false);
    }
    #endregion
}
