using UnityEngine;
using UnityEngine.EventSystems;

public class KetteiBotton : MonoBehaviour, IPointerClickHandler
{

    public void OnPointerClick(PointerEventData eventData)//対象Imageをクリックすると実行
    {
        int id1 = TextureSelector_N.Instance.SelectID();
        int id2 = TextureSelector_O.Instance.SelectID();

        if (id1 == -1 || id2== -1)
        {
            Debug.LogWarning("サイコロを選択してください");
        }
        else
        {
            Debug.Log($"{id1},{id2}");
        }
            
    }
}
