/// <summary>
/// 戦闘中の行動種別を表す列挙型
/// 敵の行動予測アイコンや汎用表示に使用します
/// </summary>
public enum BattleActionType
{
    /// <summary>攻撃行動</summary>
    Attack,

    /// <summary>防御行動</summary>
    Defense,

    /// <summary>バフ付与</summary>
    Buff,

    /// <summary>デバフ付与</summary>
    Debuff,

    /// <summary>不明・その他</summary>
    Unknown
}
