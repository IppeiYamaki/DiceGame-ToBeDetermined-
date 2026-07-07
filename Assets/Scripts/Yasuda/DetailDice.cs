using UnityEngine;
using UnityEngine.UI;

public class DetailDice : MonoBehaviour
{
    //選んだサイコロの面情報を表示する


    public Image[] detailImage;
    public Image[] detailImage2;
    public Sprite[] detailSprites;

   
    public Test_DiceData test;//データ獲得
   
    void Start()
    {
        HideDetail(0);
        HideDetail(1);
    }

    public void ShowDetail(int id, int group)//サイコロからデータを取るように変更したい
    {
        Debug.Log("ShowDetail 呼び出し : " + id);

        if(group==0)
        {
            for (int i = 0; i < detailImage.Length; i++)
            {
                int num = test.GetNumber(group, id, i);//この部分でサイコロの面データ（数字）を受け取る
                detailImage[i].sprite = detailSprites[num];//数字にそったスプライトにする
                detailImage[i].gameObject.SetActive(true);
            }
        }
        else
        {
            for (int i = 0; i < detailImage2.Length; i++)
            {
                int num = test.GetNumber(group, id, i);//この部分でサイコロの面データ（数字）を受け取る
                detailImage2[i].sprite = detailSprites[num];//数字にそったスプライトにする
                detailImage2[i].gameObject.SetActive(true);
            }
        }

            
        
    }
    public void HideDetail(int g)
    {
        if(g==0)
        {
            for (int i = 0; i < detailImage.Length; i++)
            {
                detailImage[i].gameObject.SetActive(false);
               
            }
        }
        else
        {
            for (int i = 0; i < detailImage2.Length; i++)
            {
                
                detailImage2[i].gameObject.SetActive(false);
            }
        }
       
    }
}
