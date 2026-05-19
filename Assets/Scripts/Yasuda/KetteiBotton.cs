using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;

public class KetteiBotton : MonoBehaviour, IPointerClickHandler
{
    //決定ボタン


    [SerializeField]
    Test_DiceData Dicedata;


#if UNITY_EDITOR
    //デバッグ用
    [SerializeField]
    DetailDice D;

    private void DebugTest()
    {
        D.HideDetail(0);
        D.HideDetail(1);

        TextureSelector_N.Instance.ReSetSelectID();
        TextureSelector_O.Instance.ReSetSelectID();
    }
#endif



    public void OnPointerClick(PointerEventData eventData)//対象Imageをクリックすると実行
    {
        int id1 = TextureSelector_N.Instance.GetSelectID();
        int id2 = TextureSelector_O.Instance.GetSelectID();

        if (id1 == -1 || id2== -1)
        {
            
            Debug.LogWarning("サイコロを選択してください");
        }
        else
        {
            Debug.Log($"{id1},{id2}");
            Dicedata.SetNumber_Test( id1, id2 );

#if UNITY_EDITOR
            //デバッグ用
            DebugTest();
#endif

            //ここでマップに移行
        }
            
    }

    
}
