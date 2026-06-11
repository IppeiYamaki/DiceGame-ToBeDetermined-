/// <summary>
/// 戦闘フローの現在フェーズ。
/// </summary>
public enum BattlePhase
{
    None,
    BattleStart,
    TurnStart,
    DrawDice,
    WaitDiceSelection,
    RollDice,
    PlayerAction,
    PlayerSelect,
    PlayerRoll,
    PlayerResult,
    EnemyAction,
    CheckBattleEnd,
    CheckStatus,
    Win,
    GameOver,
    BattleWin,
    BattleLose
}
