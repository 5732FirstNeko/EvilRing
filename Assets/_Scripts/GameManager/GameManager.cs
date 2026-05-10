using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager instance { get; private set; }

    public static Material UnlitMaterial;
    public static Material litMaterial;

    public static Color purple = new Color(0.5f, 0f, 0.5f, 1f);

    public GameState gameState;

    public Light2D globalLight;
    public List<Light2D> otherlights;
    public float[] otherLightIntensity;

    public Collider2D BackGroundCollider;
    [SerializeField] private Material unlitmaterial;
    [SerializeField] private Material litmaterial;

    [SerializeField] public string startSceneName;

    public int currentLevel = 0;

    [SerializeField] private GameObject sceneChangeObject;

    [SerializeField] public AudioClip standrdAudio;
    [SerializeField] private AudioClip battleAudio;
    [SerializeField] private AudioClip finalBossAudio;

    public bool isHaveDrag 
    {
        get => InventoryManager.instance.isHaveDrag || UnitCardSystem.instance.isHaveCardDrag;
    }

    private void Awake()
    {
        instance = this;

        UnlitMaterial = unlitmaterial;
        litMaterial = litmaterial;
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
            gameState = GameState.Preparation;
            UIManager.instance.BattleButtonDisPlaty();
            UnitCardSystem.instance.RefreshHostitlyUnit();
            DialogueSystem.instance.GameLoseDialogue();
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
            SceneManager.LoadSceneAsync(1, LoadSceneMode.Single);
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

    #region SceneLoad

    public void StartLoadScene(string sceneName, float waitTime = 0.5f)
    {
        StartCoroutine(LoadSceneCoroutine(sceneName, waitTime));
    }

    private IEnumerator LoadSceneCoroutine(string sceneName, float waitTime)
    {
        sceneChangeObject.SetActive(true);
        yield return new WaitForSecondsRealtime(waitTime);
        DOTween.KillAll();
        Time.timeScale = 1f;

        AsyncOperation async = SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex);

        while (!async.isDone)
        {
            yield return null;
        }
    }
    #endregion

    public static void LookAtTarget(Transform self, Vector3 targetPos, Vector3 customForward)
    {
        if (self == null) return;
        customForward.Normalize();
        Vector3 dir = targetPos - self.position;
        if (dir.magnitude < 0.01f) return; 

        float angle = Vector2.SignedAngle(customForward, dir);
        self.rotation = Quaternion.Euler(0, 0, angle);
    }
}
