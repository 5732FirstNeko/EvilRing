using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

//半山腰太挤，你总得去山顶看看//
public class TimerManager : MonoBehaviour
{
    private static TimerManager Instance;
    public static TimerManager instance
    {
        get
        {
            if (Instance == null)
            {
                GameObject Object = new GameObject(typeof(TimerManager).Name);
                Instance = Object.AddComponent<TimerManager>();
                DontDestroyOnLoad(Object);
            }
            return Instance;
        }
    }

    #region TimerValue
    private Dictionary<string, Timer> timers = new Dictionary<string, Timer>();
    public List<string> timersToRemove { get; private set; } = new List<string>();
    #endregion

    #region BulletTimeValue
    private float originFixedDeletaTime;
    #endregion

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
        originFixedDeletaTime = Time.fixedDeltaTime;
    }

    private void Update()
    {
        TimerUpdate();
    }

    #region Timer
    private void TimerUpdate()
    {
        foreach (Timer timer in timers.Values.ToList())
        {
            timer.UpdateTimer();
        }

        // 遍历结束后移除计时器
        foreach (string timerName in timersToRemove)
        {
            if (timers.ContainsKey(timerName))
            {
                timers.Remove(timerName);
            }
        }
        timersToRemove.Clear();
    }

    public void StartTimer(string timerName, float duration, System.Action callback)
    {
        if (timers.ContainsKey(timerName))
        {
            StopTimer(timerName);
        }

        Timer timer = new Timer(timerName, duration, callback);
        timers.Add(timerName, timer);
    }

    public void StopTimer(string timerName)
    {
        if (timers.ContainsKey(timerName))
        {
            timers.Remove(timerName);
        }
    }

    public void PauseTimer(string timerName)
    {
        if (timers.ContainsKey(timerName))
        {
            timers[timerName].Pause();
        }
    }

    public void ReStartTimer(string timerName)
    {
        if (timers.ContainsKey(timerName))
        {
            timers[timerName].Start();
        }
    }

    public void ResumeTimer(string timerName)
    {
        if (timers.ContainsKey(timerName))
        {
            timers[timerName].Resume();
        }
    }

    public float GetRemainingTime(string timerName)
    {
        if (timers.ContainsKey(timerName))
        {
            return timers[timerName].RemainingTime;
        }
        return 0f;
    }
    #endregion
}
