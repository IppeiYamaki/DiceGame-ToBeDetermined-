using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.InputSystem;


// 実際のサイコロ（記録用）
public class DiceRecorder : MonoBehaviour
{
    [SerializeField]
    private int useIdIndex = 0; // 使用する recordingId のインデックス
    public DiceRecording recording;
    public RandomDice randomDice;
    public DiceRole diceRole;
    public string recordingId = "Dice1";  // ← Inspector で ID を設定可能に
    public float recordInterval = 0.02f;  // 0.02秒ごとに記録（FixedUpdate に近い）

    private float lastRecordTime = 0f;
    private bool isRecording = false;


    private string GetRecordingId(DiceRecorderEvent e)
    {
        return useIdIndex switch
        {
            0 => e.recordingId1,
            1 => e.recordingId2,
            2 => e.recordingId3,
            _ => e.recordingId1
        };
    }

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

    }
    private void Update()
    {
        if (Keyboard.current.digit1Key.wasPressedThisFrame)
            recordingId = GetRecordingId(diceRole.recordingId[0]);
        else if (Keyboard.current.digit2Key.wasPressedThisFrame)
            recordingId = GetRecordingId(diceRole.recordingId[1]);
        else if (Keyboard.current.digit3Key.wasPressedThisFrame)
            recordingId = GetRecordingId(diceRole.recordingId[2]);
        else if (Keyboard.current.digit4Key.wasPressedThisFrame)
            recordingId = GetRecordingId(diceRole.recordingId[3]);
        else if (Keyboard.current.digit5Key.wasPressedThisFrame)
            recordingId = GetRecordingId(diceRole.recordingId[4]);
        else if (Keyboard.current.digit6Key.wasPressedThisFrame)
            recordingId = GetRecordingId(diceRole.recordingId[5]);
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
        string dir = Path.Combine(Application.streamingAssetsPath, "DiceRecordings");
        string fullPath = Path.Combine(dir, fileName);

        if (!Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);
        }

        DiceRecordingSaver.SaveToJSON(fullPath, recording);
    }
}
