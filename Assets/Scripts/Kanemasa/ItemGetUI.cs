using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemGetUI : MonoBehaviour
{
    [SerializeField] private GameObject m_panel;
    //[SerializeField] private Image m_itemIcon;
    [SerializeField] private TMP_Text m_itemName;
        private void Start()
    {
        m_panel.SetActive(false);
    }

    public void Show(string itemName/*, Sprite icon*/)
    {
        m_panel.SetActive(true);

        m_itemName.text = itemName;
        //m_itemIcon.sprite = icon;
    }

    public void Close()
    {
        m_panel.SetActive(false);
    }
}