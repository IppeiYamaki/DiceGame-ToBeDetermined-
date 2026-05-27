using System.Collections.Generic;
using System.Linq;

/// <summary>
/// ダイスの役（コンボ）判定ロジッククラス
/// 3個のダイスの出目から成立する役を判定し、最も倍率の高い役を返します
/// 
/// 使用方法:
/// <code>
/// int[] numbers = { 3, 3, 3 };
/// List&lt;DiceRoleDefinition&gt; roles = DiceMasterRegistry.Active.AllRoleDefinitions.ToList();
/// DiceRoleEvaluateResult result = DiceRoleEvaluator.Evaluate(numbers, roles);
/// if (result.IsRoleMatched)
/// {
///     Debug.Log($"役: {result.Role.RoleName}, 倍率: {result.Multiplier}");
/// }
/// </code>
/// </summary>
public static class DiceRoleEvaluator
{
    // ─────────────────────────────────────────────────────────
    // 公開 API
    // ─────────────────────────────────────────────────────────

    /// <summary>
    /// 3個のダイスの出目から成立する役を判定し、最も倍率の高い役を返します
    /// 複数の役が成立した場合は、最高倍率のものを優先します
    /// 同倍率の場合は、リストの先頭にあるものを優先します
    /// </summary>
    /// <param name="rolledNumbers">3個のダイスの出目（1～6）</param>
    /// <param name="availableRoles">判定対象の役リスト</param>
    /// <returns>成立した役の判定結果（成立しなかった場合は倍率 1.0f）</returns>
    public static DiceRoleEvaluateResult Evaluate(int[] rolledNumbers, List<DiceRoleDefinition> availableRoles)
    {
        // 入力チェック
        if (rolledNumbers == null || rolledNumbers.Length != 3)
        {
            UnityEngine.Debug.LogWarning("[DiceRoleEvaluator] rolledNumbers は 3個の配列である必要があります。");
            return DiceRoleEvaluateResult.None;
        }

        if (availableRoles == null || availableRoles.Count == 0)
        {
            return DiceRoleEvaluateResult.None;
        }

        // 成立した役を収集
        List<DiceRoleDefinition> matchedRoles = new List<DiceRoleDefinition>();

        foreach (DiceRoleDefinition role in availableRoles)
        {
            if (role == null) continue;

            bool isMatched = CheckRole(rolledNumbers, role);
            if (isMatched)
            {
                matchedRoles.Add(role);
            }
        }

        // 成立した役がない場合
        if (matchedRoles.Count == 0)
        {
            return DiceRoleEvaluateResult.None;
        }

        // 最も倍率の高い役を選択（同倍率の場合は先頭優先）
        DiceRoleDefinition bestRole = matchedRoles
            .OrderByDescending(r => r.Multiplier)
            .First();

        return new DiceRoleEvaluateResult(bestRole, bestRole.Multiplier);
    }

    // ─────────────────────────────────────────────────────────
    // 内部実装：役判定メソッド
    // ─────────────────────────────────────────────────────────

    /// <summary>
    /// 指定した役が成立するかどうかを判定します
    /// </summary>
    /// <param name="rolledNumbers">3個のダイスの出目</param>
    /// <param name="role">判定する役</param>
    /// <returns>役が成立する場合は true</returns>
    private static bool CheckRole(int[] rolledNumbers, DiceRoleDefinition role)
    {
        switch (role.ConditionType)
        {
            case DiceRoleConditionType.ExactNumbers:
                return CheckExactNumbers(rolledNumbers, role.AllowedNumbers);

            case DiceRoleConditionType.AllSame:
                return CheckAllSame(rolledNumbers, role.AllowedNumbers);

            case DiceRoleConditionType.AllInAllowedSet:
                return CheckAllInAllowedSet(rolledNumbers, role.AllowedNumbers);

            case DiceRoleConditionType.SameNumberCount:
                return CheckSameNumberCount(rolledNumbers, role.AllowedNumbers, role.RequiredSameCount);

            default:
                return false;
        }
    }

    /// <summary>
    /// ExactNumbers: 指定した数字構成と完全一致（順不同）
    /// 例: ピンゾロ（1,1,1）、シゴロ（4,5,6）、ヒフミ（1,2,3）
    /// </summary>
    private static bool CheckExactNumbers(int[] rolledNumbers, IReadOnlyList<int> allowedNumbers)
    {
        if (allowedNumbers.Count != 3) return false;

        // ソートして比較
        int[] sortedRolled = rolledNumbers.OrderBy(n => n).ToArray();
        int[] sortedAllowed = allowedNumbers.OrderBy(n => n).ToArray();

        return sortedRolled.SequenceEqual(sortedAllowed);
    }

    /// <summary>
    /// AllSame: 3つすべて同じ数字
    /// 例: ゾロ目全般（2,2,2 / 3,3,3 / 4,4,4 / 5,5,5 / 6,6,6）
    /// AllowedNumbers で許可する数字を指定できます（例: [2,3,4,5,6] でピンゾロ除外）
    /// </summary>
    private static bool CheckAllSame(int[] rolledNumbers, IReadOnlyList<int> allowedNumbers)
    {
        // 3つすべて同じ数字か
        if (rolledNumbers[0] != rolledNumbers[1] || rolledNumbers[1] != rolledNumbers[2])
        {
            return false;
        }

        // AllowedNumbers が指定されている場合は、その数字のみ許可
        if (allowedNumbers.Count > 0)
        {
            return allowedNumbers.Contains(rolledNumbers[0]);
        }

        return true;
    }

    /// <summary>
    /// AllInAllowedSet: 指定した数字セット内のみで構成（重複OK）
    /// 例: 偶数のみ（2,4,6の中から3つ）、奇数のみ（1,3,5の中から3つ）
    /// </summary>
    private static bool CheckAllInAllowedSet(int[] rolledNumbers, IReadOnlyList<int> allowedNumbers)
    {
        if (allowedNumbers.Count == 0) return false;

        // すべての出目が AllowedNumbers に含まれているか
        foreach (int number in rolledNumbers)
        {
            if (!allowedNumbers.Contains(number))
            {
                return false;
            }
        }

        return true;
    }

    /// <summary>
    /// SameNumberCount: 同じ数字が指定数以上ある
    /// 例: ワンペア（同じ数字が2個以上）
    /// </summary>
    private static bool CheckSameNumberCount(int[] rolledNumbers, IReadOnlyList<int> allowedNumbers, int requiredSameCount)
    {
        // 各数字の出現回数をカウント
        Dictionary<int, int> countMap = new Dictionary<int, int>();
        foreach (int number in rolledNumbers)
        {
            if (countMap.ContainsKey(number))
            {
                countMap[number]++;
            }
            else
            {
                countMap[number] = 1;
            }
        }

        // 指定数以上出現している数字があるか
        foreach (var pair in countMap)
        {
            if (pair.Value >= requiredSameCount)
            {
                // AllowedNumbers が指定されている場合は、その数字のみ許可
                if (allowedNumbers.Count > 0)
                {
                    if (allowedNumbers.Contains(pair.Key))
                    {
                        return true;
                    }
                }
                else
                {
                    return true;
                }
            }
        }

        return false;
    }
}
