using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// ダイスを振って結果を計算するロジッククラス
/// 3個のダイスIDを受け取り、各ダイスをランダムロール、属性合計、役判定を行い、
/// 倍率適用後の最終値を含む DiceRollResult を返します
/// 
/// 使用方法:
/// <code>
/// string[] diceIds = { "abc123...", "def456...", "ghi789..." };
/// List&lt;DiceRoleDefinition&gt; roles = DiceMasterRegistry.Active.AllRoleDefinitions.ToList();
/// DiceRollResult result = DiceRollCalculator.Roll(diceIds, roles);
/// Debug.Log($"攻撃: {result.FinalAttributeValues[DiceAttributeType.Attack]}");
/// </code>
/// </summary>
public static class DiceRollCalculator
{
    // ─────────────────────────────────────────────────────────
    // 公開 API
    // ─────────────────────────────────────────────────────────

    /// <summary>
    /// 3個のダイスを振って結果を計算します
    /// 各ダイスをランダムロール、属性合計、役判定を行い、
    /// 倍率適用後の最終値を含む DiceRollResult を返します
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

        // 属性別の合計値を計算（倍率適用前）
        Dictionary<DiceAttributeType, int> attributeTotals = CalculateAttributeTotals(rollData);

        // 出た目の数字のみを抽出して役判定
        int[] numbers = rollData.Select(r => r.Number).ToArray();
        DiceRoleEvaluateResult evaluateResult = DiceRoleEvaluator.Evaluate(numbers, availableRoles);

        // 倍率を適用した最終値を計算
        Dictionary<DiceAttributeType, int> finalValues = ApplyMultiplier(attributeTotals, evaluateResult.Multiplier);

        // 結果を返す
        return new DiceRollResult(rollData, evaluateResult, attributeTotals, finalValues);
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

        // 6面の中からランダムに1面を選択
        int faceIndex = Random.Range(0, 6);
        DiceFaceData faceData = diceDefinition.Faces[faceIndex];

        // 属性別の合計値を計算
        Dictionary<DiceAttributeType, int> attributeTotals = new Dictionary<DiceAttributeType, int>();
        foreach (DiceFaceElement element in faceData.Elements)
        {
            if (attributeTotals.ContainsKey(element.AttributeType))
            {
                attributeTotals[element.AttributeType]++;
            }
            else
            {
                attributeTotals[element.AttributeType] = 1;
            }
        }

        return new DiceRollData(diceId, faceIndex, faceData.Number, attributeTotals);
    }

    // ─────────────────────────────────────────────────────────
    // 内部実装：属性合計計算
    // ─────────────────────────────────────────────────────────

    /// <summary>
    /// 3個のダイスロール結果から属性別の合計値を計算します
    /// </summary>
    /// <param name="rollData">3個のダイスロール結果</param>
    /// <returns>属性別の合計値</returns>
    private static Dictionary<DiceAttributeType, int> CalculateAttributeTotals(DiceRollData[] rollData)
    {
        Dictionary<DiceAttributeType, int> totals = new Dictionary<DiceAttributeType, int>();

        foreach (DiceRollData data in rollData)
        {
            foreach (var pair in data.AttributeTotals)
            {
                if (totals.ContainsKey(pair.Key))
                {
                    totals[pair.Key] += pair.Value;
                }
                else
                {
                    totals[pair.Key] = pair.Value;
                }
            }
        }

        return totals;
    }

    /// <summary>
    /// 属性別の合計値に倍率を適用した最終値を計算します
    /// </summary>
    /// <param name="attributeTotals">属性別の合計値（倍率適用前）</param>
    /// <param name="multiplier">適用する倍率</param>
    /// <returns>属性別の最終値（倍率適用後）</returns>
    private static Dictionary<DiceAttributeType, int> ApplyMultiplier(
        Dictionary<DiceAttributeType, int> attributeTotals,
        float multiplier)
    {
        Dictionary<DiceAttributeType, int> finalValues = new Dictionary<DiceAttributeType, int>();

        foreach (var pair in attributeTotals)
        {
            finalValues[pair.Key] = Mathf.FloorToInt(pair.Value * multiplier);
        }

        return finalValues;
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
            new Dictionary<DiceAttributeType, int>(),
            new Dictionary<DiceAttributeType, int>()
        );
    }

    /// <summary>
    /// エラー時に返す空の DiceRollData を生成します
    /// </summary>
    private static DiceRollData CreateEmptyRollData(string diceId)
    {
        return new DiceRollData(diceId, 0, 1, new Dictionary<DiceAttributeType, int>());
    }
}
