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

    private Rigidbody rb;
    private bool isShaking = true;
    private bool isDropping = false;
    private bool notLooped = false;

    public float rotateSpeed = 1f;
    public TMP_Text randomText;


    void Start()
    {
        this.transform.position = spawn;
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            isDropping = false;
            diceRecorder.StartRecording();
        }
        if (Keyboard.current.rKey.wasPressedThisFrame)
            isShaking = true;
        DropDice();
        ShakeDice();

    }
    void ShakeDice()
    {
        if (isShaking)
        {
            this.transform.rotation = Quaternion.Euler(0, 0, 0);
            this.transform.position = spawn;
            isDropping = true;
            isShaking = false;

        }
    }
    void DropDice()
    {
        if (isDropping)
        {
            rb.constraints = RigidbodyConstraints.FreezePosition;
            notLooped = true;
        }
        else
        {
            if (notLooped)
            {
                rb.constraints = RigidbodyConstraints.FreezeRotation;
                rb.constraints = RigidbodyConstraints.None;
                this.transform.position = spawn;
                this.transform.rotation = Quaternion.Euler(0, 0, 0);
                //int rotatex = 1;
                //int rotatey = 2;
                //int rotatez = 2;
                int rotatex = Random.Range(1, 3);
                int rotatey = Random.Range(1, 3);
                int rotatez = Random.Range(1, 3);

                torque = new Vector3(rotatex, rotatey, rotatez);
                rb.AddTorque(torque * rotateSpeed, ForceMode.Force);
                notLooped = false;
            }

        }
    }


    public void Dice1Event(Collider col)
    {
        Debug.Log("1");
        randomText.text = 1.ToString();
        diceRecorder.StopRecording();
    }

    public void Dice2Event(Collider col)
    {
        Debug.Log("2");
        randomText.text = 2.ToString();
        diceRecorder.StopRecording();
    }

    public void Dice3Event(Collider col)
    {
        Debug.Log("3");
        randomText.text = 3.ToString();
        diceRecorder.StopRecording();
    }

    public void Dice4Event(Collider col)
    {
        Debug.Log("4");
        randomText.text = 4.ToString();
        diceRecorder.StopRecording();
    }

    public void Dice5Event(Collider col)
    {
        Debug.Log("5");
        randomText.text = 5.ToString();
        diceRecorder.StopRecording();
    }

    public void Dice6Event(Collider col)
    {
        Debug.Log("6");
        randomText.text = 6.ToString();
        diceRecorder.StopRecording();
    }


}
