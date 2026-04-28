//using UnityEngine;


//public class DiceLogic : MonoBehaviour
//{
//    public float liftAmount = 1.0f; 
//    private bool isUp = false;
//    private Vector3 originalPos;

//    void Start()
//    {
//        originalPos = transform.position;
//    }


//    public void ToggleRiseFall()
//    {
//        if (!isUp)
//        {

//            transform.position = originalPos + new Vector3(0, liftAmount, 0);
//            //isUp = true;
//        }
//        else
//        {

//            transform.position = originalPos;
//            isUp = false;
//        }
//    }

//    void Update()
//    {

//        transform.Rotate(Vector3.up * 30 * Time.deltaTime);
//        transform.Rotate(Vector3.right * 15 * Time.deltaTime);
//    }
//}
using UnityEngine;
using System.Collections; 

public class DiceLogic : MonoBehaviour
{
    public float liftAmount = 0.6f;
    public float duration = 0.2f;    

    private bool isUp = false;
    private Vector3 originalPos;
    private Coroutine currentRoutine; 

    void Start()
    {
        originalPos = transform.position;
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
        Vector3 startPos = transform.position;

        while (elapsed < duration)
        {
           
            transform.position = Vector3.Lerp(startPos, target, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        transform.position = target; 
    }

    void Update()
    {
       
        transform.Rotate(Vector3.up * 40 * Time.deltaTime);
    }
}