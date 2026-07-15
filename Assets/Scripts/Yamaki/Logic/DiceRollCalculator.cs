using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// ダイスを振って結果を計算するロジッククラス
/// 3個のダイスIDを受け取り、各ダイスをランダムロール、出目合計、役判定を行い、
/// 倍率適用後の出目値を含む DiceRollResult を返します
/// 
/// 使用方法:
/// <code>
/// string[] diceIds = { "abc123...", "def456...", "ghi789..." };
/// List&lt;DiceRoleDefinition&gt; roles = DiceMasterRegistry.Active.AllRoleDefinitions.ToList();
/// DiceRollResult result = DiceRollCalculator.Roll(diceIds, roles);
/// Debug.Log($"最終値: {result.FinalValue}");
/// </code>
/// </summary>
public static class DiceRollCalculator
{
    // ─────────────────────────────────────────────────────────
    // 公開 API
    // ─────────────────────────────────────────────────────────

    /// <summary>
    /// 3個のダイスを振って結果を計算します
    /// 各ダイスをランダムロール、出目合計、役判定を行い、
    /// 倍率適用後の出目値を含む DiceRollResult を返します
    /// </summary>
    /// <param name="diceIds">振るダイスの永続 ID（3個）</param>
    /// <param name="availableRoles">判定対象の役リスト</param>
    /// <returns>ロール結果全体</returns>
    public static DiceRollResult Roll(string[] diceIds, List<DiceRoleDefinition> availableRoles)
    {
        // 入力チェック
        if (diceIds == null || diceIds.Length != 3)
        {
            Debug.LogWarning("[DiceRollCalculator] diceIds は 3個の配列である必要があります。");
            return CreateEmptyResult();
        }

        if (DiceMasterRegistry.Active == null)
        {
            Debug.LogError("[DiceRollCalculator] DiceMasterRegistry.Active が設定されていません。");
            return CreateEmptyResult();
        }

        // 各ダイスをロール
        DiceRollData[] rollData = new DiceRollData[3];
        for (int i = 0; i < 3; i++)
        {
            rollData[i] = RollSingleDice(diceIds[i]);
        }

        // 出た目の数字のみを抽出して役判定
        int[] numbers = rollData.Select(r => r.Number).ToArray();
        int totalNumber = numbers.Sum();
        DiceRoleEvaluateResult evaluateResult = DiceRoleEvaluator.Evaluate(numbers, availableRoles);

        // 倍率を適用した最終値を計算
        int finalValue = Mathf.FloorToInt(totalNumber * evaluateResult.Multiplier);

        // 結果を返す
        return new DiceRollResult(rollData, evaluateResult, totalNumber, finalValue);
    }

    /// <summary>
    /// 外部のダイスロール演出などで確定した出目からロール結果を計算します。
    /// ダイス選択と出目表示は外部で行い、役判定とActionPoint計算だけを共通化したい場合に使用します。
    /// </summary>
    /// <param name="diceIds">振ったダイスの永続 ID（3個）</param>
    /// <param name="numbers">外部ロールで確定した出目（3個）</param>
    /// <param name="availableRoles">判定対象の役リスト</param>
    /// <returns>ロール結果全体</returns>
    public static DiceRollResult CalculateFromRolledNumbers(string[] diceIds, int[] numbers, List<DiceRoleDefinition> availableRoles)
    {
        if (diceIds == null || diceIds.Length != 3)
        {
            Debug.LogWarning("[DiceRollCalculator] diceIds は 3個の配列である必要があります。");
            return CreateEmptyResult();
        }

        if (numbers == null || numbers.Length != 3)
        {
            Debug.LogWarning("[DiceRollCalculator] numbers は 3個の配列である必要があります。");
            return CreateEmptyResult();
        }

        DiceRollData[] rollData = new DiceRollData[3];
        for (int i = 0; i < 3; i++)
        {
            int safeNumber = Mathf.Max(1, numbers[i]);
            int faceIndex = ResolveFaceIndex(diceIds[i], safeNumber);
            rollData[i] = new DiceRollData(diceIds[i], faceIndex, safeNumber);
        }

        int totalNumber = rollData.Sum(r => r.Number);
        DiceRoleEvaluateResult evaluateResult = DiceRoleEvaluator.Evaluate(rollData.Select(r => r.Number).ToArray(), availableRoles);
        int finalValue = Mathf.FloorToInt(totalNumber * evaluateResult.Multiplier);

        return new DiceRollResult(rollData, evaluateResult, totalNumber, finalValue);
    }

    // ─────────────────────────────────────────────────────────
    // 内部実装：ダイスロール
    // ─────────────────────────────────────────────────────────

    /// <summary>
    /// 1個のダイスを振って結果を返します
    /// </summary>
    /// <param name="diceId">振るダイスの永続 ID</param>
    /// <returns>ロール結果</returns>
    private static DiceRollData RollSingleDice(string diceId)
    {
        // ダイス定義を取得
        DiceDefinition diceDefinition = DiceMasterRegistry.Active.ResolveDefinition(diceId);
        if (diceDefinition == null)
        {
            Debug.LogWarning($"[DiceRollCalculator] ダイス ID '{diceId}' が見つかりません。");
            return CreateEmptyRollData(diceId);
        }

        if (diceDefinition.FaceCount <= 0)
        {
            Debug.LogWarning($"[DiceRollCalculator] ダイス ID '{diceId}' に面が設定されていません。");
            return CreateEmptyRollData(diceId);
        }

        // 設定されている面の中からランダムに1面を選択
        int faceIndex = Random.Range(0, diceDefinition.FaceCount);
        DiceFaceData faceData = diceDefinition.Faces[faceIndex];

        return new DiceRollData(diceId, faceIndex, faceData.Number);
    }

    /// <summary>
    /// 外部指定の出目に対応する面Indexを可能な範囲で解決します。
    /// </summary>
    private static int ResolveFaceIndex(string diceId, int number)
    {
        if (DiceMasterRegistry.Active == null || string.IsNullOrEmpty(diceId))
        {
            return 0;
        }

        DiceDefinition diceDefinition = DiceMasterRegistry.Active.ResolveDefinition(diceId);
        if (diceDefinition == null || diceDefinition.Faces == null)
        {
            return 0;
        }

        for (int i = 0; i < diceDefinition.Faces.Count; i++)
        {
            if (diceDefinition.Faces[i].Number == number)
            {
                return i;
            }
        }

        return 0;
    }

    // ─────────────────────────────────────────────────────────
    // 内部実装：エラー時の空データ生成
    // ─────────────────────────────────────────────────────────

    /// <summary>
    /// エラー時に返す空の DiceRollResult を生成します
    /// </summary>
    private static DiceRollResult CreateEmptyResult()
    {
        DiceRollData[] emptyRollData = new DiceRollData[3];
        for (int i = 0; i < 3; i++)
        {
            emptyRollData[i] = CreateEmptyRollData("");
        }

        return new DiceRollResult(
            emptyRollData,
            DiceRoleEvaluateResult.None,
            0,
            0
        );
    }

    /// <summary>
    /// エラー時に返す空の DiceRollData を生成します
    /// </summary>
    private static DiceRollData CreateEmptyRollData(string diceId)
    {
        return new DiceRollData(diceId, 0, 1);
    }
}
