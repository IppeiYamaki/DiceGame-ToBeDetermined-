using System.IO;
using UnityEngine;
using UnityEngine.InputSystem;

public class DicePlayer : MonoBehaviour
{
    public DiceRecording recording;
    public string recordingPath;
    public float playbackSpeed = 1.0f;  // 1.0 = 通常再生 時間比

    private Rigidbody rb;
    private int currentFrameIndex = 0;
    private float playbackTime = 0f;
    private bool isPlaying = false;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }
    void LoadRecordingFromJson()
    {
        // たとえば Android / PC 共通のパスで
        string path = Path.Combine(Application.persistentDataPath, "DiceRecordings/Dice1.json", recordingPath);

        DiceRecording rec = DiceRecordingSaver.LoadFromJSONFile(path);
        if (rec != null)
        {
            recording = rec;
        }
        else
        {
            Debug.LogWarning("DiceRecording が読み込めなかった: " + path);
        }
    }

    public void Play()
    {
        if (recording == null || recording.frames.Count == 0) return;

        // 本物の物理を停める
        rb.isKinematic = true;
        rb.angularVelocity = Vector3.zero;
        rb.linearVelocity = Vector3.zero;

        // 再生開始
        playbackTime = 0f;
        currentFrameIndex = 0;
        isPlaying = true;
    }

    void Update()
    {
        if (Keyboard.current.qKey.wasPressedThisFrame)
        {
            rb.isKinematic = false;

        }
        if (Keyboard.current.pKey.wasPressedThisFrame)
        {
            LoadRecordingFromJson();
            Play();
        }
        if (!isPlaying) return;

        // 再生時間の経過を計算（実時間ではなく再生時間）
        playbackTime += Time.deltaTime * playbackSpeed;

        // 最初の記録時刻を0秒に揃える
        float firstTime = recording.frames[0].time;

        // 見たい「再生時刻」
        float seekTime = playbackTime + firstTime;

        // 該当フレームを検索して再生
        for (int i = 0; i < recording.frames.Count - 1; i++)
        {
            var a = recording.frames[i];
            var b = recording.frames[i + 1];

            if (seekTime < b.time)
            {
                // a と b の間を補間
                float t = Mathf.Clamp01((seekTime - a.time) / (b.time - a.time));
                transform.position = Vector3.Lerp(a.position, b.position, t);
                transform.rotation = Quaternion.Slerp(a.rotation, b.rotation, t);
                currentFrameIndex = i;
                return;
            }
        }

        // 最後まで到達したら、最後のフレームをそのまま表示して停止
        if (currentFrameIndex >= recording.frames.Count - 1)
        {
            int lastIndex = recording.frames.Count - 1;
            transform.position = recording.frames[lastIndex].position;
            transform.rotation = recording.frames[lastIndex].rotation;
            isPlaying = false;
        }
    }
}
