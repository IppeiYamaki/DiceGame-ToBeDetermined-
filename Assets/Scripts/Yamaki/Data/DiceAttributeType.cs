/// <summary>
/// ダイスの面が持つ属性の種別
/// 各面の効果（攻撃、防御など）を表します
/// 
/// 使用例:
/// - Attack: 敵へのダメージ
/// - Defense: 敵からの攻撃を防御
/// 
/// 将来的に新しい属性（回復、バフなど）を追加可能です
/// </summary>
public enum DiceAttributeType
{
    /// <summary>攻撃属性（敵にダメージを与える）</summary>
    Attack,

    /// <summary>防御属性（敵からの攻撃を軽減する）</summary>
    Defense
}
