/// <summary>
/// 戦闘中の攻撃値、最終ダメージを計算するロジッククラス
/// </summary>
public static class BattleCalculator
{
    /// <summary>
    /// ダイスロール結果からActionPointを計算します。
    /// 新ルールでは倍率適用後の出目合計をActionPointとして扱います。
    /// </summary>
    /// <param name="rollResult">ダイスロール結果</param>
    /// <returns>獲得ActionPoint</returns>
    public static int CalculateActionPoint(DiceRollResult rollResult)
    {
        return rollResult.FinalValue > 0 ? rollResult.FinalValue : 0;
    }

    /// <summary>
    /// 指定コストのActionPointを消費できるか判定します。
    /// </summary>
    public static bool CanSpendActionPoint(int currentActionPoint, int cost)
    {
        return cost >= 0 && currentActionPoint >= cost;
    }

    /// <summary>
    /// ActionPointを消費し、成功したかを返します。
    /// </summary>
    public static bool TrySpendActionPoint(ref int currentActionPoint, int cost)
    {
        if (!CanSpendActionPoint(currentActionPoint, cost))
        {
            return false;
        }

        currentActionPoint -= cost;
        return true;
    }

    /// <summary>
    /// ActionPointを増加させます。
    /// </summary>
    public static int AddActionPoint(int currentActionPoint, int amount)
    {
        int safeCurrent = currentActionPoint > 0 ? currentActionPoint : 0;
        int safeAmount = amount > 0 ? amount : 0;
        return safeCurrent + safeAmount;
    }

    /// <summary>
    /// 攻撃コストから攻撃値を計算します。
    /// </summary>
    public static int CalculateAttackValue(int spentActionPoint)
    {
        return spentActionPoint > 0 ? spentActionPoint : 0;
    }

    /// <summary>
    /// 防御コストからBlock獲得値を計算します。
    /// </summary>
    public static int CalculateBlockValue(int spentActionPoint)
    {
        return spentActionPoint > 0 ? spentActionPoint : 0;
    }

    /// <summary>
    /// Blockで攻撃値を軽減した結果を計算します。
    /// </summary>
    public static BlockDamageResult CalculateBlockDamage(int attackValue, int currentBlock)
    {
        int safeAttack = attackValue > 0 ? attackValue : 0;
        int safeBlock = currentBlock > 0 ? currentBlock : 0;
        int blockedAmount = safeAttack < safeBlock ? safeAttack : safeBlock;
        int damage = safeAttack - blockedAmount;
        int remainingBlock = safeBlock - blockedAmount;
        return new BlockDamageResult(damage, blockedAmount, remainingBlock);
    }

    /// <summary>
    /// 回復量を有効範囲へ補正します。
    /// </summary>
    public static int CalculateHealValue(int healValue)
    {
        return healValue > 0 ? healValue : 0;
    }

    /// <summary>
    /// ダイスロール結果から攻撃値を取得します。
    /// 通常サイコロでは倍率適用後の出目値を攻撃値として扱います。
    /// </summary>
    /// <param name="rollResult">ダイスロール結果</param>
    /// <returns>役倍率適用後の攻撃値</returns>
    public static int GetAttackValue(DiceRollResult rollResult)
    {
        return rollResult.FinalValue;
    }

    /// <summary>
    /// ダイスロール結果から防御値を取得します。
    /// 通常サイコロには防御属性がないため0を返します。
    /// </summary>
    /// <param name="rollResult">ダイスロール結果</param>
    /// <returns>防御値</returns>
    public static int GetDefenseValue(DiceRollResult rollResult)
    {
        return 0;
    }

    /// <summary>
    /// プレイヤーの攻撃による敵への最終ダメージを計算します
    /// </summary>
    /// <param name="attackValue">攻撃値</param>
    /// <param name="defenseValue">防御値</param>
    /// <returns>最終ダメージ</returns>
    public static int CalculateDamage(int attackValue, int defenseValue)
    {
        int damageValue = attackValue - defenseValue;
        return damageValue > 0 ? damageValue : 0;
    }

}

/// <summary>
/// Block軽減後のダメージ計算結果。
/// </summary>
public readonly struct BlockDamageResult
{
    public BlockDamageResult(int damage, int blockedAmount, int remainingBlock)
    {
        Damage = damage;
        BlockedAmount = blockedAmount;
        RemainingBlock = remainingBlock;
    }

    /// <summary>HPへ通るダメージ。</summary>
    public int Damage { get; }

    /// <summary>Blockで軽減した量。</summary>
    public int BlockedAmount { get; }

    /// <summary>残ったBlock。</summary>
    public int RemainingBlock { get; }
}
