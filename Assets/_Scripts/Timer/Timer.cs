using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

//半山腰太挤，你总得去山顶看看//
public class Timer
{
    private string timerName;
    private float duration;
    private float elapsedTime;
    private event Action callBack;
    private bool isPausing;

    public float RemainingTime => duration - elapsedTime;

    public Timer(string timerName,float duration,Action callBack)
    {
        this.timerName = timerName;
        this.duration = duration;
        this.callBack = callBack;
        this.isPausing = false;
        this.elapsedTime = 0f;
    }

    public void UpdateTimer()
    {
        if (!isPausing)
        {
            elapsedTime += Time.unscaledDeltaTime;

            if (elapsedTime >= duration)
            {
                elapsedTime = duration;
                callBack?.Invoke();

                // 记录需要移除的计时器
                TimerManager.instance.timersToRemove.Add(timerName);
            }
        }
    }

    public void Pause()
    {
        isPausing = true;
    }

    public void Start()
    {
        isPausing = false;
    }

    public void Resume()
    {
        elapsedTime = 0;
    }
}
