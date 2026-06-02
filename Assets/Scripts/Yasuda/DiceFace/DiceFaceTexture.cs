using UnityEngine;
using UnityEngine.UI;

public class DiceFaceTexture : MonoBehaviour
{
    public SpriteRenderer[] detailImage;
    public Sprite[] detailSprites;

    
    private int number = 0;

    void Start()
    {
        for (int i = 0; i < detailImage.Length; i++)
        {
            //デフォルトスプライトを設定
            detailImage[i].sprite = detailSprites[i];
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {

            if (number < 0)
            {
                for (int i = 0; i < detailImage.Length; i++)
                {
                    //スプライト統一
                    detailImage[i].sprite = detailSprites[i];
                }
            }
            else
            {
                for (int i = 0; i < detailImage.Length; i++)
                {
                    //スプライト統一
                    detailImage[i].sprite = detailSprites[number];
                }
            }

            number++;
            if(number >= detailImage.Length)
            {
                number = -1;
            }
        }
    }
}
