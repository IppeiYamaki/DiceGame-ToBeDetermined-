using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 敵の行動を選択するロジッククラス
/// 現在HPから該当するHP帯を特定し、ウェイト抽選で行動グループを1つ返します
/// 
/// 使い方:
/// <code>
/// EnemyWeightedActionGroup selectedGroup = EnemyActionSelector.SelectActionGroup(enemyDefinition, currentHp);
/// if (selectedGroup != null)
/// {
///     foreach (EnemyActionEntry action in selectedGroup.Actions)
///     {
///         // 行動を実行
///     }
/// }
/// </code>
/// </summary>
public static class EnemyActionSelector
{
    /// <summary>
    /// 敵の現在HPに基づいて行動グループを選択します
    /// </summary>
    /// <param name="enemyDefinition">敵定義</param>
    /// <param name="currentHp">現在HP</param>
    /// <returns>選択された行動グループ（選択できない場合は null）</returns>
    public static EnemyWeightedActionGroup SelectActionGroup(EnemyDefinition enemyDefinition, int currentHp)
    {
        if (enemyDefinition == null)
        {
            Debug.LogWarning("[EnemyActionSelector] EnemyDefinition が null です。");
            return null;
        }

        int maxHp = enemyDefinition.MaxHp;
        if (maxHp <= 0)
        {
            Debug.LogWarning("[EnemyActionSelector] MaxHp が 0 以下です。");
            return null;
        }

        // 現在HPを%に変換（切り上げ）
        int hpPercent = Mathf.CeilToInt((currentHp * 100f) / maxHp);
        hpPercent = Mathf.Clamp(hpPercent, 0, 100);

        // 該当するHP帯を特定
        EnemyHpRangeActionSet targetRangeSet = FindMatchingHpRange(enemyDefinition.HpRangeActionSets, hpPercent);
        if (targetRangeSet == null)
        {
            Debug.LogWarning($"[EnemyActionSelector] HP {hpPercent}% に該当する HP帯が見つかりません。");
            return null;
        }

        // 行動グループをウェイト抽選
        return SelectGroupByWeight(targetRangeSet.ActionGroups);
    }

    /// <summary>
    /// HP%に該当するHP帯を検索します
    /// </summary>
    /// <param name="hpRangeSets">HP帯リスト</param>
    /// <param name="hpPercent">現在HP%</param>
    /// <returns>該当するHP帯（見つからない場合は null）</returns>
    private static EnemyHpRangeActionSet FindMatchingHpRange(IReadOnlyList<EnemyHpRangeActionSet> hpRangeSets, int hpPercent)
    {
        if (hpRangeSets == null || hpRangeSets.Count == 0)
        {
            return null;
        }

        foreach (EnemyHpRangeActionSet rangeSet in hpRangeSets)
        {
            if (rangeSet.LowerPercent <= hpPercent && hpPercent <= rangeSet.UpperPercent)
            {
                return rangeSet;
            }
        }

        return null;
    }

    /// <summary>
    /// 行動グループリストからウェイト抽選で1つ選択します
    /// </summary>
    /// <param name="actionGroups">行動グループリスト</param>
    /// <returns>選択された行動グループ（選択できない場合は null）</returns>
    private static EnemyWeightedActionGroup SelectGroupByWeight(IReadOnlyList<EnemyWeightedActionGroup> actionGroups)
    {
        if (actionGroups == null || actionGroups.Count == 0)
        {
            Debug.LogWarning("[EnemyActionSelector] 行動グループが設定されていません。");
            return null;
        }

        // ウェイト合計を計算
        int totalWeight = 0;
        foreach (EnemyWeightedActionGroup group in actionGroups)
        {
            if (group == null) continue;
            totalWeight += group.Weight;
        }

        if (totalWeight <= 0)
        {
            Debug.LogWarning("[EnemyActionSelector] 有効なウェイトが設定されていません。");
            return null;
        }

        // ランダムでウェイト抽選
        int randomValue = Random.Range(0, totalWeight);
        int currentWeight = 0;

        foreach (EnemyWeightedActionGroup group in actionGroups)
        {
            if (group == null) continue;

            currentWeight += group.Weight;
            if (randomValue < currentWeight)
            {
                return group;
            }
        }

        // フォールバック（通常は到達しない）
        Debug.LogWarning("[EnemyActionSelector] ウェイト抽選でグループを選択できませんでした。");
        return actionGroups.Count > 0 ? actionGroups[0] : null;
    }
}
