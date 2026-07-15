using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Yasuda版ItemDataと効果情報(ItemEffectEntry)の対応表を保持するScriptableObject。
/// ItemData自体は効果データを持たないため、戦闘での効果解決はこのテーブル経由で行います。
///
/// 作成方法:
/// Project ビュー右クリック → Create → DiceGame/ItemEffectTable
///
/// 運用:
/// - 新しいItemDataを作成したら、このテーブルへエントリを1件追加して効果を設定します
/// - RunPlayerManager の Inspector にこのアセットを登録します
/// </summary>
[CreateAssetMenu(fileName = "ItemEffectTable", menuName = "DiceGame/ItemEffectTable")]
public class ItemEffectTable : ScriptableObject
{
    [SerializeField]
    [Header("アイテム効果一覧")]
    [Tooltip("ItemData 1件につき1エントリを登録します")]
    private List<ItemEffectEntry> m_entries = new List<ItemEffectEntry>();

    private Dictionary<ItemData, ItemEffectEntry> m_entryByItem;

    /// <summary>登録済みエントリ一覧(読み取り専用)。</summary>
    public IReadOnlyList<ItemEffectEntry> Entries => m_entries;

    /// <summary>
    /// ItemDataに対応する効果エントリを取得します。
    /// </summary>
    public bool TryGetEntry(ItemData item, out ItemEffectEntry entry)
    {
        entry = null;
        if (item == null) return false;

        EnsureLookup();
        if (m_entryByItem.TryGetValue(item, out entry) && entry != null)
        {
            return true;
        }

        Debug.LogWarning($"[ItemEffectTable] ItemData '{item.itemName}' (ID={item.ItemID}) の効果エントリが未登録です。");
        return false;
    }

    /// <summary>
    /// ItemDataに対応する効果エントリを取得します(未登録の場合はnull)。
    /// </summary>
    public ItemEffectEntry GetEntry(ItemData item)
    {
        return TryGetEntry(item, out ItemEffectEntry entry) ? entry : null;
    }

    private void EnsureLookup()
    {
        if (m_entryByItem != null) return;

        m_entryByItem = new Dictionary<ItemData, ItemEffectEntry>();
        var seenIds = new HashSet<int>();

        foreach (ItemEffectEntry entry in m_entries)
        {
            if (entry == null || entry.Item == null) continue;

            if (m_entryByItem.ContainsKey(entry.Item))
            {
                Debug.LogError($"[ItemEffectTable] ItemData '{entry.Item.itemName}' のエントリが重複しています。最初のエントリを使用します。");
                continue;
            }

            if (!seenIds.Add(entry.Item.ItemID))
            {
                Debug.LogWarning($"[ItemEffectTable] ItemID={entry.Item.ItemID} が複数のItemDataで重複しています。ItemDataアセットのIDを確認してください。");
            }

            m_entryByItem.Add(entry.Item, entry);
        }
    }

    private void OnEnable()
    {
        m_entryByItem = null;
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        m_entryByItem = null;
    }
#endif
}
