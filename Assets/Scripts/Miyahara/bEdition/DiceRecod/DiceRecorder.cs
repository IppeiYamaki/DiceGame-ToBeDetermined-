using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.InputSystem;


// 実際のサイコロ（記録用）
public class DiceRecorder : MonoBehaviour
{
    public DiceRecording recording;
    public RandomDice randomDice;
    public string recordingId = "Dice1";  // ← Inspector で ID を設定可能に
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

    private void Start()
    {
        recordingId = randomDice.recordingId[0]; // RandomDice から ID を取得
    }
    private void Update()
    {
        if (Keyboard.current.digit1Key.wasPressedThisFrame)
            recordingId = randomDice.recordingId[0];
        else if (Keyboard.current.digit2Key.wasPressedThisFrame)
            recordingId = randomDice.recordingId[1];
        else if (Keyboard.current.digit3Key.wasPressedThisFrame)
            recordingId = randomDice.recordingId[2];
        else if(Keyboard.current.digit4Key.wasPressedThisFrame)
            recordingId = randomDice.recordingId[3];
        else if(Keyboard.current.digit5Key.wasPressedThisFrame)
            recordingId = randomDice.recordingId[4];
        else if(Keyboard.current.digit6Key.wasPressedThisFrame)
            recordingId = randomDice.recordingId[5];
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
        // 独特なファイル名を生成
        string fileName = recordingId + ".json";
        string dir = Path.Combine(Application.persistentDataPath, "DiceRecordings");
        string fullPath = Path.Combine(dir, fileName);

        if (!Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);
        }

        DiceRecordingSaver.SaveToJSON(fullPath, recording);
    }
}
