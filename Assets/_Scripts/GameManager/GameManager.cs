using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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

    public GameState gameState;

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
    }

    private void Start()
    {
        UIManager.instance.battleButton.onClick.AddListener(() => 
        {
            GameBattleInit();
            GameBattle();
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
            if (plat.unitData != null)
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
        InventoryManager.instance.GlobalInventoryBind();
        InventoryManager.instance.UnitInventoryBind();
    }

    public void GameBattle()
    {
        gameState = GameState.Game;
        BattleSystem.instance.BattleFunction();
    }

    public void GameBattleEnd(bool iswin)
    {
        gameState = GameState.Preparation;
        InventoryManager.instance.GlobalInventoryUnbind();
        InventoryManager.instance.UnitInventoryUnBind();
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
