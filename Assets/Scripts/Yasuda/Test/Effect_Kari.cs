using UnityEngine;
using UnityEngine.UI;

public class Effect_Kari : MonoBehaviour
{
    
    private Color color_Kaihuku = Color.greenYellow;
    private Color color_Doku = Color.green;
    private Color color_Mahi = Color.yellow;
    private Color color_Hissatuwaza = Color.red;
    private Color color_Buff = Color.orange;

    private float colorA = 0.0f;

    private Image effect;
    private bool useEffect;

    private float effectTime = 0;
    [Header("エフェクト全体の時間（1 = 1秒）")]
    public float effectAllTime = 1.0f;

    

    void Start()
    {
        effect = GetComponent<Image>();
        useEffect = false;
    }

    
    void Update()
    {
        if(useEffect)
        {
            if (effectTime < effectAllTime)
            {
               //colorA += 0.5f / (effectAllTime / 2);
               //if (colorA >=0.5f)
               //{
               //    colorA = 0.5f;
               //}
               colorA = 0.3f;
            }
            //else if (effectTime < all)
            //{
            //   colorA -= 0.5f / (effectAllTime / 2);
            //   if (colorA <= 0.0f)
            //   {
            //       colorA = 0.0f;
            //   }
            //   
            //}
            else
            {
                colorA = 0.0f;
                useEffect = false;
            }
            effectTime += Time.deltaTime;
            Color color = effect.color;
            color.a = colorA;
            effect.color = color;
            //Debug.Log(colorA);
        }
    }

    public void SetEffect(int id)
    {
        Color color = effect.color;
        switch (id)
        {
            case 0:
                color = color_Kaihuku; break;
            case 1:
                color = color_Doku; break;
            case 2:
                color = color_Mahi; break;
            case 3:
                color = color_Hissatuwaza; break;
            case 4:
                color = color_Buff; break;
           
            default:
                color = Color.white; break;
        }   

        effect.color = color;
        useEffect = true;
        effectTime = 0;
    }

  

}
