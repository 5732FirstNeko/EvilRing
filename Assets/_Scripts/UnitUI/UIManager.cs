using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using DG.Tweening.Core.Easing;
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

    public GameObject friendlyUnitDataUIObjectRight;
    public GameObject friendlyUnitDataUIObjectLeft;
    [SerializeField] private Text friendlyCostTextRight;
    [SerializeField] private Text friendlyCostTextLeft;
    [SerializeField] private List<Text> friendlySkillTextsRight;
    [SerializeField] private List<Text> friendlySkillTextsLeft;

    public GameObject hostitlyUnitDataUIObjectLeft;
    public GameObject hostitlyUnitDataUIObjectRight;
    [SerializeField] private List<Text> hostitlySkillTextsLeft;
    [SerializeField] private List<Text> hostitlySkillTextsRight;

    public Image friendlyUnitInstance;
    [SerializeField] private RectMask2D mask;

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
    }

    #region UnitDataDisPlayFunction
    public void FriendlyUnitDataDisplay(UnitDataSo unitData, UnitSite site)
    {
        if (site == UnitSite.first || site == UnitSite.second)
        {
            friendlyUnitDataUIObjectRight.gameObject.SetActive(true);
            friendlyCostTextRight.text = "cost : " + unitData.cost;
            for (int i = 0; i < friendlySkillTextsRight.Count; i++)
            {
                friendlySkillTextsRight[i].gameObject.SetActive(true);
                friendlySkillTextsRight[i].text = unitData.Skills[i].description;
            }
        }
        else if(site == UnitSite.third || site == UnitSite.fourth)
        {
            friendlyUnitDataUIObjectLeft.gameObject.SetActive(true);
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
        friendlyUnitDataUIObjectRight.gameObject.SetActive(true);
        friendlyCostTextRight.text = "cost : " + unitData.cost;
        for (int i = 0; i < unitData.Skills.Count; i++)
        {
            friendlySkillTextsRight[i].gameObject.SetActive(true);
            friendlySkillTextsRight[i].text = unitData.Skills[i].description;
        }
    }

    public void FriendlyUnitDataUnDisplay()
    {
        friendlyUnitDataUIObjectRight.gameObject.SetActive(false);
        for (int i = 0; i < friendlySkillTextsRight.Count; i++)
        {
            friendlySkillTextsRight[i].gameObject.SetActive(false);
        }

        friendlyUnitDataUIObjectLeft.gameObject.SetActive(false);
        for (int i = 0; i < friendlySkillTextsLeft.Count; i++)
        {
            friendlySkillTextsLeft[i].gameObject.SetActive(false);
        }
    }

    public void HostitlyUnitDataDisplay(UnitDataSo unitData, UnitSite site)
    {
        if (site == UnitSite.first || site == UnitSite.second)
        {
            hostitlyUnitDataUIObjectLeft.gameObject.SetActive(true);
            for (int i = 0; i < unitData.Skills.Count; i++)
            {
                hostitlySkillTextsLeft[i].gameObject.SetActive(true);
                hostitlySkillTextsLeft[i].text = unitData.Skills[i].description;
            }
        }
        else if (site == UnitSite.third || site == UnitSite.fourth)
        {
            hostitlyUnitDataUIObjectRight.gameObject.SetActive(true);
            for (int i = 0; i < unitData.Skills.Count; i++)
            {
                hostitlySkillTextsRight[i].gameObject.SetActive(true);
                hostitlySkillTextsRight[i].text = unitData.Skills[i].description;
            }
        }
    }

    public void HostitlyUnitDataDisplay(UnitDataSo unitData)
    {
        hostitlyUnitDataUIObjectLeft.gameObject.SetActive(true);
        for (int i = 0; i < unitData.Skills.Count; i++)
        {
            hostitlySkillTextsLeft[i].gameObject.SetActive(true);
            hostitlySkillTextsLeft[i].text = unitData.Skills[i].description;
        }
    }

    public void HostitlyUnitDataUnDisplay()
    {
        hostitlyUnitDataUIObjectLeft.gameObject.SetActive(false);
        for (int i = 0; i < hostitlySkillTextsLeft.Count; i++)
        {
            hostitlySkillTextsLeft[i].gameObject.SetActive(false);
        }

        hostitlyUnitDataUIObjectRight.gameObject.SetActive(false);
        for (int i = 0; i < hostitlySkillTextsRight.Count; i++)
        {
            hostitlySkillTextsRight[i].gameObject.SetActive(false);
        }
    }
    #endregion

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
}
