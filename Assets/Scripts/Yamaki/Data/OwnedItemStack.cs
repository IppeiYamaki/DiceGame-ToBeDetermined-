using System;
using UnityEngine;

/// <summary>
/// プレイヤーが所持するアイテム1枠(スタック)を表します。
/// 同一のItemDataは1枠にまとめ、所持数(Count)で管理します。
/// </summary>
[Serializable]
public class OwnedItemStack
{
    [SerializeField]
    [Header("アイテム(Yasuda版 ItemData)")]
    private ItemData m_item;

    [SerializeField]
    [Header("所持数")]
    [Min(1)]
    private int m_count = 1;

    /// <summary>所持しているItemData。</summary>
    public ItemData Item => m_item;

    /// <summary>所持数。</summary>
    public int Count => m_count;

    public OwnedItemStack(ItemData item, int count = 1)
    {
        m_item = item;
        m_count = Mathf.Max(1, count);
    }

    /// <summary>所持数を1増やします。</summary>
    public void Increment()
    {
        m_count++;
    }

    /// <summary>所持数を1減らし、残数を返します。</summary>
    public int Decrement()
    {
        m_count = Mathf.Max(0, m_count - 1);
        return m_count;
    }

    /// <summary>所持数を有効範囲(1以上)へ補正します(Inspector編集用)。</summary>
    public void NormalizeCount()
    {
        m_count = Mathf.Max(1, m_count);
    }
}
