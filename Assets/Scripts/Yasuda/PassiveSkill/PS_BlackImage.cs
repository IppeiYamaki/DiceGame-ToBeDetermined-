using UnityEngine;
using UnityEngine.UI;

public class PS_BlackImage : MonoBehaviour
{
    //ここでのPSはPassiveSkillのことを指す。このシーンだけ使う

    private Image m_image;

    //決定ボタンのみtrueにする
    public bool m_selectImage = false;

    void Start()
    {
        if(m_selectImage)
        {
            m_image = GetComponent<Image>();
            Color color = m_image.color;
            color.a = 0.7f;
            m_image.color = color;
        }
        else
        {
            m_image = GetComponent<Image>();
            Color color = m_image.color;
            color.a = 0.0f;
            m_image.color = color;
        }
       
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
            color.a = 0.7f;
        }
        m_image.color = color;
    }


 
}
