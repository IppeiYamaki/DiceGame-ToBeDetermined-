using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 行動予約レーン全体を管理するView。
/// 予約エントリをコンテナへ上から順に動的生成します。
/// コンテナをScrollRectのContentにすることで、枠に収まらない場合はスクロールできます。
/// </summary>
public class ActionReservationLaneView : MonoBehaviour
{
    [SerializeField]
    [Header("予約エントリPrefab（ReservationEntryViewを想定）")]
    private GameObject m_entryPrefab;

    [SerializeField]
    [Header("エントリのコンテナTransform（ScrollRectのContentを想定）")]
    [Tooltip("VerticalLayoutGroup + ContentSizeFitter を付けた ScrollRect の Content を指定してください")]
    private Transform m_entryContainer;

    [SerializeField]
    [Header("ScrollRect（任意）")]
    [Tooltip("指定すると予約追加時に自動で最下部までスクロールします")]
    private ScrollRect m_scrollRect;

    [SerializeField]
    [Header("アイコン設定（BattleIconSettings）")]
    [Tooltip("行動/対象の汎用アイコン設定を割り当てます（Inspectorで設定してください）")]
    private BattleIconSettings m_iconSettings;

    private readonly List<GameObject> m_spawned = new List<GameObject>();

    /// <summary>
    /// 予約リストを表示し、各エントリのキャンセル要求はonCancelRequestedへインデックスで通知します。
    /// </summary>
    public void Refresh(IReadOnlyList<PlannedAction> actions, Action<int> onCancelRequested)
    {
        int previousCount = m_spawned.Count;

        // 既存の子を消す
        foreach (var go in m_spawned)
        {
            if (go != null) Destroy(go);
        }
        m_spawned.Clear();

        if (actions == null || m_entryPrefab == null || m_entryContainer == null) return;

        for (int i = 0; i < actions.Count; i++)
        {
            var go = Instantiate(m_entryPrefab, m_entryContainer);
            var view = go.GetComponent<ReservationEntryView>();
            if (view != null)
            {
                int idx = i;
                view.Setup(idx, actions[i], onCancelRequested, m_iconSettings);
            }
            m_spawned.Add(go);
        }

        // 予約が増えた場合は最下部（最新の予約）までスクロール
        if (m_scrollRect != null && actions.Count > previousCount && isActiveAndEnabled)
        {
            StartCoroutine(ScrollToBottomNextFrame());
        }
    }

    /// <summary>
    /// レイアウト再計算後（次フレーム）に最下部へスクロールします。
    /// </summary>
    private IEnumerator ScrollToBottomNextFrame()
    {
        yield return null;
        if (m_scrollRect != null)
        {
            m_scrollRect.verticalNormalizedPosition = 0f;
        }
    }
}
