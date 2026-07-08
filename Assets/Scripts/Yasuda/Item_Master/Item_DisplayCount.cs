using TMPro;
using UnityEngine;

public class Item_DisplayCount : MonoBehaviour
{
    [SerializeField]
    private TMP_Text m_NokoriItemCount;
    private int m_ItemCount = 0;

    public void SetItemCount(int itemCount)
    {
        //ここで出目の合計を受け取る
        m_ItemCount = itemCount;
        SetText();
    }
    public void UseItemCount()
    {
        //ここで出目の合計を受け取る
        m_ItemCount--;
        SetText();
    }

    private void SetText()
    {
        m_NokoriItemCount.text = m_ItemCount.ToString();
    }
}
