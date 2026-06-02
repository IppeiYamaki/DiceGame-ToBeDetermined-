using UnityEngine;

public class DiceRecordingBank : MonoBehaviour
{
    public DiceRecording[] recordings; // 0: 1の目、1: 2の目 … など、6個以上をInspectorでセット

    public void SetupRecordingFor(int faceIndex)
    {
        if (recordings == null) return;
        // 事前用意した 6パターンの録画を Inspector で割り当てておく
    }
}
