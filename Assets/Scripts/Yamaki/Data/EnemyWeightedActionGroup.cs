using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ウェイト付き行動グループを表す Serializable クラス
/// 複数の行動（攻撃&防御 等）を1つのグループとし、ウェイト抽選の単位とします
/// 
/// 使い方:
/// - EnemyHpRangeActionSet のリスト内で使用します
/// - Weight を設定してウェイト抽選の確率を決定します
/// - Actions に同時実行したい EnemyActionEntry を複数追加します
/// 
/// 例:
/// - Weight: 3, Actions: [攻撃] → 確率3の単純攻撃
/// - Weight: 1, Actions: [攻撃, 防御] → 確率1の攻撃&防御コンボ
/// </summary>
[System.Serializable]
public class EnemyWeightedActionGroup
{
    [SerializeField]
    [Header("ウェイト値")]
    [Tooltip("このグループが選ばれる確率の重み\n" +
             "全グループのウェイト合計に対する割合で抽選されます\n" +
             "例: グループA(3)、グループB(1) → Aが75%、Bが25%")]
    [Min(1)]
    private int m_weight = 1;

    [SerializeField]
    [Header("行動リスト")]
    [Tooltip("このグループで同時実行する行動のリスト\n" +
             "複数指定すると順次実行されます（攻撃＆防御、バフ＆攻撃 等）")]
    private List<EnemyActionEntry> m_actions = new List<EnemyActionEntry>();

    // ─────────────────────────────────────────────────────────
    // 読み取り専用プロパティ
    // ─────────────────────────────────────────────────────────

    /// <summary>
    /// ウェイト値（確率の重み）
    /// </summary>
    public int Weight => m_weight;

    /// <summary>
    /// このグループで実行する行動のリスト（読み取り専用）
    /// </summary>
    public IReadOnlyList<EnemyActionEntry> Actions => m_actions;
}
