
using UnityEngine;
using UnityEngine.UI;

public class LookFaces : MonoBehaviour
{
    //public static LookFaces Instance;
    public GameObject[] detailObjects;

    private int currentID = -1;

    void Start()
    {
        for (int i = 0; i < detailObjects.Length; i++)
        {
            detailObjects[i].SetActive(false);
        }
    }

    public void ShowDetail(int id)//idに応じて表示する
    {
        Debug.Log("ShowDetail 呼び出し : " + id);
        if (id > detailObjects.Length || id < 0)
        {
            Debug.LogError("idが無効です");
            return;
        }

        if(currentID == id)
        {
            for (int i = 0; i < detailObjects.Length; i++)
            {
                detailObjects[i].SetActive(false);
                currentID = -1;
               
            }
            Debug.Log("非表示化");
        }
        else 
        {
            for (int i = 0; i < detailObjects.Length; i++)
            {
                if (i == id)
                {
                    detailObjects[i].SetActive(true);
                    currentID = i;
                    Debug.Log(i + "表示");
                }
                else
                {
                    detailObjects[i].SetActive(false);
                }
            }
        }   
    }

    public void HideDetail()
    {
        for (int i = 0; i < detailObjects.Length; i++)
        {
            detailObjects[i].SetActive(false);
            currentID = -1;
        }
    }
}
