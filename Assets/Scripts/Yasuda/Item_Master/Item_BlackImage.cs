using UnityEngine;
using UnityEngine.UI;

public class Item_BlackImage : MonoBehaviour
{
    private Image m_image;
    private int m_itemId = -1;

   
    public void SetItemNumber(int id)
    {
        m_itemId = id;
        Debug.Log(id+"‚¾‚æ");

        m_image = GetComponent<Image>();
        Color color = m_image.color;
        if (Item_DataMaster.Instance.GetItemCount(m_itemId) > 0)
        {
            //Debug.Log("true‚¾‚æ");
            color.a = 0.0f;
        }
        else
        {
            //Debug.Log("false‚¾‚æ");
            color.a = 0.8f;
        }
        m_image.color = color;
    }


    public void AlphaChangeImage(bool can)
    {
        Color color = m_image.color;
        if (can)
        {
            color.a = 0.0f;
        }
        else
        {
            color.a = 0.8f;
        }
        m_image.color = color;
    }





}
