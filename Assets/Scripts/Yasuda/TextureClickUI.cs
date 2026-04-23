using UnityEngine;
using UnityEngine.EventSystems;


public class TextureClickUI : MonoBehaviour, IPointerClickHandler
{
  

    public int textureID;

    public void OnPointerClick(PointerEventData eventData)//対象Imageをクリックすると実行
    {
        TextureSelector.Instance.Select(textureID);
    }
}
