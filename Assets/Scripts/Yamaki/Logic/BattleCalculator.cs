/// <summary>
/// 戦闘中の攻撃値、最終ダメージを計算するロジッククラス
/// </summary>
public static class BattleCalculator
{
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
