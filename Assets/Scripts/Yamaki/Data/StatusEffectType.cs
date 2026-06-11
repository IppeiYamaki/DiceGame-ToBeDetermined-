/// <summary>
/// バフ/デバフの種類を表す列挙型
/// UI表示時に汎用アイコンを選択する際に使用します
/// </summary>
public enum StatusEffectType
{
    /// <summary>プレイヤーに有利な効果（バフ）</summary>
    Buff,

    /// <summary>プレイヤーに不利な効果（デバフ）</summary>
    Debuff
}
