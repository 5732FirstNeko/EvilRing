using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartSceneManager : MonoBehaviour
{
    [SerializeField] private string gameSceneName;

    [SerializeField] private GameObject canves;
    [SerializeField] private GameObject thanksObject;

    [SerializeField] private GameObject sceneChangeObject;
    [SerializeField] private AudioClip gameSceneBGM;

    private void Awake()
    {
        canves.SetActive(true);
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
        // 1. 等待过渡时间（可以在这里加黑屏/淡入淡出动画）
        AudioManager.instance.PauseBGM();
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

        AudioManager.instance.PlayBGM(gameSceneBGM);

        // 5. 加载完成后自动切换场景
        //asyncLoad.allowSceneActivation = true;
    }

    public void ExitGame()
    {
        GameSaveAndLoadSystem.SaveGame(out string error);
        Application.Quit();
    }

    public void Thanks()
    {
        thanksObject.SetActive(!thanksObject.activeSelf);
    }
}
