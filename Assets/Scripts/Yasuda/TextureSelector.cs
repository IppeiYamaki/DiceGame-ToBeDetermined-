using UnityEngine;
using UnityEngine.UI;

public class TextureSelector : MonoBehaviour
{
    public static TextureSelector Instance;
    public Image[] images;

    [Range(0f, 1f)]
    public float unselectedBrightness = 0.4f;

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
        currentSelected = id;
       
        for (int i = 0; i < images.Length; i++)
        {

            if(i == id)
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
