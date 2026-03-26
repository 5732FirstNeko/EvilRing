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

    #region UnitCardUIValue
    [Header("UnitCardSystem")]
    public RectTransform friendlyUnitDataUIRightRect;
    public RectTransform friendlyUnitDataUILeftRect;

    [SerializeField] private Text friendlyCostTextRight;
    [SerializeField] private Text friendlyCostTextLeft;
    [SerializeField] private List<Text> friendlySkillTextsRight;
    [SerializeField] private List<Text> friendlySkillTextsLeft;

    public RectTransform hostitlyUnitDataUILeftRect;
    public RectTransform hostitlyUnitDataUIRightRect;
    [SerializeField] private List<Text> hostitlySkillTextsLeft;
    [SerializeField] private List<Text> hostitlySkillTextsRight;

    [SerializeField] private float UnitDataUIHeight;

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

    [SerializeField] private float HierarchTreeNOdeUIHeight;
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
        HostitlyUICloseAnimation();
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

        HierarchTreeNOdeUIHeight = hierarchicalTreeNodeUIRect.anchoredPosition.y;
        hierarchicalTreeNodeUIRect.anchoredPosition
            = new Vector2(hierarchicalTreeNodeUIRect.anchoredPosition.x, 0);
        hierarchicalTreeNodeUIRect.transform.localScale = Vector3.zero;
    }

    #region UnitDataDisPlayFunction
    public void FriendlyUnitDataDisplay(UnitDataSo unitData, UnitSite site)
    {
        if (site == UnitSite.first || site == UnitSite.second)
        {
            friendlyUnitDataUIRightRect.gameObject.SetActive(true);

            UIDisPlayFunction(friendlyUnitDataUIRightRect, UnitDataUIHeight);

            friendlyCostTextRight.text = "cost : " + unitData.cost;
            for (int i = 0; i < friendlySkillTextsRight.Count; i++)
            {
                friendlySkillTextsRight[i].gameObject.SetActive(true);
                friendlySkillTextsRight[i].text = unitData.Skills[i].description;
            }
        }
        else if(site == UnitSite.third || site == UnitSite.fourth)
        {
            friendlyUnitDataUILeftRect.gameObject.SetActive(true);

            UIDisPlayFunction(friendlyUnitDataUILeftRect, UnitDataUIHeight);

            friendlyCostTextLeft.text = "cost : " + unitData.cost;
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

        friendlyCostTextRight.text = "cost : " + unitData.cost;
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
            hostitlyUnitDataUILeftRect.gameObject.SetActive(true);

            UIDisPlayFunction(hostitlyUnitDataUILeftRect, UnitDataUIHeight);

            for (int i = 0; i < unitData.Skills.Count; i++)
            {
                hostitlySkillTextsLeft[i].gameObject.SetActive(true);
                hostitlySkillTextsLeft[i].text = unitData.Skills[i].description;
            }
        }
        else if (site == UnitSite.third || site == UnitSite.fourth)
        {
            hostitlyUnitDataUIRightRect.gameObject.SetActive(true);

            UIDisPlayFunction(hostitlyUnitDataUIRightRect, UnitDataUIHeight);

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

    public void HostitlyUICloseAnimation()
    {
        DOTween.To(
            () => (float)mask.softness.x,
            (x) => mask.softness = new Vector2Int((int)x, mask.softness.y),
            150,
            1f
        ).SetEase(Ease.OutQuad);

        DOTween.To(
            () => (float)mask.padding.x,
            (x) => mask.padding = new Vector4((int)x, 0, 0, 0),
            700,
            3f
        ).SetEase(Ease.OutQuart).SetDelay(1f);
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
    #endregion

    #region HierarchTreeFunction
    public void HierarchicalTreeUIChange()
    {
        if (!hierarchicalTreeObject.activeSelf)
        {
            HierarchicalTreeSystem.instance.HierarchicalTreeLockStateUpdate();
        }

        hierarchicalTreeObject.SetActive(!hierarchicalTreeObject.activeSelf);
    }

    public void HierarchicalTreeNodeUIDisplay(HierarchicalTreeNode node)
    {
        hierarchicalTreeNodeUIRect.gameObject.SetActive(true);

        goldCost.text = InventoryManager.Instance.gold >= node.goldCost ? 
            new StringBuilder("gold : " + "<color=green>" + node.goldCost + "</color>").ToString() :
            new StringBuilder("gold : " + "<color=red>" + node.goldCost + "</color>").ToString();
        ghostCost.text = InventoryManager.Instance.ghost >= node.ghostCost ?
            new StringBuilder("ghost : " + "<color=green>" + node.ghostCost + "</color>").ToString() :
            new StringBuilder("ghost : " + "<color=red>" + node.ghostCost + "</color>").ToString();
        hierarchicalTreeNodeIcon.sprite = node.hierarchicalTreeNodeData.sprite;
        hierarchicalTreeNodeDescriptions.text = node.hierarchicalTreeNodeData.descriptions;

        UIDisPlayFunction(hierarchicalTreeNodeUIRect, HierarchTreeNOdeUIHeight);
    }

    public void HierarchicalTreeNodeUIUnDisplay()
    {
        UIUnDisPlayFunction(hierarchicalTreeNodeUIRect);
    }
    #endregion

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
}
