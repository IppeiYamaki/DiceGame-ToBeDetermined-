using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class RandomDice : MonoBehaviour
{
    
    [SerializeField] private Vector3 torque = new Vector3(1, 1, 1);  // 回転軸
    [SerializeField] private Vector3 spawn = new Vector3(-4, 5, 0);   // 出現位置
    [SerializeField] private DiceRecorder diceRecorder;  // 録画用コンポーネント
    public List<string> recordingId = new List<string>();

    private Rigidbody rb;
    [SerializeField] private int notStoppedDice = 0;
    public DiceRole role;
    private bool notLooped = false;
    [SerializeField]private bool isRecording = false;
    public bool isStopped = false;
    public int stopCount = 0;

    [HideInInspector] public int diceValue;
    public float rotateSpeed = 1f;
    public TMP_Text randomText;


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
        role = GetComponent<DiceRole>();
    }

    // Update is called once per frame
    void Update()
    {
        switch (state)
        {
            case DiceState.Idle:
                IdolDice();
                break;
            case DiceState.Dropping:
                DropDice();
                break;
            case DiceState.RecordPlaying:
                RecordPlaying();
                break;
            case DiceState.Stopped:
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
                //case DiceState.Dropping:
                //    state = DiceState.Stopped;
                //    break;
                //case DiceState.Stopped:
                //    state = DiceState.NextEvent;
                //    break;
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
    void IdolDice()
    {
        if (notLooped)
        {
            rb.isKinematic = false;
            this.transform.position = spawn;
            this.transform.rotation = Quaternion.Euler(0, 0, 0);
            rb.constraints = RigidbodyConstraints.None;
            rb.constraints = RigidbodyConstraints.FreezePosition;



            //int rotatex = 1;
            //int rotatey = 2;
            //int rotatez = 2;
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
    void RecordPlaying()
    {
        notLooped = true;
    }

    void StopDice()
    {
        stopCount = 0;
        notStoppedDice = 0;
    }

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
}
