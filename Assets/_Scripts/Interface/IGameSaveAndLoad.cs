using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//半山腰太挤，你总得去山顶看看//
public interface IGameSaveAndLoad
{
    public string GenerateUniqueID();

    // 返回序列化后的JSON字符串
    public string CaptureData();

    // 接收JSON字符串进行恢复
    public void RestoreData(string jsonData);
}
