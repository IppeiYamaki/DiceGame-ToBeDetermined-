using UnityEngine;

public class MoveDice : MonoBehaviour
{
    [SerializeField]
    private float moveAmount = 20f;

    [SerializeField]
    private float speed = 1f;

    [SerializeField]
    private float offSet = 0.0f;

    private bool moveFrag = true;
    private Vector3 startPos;

    void Start()
    {
        startPos = transform.localPosition;

        //Vector3 pos = transform.localPosition;
        //pos.y += offSet;
        //transform.localPosition = pos;
    }

    void Update()
    {
        if (moveFrag)
        {
            float y =
            Mathf.Sin(
                (Time.time + offSet)
                * speed)
            * moveAmount;

            transform.localPosition =
                startPos + new Vector3(0, y, 0);
        }
       

        if (Input.GetKeyDown(KeyCode.Space))
        {
            SwitchMove();
        }
    }

    public void SwitchMove()
    {
        moveFrag ^= true;
        Debug.Log("moveFrag = " + moveFrag);
    }
}
