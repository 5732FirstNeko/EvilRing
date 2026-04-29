using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

//半山腰太挤，你总得去山顶看看//
public class AudioManager : MonoBehaviour
{
    public static AudioManager instance
    {
        get
        {
            if (Instance == null)
            {
                GameObject Object = new GameObject(typeof(ObjectPoolSystem).Name);
                Instance = Object.AddComponent<AudioManager>();
                DontDestroyOnLoad(Object);
            }
            return Instance;
        }
    }
    private static AudioManager Instance;

    [Header("BGMSetting")]
    [SerializeField] private float bgmFadeTime;
    [SerializeField] private float defaultBgmVolume; // BGM默认音量
    [SerializeField] private AudioSource bgmSource;

    private AudioClip currentBGM;

    [Header("SFXSetting")]
    [SerializeField] private float defaultSFXVolume;

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

        if (bgmSource == null)
        {
            bgmSource = gameObject.AddComponent<AudioSource>();
            bgmSource.loop = true; // BGM默认循环
            bgmSource.volume = defaultBgmVolume;
            bgmSource.spatialBlend = 0; // 2D音效（全场景听到）
        }
    }

    #region BGMPlay
    public void PlayBGM(AudioClip bgmClip,bool isloop = true)
    {
        if (bgmClip == null)
        {
            Debug.LogError("BGM File is null !");
            return;
        }

        if (currentBGM == bgmClip && bgmSource.isPlaying) return;

        currentBGM = bgmClip;
        bgmSource.loop = isloop;

        StartCoroutine(FadeBgmAndPlayNew(currentBGM));
    }

    public void PauseBGM()
    {
        if (bgmSource.isPlaying)
        {
            bgmSource.Pause();
        }
    }

    public void ResumeBGM()
    {
        if (!bgmSource.isPlaying)
        {
            bgmSource.UnPause();
        }
    }

    public void StopBGM()
    {
        StartCoroutine(FadeBgmToStop());
        currentBGM = null;
    }

    public void SetBgmVolume(float volume)
    {
        bgmSource.volume = Mathf.Clamp01(volume);
    }

    private IEnumerator FadeBgmAndPlayNew(AudioClip newBgm)
    {
        if (bgmSource.isPlaying)
        {
            float startVolume = bgmSource.volume;
            for (float t = 0; t < bgmFadeTime; t += Time.deltaTime)
            {
                bgmSource.volume = Mathf.Lerp(startVolume, 0, t / bgmFadeTime);
                yield return null;
            }
            bgmSource.Stop();
        }

        bgmSource.clip = newBgm;
        bgmSource.Play();
        for (float t = 0; t < bgmFadeTime; t += Time.deltaTime)
        {
            bgmSource.volume = Mathf.Lerp(0, defaultBgmVolume, t / bgmFadeTime);
            yield return null;
        }
        bgmSource.volume = defaultBgmVolume;
    }

    private IEnumerator FadeBgmToStop()
    {
        float startVolume = bgmSource.volume;
        for (float t = 0; t < bgmFadeTime; t += Time.deltaTime)
        {
            bgmSource.volume = Mathf.Lerp(startVolume, 0, t / bgmFadeTime);
            yield return null;
        }
        bgmSource.Stop();
        bgmSource.volume = defaultBgmVolume; // 恢复默认音量，方便下次播放
    }
    #endregion

    #region SFXPlay
    public void PlaySFX(AudioClip sfxClip, float volume = -1, float pitch = 1f)
    {
        PlaySFXAtPosition(sfxClip, Vector3.zero, volume, pitch, 0); // spatialBlend=0 → 2D
    }

    //"spatialBlend"空间混合（0=2D，1=3D）
    private void PlaySFXAtPosition(AudioClip sfxClip, Vector3 position, 
        float volume = -1, float pitch = 1f, float spatialBlend = 1f)
    {
        if (sfxClip == null)
        {
            Debug.LogError("SFXClip is null !");
            return;
        }

        AudioSource audioSource = ObjectPoolSystem.instance.GetAudioSourceElement();
        audioSource.gameObject.SetActive(true);

        audioSource.clip = sfxClip;
        audioSource.volume = volume < 0 ? defaultSFXVolume : Mathf.Clamp01(volume);
        audioSource.pitch = Mathf.Clamp(pitch, 0.5f, 2f);
        audioSource.spatialBlend = Mathf.Clamp01(spatialBlend);
        audioSource.transform.position = position;

        audioSource.Play();
        StartCoroutine(RecycleSfxSourceAfterPlay(audioSource,sfxClip.length));
    }

    private IEnumerator RecycleSfxSourceAfterPlay(AudioSource source, float clipLength)
    {
        yield return new WaitForSeconds(clipLength + 0.1f);
        if (source != null)
        {
            source.clip = null;
            ObjectPoolSystem.instance.RecycleElement(source);
        }
    }

    #endregion
}
