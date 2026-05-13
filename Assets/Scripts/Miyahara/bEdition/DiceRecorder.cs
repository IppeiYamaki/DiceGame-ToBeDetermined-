using System.Collections.Generic;
using UnityEngine;


// 実際のサイコロ（記録用）
public class DiceRecorder : MonoBehaviour
{
    public DiceRecording recording;
    public float recordInterval = 0.02f;  // 0.02秒ごとに記録（FixedUpdate に近い）

    private float lastRecordTime = 0f;
    private bool isRecording = false;

    public void StartRecording()
    {
        recording.frames.Clear();
        isRecording = true;
        lastRecordTime = 0f;
    }

    public void StopRecording()
    {
        isRecording = false;
        SaveRecording();
    }

    private void FixedUpdate()
    {
        if (!isRecording) return;

        float t = Time.time;
        if (t - lastRecordTime > recordInterval)
        {
            recording.frames.Add(new DiceFrame(transform.position, transform.rotation, t));
            lastRecordTime = t;
        }
    }

    public void SaveRecording()
    {
        string path = Application.persistentDataPath + "/DiceRecordings/Dice1.json";
        DiceRecordingSaver.SaveToJSON(path, recording);
    }
}
