using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;



public class RandomDice : MonoBehaviour
{


    [SerializeField]
    private Vector3 torque = new Vector3(1, 1, 1);  // 回転軸
    [SerializeField]
    private Vector3 spawn = new Vector3(-4, 5, 0);   // 出現位置
    [SerializeField]
    private DiceRecorder diceRecorder;  // 録画用コンポーネント
    private Rigidbody rb; // Rigidbodyコンポーネントへの参照
    [SerializeField]
    private int notStoppedDice = 0; // サイコロが停止していないフレーム数のカウンタ
    private bool notLooped = false; // ドロップ開始後の一度だけの処理を制御するフラグ
    private bool notLooped2 = false; // サイコロが停止していないかどうかのフラグ
    [SerializeField]
    private bool isRecording = false; // 録画中かどうかのフラグ
    [SerializeField]
    private GameObject rotateDice; // 回転するサイコロのゲームオブジェクトへの参照

    public int changeDiceValue = 0; // サイコロの目の値を変更するための変数
    public DiceRole role; // DiceRoleコンポーネントへの参照
    public bool isStopped = false; // サイコロが停止しているかどうかのフラグ
    public int stopCount = 0;// サイコロが停止しているフレーム数のカウンタ
    [HideInInspector]
    public int diceValue;// サイコロの目の値
    public float rotateSpeed = 1f;// 回転の速さ
    public TMP_Text randomText;// サイコロの目の値を表示するテキスト
    [SerializeField]
    private int useIdIndex = 0;
    [SerializeField]
    private int recordingIdIndex = 0;

    public int debugtako = 0;




    // サイコロの状態を表す列挙型
    public enum DiceState
    {
        Idle,
        Dropping,
        RecordPlaying,
        Stopped,
        NextEvent
    }
    public DiceState state;
    void Start()
    {
        isRecording = false;
        notLooped = true;
        this.transform.position = spawn;
        rb = GetComponent<Rigidbody>();
        //role = GetComponent<DiceRole>();

    }



    // Update is called once per frame
    void Update()
    {
        if (Keyboard.current.digit1Key.wasPressedThisFrame)
            recordingIdIndex = 0;
        else if (Keyboard.current.digit2Key.wasPressedThisFrame)
            recordingIdIndex = 1;
        else if (Keyboard.current.digit3Key.wasPressedThisFrame)
            recordingIdIndex = 2;
        else if (Keyboard.current.digit4Key.wasPressedThisFrame)
            recordingIdIndex = 3;
        else if (Keyboard.current.digit5Key.wasPressedThisFrame)
            recordingIdIndex = 4;
        else if (Keyboard.current.digit6Key.wasPressedThisFrame)
            recordingIdIndex = 5;

        switch (state)
        {
            case DiceState.Idle:
                IdolDice();
                if (!notLooped2)
                {
                    rotateDice.transform.rotation = Quaternion.Euler(0, 0, 0);
                    notLooped2 = true;
                }
                break;
            case DiceState.Dropping:
                DropDice();

                break;
            case DiceState.RecordPlaying:
                RecordPlaying();
                if (notLooped2)
                {
                    PlayDiceRotate((int)GetRecordingId(role));
                    
                    notLooped2 = false;
                }
                break;
            case DiceState.Stopped:
                Debug.Log("ダイスナンバー" + debugtako +"もとの番号"+ (int)GetRecordingId(role) + "変えたいダイスナンバー" + changeDiceValue + "変わった番号" + diceValue);
                StopDice();
                break;
            case DiceState.NextEvent:
                NextEvent();
                break;
        }

        if (Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            switch (state)
            {
                case DiceState.Idle:
                    state = DiceState.Dropping;
                    break;
                case DiceState.NextEvent:
                    state = DiceState.Idle;
                    break;
            }

            if (diceRecorder != null && isRecording == true)
                diceRecorder.StartRecording();
        }

        if (Keyboard.current.rKey.wasPressedThisFrame)
        {
            isRecording = !isRecording;
        }



    }

    private float GetRecordingId(DiceRole r)
    {
        return useIdIndex switch
        {
            0 => r.recordingId[recordingIdIndex].DiceValue.x,
            1 => r.recordingId[recordingIdIndex].DiceValue.y,
            2 => r.recordingId[recordingIdIndex].DiceValue.z,
            _ => r.recordingId[recordingIdIndex].DiceValue.x
        };
    }

    // サイコロを初期位置に戻し、回転を加える処理
    void IdolDice()
    {
        if (notLooped)
        {
            rb.isKinematic = false;
            this.transform.position = spawn;
            this.transform.rotation = Quaternion.Euler(0, 0, 0);
            rb.constraints = RigidbodyConstraints.None;
            rb.constraints = RigidbodyConstraints.FreezePosition;

            float rotatex = Random.Range(-3f, 3f);
            float rotatey = Random.Range(1f, 3f);
            float rotatez = Random.Range(-3f, 3f);

            torque = new Vector3(rotatex, rotatey, rotatez);
            rb.AddTorque(torque * rotateSpeed, ForceMode.Force);
            notLooped = false;
            stopCount = 0;
            randomText.text = 0.ToString();
            isStopped = false;
        }
    }

    // サイコロを落とす処理
    void DropDice()
    {
        rb.constraints = RigidbodyConstraints.None;

        notLooped = true;
        notStoppedDice += 1;
        if (notStoppedDice > 800)
        {
            rb.AddForce(Vector3.up * 50, ForceMode.Force);
            rb.AddTorque(torque * rotateSpeed, ForceMode.Force);
            notStoppedDice = 0;
        }
    }

    // 録画再生中の処理
    void RecordPlaying()
    {
        notLooped = true;
    }
    // サイコロが停止しているときの処理
    void StopDice()
    {
        stopCount = 0;
        notStoppedDice = 0;
    }

    // 次のイベントに移行する処理
    void NextEvent()
    {
        isStopped = false;
        stopCount = 0;
        notStoppedDice = 0;
    }


    public void Dice1Event()
    {
        stopCount++;
        if (stopCount > 30)
        {
            Debug.Log("1");
            randomText.text = 1.ToString();
            diceValue = 1;
            isStopped = true;
            state = DiceState.Stopped;
            if (diceRecorder != null && isRecording == true)
                diceRecorder.StopRecording();
        }
    }

    public void Dice2Event()
    {
        stopCount++;
        if (stopCount > 30)
        {
            Debug.Log("2");
            randomText.text = 2.ToString();
            diceValue = 2;
            isStopped = true;
            state = DiceState.Stopped;
            if (diceRecorder != null && isRecording == true)
                diceRecorder.StopRecording();
        }
    }

    public void Dice3Event()
    {
        stopCount++;
        if (stopCount > 30)
        {
            Debug.Log("3");
            randomText.text = 3.ToString();
            diceValue = 3;
            isStopped = true;
            state = DiceState.Stopped;
            if (diceRecorder != null && isRecording == true)
                diceRecorder.StopRecording();
        }
    }

    public void Dice4Event()
    {
        stopCount++;
        if (stopCount > 30)
        {
            Debug.Log("4");
            randomText.text = 4.ToString();
            diceValue = 4;
            isStopped = true;
            state = DiceState.Stopped;
            if (diceRecorder != null && isRecording == true)
                diceRecorder.StopRecording();
        }
    }

    public void Dice5Event()
    {
        stopCount++;
        if (stopCount > 30)
        {
            Debug.Log("5");
            randomText.text = 5.ToString();
            diceValue = 5;
            isStopped = true;
            state = DiceState.Stopped;
            if (diceRecorder != null && isRecording == true)
                diceRecorder.StopRecording();
        }
    }

    public void Dice6Event()
    {
        stopCount++;
        if (stopCount > 30)
        {
            Debug.Log("6");
            randomText.text = 6.ToString();
            diceValue = 6;
            isStopped = true;
            state = DiceState.Stopped;
            if (diceRecorder != null && isRecording == true)
                diceRecorder.StopRecording();
        }
    }
    public void PlayDiceRotate(int diceValue)
    {
        if (diceValue == 1)
        {
            if (changeDiceValue == 1)
            {
                rotateDice.transform.rotation *= Quaternion.Euler(0,0,0);
            }
            else if (changeDiceValue == 2)
            {
                rotateDice.transform.rotation *= Quaternion.Euler(0, 0, 90);
            }
            else if (changeDiceValue == 3)
            {
                rotateDice.transform.rotation *= Quaternion.Euler(-90, 0, 0);
            }
            else if (changeDiceValue == 4)
            {
                rotateDice.transform.rotation *= Quaternion.Euler(90, 0, 0);
            }
            else if (changeDiceValue == 5)
            {
                rotateDice.transform.rotation *= Quaternion.Euler(0, 0, -90);
            }
            else if (changeDiceValue == 6)
            {
                rotateDice.transform.rotation *= Quaternion.Euler(180, 0, 0);
            }
        }
        else if (diceValue == 2)
        {
            if (changeDiceValue == 1)
            {
                rotateDice.transform.rotation *= Quaternion.Euler(0, 0, -90);
            }
            else if (changeDiceValue == 2)
            {
                rotateDice.transform.rotation *= Quaternion.Euler(0, 0, 0);
            }
            else if (changeDiceValue == 3)
            {
                rotateDice.transform.rotation *= Quaternion.Euler(0, 90, 0);
            }
            else if (changeDiceValue == 4)
            {
                rotateDice.transform.rotation *= Quaternion.Euler(0, -90, 0);
            }
            else if (changeDiceValue == 5)
            {
                rotateDice.transform.rotation *= Quaternion.Euler(0, 0, 180);
            }
            else if (changeDiceValue == 6)
            {
                rotateDice.transform.rotation *= Quaternion.Euler(0, 0, 90);
            }
        }
        else if (diceValue == 3)
        {
            if (changeDiceValue == 1)
            {
                rotateDice.transform.rotation *= Quaternion.Euler(90, 0, 0);
            }
            else if (changeDiceValue == 2)
            {
                rotateDice.transform.rotation *= Quaternion.Euler(0, -90, 0);
            }
            else if (changeDiceValue == 3)
            {
                rotateDice.transform.rotation *= Quaternion.Euler(0, 0, 0);
            }
            else if (changeDiceValue == 4)
            {
                rotateDice.transform.rotation *= Quaternion.Euler(180, 0, 0);
            }
            else if (changeDiceValue == 5)
            {
                rotateDice.transform.rotation *= Quaternion.Euler(0, 90, 0);
            }
            else if (changeDiceValue == 6)
            {
                rotateDice.transform.rotation *= Quaternion.Euler(-90, 0, 0);
            }

        }
        else if (diceValue == 4)
        {
            if (changeDiceValue == 1)
            {
                rotateDice.transform.rotation *= Quaternion.Euler(-90, 0, 0);
            }
            else if (changeDiceValue == 2)
            {
                rotateDice.transform.rotation *= Quaternion.Euler(0, 90, 0);
            }
            else if (changeDiceValue == 3)
            {
                rotateDice.transform.rotation *= Quaternion.Euler(180, 0, 0);
            }
            else if (changeDiceValue == 4)
            {
                rotateDice.transform.rotation *= Quaternion.Euler(0, 0, 0);
            }
            else if (changeDiceValue == 5)
            {
                rotateDice.transform.rotation *= Quaternion.Euler(0, -90, 0);
            }
            else if (changeDiceValue == 6)
            {
                rotateDice.transform.rotation *= Quaternion.Euler(90, 0, 0);
            }
        }
        else if (diceValue == 5)
        {
            if (changeDiceValue == 1)
            {
                rotateDice.transform.rotation *= Quaternion.Euler(0, 0, 90);
            }
            else if (changeDiceValue == 2)
            {
                rotateDice.transform.rotation *= Quaternion.Euler(180, 0, 0);
            }
            else if (changeDiceValue == 3)
            {
                rotateDice.transform.rotation *= Quaternion.Euler(0, -90, 0);
            }
            else if (changeDiceValue == 4)
            {
                rotateDice.transform.rotation *= Quaternion.Euler(0, 90, 0);
            }
            else if (changeDiceValue == 5)
            {
                rotateDice.transform.rotation *= Quaternion.Euler(0, 0, 0);
            }
            else if (changeDiceValue == 6)
            {
                rotateDice.transform.rotation *= Quaternion.Euler(-180, 0, 0);
            }
        }
        else if (diceValue == 6)
        {
            if (changeDiceValue == 1)
            {
                rotateDice.transform.rotation *= Quaternion.Euler(180, 0, 0);
            }
            else if (changeDiceValue == 2)
            {
                rotateDice.transform.rotation *= Quaternion.Euler(0, 0, -90);
            }
            else if (changeDiceValue == 3)
            {
                rotateDice.transform.rotation *= Quaternion.Euler(90, 0, 0);
            }
            else if (changeDiceValue == 4)
            {
                rotateDice.transform.rotation *= Quaternion.Euler(-90, 0, 0);
            }
            else if (changeDiceValue == 5)
            {
                rotateDice.transform.rotation *= Quaternion.Euler(0, 0, 90);
            }
            else if (changeDiceValue == 6)
            {
                rotateDice.transform.rotation *= Quaternion.Euler(0, 0, 0);
            }
        }
    }
}
