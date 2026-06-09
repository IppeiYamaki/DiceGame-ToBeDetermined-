using System;
using System.Collections.Generic;

/// <summary>
/// 1個のダイスを振った結果を表すデータ
/// ロール後の出目、数字、属性別の合計値を保持します
/// 
/// 使用例:
/// - ダイスID: "abc123..."
/// - 出た面のインデックス: 2（3面目）
/// - 出た数字: 3
/// - 属性別合計: { Attack: 1, Defense: 2 }
/// </summary>
[Serializable]
public struct DiceRollData
{
    // ─────────────────────────────────────────────────────────
    // フィールド
    // ─────────────────────────────────────────────────────────

    /// <summary>振ったダイスの永続 ID</summary>
    public string DiceId;

    /// <summary>出た面のインデックス（0～5）</summary>
    public int FaceIndex;

    /// <summary>出た面の数字（1～6）</summary>
    public int Number;

    /// <summary>出た面の属性別合計値（攻撃: 2、防御: 1 など）</summary>
    public Dictionary<DiceAttributeType, int> AttributeTotals;

    // ─────────────────────────────────────────────────────────
    // コンストラクタ
    // ─────────────────────────────────────────────────────────

    /// <summary>
    /// DiceRollData を生成します
    /// </summary>
    /// <param name="diceId">振ったダイスの永続 ID</param>
    /// <param name="faceIndex">出た面のインデックス（0～5）</param>
    /// <param name="number">出た面の数字（1～6）</param>
    /// <param name="attributeTotals">出た面の属性別合計値</param>
    public DiceRollData(string diceId, int faceIndex, int number, Dictionary<DiceAttributeType, int> attributeTotals)
    {
        DiceId = diceId;
        FaceIndex = faceIndex;
        Number = number;
        AttributeTotals = attributeTotals ?? new Dictionary<DiceAttributeType, int>();
    }
}
