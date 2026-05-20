using UnityEngine;
using UnityEngine.EventSystems;
public class KoukansinaiBotton : MonoBehaviour, IPointerClickHandler
{
    //交換しないボタン
    public void OnPointerClick(PointerEventData eventData)//対象Imageをクリックすると実行
    {
        Debug.Log("交換しない");
    }
}
