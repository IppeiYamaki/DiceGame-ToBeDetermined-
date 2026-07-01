using NUnit.Framework;
using System.IO;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class DicePlayer : MonoBehaviour
{
    [SerializeField]
    private int useIdIndex = 0; // 使用する recordingId のインデックス
    public DiceRecording recording;
    public RandomDice randomDice;
    public DiceRole diceRole;
    public string recordingId = "Dice1";  // ← Inspector で ID を設定
    public float playbackSpeed = 1.0f;

    private Rigidbody rb;
    private float playbackTime = 0f;
    private bool isPlaying = false;


    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

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

    public void LoadRecordingAndPlay()
    {
        string dir = Path.Combine(Application.streamingAssetsPath, "DiceRecordings");
        string fileName = recordingId + ".json";
        string fullPath = Path.Combine(dir, fileName);

        DiceRecording rec = DiceRecordingSaver.LoadFromJSONFile(fullPath);
        if (rec != null)
        {
            recording = rec;
            Play();
            randomDice.state = RandomDice.DiceState.RecordPlaying;
        }
        else
        {
            Debug.LogWarning("DiceRecording が読み込めなかった: " + fullPath);
        }
    }

    public void Play()
    {
        if (recording == null || recording.frames.Count == 0) return;
        rb.angularVelocity = Vector3.zero;
        rb.linearVelocity = Vector3.zero;
        rb.isKinematic = true;


        playbackTime = 0f;
        isPlaying = true;
    }

    void Update()
    {

        recordingId = GetRecordingId(diceRole.recordingId[randomDice.recordingIdIndex]);

        if (!isPlaying)
        {
            //,ID を押したら読み込んで再生
            if (Keyboard.current?.spaceKey.wasPressedThisFrame == true && randomDice.state == RandomDice.DiceState.Idle&&randomDice.isNextStep==true)
            {
                LoadRecordingAndPlay();
            }
            return;
        }

        playbackTime += Time.deltaTime * playbackSpeed;
        float firstTime = recording.frames[0].time;
        float seekTime = playbackTime + firstTime;

        for (int i = 0; i < recording.frames.Count - 1; i++)
        {
            var a = recording.frames[i];
            var b = recording.frames[i + 1];

            if (seekTime < b.time)
            {
                float t = Mathf.Clamp01((seekTime - a.time) / (b.time - a.time));
                transform.position = Vector3.Lerp(a.position, b.position, t);
                transform.rotation = Quaternion.Slerp(a.rotation, b.rotation, t);
                return;
            }
        }

        // 最後のフレームを表示して停止
        int lastIndex = recording.frames.Count - 1;
        transform.position = recording.frames[lastIndex].position;
        transform.rotation = recording.frames[lastIndex].rotation;
        isPlaying = false;

        // 物理を再度有効化
        rb.isKinematic = false;
    }
}
