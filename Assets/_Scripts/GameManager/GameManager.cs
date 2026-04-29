using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;
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

    [SerializeField] private string startSceneName;

    public int currentLevel = 0;

    [SerializeField] private GameObject sceneChangeObject;

    [SerializeField] private AudioClip standrdAudio;
    [SerializeField] private AudioClip battleAudio;
    [SerializeField] private AudioClip finalBossAudio;
    [SerializeField] private AudioClip startSceneAudio;

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

        DialogueSystem.instance.GameStartDialogue();
    }

    #region BattleFunction
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

        AudioManager.instance.PlayBGM(battleAudio);
        UIManager.instance.BattleButtonUnDisPlay();

        BattleSystem.instance.BattleInit();

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
            currentLevel++;

            if (currentLevel == FactorySystem.instance.hostilityWaveDataList.Count)
            {
                GameFinalEnd(true);
                return;
            }

            AudioManager.instance.PlayBGM(standrdAudio);
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
            GameFinalEnd(false);
        }
    }

    public void GameFinalEnd(bool isWin)
    {
        UIManager.instance.CloseAllUI();
        if (isWin)
        {
            DialogueSystem.instance.GameWinDialogue();
        }
        else
        {
            DialogueSystem.instance.GameLoseDialogue();
        }

        TimerManager.instance.StartTimer(name + "DialogueEnd", 6f, 
            () => 
            {
                StartLoadScene(startSceneName);
            });
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
    #endregion

    private void Update()
    {
        #if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.Space))
        {
            
        }
        #endif
    }

    public enum GameState 
    {
        Preparation,
        Game
    }

    public static string GetPath(Transform transform)
    {
        string path = transform.name;
        while (transform.parent != null)
        {
            transform = transform.parent;
            path = $"{transform.name}/{path}";
        }
        return path;
    }
    public void StartLoadScene(string sceneName, float waitTime = 0.5f)
    {
        StartCoroutine(LoadSceneCoroutine(sceneName, waitTime));
    }

    private IEnumerator LoadSceneCoroutine(string sceneName, float waitTime)
    {
        // 1. 等待过渡时间（可以在这里加黑屏/淡入淡出动画）
        yield return new WaitForSeconds(waitTime);

        sceneChangeObject.SetActive(true);
        // 2. 异步加载场景（不会卡顿游戏）
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);

        // 3. 禁止加载完成后自动切换（可选，想控制切换时机用）
        //asyncLoad.allowSceneActivation = false;

        // 4. 等待场景加载完成
        while (!asyncLoad.isDone)
        {
            // 可以在这里打印加载进度：asyncLoad.progress
            yield return null;
        }

        AudioManager.instance.PlayBGM(startSceneAudio);

        // 5. 加载完成后自动切换场景
        //asyncLoad.allowSceneActivation = true;
    }
}
