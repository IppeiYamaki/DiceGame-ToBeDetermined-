using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class RotateSloat : MonoBehaviour
{
    public DiceManager diceManager;
    public float rotateSpeed = -3f;
    private void Start()
    {
        GameObject diceManagerObject = GameObject.Find("DiceManager");
        diceManager = diceManagerObject.GetComponent<DiceManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (diceManager.diceState == DiceManager.DiceState.Rotating)
        {
            Rotate();
        }
    }
    // ダイスを回転させるメソッド
    void Rotate()
    {
        float y = transform.eulerAngles.x;
        if (diceManager.isRotating)
        {
            Rigidbody rb = this.GetComponent<Rigidbody>();
            rb.angularVelocity = new Vector3(rotateSpeed, 0, 0);
        }
        else
        {
            Rigidbody rb = this.GetComponent<Rigidbody>();
            rb.angularVelocity = Vector3.zero;
        }

    }

}
