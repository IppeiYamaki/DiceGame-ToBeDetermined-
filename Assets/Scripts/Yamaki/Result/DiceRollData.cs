using System;

/// <summary>
/// 1個のダイスを振った結果を表すデータ
/// ロール後の面インデックスと目の値を保持します
/// 
/// 使用例:
/// - ダイスID: "abc123..."
/// - 出た面のインデックス: 2（3面目）
/// - 出た数字: 3
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

    // ─────────────────────────────────────────────────────────
    // コンストラクタ
    // ─────────────────────────────────────────────────────────

    /// <summary>
    /// DiceRollData を生成します
    /// </summary>
    /// <param name="diceId">振ったダイスの永続 ID</param>
    /// <param name="faceIndex">出た面のインデックス（0～5）</param>
    /// <param name="number">出た面の数字</param>
    public DiceRollData(string diceId, int faceIndex, int number)
    {
        DiceId = diceId;
        FaceIndex = faceIndex;
        Number = number;
    }
}
