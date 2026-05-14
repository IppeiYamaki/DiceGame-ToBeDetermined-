using UnityEngine;
using UnityEngine.UI;

public class DetailDice : MonoBehaviour
{
    public Image[] detailImage;
    public Sprite[] detailSprites;

    public Test_DiceData test;//データ獲得
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        HideDetail();
    }

    public void ShowDetail(int id, int group)//サイコロからデータを取るように変更したい
    {
        Debug.Log("ShowDetail 呼び出し : " + id);
        for (int i = 0; i < 6; i++)
        {
            int num = test.GetNumber(group, id, i);//この部分でサイコロの面データ（数字）を受け取る
            detailImage[i].sprite = detailSprites[num];//数字にそったスプライトにする
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
