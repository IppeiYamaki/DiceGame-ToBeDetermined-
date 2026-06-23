using UnityEngine;
using UnityEngine.UI;

public class DiceFaceTexture : MonoBehaviour
{
    public SpriteRenderer[] detailImage;
    public Sprite[] useSprites;

    public RandomDice randomDice;



    void Start()
    {

        for (int i = 0; i < detailImage.Length; i++)
        {
            //デフォルトスプライトを設定
            detailImage[i].sprite = useSprites[i];
            Debug.Log("スプライト設定完了: " + ((randomDice.diceDefinition.Faces[i].Number)-1));
        }
    }

    void Update()
    {

        for (int i = 0; i < detailImage.Length; i++)
        {
            //スプライト統一
            detailImage[i].sprite = useSprites[(randomDice.diceDefinition.Faces[i].Number) - 1];
        }
    }


}
