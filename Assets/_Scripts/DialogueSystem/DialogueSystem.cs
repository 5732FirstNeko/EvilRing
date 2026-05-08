using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.UI;


public class DialogueSystem : MonoBehaviour, IGameSaveAndLoad
{
    public static DialogueSystem instance { get;private set; }

    private bool isTeched = false;
    [SerializeField] private float dialogueDiaplayTime;
    [SerializeField] private Image mask;
    [SerializeField] private Text dialogueText;
    [SerializeField] private List<string> gameStartDialogues;
    [SerializeField] private List<string> gameLoseDialogue;
    [SerializeField] private List<string> gameWinDialogue;

    [SerializeField] private GameObject teachObject;
    [SerializeField] private Image teachImage;
    [SerializeField] private Text teachText;
    [SerializeField] private Button leftButton;
    [SerializeField] private Button rightButton;
    [SerializeField] private Button closeButton;
    [SerializeField] private Button showButton;
    [SerializeField] private List<Sprite> teachSprites;
    [SerializeField] private List<string> teachTextContent;

    [SerializeField] private Button stopButton;
    [SerializeField] private Button continueButton;
    [SerializeField] private Button startMenuButton;
    [SerializeField] private Button ExitButton;
    [SerializeField] private GameObject stopPanel;
    [SerializeField] private GameObject exitWarnPanel;
    [SerializeField] private Button exitGameButton;
    [SerializeField] private Button unContinueExitButton;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        currentIndex = 0;

        leftButton.onClick.AddListener(LeftButton);
        rightButton.onClick.AddListener(RightButton);
        closeButton.onClick.AddListener(CloseTeachUI);
        showButton.onClick.AddListener(SHowTeachUI);

        stopButton.onClick.AddListener(StopButtonAction);
        continueButton.onClick.AddListener(ContinueButtonAction);
        startMenuButton.onClick.AddListener(StartMenuButtonAction);
        ExitButton.onClick.AddListener(ExitButtonActon);
        unContinueExitButton.onClick.AddListener(UnContinueExitAction);
    }

    #region DialogueFunction
    private int currentIndex;

    public void GameStartDialogue()
    {
        mask.gameObject.SetActive(true);
        dialogueText.gameObject.SetActive(true);

        dialogueText.text = gameStartDialogues[0];
        UIManager.instance.CloseAllUI();

        for (int i = 1; i < gameStartDialogues.Count; i++)
        {
            int time = i;
            DOVirtual.DelayedCall(time * dialogueDiaplayTime,
                () =>
                {
                    if (dialogueText != null)
                    {
                        dialogueText.text = gameStartDialogues[time];
                    }
                });
        }

        DOVirtual.DelayedCall(gameStartDialogues.Count * dialogueDiaplayTime + 0.25f,
            UIManager.instance.ShowAllUI);

        DOVirtual.DelayedCall(gameStartDialogues.Count * dialogueDiaplayTime,
            () =>
            {
                Debug.Log("mask !");
                mask.DOFade(0, 1f);

                TimerManager.instance.StartTimer(name + "Mask Close !", 1f, 
                    () =>
                    {
                        mask.gameObject.SetActive(false);
                        GameManager.instance.GameSceneStart();
                        TeachingDialogue();
                    });
                dialogueText.gameObject.SetActive(false);
            });
    }

    public void GameLoseDialogue()
    {
        AudioManager.instance.StopBGM();
        mask.gameObject.SetActive(true);
        dialogueText.gameObject.SetActive(true);

        mask.color = Color.black;
        dialogueText.text = gameLoseDialogue[0];
        UIManager.instance.CloseAllUI();

        for (int i = 1; i < gameLoseDialogue.Count; i++)
        {
            int time = i;
            DOVirtual.DelayedCall(time * dialogueDiaplayTime,
                () =>
                {
                    dialogueText.text = gameLoseDialogue[time];
                });
        }

        DOVirtual.DelayedCall(gameLoseDialogue.Count * dialogueDiaplayTime + 0.25f,
            UIManager.instance.ShowAllUI);

        DOVirtual.DelayedCall(gameLoseDialogue.Count * dialogueDiaplayTime,
            () =>
            {
                mask.DOFade(1, 1f).OnComplete(
                    () =>
                    {
                        GameManager.instance.StartLoadScene(GameManager.instance.startSceneName);
                    });
                dialogueText.gameObject.SetActive(false);
            });
    }

    public void GameWinDialogue()
    {
        mask.gameObject.SetActive(true);
        dialogueText.gameObject.SetActive(true);

        mask.color = Color.black;
        dialogueText.text = gameWinDialogue[0];
        UIManager.instance.CloseAllUI();

        for (int i = 1; i < gameWinDialogue.Count; i++)
        {
            int time = i;
            DOVirtual.DelayedCall(time * dialogueDiaplayTime,
                () =>
                {
                    dialogueText.text = gameWinDialogue[time];
                });
        }

        DOVirtual.DelayedCall(gameWinDialogue.Count * dialogueDiaplayTime - 0.25f,
            UIManager.instance.ShowAllUI);

        DOVirtual.DelayedCall(gameWinDialogue.Count * dialogueDiaplayTime,
            () =>
            {
                mask.DOFade(1, 1f).OnComplete(
                    () =>
                    {
                        GameManager.instance.StartLoadScene(GameManager.instance.startSceneName);
                    });
                dialogueText.gameObject.SetActive(false);
            });
    }

    public void TeachingDialogue()
    {
        if (isTeched)
        {
            return;
        }

        teachObject.SetActive(true);
        //teachImage.sprite = teachSprites[0];
        teachText.text = teachTextContent[0];
        currentIndex = 0;
    }

    private void LeftButton()
    {
        int index = currentIndex - 1;

        if (index >= 0)
        {
            teachImage.sprite = teachSprites[index];
            teachText.text = teachTextContent[index];
            currentIndex = index;
        }
    }

    private void RightButton()
    {
        int index = currentIndex + 1;

        if (index < teachSprites.Count && index < teachTextContent.Count)
        {
            teachImage.sprite = teachSprites[index];
            teachText.text = teachTextContent[index];
            currentIndex = index;
        }
    }

    private void CloseTeachUI()
    {
        teachObject.SetActive(false);
    }

    private void SHowTeachUI()
    {
        teachObject.SetActive(true);
    }
    #endregion

    #region ButtonAction
    private void StopButtonAction()   
    {
        stopPanel.SetActive(true);
        Time.timeScale = 0f;
    }

    private void ContinueButtonAction()
    {
        Time.timeScale = 1f;
        stopPanel.SetActive(false);
    }

    private void StartMenuButtonAction()
    {
        exitWarnPanel.SetActive(true);
        exitGameButton.onClick.RemoveAllListeners();
        exitGameButton.onClick.AddListener(ExitToMenuAction);
    }

    private void ExitButtonActon()
    {
        exitWarnPanel.SetActive(true);
        exitGameButton.onClick.RemoveAllListeners();
        exitGameButton.onClick.AddListener(ExitToDesktopAction);
    }

    private void ExitToMenuAction()
    {
        //GameSaveAndLoadSystem.SaveGame(out string error);
        mask.gameObject.SetActive(true);
        mask.color = Color.black;

        DOVirtual.DelayedCall(1f, 
            () => 
            {
                Debug.Log("StartMenu");
                GameManager.instance.StartLoadScene(GameManager.instance.startSceneName);
            });
    }

    private void ExitToDesktopAction()
    {
        //GameSaveAndLoadSystem.SaveGame(out string error);
        mask.gameObject.SetActive(true);
        mask.color = Color.black;

        DOVirtual.DelayedCall(1f,
           () =>
           {
               Application.Quit();
           });
    }

    private void UnContinueExitAction()
    {
        exitWarnPanel.SetActive(false);
    }
    #endregion

    public string GenerateUniqueID()
    {
        return GameManager.GetPath(transform);
    }

    public string CaptureData()
    {
        GameData data = new GameData
        {
            _isTeached = this.isTeched,
            _isDialogueTextActive = dialogueText.gameObject.activeSelf
        };

        return JsonConvert.SerializeObject(data, Formatting.Indented);
    }

    public void RestoreData(string jsonData)
    {
        GameData data = JsonConvert.DeserializeObject<GameData>(jsonData);
        isTeched = data._isTeached;
        dialogueText.gameObject.SetActive(data._isDialogueTextActive);
    }

    public class GameData 
    {
        public bool _isTeached;
        public bool _isDialogueTextActive;
    }
}
