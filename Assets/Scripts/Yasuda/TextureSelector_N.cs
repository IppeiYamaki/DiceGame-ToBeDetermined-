using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class TextureSelector_N : MonoBehaviour
{
    public static TextureSelector_N Instance;
  
    public Image[] images;

    public DetailDice dice;//サイコロの面（詳細）表示

    [Range(0f, 1f)]

   

    public float unselectedBrightness = 0.4f;//選択されていないサイコロの背景カラー   
    public float3 defaultColor = new Vector3(0.1f, 0.7f, 0.1f);//未選択時の背景カラー

    private int currentSelected = -1;


    void Awake()
    {
        Instance = this;
    }

    public int SelectID()
    {
        return currentSelected;
    }

    public void Select(int id)
    {
        if (currentSelected == id)
        {
            //同じサイコロを押したら詳細を消す
            currentSelected = -1;
            dice.HideDetail();
        }
        else
        {
            //詳細を表示
            currentSelected = id;
            dice.ShowDetail(currentSelected);
        }

       
        UpdateView();//表示更新

       
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
