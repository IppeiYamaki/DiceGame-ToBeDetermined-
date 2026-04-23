using UnityEngine;
using UnityEngine.EventSystems;

public class KetteiBotton : MonoBehaviour, IPointerClickHandler
{

    public void OnPointerClick(PointerEventData eventData)//対象Imageをクリックすると実行
    {
        int id = TextureSelector.Instance.SelectID();

        if(id == -1)
        {
            Debug.LogWarning("サイコロを選択してください");
        }
        else
        {
            Debug.Log(id);
        }
            
    }
}
