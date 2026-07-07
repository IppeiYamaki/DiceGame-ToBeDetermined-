using System;

/// <summary>
/// 3個のダイスをロールした全体結果を表すデータ
/// 各ダイスのロール結果、成立した役、出目合計、倍率適用後の最終値を保持します
/// 
/// 使用例:
/// - 各ダイスのロール結果: [ { 出目: 3 } , { 出目: 3 }, { 出目: 3 } ]
/// - 成立役: ゾロ目（倍率 2.0f）
/// - 出目合計（倍率適用前）: 9
/// - 最終値（倍率適用後）: 18
/// </summary>
[Serializable]
public struct DiceRollResult
{
    // ─────────────────────────────────────────────────────────
    // フィールド
    // ─────────────────────────────────────────────────────────

    /// <summary>各ダイスのロール結果（3個分）</summary>
    public DiceRollData[] RollData;

    /// <summary>成立した役の判定結果</summary>
    public DiceRoleEvaluateResult EvaluateResult;

    /// <summary>出目合計（倍率適用前）</summary>
    public int TotalNumber;

    /// <summary>最終値（倍率適用後）</summary>
    public int FinalValue;

    // ─────────────────────────────────────────────────────────
    // コンストラクタ
    // ─────────────────────────────────────────────────────────

    /// <summary>
    /// DiceRollResult を生成します
    /// </summary>
    /// <param name="rollData">各ダイスのロール結果（3個分）</param>
    /// <param name="evaluateResult">成立した役の判定結果</param>
    /// <param name="totalNumber">出目合計（倍率適用前）</param>
    /// <param name="finalValue">最終値（倍率適用後）</param>
    public DiceRollResult(
        DiceRollData[] rollData,
        DiceRoleEvaluateResult evaluateResult,
        int totalNumber,
        int finalValue)
    {
        RollData = rollData ?? new DiceRollData[3];
        EvaluateResult = evaluateResult;
        TotalNumber = totalNumber;
        FinalValue = finalValue;
    }

    // ─────────────────────────────────────────────────────────
    // ユーティリティプロパティ
    // ─────────────────────────────────────────────────────────

    /// <summary>
    /// 役が成立したかどうか
    /// </summary>
    public bool IsRoleMatched => EvaluateResult.IsRoleMatched;

    /// <summary>
    /// 出た目の数字のみを配列で取得します（役判定用）
    /// </summary>
    public int[] Numbers
    {
        get
        {
            int[] numbers = new int[3];
            for (int i = 0; i < 3 && i < RollData.Length; i++)
            {
                numbers[i] = RollData[i].Number;
            }
            return numbers;
        }
    }
}
