/// <summary>
/// 戦闘中の攻撃値、防御値、最終ダメージを計算するロジッククラス
/// </summary>
public static class BattleCalculator
{
    /// <summary>
    /// ダイスロール結果から攻撃値を取得します
    /// </summary>
    /// <param name="rollResult">ダイスロール結果</param>
    /// <returns>役倍率適用後の攻撃値</returns>
    public static int GetAttackValue(DiceRollResult rollResult)
    {
        return GetFinalAttributeValue(rollResult, DiceAttributeType.Attack);
    }

    /// <summary>
    /// ダイスロール結果から防御値を取得します
    /// </summary>
    /// <param name="rollResult">ダイスロール結果</param>
    /// <returns>役倍率適用後の防御値</returns>
    public static int GetDefenseValue(DiceRollResult rollResult)
    {
        return GetFinalAttributeValue(rollResult, DiceAttributeType.Defense);
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

    /// <summary>
    /// ダイスロール結果から指定属性の最終値を取得します
    /// </summary>
    /// <param name="rollResult">ダイスロール結果</param>
    /// <param name="attributeType">取得する属性タイプ</param>
    /// <returns>指定属性の最終値。存在しない場合は0</returns>
    private static int GetFinalAttributeValue(DiceRollResult rollResult, DiceAttributeType attributeType)
    {
        if (rollResult.FinalAttributeValues == null)
        {
            return 0;
        }

        return rollResult.FinalAttributeValues.TryGetValue(attributeType, out int value) ? value : 0;
    }
}
