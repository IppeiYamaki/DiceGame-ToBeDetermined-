using UnityEngine;
using UnityEngine.UI;

public class DiceHyouzi : MonoBehaviour
{
    //選んだサイコロの面情報を表示する


    public Image[] detailImage;
    public Sprite[] detailSprites;


    public Test_DiceData test;//データ獲得

    void Start()
    {
        for (int i = 0; i < detailImage.Length; i++)
        {
            detailImage[i].sprite = detailSprites[0];
        }
           
    }

    public void ShowDetail(int i, int id)//サイコロからデータを取るように変更したい
    {
        

        //for (int i = 0; i < detailImage.Length; i++)
        //{
        //    int num = test.GetNumber(1, id, i);//この部分でサイコロの面データ（数字）を受け取る
        //    detailImage[i].sprite = detailSprites[num];//数字にそったスプライトにする
        //   
        //}


        detailImage[i].sprite = detailSprites[id];

    }
  
}
