using System.IO;
using UnityEngine;
using UnityEngine.InputSystem;

public class DicePlayer : MonoBehaviour
{
    public DiceRecording recording;
    public RandomDice randomDice; 
    public string recordingId = "Dice1";  // ← Inspector で ID を設定
    public float playbackSpeed = 1.0f;

    private Rigidbody rb;
    private float playbackTime = 0f;
    private bool isPlaying = false;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        recordingId = randomDice.recordingId[0]; // RandomDice から ID を取得
    }

    public void LoadRecordingAndPlay()
    {
        string dir = Path.Combine(Application.persistentDataPath, "DiceRecordings");
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

        rb.isKinematic = true;
        rb.angularVelocity = Vector3.zero;
        rb.linearVelocity = Vector3.zero;

        playbackTime = 0f;
        isPlaying = true;
    }

    void Update()
    {
        if (Keyboard.current.digit1Key.wasPressedThisFrame)
            recordingId = randomDice.recordingId[0];
        else if (Keyboard.current.digit2Key.wasPressedThisFrame)
            recordingId = randomDice.recordingId[1];
        else if (Keyboard.current.digit3Key.wasPressedThisFrame)
            recordingId = randomDice.recordingId[2];
        else if (Keyboard.current.digit4Key.wasPressedThisFrame)
            recordingId = randomDice.recordingId[3];
        else if (Keyboard.current.digit5Key.wasPressedThisFrame)
            recordingId = randomDice.recordingId[4];
        else if (Keyboard.current.digit6Key.wasPressedThisFrame)
            recordingId = randomDice.recordingId[5];



        if (!isPlaying)
        {
            //,ID を押したら読み込んで再生
            if (Keyboard.current?.pKey.wasPressedThisFrame == true && randomDice.state == RandomDice.DiceState.Idle)
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
