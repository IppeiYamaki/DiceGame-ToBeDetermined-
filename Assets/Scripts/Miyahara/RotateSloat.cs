using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateSloat : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        Rigidbody rb = this.GetComponent<Rigidbody>();
        Vector3 force = new Vector3(0.0f, 0.0f, 1.0f);    // óÕÇê›íË
        rb.MoveRotation(Quaternion.Euler(force * Time.deltaTime * 100.0f) * rb.rotation);
    }
}
