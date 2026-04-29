using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.UI;


public class DialogueSystem : MonoBehaviour, IGameSaveAndLoad
{
    public static DialogueSystem Instance;
    public static DialogueSystem instance
    {
        get
        {
            if (Instance == null)
            {
                GameObject Object = new GameObject(typeof(DialogueSystem).Name);
                Instance = Object.AddComponent<DialogueSystem>();
                DontDestroyOnLoad(Object);
            }
            return Instance;
        }
    }

    private bool isTeched = false;
    [SerializeField] private float dialogueDiaplayTime;
    [SerializeField] private SpriteRenderer mask;
    [SerializeField] private TextMesh dialogueText;
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
        currentIndex = 0;

        leftButton.onClick.AddListener(LeftButton);
        rightButton.onClick.AddListener(RightButton);
        closeButton.onClick.AddListener(CloseTeachUI);
        showButton.onClick.AddListener(SHowTeachUI);
    }

    #region DialogueFunction
    private int currentIndex;

    public void GameStartDialogue()
    {
        mask.gameObject.SetActive(true);
        dialogueText.gameObject.SetActive(true);

        mask.color = Color.black;
        dialogueText.text = gameStartDialogues[0];
        UIManager.instance.CloseAllUI();

        for (int i = 1; i < gameStartDialogues.Count; i++)
        {
            int time = i;
            DOVirtual.DelayedCall(time * dialogueDiaplayTime,
                () =>
                {
                    dialogueText.text = gameStartDialogues[time];
                });
        }

        DOVirtual.DelayedCall(gameStartDialogues.Count * dialogueDiaplayTime + 0.25f,
            UIManager.instance.ShowAllUI);

        DOVirtual.DelayedCall(gameStartDialogues.Count * dialogueDiaplayTime,
            () =>
            {
                mask.DOFade(0, 1f).OnComplete(
                    () => 
                    {
                        GameManager.instance.GameSceneStart();
                        TeachingDialogue();
                    });
                dialogueText.gameObject.SetActive(false);
            });
    }

    public void GameLoseDialogue()
    {
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
                mask.DOFade(0, 1f).OnComplete(
                    () =>
                    {
                        GameManager.instance.GameSceneStart();
                        TeachingDialogue();
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
                mask.DOFade(0, 1f).OnComplete(
                    () =>
                    {
                        GameManager.instance.GameSceneStart();
                        TeachingDialogue();
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
