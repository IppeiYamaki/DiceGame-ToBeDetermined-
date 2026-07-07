using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(BoxCollider2D))]
public class Chest : MonoBehaviour
{
    [Header("宝箱画像")]
    [SerializeField] private Sprite m_closeSprite;
    [SerializeField] private Sprite m_openSprite;

    [Header("マネージャー")]
    [SerializeField] private ChestManager m_chestManager;

    [Header("アイテム獲得告知窓")]
    [SerializeField] private ItemGetUI m_itemGetUI;

    private SpriteRenderer m_spriteRenderer;

    // アイテム抽選クラス
    private LootChest m_lootChest = new LootChest();

    // この宝箱が開いたか
    private bool m_isOpened = false;

    private void Start()
    {
        m_spriteRenderer = GetComponent<SpriteRenderer>();

        if (m_closeSprite != null)
        {
            m_spriteRenderer.sprite = m_closeSprite;
        }
    }

    private void OnMouseDown()
    {

        // UIの上をクリックしているときは何もしない
        if (EventSystem.current.IsPointerOverGameObject())
            return;

        // この宝箱が開いている
        if (m_isOpened)
            return;

        // 他の宝箱が開いている
        if (!m_chestManager.CanOpenChest())
        {
            Debug.Log("すでに別の宝箱が開いています。");
            return;
        }

        OpenChest();
    }

    private void OpenChest()
    {
        m_isOpened = true;

        // 全体に通知
        m_chestManager.OpenedChest();

        // 画像変更
        if (m_openSprite != null)
        {
            m_spriteRenderer.sprite = m_openSprite;
        }

        // アイテム抽選
        LootChest.ItemId reward = m_lootChest.Open();

        m_itemGetUI.Show($"{reward} を獲得しました！！！");

        Debug.Log($"取得アイテム : {reward}");
    }
}