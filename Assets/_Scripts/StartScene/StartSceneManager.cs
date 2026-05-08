using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class StartSceneManager : MonoBehaviour
{
    [SerializeField] private string gameSceneName;

    [SerializeField] private GameObject canves;
    [SerializeField] private GameObject thanksObject;

    [SerializeField] private Button startGameButton;
    [SerializeField] private Button exitGameButton;
    [SerializeField] private Button thanksButton;
    [SerializeField] private Button thanksCloseButton;

    [SerializeField] private GameObject sceneChangeObject;
    [SerializeField] private AudioClip startSceneBGM;

    [SerializeField] private GameObject startMenu;

    private void Awake()
    {
        canves.SetActive(true);
        startGameButton.onClick.AddListener(StartGame);
        exitGameButton.onClick.AddListener(ExitGame);
        thanksButton.onClick.AddListener(Thanks);
        thanksCloseButton.onClick.AddListener(Thanks);
    }

    private void Start()
    {
        AudioManager.instance.PlayBGM(startSceneBGM);
    }

    public void StartGame()
    {
        StartLoadScene(gameSceneName);
    }

    public void StartLoadScene(string sceneName, float waitTime = 0.5f)
    {
        canves.SetActive(false);
        StartCoroutine(LoadSceneCoroutine(sceneName, waitTime));
    }

    private IEnumerator LoadSceneCoroutine(string sceneName, float waitTime)
    {
        yield return new WaitForSecondsRealtime(waitTime);
        sceneChangeObject.SetActive(true);
        AudioManager.instance.StopBGM();

        yield return new WaitForSecondsRealtime(waitTime);
        sceneChangeObject.SetActive(false);

        startMenu.SetActive(false);
        AudioManager.instance.PlayBGM(GameManager.instance.standrdAudio);

        DialogueSystem.instance.GameStartDialogue();
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    public void Thanks()
    {
        thanksObject.SetActive(!thanksObject.activeSelf);
    }
}
