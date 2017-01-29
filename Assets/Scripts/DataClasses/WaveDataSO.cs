using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName="WaveData", menuName="Level/Wave Data")]
public class WaveDataSO : ScriptableObject {
    [SerializeField]
    private WaveData[] waveData;
    public WaveData[] WaveData { get { return waveData; } }
}
