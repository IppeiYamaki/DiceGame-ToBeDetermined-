using UnityEngine;
using UnityEngine.InputSystem;

public class RandomDice : MonoBehaviour
{
    [SerializeField] private Vector3 torque = new Vector3(1, 1, 1);  // âÒì]é≤
    [SerializeField] private Vector3 spawn = new Vector3(-4, 5, 0);   // èoåªà íu

    private Rigidbody rb;
    private bool isShaking = true;
    private bool isDropping = false;

    public float rotateSpeed = 1f;


    void Start()
    {
        this.transform.position = spawn;
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Keyboard.current.spaceKey.wasPressedThisFrame)
            isDropping = false;
        if (Keyboard.current.rKey.wasPressedThisFrame)
            isShaking = true;
        DropDice();
        ShakeDice();

    }
    void ShakeDice()
    {
        if (isShaking)
        {
            this.transform.position = spawn;
            rb.AddTorque(torque * rotateSpeed, ForceMode.Force);
            isDropping = true;
            isShaking = false;

        }
    }
    void DropDice()
    {
        if (isDropping)
            rb.useGravity = false;
        else
            rb.useGravity = true;
    }


}
