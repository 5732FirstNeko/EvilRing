using System.Collections;
using System.Collections.Generic;
using System.Text;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager instance { get; private set; }

    public Button battleButton;
    #region UnitCardUIValue
    [SerializeField] private GameObject canves;

    [Header("UnitCardSystem")]
    public RectTransform friendlyUnitDataUIRightRect;
    public RectTransform friendlyUnitDataUILeftRect;

    [SerializeField] private Text friendlyCostTextRight;
    [SerializeField] private Text friendlyCostTextLeft;
    [SerializeField] private Text friendlyNameTextRight;
    [SerializeField] private Text friendlyNameTextLeft;
    [SerializeField] private Text friendlyHPTextRight;
    [SerializeField] private Text friendlyHPTextLeft;
    [SerializeField] private Text friendlySpeedTextRight;
    [SerializeField] private Text friendlySpeedTextLeft;
    [SerializeField] private List<Text> friendlySkillTextsRight;
    [SerializeField] private List<Text> friendlySkillTextsLeft;

    public RectTransform hostitlyUnitDataUILeftRect;
    public RectTransform hostitlyUnitDataUIRightRect;
    [SerializeField] private Text hostitlyNameTextLeft;
    [SerializeField] private Text hostitlyNameTextRight;
    [SerializeField] private Text hostitlyHPTextLeft;
    [SerializeField] private Text hostitlyHPTextRight;
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
    [SerializeField] public Button hierarchicalUnLockButton;

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
        instance = this;

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

            friendlyCostTextRight.text = InventoryManager.instance.gold >= unitData.cost ?
                new StringBuilder( "黄金花费 : " + "<color=white>" + unitData.cost + "</color>").ToString() : 
                new StringBuilder( "黄金花费 : " + "<color=red>" + unitData.cost + "</color>").ToString();
            friendlySpeedTextRight.text = "速度 : " + unitData.Speed;
            friendlyNameTextRight.text = unitData.cardName;
            friendlyHPTextRight.text = "生命值 : " + (int)unitData.HP;

            friendlySkillTextsRight[0].gameObject.SetActive(true);
            friendlySkillTextsRight[0].text = unitData.description;
        }
        else if(site == UnitSite.third || site == UnitSite.fourth)
        {
            friendlyUnitDataUILeftRect.DOKill();
            friendlyUnitDataUILeftRect.gameObject.SetActive(true);

            UIDisPlayFunction(friendlyUnitDataUILeftRect, UnitDataUIHeight);

            friendlyCostTextLeft.text = InventoryManager.instance.gold >= unitData.cost ?
                new StringBuilder("黄金花费 : " + "<color=white>" + unitData.cost + "</color>").ToString() :
                new StringBuilder("黄金花费 : " + "<color=red>" + unitData.cost + "</color>").ToString();
            friendlySpeedTextLeft.text = "速度 : " + unitData.Speed;
            friendlyNameTextLeft.text = unitData.cardName;
            friendlyHPTextLeft.text = "生命值 : " + (int)unitData.HP;
            friendlySkillTextsRight[0].gameObject.SetActive(true);
            friendlySkillTextsRight[0].text = unitData.description;
        }
    }

    public void FriendlyUnitDataDisplay(UnitDataSo unitData)
    {
        friendlyUnitDataUIRightRect.gameObject.SetActive(true);

        UIDisPlayFunction(friendlyUnitDataUIRightRect, UnitDataUIHeight);

        friendlyCostTextRight.text = InventoryManager.instance.gold >= unitData.cost ?
                new StringBuilder("黄金花费 : " + "<color=white>" + unitData.cost + "</color>").ToString() :
                new StringBuilder("黄金花费 : " + "<color=red>" + unitData.cost + "</color>").ToString();
        friendlySpeedTextRight.text = "速度 : " + unitData.Speed;
        friendlyNameTextRight.text = unitData.cardName;
        friendlyHPTextRight.text = "生命值 : " + (int)unitData.HP;
        friendlySkillTextsRight[0].gameObject.SetActive(true);
        friendlySkillTextsRight[0].text = unitData.description;
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
            hostitlySpeedTextLeft.text = "速度 : " + unitData.Speed;
            hostitlyNameTextLeft.text = unitData.cardName;
            hostitlyHPTextLeft.text = "生命值 : " + (int)unitData.HP;
            hostitlySkillTextsLeft[0].gameObject.SetActive(true);
            hostitlySkillTextsLeft[0].text = unitData.description;
        }
        else if (site == UnitSite.third || site == UnitSite.fourth)
        {
            hostitlyUnitDataUIRightRect.DOKill();
            hostitlyUnitDataUIRightRect.gameObject.SetActive(true);

            UIDisPlayFunction(hostitlyUnitDataUIRightRect, UnitDataUIHeight);
            hostitlySpeedTextRight.text = "速度 : " + unitData.Speed;
            hostitlyNameTextRight.text = unitData.cardName;
            hostitlyHPTextRight.text = "生命值 : " + (int)unitData.HP;
            hostitlySkillTextsRight[0].gameObject.SetActive(true);
            hostitlySkillTextsRight[0].text = unitData.description;
        }
    }

    public void HostitlyUnitDataDisplay(UnitDataSo unitData)
    {
        hostitlyUnitDataUILeftRect.gameObject.SetActive(true);

        UIDisPlayFunction(hostitlyUnitDataUILeftRect, UnitDataUIHeight);
        hostitlySpeedTextLeft.text = "速度 : " + unitData.Speed;
        hostitlyNameTextLeft.text = unitData.cardName;
        hostitlyHPTextLeft.text = "生命值 : " + (int)unitData.HP;
        hostitlySkillTextsLeft[0].gameObject.SetActive(true);
        hostitlySkillTextsLeft[0].text = unitData.description;
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
        UnitCardSystem.instance.refreshButton.gameObject.
            SetActive(!UnitCardSystem.instance.refreshButton.gameObject.activeSelf);
    }

    public void HierarchicalTreeNodeUIDisplay(HierarchicalTreeNode node)
    {
        hierarchicalTreeNodeUIRect.gameObject.SetActive(true);

        if (node.isLocked)
        {
            hierarchicalUnLockButton.onClick.AddListener(node.UnLockAction);

            bool canUnLock = true;
            foreach (var n in node.preconditionNodes)
            {
                if (n.isLocked)
                {
                    canUnLock = false;
                    break;
                }
            }
            if (canUnLock)
            {
                hierarchicalUnLockButton.gameObject.SetActive(true);
            }
        }
        else
        {
            hierarchicalUnLockButton.gameObject.SetActive(false);
        }

        goldCost.text = InventoryManager.instance.gold >= node.goldCost ? 
            new StringBuilder("黄金花费 : " + "<color=green>" + node.goldCost + "</color>").ToString() :
            new StringBuilder("黄金花费 : " + "<color=red>" + node.goldCost + "</color>").ToString();
        ghostCost.text = InventoryManager.instance.ghost >= node.ghostCost ?
            new StringBuilder("灵魂花费 : " + "<color=green>" + node.ghostCost + "</color>").ToString() :
            new StringBuilder("灵魂花费 : " + "<color=red>" + node.ghostCost + "</color>").ToString();
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
        UIRectTransfrom.DOKill();
        UIRectTransfrom.DOScale(Vector3.one, 0.25f);
        UIRectTransfrom.DOAnchorPosY(height, 0.25f);
    }

    private void UIUnDisPlayFunction(RectTransform UIRectTransfrom)
    {
        UIRectTransfrom.DOKill();
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

    public void CloseAllUI()
    {
        canves.SetActive(false);
    }

    public void ShowAllUI()
    {
        canves.SetActive(true);
    }
    #endregion
}
