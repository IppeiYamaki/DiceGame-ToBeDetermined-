using UnityEngine;
using UnityEngine.UI;

public class DetailDice : MonoBehaviour
{
    public Image[] detailImage;
    public Sprite[] detailSprites;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        HideDetail();
    }

    public void ShowDetail(int id)//サイコロからデータを取るように変更したい
    {
        Debug.Log("ShowDetail 呼び出し : " + id);
        for (int i = 0; i < 6; i++)
        {
            detailImage[i].sprite = detailSprites[id];
            detailImage[i].gameObject.SetActive(true);
        }
        
    }
    public void HideDetail()
    {
        for (int i = 0; i < 6; i++)
        {
            detailImage[i].gameObject.SetActive(false);
        }
    }
}
