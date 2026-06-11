using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// HP帯ごとの行動セットを表す Serializable クラス
/// HP の割合（%）で範囲を指定し、その範囲内での行動グループリストを定義します
/// 
/// 使い方:
/// - EnemyDefinition のリスト内で使用します
/// - UpperPercent と LowerPercent で HP 範囲を指定します（例: 100～51, 50～20, 19～0）
/// - ActionGroups にウェイト付き行動グループを追加します
/// 
/// 例:
/// - UpperPercent: 100, LowerPercent: 51 → HP 51%～100% の時の行動
/// - UpperPercent: 50, LowerPercent: 0 → HP 0%～50% の時の行動
/// </summary>
[System.Serializable]
public class EnemyHpRangeActionSet
{
    [SerializeField]
    [Header("HP上限（%）")]
    [Tooltip("このHP帯の上限パーセント（0～100）\n" +
             "例: 100 なら「HP満タン時も含む」")]
    [Range(0, 100)]
    private int m_upperPercent = 100;

    [SerializeField]
    [Header("HP下限（%）")]
    [Tooltip("このHP帯の下限パーセント（0～100）\n" +
             "例: 0 なら「HP 0 の瞬間も含む」")]
    [Range(0, 100)]
    private int m_lowerPercent = 0;

    [SerializeField]
    [Header("行動グループリスト")]
    [Tooltip("この HP 帯で抽選される行動グループのリスト\n" +
             "各グループのウェイト値に応じて1つが選ばれます")]
    private List<EnemyWeightedActionGroup> m_actionGroups = new List<EnemyWeightedActionGroup>();

    // ─────────────────────────────────────────────────────────
    // 読み取り専用プロパティ
    // ─────────────────────────────────────────────────────────

    /// <summary>
    /// HP上限パーセント（0～100）
    /// </summary>
    public int UpperPercent => m_upperPercent;

    /// <summary>
    /// HP下限パーセント（0～100）
    /// </summary>
    public int LowerPercent => m_lowerPercent;

    /// <summary>
    /// この HP 帯で使用する行動グループのリスト（読み取り専用）
    /// </summary>
    public IReadOnlyList<EnemyWeightedActionGroup> ActionGroups => m_actionGroups;

    // ─────────────────────────────────────────────────────────
    // エディタ用範囲補正セッター（internal）
    // ─────────────────────────────────────────────────────────

    /// <summary>
    /// HP上限を設定します（エディタスクリプト専用）
    /// </summary>
    /// <param name="value">新しい上限パーセント（0～100）</param>
    internal void SetUpperPercent(int value)
    {
        m_upperPercent = Mathf.Clamp(value, 0, 100);
    }

    /// <summary>
    /// HP下限を設定します（エディタスクリプト専用）
    /// </summary>
    /// <param name="value">新しい下限パーセント（0～100）</param>
    internal void SetLowerPercent(int value)
    {
        m_lowerPercent = Mathf.Clamp(value, 0, 100);
    }
}
