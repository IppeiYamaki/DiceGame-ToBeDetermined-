
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

    public void ShowDetail(int id)
    {
        Debug.Log("ShowDetail ŒÄ‚Ño‚µ : " + id);
        if (id>detailObjects.Length)
        {
            Debug.LogError("id‚ª“o˜^”‚æ‚è‘å‚«‚¢‚Å‚·");
            return;
        }

        if(currentID == id)
        {
            detailObjects[id].SetActive(false);
            currentID = -1;
        }
        else 
        {
            for (int i = 0; i < detailObjects.Length; i++)
            {


                if (i == id)
                {
                    detailObjects[i].SetActive(true);
                    currentID = i;
                }
                else
                {
                    detailObjects[i].SetActive(false);
                }
            }
        }

           
    }

}
