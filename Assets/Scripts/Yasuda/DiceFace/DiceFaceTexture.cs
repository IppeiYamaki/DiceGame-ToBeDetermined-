using UnityEngine;
using UnityEngine.UI;

public class DiceFaceTexture : MonoBehaviour
{
    public SpriteRenderer[] detailImage;
    public TextureDefinition[] diceDefinition;
    public RandomDice randomDice;

    [System.Serializable]
    public class TextureDefinition
    {
        public DiceDefinition diceDefinition;
        public Sprite[] useSprites;
    };


    void Start()
    {

    }

    void Update()
    {
    }

    //新しいダイスを取得したときに呼び出す
    public void SetDiceFaceTexture()
    {
        for (int i = 0; i < diceDefinition.Length; i++)
        {
            if (randomDice.diceDefinition == diceDefinition[i].diceDefinition)
            {
                for (int j = 0; j < detailImage.Length; j++)
                {

                    //スプライト統一
                    detailImage[j].sprite = diceDefinition[i].useSprites[(randomDice.DiceFace[j]) - 1];
                }
            }
        }
    }


}
