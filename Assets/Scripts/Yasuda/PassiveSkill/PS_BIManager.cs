using UnityEngine;

public class PS_BIManager : MonoBehaviour
{
    [SerializeField]
    PS_BlackImage[] m_ps_BlackImages;


    public void SetAlpha(int number)
    {
        for(int i=0;i<m_ps_BlackImages.Length;i++)
        {
            if (i == number)
            {
                m_ps_BlackImages[i].AlphaChangeImage(true);
            }
            else
            {
                m_ps_BlackImages[i].AlphaChangeImage(false);
            }
        }
    }
}
