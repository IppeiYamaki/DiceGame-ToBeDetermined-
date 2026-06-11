using System;

/// <summary>
/// 1ターン分の戦闘結果データ
/// ダイスロール結果、プレイヤー行動結果、敵行動結果を保持します
/// </summary>
[Serializable]
public struct BattleTurnResult
{
    /// <summary>ターン番号</summary>
    public int TurnNumber;

    /// <summary>プレイヤーのダイスロール結果</summary>
    public DiceRollResult DiceRollResult;

    /// <summary>プレイヤー行動結果</summary>
    public BattleActionResult PlayerActionResult;

    /// <summary>敵行動結果</summary>
    public BattleActionResult EnemyActionResult;

    /// <summary>
    /// BattleTurnResult を生成します
    /// </summary>
    /// <param name="turnNumber">ターン番号</param>
    /// <param name="diceRollResult">プレイヤーのダイスロール結果</param>
    /// <param name="playerActionResult">プレイヤー行動結果</param>
    /// <param name="enemyActionResult">敵行動結果</param>
    public BattleTurnResult(
        int turnNumber,
        DiceRollResult diceRollResult,
        BattleActionResult playerActionResult,
        BattleActionResult enemyActionResult)
    {
        TurnNumber = turnNumber;
        DiceRollResult = diceRollResult;
        PlayerActionResult = playerActionResult;
        EnemyActionResult = enemyActionResult;
    }
}
