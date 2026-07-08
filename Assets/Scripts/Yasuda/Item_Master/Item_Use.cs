using UnityEngine;
using UnityEngine.EventSystems;

public class Item_Use : MonoBehaviour, IPointerClickHandler
{
    [SerializeField]
    Item_BlackImage m_blackImage;
    [SerializeField]
    Item_BlackImage m_blackImage2;
    //[SerializeField]
    //Item_BlackImage_Number m_blackImage2;

    [SerializeField]
    Item_DisplayCount m_displayCount;

    public int m_itemId;

    void Start()
    {
        //Debug.Log("Item_UseのStart()");
        m_blackImage.SetItemNumber(m_itemId);
        m_blackImage2.SetItemNumber(m_itemId);
        m_displayCount.SetItemCount(Item_DataMaster.Instance.GetItemCount(m_itemId));
    }


    public void OnPointerClick(PointerEventData eventData)//対象Imageをクリックすると実行
    {

       
        if (Item_DataMaster.Instance.CheckUseItem(m_itemId))
        {
            int itemCount = Item_DataMaster.Instance.GetItemCount(m_itemId);
            m_displayCount.UseItemCount();
            if (itemCount <= 1)
            {
                m_blackImage.AlphaChangeImage(false);
                m_blackImage2.AlphaChangeImage(false);
            }
            Item_DataMaster.Instance.UseItem(m_itemId);
        }
        else
        {
            Debug.Log("使えない");
        }
       




    }
        
   
}
