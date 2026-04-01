using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "newUnitWaveData", menuName = "Data/HostilityWave Data")]
public class HostilityWaveDataSo : ScriptableObject
{
    public int ghostCost;
    public List<UnitDataSo> hostilityDataList;
}
