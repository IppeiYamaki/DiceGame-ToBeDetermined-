using UnityEngine;
using TMPro;

public class DiceItem : MonoBehaviour
{
    [Header("UI引用")]
    public TextMeshPro numberText; 

    [Header("設定パラメータ")]
    public float rollSpeed = 1000f;    
    public float idleSpinSpeed = 40f;  
    public float liftAmount = 0.6f;  

    private bool isUp = false;
    private bool canLottery = false;
    private bool isRolling = false;
    private bool isDecided = false;


    public bool IsRaised => isUp;

    void Start()
    {
        if (numberText != null) numberText.text = "";
    }


    public void ToggleRiseFall()
    {
        if (canLottery) return; 
        isUp = !isUp;

 
        transform.localPosition += isUp ? new Vector3(0, liftAmount, 0) : new Vector3(0, -liftAmount, 0);
    }

 
    public void PrepareForLottery()
    {
        canLottery = true;
    }

    void Update()
    {
      
        if (!isRolling)
        {
            transform.Rotate(Vector3.up * idleSpinSpeed * Time.deltaTime);
        }


        if (!canLottery) return;

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (!isRolling && !isDecided)
            {
                isRolling = true;
            }
            else if (isRolling)
            {
                isRolling = false;
                isDecided = true;
               
                transform.rotation = Quaternion.Euler(Random.Range(0, 4) * 90, Random.Range(0, 4) * 90, Random.Range(0, 4) * 90);
            }
        }

        if (isRolling)
        {
           
            transform.Rotate(new Vector3(1, 1, 1) * rollSpeed * Time.deltaTime);
            if (numberText != null) numberText.text = Random.Range(1, 7).ToString();
        }
    }
}