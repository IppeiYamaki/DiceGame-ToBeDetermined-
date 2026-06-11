using System;

/// <summary>
/// 1回の攻撃/防御処理で発生した結果データ
/// 攻撃値、防御値、最終ダメージ、HP変化を保持します
/// </summary>
[Serializable]
public struct BattleActionResult
{
    /// <summary>攻撃値</summary>
    public int AttackValue;

    /// <summary>防御値</summary>
    public int DefenseValue;

    /// <summary>防御適用後の最終ダメージ</summary>
    public int DamageValue;

    /// <summary>実際に減少したHP</summary>
    public int HpChangedValue;

    /// <summary>
    /// BattleActionResult を生成します
    /// </summary>
    /// <param name="attackValue">攻撃値</param>
    /// <param name="defenseValue">防御値</param>
    /// <param name="damageValue">防御適用後の最終ダメージ</param>
    /// <param name="hpChangedValue">実際に減少したHP</param>
    public BattleActionResult(int attackValue, int defenseValue, int damageValue, int hpChangedValue)
    {
        AttackValue = attackValue;
        DefenseValue = defenseValue;
        DamageValue = damageValue;
        HpChangedValue = hpChangedValue;
    }
}
