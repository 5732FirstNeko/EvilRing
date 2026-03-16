using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "newUnitWaveData", menuName = "Data/HostilityWave Data")]
public class HostilityWaveDataSo : ScriptableObject
{
    public List<UnitDataSo> hostilityData;
}
