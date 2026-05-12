using UnityEngine;
using UnityEngine.EventSystems;


public class TextureClickUI_Beta : MonoBehaviour, IPointerClickHandler
{
    public int textureID, groupID;
    public LookFaces lookFaces;

    public void OnPointerClick(PointerEventData eventData)//対象Imageをクリックすると実行
    {
        if(groupID == 0)
        {
            TextureSelector_N.Instance.Select(textureID);
            lookFaces.ShowDetail(0);//面表示テスト
        }
        else
        {
            TextureSelector_O.Instance.Select(textureID);
            lookFaces.ShowDetail(1);
        }
            
    }
}
