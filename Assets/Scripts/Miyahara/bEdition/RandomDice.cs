using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;




public class RandomDice : MonoBehaviour
{
    //private

    private Rigidbody rb; // Rigidbodyコンポーネントへの参照
    private bool notLooped = false; // ドロップ開始後の一度だけの処理を制御するフラグ
    private bool notLooped2 = false; // サイコロが停止していないかどうかのフラグ
    private float stepIntervalTime = 0.1f; // サイコロが停止していると判定するまでの時間の間隔
    private int changeDiceValue = 0; // 出したい目

    //public
    public List<int> DiceFace = new List<int>(); // サイコロの目の値を格納するリスト
    public Vector3 spawn = new Vector3(-4, 5, 0);   // 出現位置
    public float rotateSpeed = 1f;// 回転の速さ
    public float stepInterval = 0.5f; // サイコロが停止していると判定するまでのフレーム数の間隔
    public int useIdIndex = 0;//ダイスの録画IDのインデックスを指定するための変数
    public int recordingIdIndex = 0;
    public bool isNextStep = false; // 次のステップに進むかどうかのフラグ


    [Header("オブジェクト参照用")]
 
    public GameObject rotateDice; // 回転するサイコロのゲームオブジェクトへの参照
    public DiceDefinition diceDefinition; // DiceDefinitionへの参照
    public TMP_Text randomText;// サイコロの目の値を表示するテキスト
    public DiceRole role; // DiceRoleコンポーネントへの参照
    public DiceRecorder diceRecorder;  // 録画用コンポーネント
    public DiceFaceTexture diceFaceTexture; // サイコロの目のテクスチャを管理するコンポーネントへの参照


    [Header("デバッグ用")]
    [SerializeField]
    private int notStoppedDice = 0; // サイコロが停止していないフレーム数のカウンタ
    [SerializeField]
    private bool isRecording = false; // 録画中かどうかのフラグ(デバッグ用)
    [SerializeField]
    private int stopCount = 0;// サイコロが停止しているフレーム数のカウンタ
    [SerializeField]
    private Vector3 torque = new Vector3(1, 1, 1);  // 回転軸

    

    public bool isStopped = false; // サイコロが停止しているかどうかのフラグ(別スクリプト判定用)
    public int diceValue;// サイコロの目の結果を格納する変数

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

        DiceDefault();

        diceFaceTexture.SetDiceFaceTexture();

        recordingIdIndex = 0;
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
                DiceDefault();
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
                if (notLooped2 && role.isPlaying)
                {
                    PlayDiceRotate((int)GetRecordingId(role));
                    notLooped2 = false;
                }
                break;
            case DiceState.Stopped:
                //Debug.Log("ダイスナンバー" + debugtako +"もとの番号"+ (int)GetRecordingId(role) + "変えたいダイスナンバー" + changeDiceValue + "変わった番号" + diceValue);
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
                    break;
                case DiceState.NextEvent:
                    state = DiceState.Idle;
                    break;
            }

            if (diceRecorder != null && isRecording == true)
                diceRecorder.StartRecording();
        }

        //if (Keyboard.current.rKey.wasPressedThisFrame)
        //{
        //    isRecording = !isRecording;
        //}



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

    private int GetRoleIdIndex(DiceRole r)
    {
        return useIdIndex switch
        {
            0 => (int)r.rolevalue.x,
            1 => (int)r.rolevalue.y,
            2 => (int)r.rolevalue.z,
            _ => (int)r.rolevalue.x
        };
    }

    // 停止中のステータス処理
    void IdolDice()
    {
        diceFaceTexture.SetDiceFaceTexture();
        stepIntervalTime += Time.deltaTime;
        if(stepIntervalTime >= stepInterval)
        {
            isNextStep = true;
        }

        if (notLooped)
        {


            recordingIdIndex += 1;
            if (recordingIdIndex > 5)
                recordingIdIndex = 0;

            rb.isKinematic = false;
            this.transform.position = spawn;
            this.transform.rotation = Quaternion.Euler(0, 0, 0);
            rb.constraints = RigidbodyConstraints.None;
            rb.constraints = RigidbodyConstraints.FreezePosition;

            float rotatex = Random.Range(-3f, 3f);
            float rotatey = Random.Range(1f, 3f);
            float rotatez = Random.Range(-3f, 3f);

            torque = new Vector3(rotatex, rotatey, rotatez);
            //rb.AddTorque(torque * rotateSpeed, ForceMode.Force);
            notLooped = false;
            stopCount = 0;
            randomText.text = 0.ToString();
            isStopped = false;
            changeDiceValue = GetRoleIdIndex(role);
            Debug.Log(GetRoleIdIndex(role));
        }
    }

    // サイコロを落とした時のステータス処理
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

    // 録画を再生したときのステータス処理
    void RecordPlaying()
    {
        notLooped = true;
    }
    // サイコロが停止しているときのステータス処理
    void StopDice()
    {
        stopCount = 0;
        notStoppedDice = 0;
    }

    // 次のイベントに移行するステータス処理
    void NextEvent()
    {
        isStopped = false;
        stopCount = 0;
        notStoppedDice = 0;
        stepIntervalTime = 0;
        isNextStep = false;
    }



    //サイコロの目が決まったときのイベント処理
    public void Dice1Event()
    {
        stopCount++;
        if (stopCount > 30)
        {
            randomText.text = DiceFace[0].ToString();
            diceValue = DiceFace[0];
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
            randomText.text = DiceFace[1].ToString();
            diceValue = DiceFace[1];
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
            randomText.text = DiceFace[2].ToString();
            diceValue = DiceFace[2];
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
            randomText.text = DiceFace[3].ToString();
            diceValue = DiceFace[3];
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
            randomText.text = DiceFace[4].ToString();
            diceValue = DiceFace[4];
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
            randomText.text = DiceFace[5].ToString();
            diceValue = DiceFace[5];
            isStopped = true;
            state = DiceState.Stopped;
            if (diceRecorder != null && isRecording == true)
                diceRecorder.StopRecording();
        }
    }

    //振る前にサイコロの目を変えるための関数
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
                rotateDice.transform.rotation *= Quaternion.Euler(0, 0, 180);
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
                rotateDice.transform.rotation *= Quaternion.Euler(0, 0, -90);
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

    void DiceDefault()
    {
        DiceFace.Clear();
        for (int i = 0; i < 6; i++)
        {
            DiceFace.Add(0);
        }


        List<int> remainNumbers = new List<int>();

        // 対応位置に入れる
        for (int i = 0; i < diceDefinition.Faces.Count; i++)
        {
            int number = diceDefinition.Faces[i].Number;
            int targetIndex = number - 1;

            // 対応位置が空いている
            if (DiceFace[targetIndex] == 0)
            {
                DiceFace[targetIndex] = number;
            }
            else
            {
                remainNumbers.Add(number);
            }
        }

        // 残りを空いている場所へ入れる
        int remainIndex = 0;

        for (int i = 0; i < DiceFace.Count; i++)
        {
            if (DiceFace[i] == 0)
            {
                DiceFace[i] = remainNumbers[remainIndex];
                remainIndex++;
            }
        }
    }
}
