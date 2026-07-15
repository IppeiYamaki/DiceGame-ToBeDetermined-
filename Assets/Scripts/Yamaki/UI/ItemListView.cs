using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// アイテム一覧を表示するView。ItemButtonViewのPrefabを生成して配置します。
/// 同一アイテムは1ボタンにスタックし、残数(所持数-予約済み数)を表示します。
/// </summary>
public class ItemListView : MonoBehaviour
{
    [SerializeField]
    [Header("ItemButtonView Prefab")]
    private GameObject m_itemButtonPrefab;

    [SerializeField]
    [Header("配置先 Transform")]
    private Transform m_container;

    private readonly List<GameObject> m_spawned = new List<GameObject>();

    public void Refresh(
        IReadOnlyList<OwnedItemStack> itemStacks,
        Func<ItemData, int> getReservedCount,
        Func<ItemData, bool> canUse,
        Action<ItemData> onUse)
    {
        foreach (var go in m_spawned)
        {
            if (go != null) Destroy(go);
        }
        m_spawned.Clear();

        if (itemStacks == null || m_itemButtonPrefab == null || m_container == null) return;

        for (int i = 0; i < itemStacks.Count; i++)
        {
            OwnedItemStack stack = itemStacks[i];
            if (stack == null || stack.Item == null) continue;

            int reserved = getReservedCount != null ? getReservedCount(stack.Item) : 0;
            int remainingCount = Mathf.Max(0, stack.Count - reserved);

            var go = Instantiate(m_itemButtonPrefab, m_container);
            var view = go.GetComponent<ItemButtonView>();
            if (view != null)
            {
                view.Setup(stack.Item, remainingCount, canUse, onUse);
            }
            m_spawned.Add(go);
        }
    }
}
