using UnityEngine;
using System.Collections;
using TMPro;

public class DiceItem : MonoBehaviour
{
    [Header("UI Component")]
    public TextMeshPro numberText; 

    [Header("Spin Settings")]
    public float rollSpinSpeed = 1000f;  
    public float idleSpinSpeed = 40f;  
    public float liftAmount = 0.6f;     

    private bool isRolling = false;
    private bool isUp = false;

    private Vector3 originalPos;
    private Coroutine currentRoutine;

    public bool IsRaised => isUp; 

    void Start()
    {
        originalPos = transform.position;
        if (numberText != null) numberText.text = ""; 
    }

    void Update()
    {
   
        if (!isRolling)
        {
            transform.Rotate(Vector3.up * idleSpinSpeed * Time.deltaTime);
        }
    }

   
    public void ToggleRiseFall()
    {
        if (currentRoutine != null) StopCoroutine(currentRoutine);
        isUp = !isUp;
        Vector3 targetPos = isUp ? originalPos + new Vector3(0, liftAmount, 0) : originalPos;
        currentRoutine = StartCoroutine(SmoothMove(targetPos));
    }

    IEnumerator SmoothMove(Vector3 target)
    {
        float elapsed = 0;
        float duration = 0.2f;
        Vector3 startPos = transform.position;
        while (elapsed < duration)
        {
            transform.position = Vector3.Lerp(startPos, target, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        transform.position = target;
    }
}