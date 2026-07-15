/// <summary>
/// 行動の対象タイプを表します。
/// </summary>
public enum ActionTargetType
{
    /// <summary>自分自身（プレイヤー）</summary>
    Self,

    /// <summary>敵単体（対象選択が必要）</summary>
    SingleEnemy,

    /// <summary>敵全体（全ての生存敵に適用）</summary>
    AllEnemies,

    /// <summary>ランダムな敵1体（実行時に生存敵からランダムで選択）</summary>
    RandomEnemy
}
