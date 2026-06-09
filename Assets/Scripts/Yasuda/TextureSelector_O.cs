using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;

public class TextureSelector_O : MonoBehaviour
{
    //持ってるサイコロのピックアップ表示

    public static TextureSelector_O Instance;
   
    public Image[] images;

    public DetailDice dice;

    [Range(0f, 1f)]

   

    public float unselectedBrightness = 0.4f;
    public float3 defaultColor = new Vector3(0.1f, 0.7f, 0.1f);

    private int currentSelected = -1;


    void Awake()
    {
        Instance = this;
    }

    public int GetSelectID()
    {
        return currentSelected;
    }
    public void ReSetSelectID()
    {
        currentSelected = -1;
        UpdateView();//表示更新
    }

    public void Select(int id)
    {
        if (currentSelected == id)
        {
            currentSelected = -1;
            dice.HideDetail(1);
        }
        else
        {
            currentSelected = id;
            dice.ShowDetail(currentSelected,1);//1は既存サイコロ
        }
       
        UpdateView();

       
    }

    void UpdateView()
    {
        for (int i = 0; i < images.Length; i++)
        {
            if(currentSelected==-1)
            {
                images[i].color = new Color(
                    defaultColor.x,
                    defaultColor.y,
                    defaultColor.z,
                    1f);
            }
            else
            {
                if (i == currentSelected)
                {

                    images[i].color = Color.white;
                }
                else
                {
                    images[i].color = new Color(
                        unselectedBrightness,
                        unselectedBrightness,
                        unselectedBrightness,
                        1f);

                }
            }


           

        }
    }
}
