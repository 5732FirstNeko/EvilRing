using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Newtonsoft.Json;

//半山腰太挤，你总得去山顶看看//
public static class GameSaveAndLoadSystem
{
    // 存储可存档对象（线程安全字典）
    private static readonly Dictionary<string, IGameSaveAndLoad> _saveableObjects = new Dictionary<string, IGameSaveAndLoad>();
    private static readonly object _dictionaryLock = new object(); // 用于线程锁

    // 存档数据结构（包含版本号和对象数据）
    [Serializable]
    public class GameSaveData
    {
        public string saveVersion = "1.0.0"; // 存档版本，用于版本兼容
        public Dictionary<string, string> objectsData = new Dictionary<string, string>(); // 改用明确类型的字典
    }

    // 注册可存档对象（确保ID唯一）
    public static void RegisterSaveable(IGameSaveAndLoad saveable)
    {
        if (saveable == null)
        {
            Debug.LogError("注册失败：存档对象为空！");
            return;
        }

        string id = saveable.GenerateUniqueID();
        if (string.IsNullOrEmpty(id))
        {
            Debug.LogError("注册失败：ID不能为空！");
            return;
        }

        lock (_dictionaryLock) // 线程安全
        {
            if (_saveableObjects.ContainsKey(id))
            {
                Debug.LogError($"注册失败：ID重复！ID={id}");
                return; // 强制要求ID唯一，不自动替换（避免加载混乱）
            }

            _saveableObjects.Add(id, saveable);
            Debug.Log($"已注册存档对象：ID={id}");
        }
    }

    // 注销可存档对象
    public static void UnregisterSaveable(string id)
    {
        if (string.IsNullOrEmpty(id)) return;

        lock (_dictionaryLock)
        {
            if (_saveableObjects.Remove(id))
            {
                Debug.Log($"已注销存档对象：ID={id}");
            }
        }
    }

    public static bool SaveGame(out string errorMsg, string fileName = "save1.json")
    {
        errorMsg = string.Empty;
        try
        {
            var saveData = new GameSaveData();

            lock (_dictionaryLock)
            {
                foreach (var kvp in _saveableObjects)
                {
                    try
                    {
                        // 捕获单个对象的序列化错误（不影响整体存档）
                        string objectJson = kvp.Value.CaptureData();
                        saveData.objectsData[kvp.Key] = objectJson;
                    }
                    catch (Exception ex)
                    {
                        Debug.LogWarning($"对象ID={kvp.Key}序列化失败：{ex.Message}");
                    }
                }
            }

            // 使用Newtonsoft.Json序列化（支持字典和复杂类型）
            string json = JsonConvert.SerializeObject(saveData, Formatting.Indented);
            string path = GetSavePath(fileName);

            // 确保目录存在
            Directory.CreateDirectory(Path.GetDirectoryName(path));
            File.WriteAllText(path, json);

            Debug.Log($"存档成功：{path}");
            return true;
        }
        catch (Exception ex)
        {
            errorMsg = $"存档失败：{ex.Message}";
            Debug.LogError(errorMsg);
            return false;
        }
    }

    public static bool LoadGame(out string errorMsg, string fileName = "save1.json")
    {
        errorMsg = string.Empty;
        try
        {
            string path = GetSavePath(fileName);
            if (!File.Exists(path))
            {
                errorMsg = $"存档文件不存在：{path}";
                Debug.LogWarning(errorMsg);
                return false;
            }

            string json = File.ReadAllText(path);
            var saveData = JsonConvert.DeserializeObject<GameSaveData>(json);

            // 简单版本校验（根据实际需求扩展）
            if (saveData.saveVersion != "1.0.0")
            {
                Debug.LogWarning($"存档版本不匹配（当前支持1.0.0，存档为{saveData.saveVersion}），可能导致数据异常");
            }

            lock (_dictionaryLock)
            {
                foreach (var kvp in saveData.objectsData)
                {
                    try
                    {
                        if (_saveableObjects.TryGetValue(kvp.Key, out var saveable))
                        {
                            saveable.RestoreData(kvp.Value);
                        }
                        else
                        {
                            Debug.LogWarning($"未找到对象ID={kvp.Key}，跳过加载");
                        }
                    }
                    catch (Exception ex)
                    {
                        Debug.LogWarning($"对象ID={kvp.Key}反序列化失败：{ex.Message}");
                    }
                }
            }

            Debug.Log($"加载成功：{path}");
            return true;
        }
        catch (Exception ex)
        {
            errorMsg = $"加载失败：{ex.Message}";
            Debug.LogError(errorMsg);
            return false;
        }
    }

    // 删除存档
    public static bool DeleteSave(out string errorMsg, string fileName = "save1.json")
    {
        errorMsg = string.Empty;
        try
        {
            string path = GetSavePath(fileName);
            if (File.Exists(path))
            {
                File.Delete(path);
                Debug.Log($"已删除存档：{path}");
                return true;
            }

            errorMsg = $"存档文件不存在：{path}";
            return false;
        }
        catch (Exception ex)
        {
            errorMsg = $"删除失败：{ex.Message}";
            Debug.LogError(errorMsg);
            return false;
        }
    }

    // 获取所有存档文件名
    public static List<string> GetAllSaveFiles()
    {
        var saveFiles = new List<string>();
        string dir = Application.persistentDataPath;
        if (Directory.Exists(dir))
        {
            foreach (var file in Directory.GetFiles(dir, "*.json"))
            {
                saveFiles.Add(Path.GetFileName(file));
            }
        }
        return saveFiles;
    }

    // 获取存档路径
    private static string GetSavePath(string fileName)
    {
        // 确保文件名以.json结尾
        if (!fileName.EndsWith(".json", StringComparison.OrdinalIgnoreCase))
        {
            fileName += ".json";
        }
        return Path.Combine(Application.persistentDataPath, fileName);
    }
}
