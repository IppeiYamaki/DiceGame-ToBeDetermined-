using UnityEngine;
using UnityEngine.EventSystems;


public class TextureClickUI_Beta : MonoBehaviour, IPointerClickHandler
{
    public int textureID, groupID;
    public DetailDice test;

    public void OnPointerClick(PointerEventData eventData)//対象Imageをクリックすると実行
    {
        if(groupID == 0)
        {
            TextureSelector_N.Instance.Select(textureID);
            
        }
        else
        {
            TextureSelector_O.Instance.Select(textureID);
     
        }

        //test.ShowDetail(textureID);
    }
}
