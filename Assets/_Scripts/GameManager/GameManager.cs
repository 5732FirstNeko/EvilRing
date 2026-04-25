using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public static GameManager instance
    {
        get
        {
            if (Instance == null)
            {
                GameObject Object = new GameObject(typeof(GameManager).Name);
                Instance = Object.AddComponent<GameManager>();
                DontDestroyOnLoad(Object);
            }
            return Instance;
        }
    }

    public static Material UnlitMaterial;

    public static Color purple = new Color(0.5f, 0f, 0.5f, 1f);

    public GameState gameState;

    public Light2D globalLight;
    public List<Light2D> otherlights;
    public float[] otherLightIntensity;

    public Collider2D BackGroundCollider;
    [SerializeField] private Material unlitmaterial;

    public bool isHaveDrag 
    {
        get => InventoryManager.instance.isHaveDrag || UnitCardSystem.instance.isHaveCardDrag;
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

        UnlitMaterial = unlitmaterial;
    }

    private void Start()
    {
        otherLightIntensity = new float[otherlights.Count];
        for (int i = 0; i < otherlights.Count; i++)
        {
            otherLightIntensity[i] = otherlights[i].intensity;
        }

        UIManager.instance.battleButton.onClick.AddListener(() => 
        {
            GameBattleInit();
        });
    }

    public void GameSceneStart()
    {
        gameState = GameState.Preparation;
        
        UIManager.instance.HostitlyUIRefreshAnimation();
        TimerManager.instance.StartTimer("HostitlyCardRefresh", 5f, ()=>
        {
            UnitCardSystem.instance.RefreshHostitlyUnit();
            UIManager.instance.BattleButtonDisPlaty();
        });

        UnitCardSystem.instance.RefreshAllFriendlyUnit();
    }

    public void GameBattleInit()
    {
        int friendlyCardCount = 0;
        foreach (var plat in UnitCardSystem.instance.GetCurrentFriendlyUnitPlats())
        {
            if (plat.unitData != null && plat.unitData != FactorySystem.instance.EmptyFriendlyUnitData)
            {
                friendlyCardCount++;
            }
        }

        if (friendlyCardCount <= 0)
        {
            UIManager.instance.UnitCardNonEnougthTip();
            return;
        }

        UIManager.instance.BattleButtonUnDisPlay();

        BattleSystem.instance.BattleInit();

        HierarchicalTreeSystem.instance.HierarchicalTreeNodeGlobalAction();
        GameBattle();
    }

    public void GameBattle()
    {
        gameState = GameState.Game;
        BattleSystem.instance.BattleFunction();
    }

    public void GameBattleEnd(bool iswin)
    {
        gameState = GameState.Preparation;
        UnitPlatPositionReset();

        if (iswin)
        {
            UnitCardSystem.instance.RecoverDefaultHostitlyUnitSprite();
            UIManager.instance.HostitlyUIRefreshAnimation();
            TimerManager.instance.StartTimer("HostitlyCardRefresh", 5f, () =>
            {
                UnitCardSystem.instance.RefreshHostitlyUnit();
                UIManager.instance.BattleButtonDisPlaty();
            });

            UnitCardSystem.instance.RefreshAllFriendlyUnit();
        }
        else
        {
            UIManager.instance.BattleButtonDisPlaty();
            UnitCardSystem.instance.RefreshAllFriendlyUnit();
        }
    }

    public void UnitPlatPositionReset()
    {
        for (int i = 0; i < BattleSystem.unitPlatQueueCount; i++)
        {
            UnitCardSystem.instance.friendlyUnitPlats[i].transform.position =
                new Vector3(BattleSystem.instance.friendlyUnitSiteFlag[i].transform.position.x, 
                UnitCardSystem.instance.friendlyUnitPlats[i].transform.position.y, 0);
        }

        for (int i = 0; i < BattleSystem.unitPlatQueueCount; i++)
        {
            UnitCardSystem.instance.hostitlyUnitPlats[i].transform.position =
                new Vector3(BattleSystem.instance.hostilityUnitSiteFlag[i].transform.position.x,
                UnitCardSystem.instance.hostitlyUnitPlats[i].transform.position.y, 0);
        }
    }

    public GameObject GameObjectInit(GameObject gameObject)
    {
        GameObject game = Instantiate(gameObject, Vector3.zero, Quaternion.identity);
        game.SetActive(false);
        return game;
    }

    public void GlobalLightControll(float insentity, float time)
    {
        DOTween.To(
            () => globalLight.intensity,
            (x) => globalLight.intensity = x,
            insentity,
            time
        );

        for (int i = 0; i < otherlights.Count; i++)
        {
            int index = i;
            DOTween.To(
            () => otherlights[index].intensity,
            (x) => otherlights[index].intensity = x,
            insentity * otherLightIntensity[index],
            time
        );
        }
    }

    private void Update()
    {
        #if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.Space))
        {
            GameSceneStart();
        }
        #endif
    }

    public enum GameState 
    {
        Preparation,
        Game
    }
}
