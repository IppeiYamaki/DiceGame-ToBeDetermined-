using UnityEngine;
using UnityEngine.EventSystems;
public class KoukansinaiBotton : MonoBehaviour, IPointerClickHandler
{
    [SerializeField]
    SceneChangeManager manager;

    //交換しないボタン
    public void OnPointerClick(PointerEventData eventData)//対象Imageをクリックすると実行
    {
        Debug.Log("交換しない");
        //ここでマップに移行
        manager.ChangeScene();
    }
}
