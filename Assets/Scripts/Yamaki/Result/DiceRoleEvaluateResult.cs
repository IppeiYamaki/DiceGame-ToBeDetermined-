using System;

/// <summary>
/// 役判定結果を表すデータ
/// 成立した役とその倍率を保持します
/// 
/// 使用例:
/// - 成立役: ピンゾロ（DiceRoleDefinition）
/// - 適用倍率: 3.0f
/// 
/// 役が成立しなかった場合は Role が null になります
/// </summary>
[Serializable]
public struct DiceRoleEvaluateResult
{
    // ─────────────────────────────────────────────────────────
    // フィールド
    // ─────────────────────────────────────────────────────────

    /// <summary>成立した役（成立しなかった場合は null）</summary>
    public DiceRoleDefinition Role;

    /// <summary>適用される倍率（役が成立しなかった場合は 1.0f）</summary>
    public float Multiplier;

    // ─────────────────────────────────────────────────────────
    // コンストラクタ
    // ─────────────────────────────────────────────────────────

    /// <summary>
    /// DiceRoleEvaluateResult を生成します
    /// </summary>
    /// <param name="role">成立した役（成立しなかった場合は null）</param>
    /// <param name="multiplier">適用倍率</param>
    public DiceRoleEvaluateResult(DiceRoleDefinition role, float multiplier)
    {
        Role = role;
        Multiplier = multiplier;
    }

    // ─────────────────────────────────────────────────────────
    // ユーティリティプロパティ
    // ─────────────────────────────────────────────────────────

    /// <summary>
    /// 役が成立したかどうか
    /// </summary>
    public bool IsRoleMatched => Role != null;

    /// <summary>
    /// 役が成立しなかった場合のデフォルト結果を返します（倍率 1.0f）
    /// </summary>
    public static DiceRoleEvaluateResult None => new DiceRoleEvaluateResult(null, 1.0f);
}
