using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//半山腰太挤，你总得去山顶看看//
public class CameraShakeManager : MonoBehaviour
{
    private static CameraShakeManager Instance;
    public static CameraShakeManager instance
    {
        get
        {
            if (Instance == null)
            {
                GameObject Object = new GameObject(typeof(CameraShakeManager).Name);
                Instance = Object.AddComponent<CameraShakeManager>();
                DontDestroyOnLoad(Object);
            }
            return Instance;
        }
    }

    [Header("Camera Settings")]
    public Camera targetCamera;
    public bool affectAllCameras = true;
    public float maxShakeOffset = 1.0f;

    [Header("Axis Control")]
    [Tooltip("允许在X轴上震动")]
    public bool enableXAxis = true;
    [Tooltip("允许在Y轴上震动")]
    public bool enableYAxis = true;
    [Tooltip("X轴震动强度乘数")]
    [Range(0f, 2f)] public float xAxisMultiplier = 1f;
    [Tooltip("Y轴震动强度乘数")]
    [Range(0f, 2f)] public float yAxisMultiplier = 1f;

    [Header("Hit Stop Settings")]
    public bool enableHitStop = true;
    [Range(0f, 1f)] public float hitStopTimeScale = 0.01f;
    public float minHitStopDuration = 0.02f;
    public float maxHitStopDuration = 0.2f;

    [Header("Debug")]
    public bool debugMode;
    public KeyCode testShakeKey = KeyCode.S;
    public KeyCode testHitStopKey = KeyCode.H;
    public KeyCode testAxisToggleKey = KeyCode.T;

    private Vector3 originalCameraPosition;
    private List<ShakeEffect> activeShakes = new List<ShakeEffect>();
    private Coroutine hitStopCoroutine;
    private bool isHitStopping;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        if (targetCamera == null)
        {
            targetCamera = Camera.main;
        }

        if (targetCamera != null)
        {
            originalCameraPosition = targetCamera.transform.position;
        }
    }

    void Update()
    {
        if (targetCamera == null && Camera.main != null)
        {
            targetCamera = Camera.main;
            originalCameraPosition = targetCamera.transform.position;
        }

        if (debugMode)
        {
            HandleDebugInput();
        }

        ProcessShakes();
    }

    private void HandleDebugInput()
    {
        if (Input.GetKeyDown(testShakeKey))
        {
            TriggerShake(0.5f, 0.3f, ShakeType.Explosion);
        }

        if (Input.GetKeyDown(testHitStopKey))
        {
            TriggerHitStop(0.1f);
        }

        if (Input.GetKeyDown(testAxisToggleKey))
        {
            // 循环切换轴向模式
            if (enableXAxis && enableYAxis)
            {
                // X轴
                enableYAxis = false;
                Debug.Log("震动模式: 仅X轴");
            }
            else if (enableXAxis && !enableYAxis)
            {
                // Y轴
                enableXAxis = false;
                enableYAxis = true;
                Debug.Log("震动模式: 仅Y轴");
            }
            else
            {
                // 双轴
                enableXAxis = true;
                enableYAxis = true;
                Debug.Log("震动模式: X+Y轴");
            }
        }
    }

    #region Public API
    /// <summary>
    /// 触发震屏效果
    /// </summary>
    /// <param name="intensity">震动强度 (0-1)</param>
    /// <param name="duration">持续时间 (秒)</param>
    /// <param name="shakeType">震动类型</param>
    /// <param name="axisControl">轴向控制 (null时使用全局设置)</param>
    public void TriggerShake(float intensity, float duration,
                            ShakeType shakeType = ShakeType.Default,
                            AxisControl axisControl = null)
    {
        if (intensity <= 0 || duration <= 0) return;

        // 使用传入的轴向控制或全局设置
        AxisControl actualAxis = axisControl ?? new AxisControl(enableXAxis, enableYAxis, xAxisMultiplier, yAxisMultiplier);

        activeShakes.Add(new ShakeEffect(intensity, duration, shakeType, actualAxis));
    }

    /// <summary>
    /// 触发特定轴向的震屏效果
    /// </summary>
    public void TriggerDirectionalShake(float intensity, float duration,
                                       bool enableX, bool enableY,  
                                       ShakeType shakeType = ShakeType.Default)
    {
        TriggerShake(intensity, duration, shakeType, new AxisControl(enableX, enableY));
    }

    /// <summary>
    /// 触发顿帧效果
    /// </summary>
    public void TriggerHitStop(float duration)
    {
        if (!enableHitStop || isHitStopping) return;

        duration = Mathf.Clamp(duration, minHitStopDuration, maxHitStopDuration);

        if (hitStopCoroutine != null)
        {
            StopCoroutine(hitStopCoroutine);
        }

        hitStopCoroutine = StartCoroutine(DoHitStop(duration));
    }

    /// <summary>
    /// 停止所有震屏效果
    /// </summary>
    public void StopAllShakes()
    {
        activeShakes.Clear();
        ResetCameraPosition();
    }

    /// <summary>
    /// 设置全局轴向控制
    /// </summary>
    public void SetAxisControl(bool enableX, bool enableY,
                              float xMultiplier = 1f, float yMultiplier = 1f)
    {
        enableXAxis = enableX;
        enableYAxis = enableY;
        xAxisMultiplier = xMultiplier;
        yAxisMultiplier = yMultiplier;
    }
    #endregion

    #region Core Functionality
    private void ProcessShakes()
    {
        if (targetCamera == null) return;

        // 如果没有震动，重置相机位置
        if (activeShakes.Count == 0)
        {
            ResetCameraPosition();
            return;
        }

        // 计算总震动偏移
        Vector3 totalOffset = Vector3.zero;

        for (int i = activeShakes.Count - 1; i >= 0; i--)
        {
            ShakeEffect shake = activeShakes[i];

            // 更新震动效果
            if (!shake.Update(Time.unscaledDeltaTime))
            {
                activeShakes.RemoveAt(i);
                continue;
            }

            // 添加震动偏移
            totalOffset += CalculateShakeOffset(shake);
        }

        // 限制最大偏移
        if (totalOffset.magnitude > maxShakeOffset)
        {
            totalOffset = totalOffset.normalized * maxShakeOffset;
        }

        // 应用偏移到相机
        if (affectAllCameras)
        {
            foreach (Camera cam in Camera.allCameras)
            {
                cam.transform.position = originalCameraPosition + totalOffset;
            }
        }
        else if (targetCamera != null)
        {
            targetCamera.transform.position = originalCameraPosition + totalOffset;
        }
    }

    private Vector3 CalculateShakeOffset(ShakeEffect shake)
    {
        float time = Time.unscaledTime * shake.frequency;
        float intensity = shake.CurrentIntensity;
        Vector3 offset = Vector3.zero;

        switch (shake.shakeType)
        {
            case ShakeType.Explosion:
                offset = new Vector3(
                    Mathf.PerlinNoise(time, 0) * 2 - 1,
                    Mathf.PerlinNoise(0, time) * 2 - 1,
                    0) * intensity * 1.5f;
                break;

            case ShakeType.Rumble:
                offset = new Vector3(
                    Mathf.Sin(time * 10) * 0.8f,
                    Mathf.Cos(time * 8) * 0.6f,
                    0) * intensity;
                break;

            case ShakeType.Vibration:
                offset = new Vector3(
                    Mathf.PerlinNoise(time * 20, 0) * 2 - 1,
                    Mathf.PerlinNoise(0, time * 20) * 2 - 1,
                    0) * intensity * 0.7f;
                break;

            default: // Default/Simple
                offset = new Vector3(
                    Mathf.PerlinNoise(time, 0) * 2 - 1,
                    Mathf.PerlinNoise(0, time) * 2 - 1,
                    0) * intensity;
                break;
        }

        // 应用轴向控制
        return ApplyAxisControl(offset, shake.axisControl);
    }

    private Vector3 ApplyAxisControl(Vector3 offset, AxisControl axisControl)
    {
        Vector3 result = Vector3.zero;

        // X轴控制
        if (axisControl.enableX)
        {
            result.x = offset.x * axisControl.xMultiplier;
        }

        // Y轴控制
        if (axisControl.enableY)
        {
            result.y = offset.y * axisControl.yMultiplier;
        }

        return result;
    }

    private void ResetCameraPosition()
    {
        if (affectAllCameras)
        {
            foreach (Camera cam in Camera.allCameras)
            {
                cam.transform.position = originalCameraPosition;
            }
        }
        else if (targetCamera != null)
        {
            targetCamera.transform.position = originalCameraPosition;
        }
    }

    private IEnumerator DoHitStop(float duration)
    {
        isHitStopping = true;
        float originalTimeScale = Time.timeScale;
        Time.timeScale = hitStopTimeScale;

        // 等待真实时间（不受timeScale影响）
        yield return new WaitForSecondsRealtime(duration);

        Time.timeScale = originalTimeScale;
        isHitStopping = false;
    }
    #endregion

    #region Helper Classes and Enums
    /// <summary>
    /// 震动类型
    /// </summary>
    public enum ShakeType
    {
        Default,    // 默认震动
        Explosion,  // 爆炸效果 - 强烈且随机
        Rumble,     // 持续性震动 - 类似引擎震动
        Vibration,  // 高频震动 - 类似手机震动
        Horizontal, // 水平震动
        Vertical    // 垂直震动
    }

    /// <summary>
    /// 轴向控制类
    /// </summary>
    public class AxisControl
    {
        public readonly bool enableX;
        public readonly bool enableY;
        public readonly float xMultiplier;
        public readonly float yMultiplier;

        public AxisControl(bool enableX, bool enableY,
                          float xMultiplier = 1f, float yMultiplier = 1f)
        {
            this.enableX = enableX;
            this.enableY = enableY;
            this.xMultiplier = xMultiplier;
            this.yMultiplier = yMultiplier;
        }
    }

    /// <summary>
    /// 震动效果类
    /// </summary>
    private class ShakeEffect
    {
        public ShakeType shakeType;
        public float intensity;
        public float duration;
        public float frequency;
        public float elapsedTime;
        public AxisControl axisControl;

        public float CurrentIntensity => intensity * (1 - (elapsedTime / duration));

        public ShakeEffect(float intensity, float duration, ShakeType shakeType, AxisControl axisControl)
        {
            this.intensity = Mathf.Clamp01(intensity);
            this.duration = duration;
            this.shakeType = shakeType;
            this.elapsedTime = 0f;
            this.axisControl = axisControl;

            // 根据类型设置频率
            switch (shakeType)
            {
                case ShakeType.Explosion:
                    frequency = 15f;
                    break;
                case ShakeType.Rumble:
                    frequency = 8f;
                    break;
                case ShakeType.Vibration:
                    frequency = 25f;
                    break;
                case ShakeType.Horizontal:
                    frequency = 12f;
                    break;
                case ShakeType.Vertical:
                    frequency = 10f;
                    break;
                default:
                    frequency = 10f;
                    break;
            }

            // 特殊震动类型自动设置轴向
            if (shakeType == ShakeType.Horizontal)
            {
                this.axisControl = new AxisControl(true, false);
            }
            else if (shakeType == ShakeType.Vertical)
            {
                this.axisControl = new AxisControl(false, true);
            }
        }

        public bool Update(float deltaTime)
        {
            elapsedTime += deltaTime;
            return elapsedTime < duration;
        }
    }

    public bool isShake = false;

    public void StartShake(float duration,float strength)
    {
        if(!isShake)
        StartCoroutine(Shake(duration, strength));
    }

    IEnumerator Shake(float duration,float strength)
    {
        isShake = true;
        Transform camera = Camera.main.transform;
        Vector3 startPostion = camera.position;

        while (duration > 0)
        {
            camera.position = (Vector3)Random.insideUnitCircle * strength + startPostion;
            duration -= Time.deltaTime;
            yield return null;
        }

        isShake = false;
    }
    #endregion
}
