using UnityEngine;
using UnityEngine.UI;

public class BlackImage : MonoBehaviour
{


    //スキル使えない時は暗くする。制作中
    private Image m_image;
    void Start()
    {
        m_image= GetComponent<Image>();
    }

    public void AlphaChangeImage(bool can)
    {
        Color color = m_image.color;
        if(can)
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
