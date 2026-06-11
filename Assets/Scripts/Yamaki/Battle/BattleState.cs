using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ダイス戦闘全体の現在状態
/// ターン数、フェーズ、プレイヤー/敵Runtime状態、手札、選択ダイス、直近結果を保持します
/// </summary>
[Serializable]
public class BattleState
{
    [SerializeField]
    [Header("ターン番号")]
    [Tooltip("現在のターン番号")]
    private int m_turnNumber = 0;

    [SerializeField]
    [Header("現在フェーズ")]
    [Tooltip("現在の戦闘フェーズ")]
    private BattlePhase m_currentPhase = BattlePhase.None;

    [SerializeField]
    [Header("プレイヤー状態")]
    [Tooltip("戦闘中のプレイヤーRuntime状態")]
    private BattleActorRuntime m_playerActor;

    [SerializeField]
    [Header("敵状態")]
    [Tooltip("戦闘中の敵Runtime状態")]
    private BattleActorRuntime m_enemyActor;

    [SerializeField]
    [Header("現在の手札ダイスID")]
    [Tooltip("現在ターンでドロー済みのダイスIDリスト")]
    private List<string> m_currentHandDiceIds = new List<string>();

    [SerializeField]
    [Header("選択ダイスID")]
    [Tooltip("ロールに使用する選択済みダイスIDリスト")]
    private List<string> m_selectedDiceIds = new List<string>();

    [SerializeField]
    [Header("戦闘終了")]
    [Tooltip("戦闘が終了しているかどうか")]
    private bool m_isBattleFinished = false;

    [SerializeField]
    [Header("勝利")]
    [Tooltip("戦闘終了時にプレイヤーが勝利したかどうか")]
    private bool m_isPlayerWin = false;

    private DiceRollResult m_latestDiceRollResult;
    private BattleTurnResult m_latestTurnResult;

    /// <summary>
    /// 現在のターン番号
    /// </summary>
    public int TurnNumber => m_turnNumber;

    /// <summary>
    /// 現在の戦闘フェーズ
    /// </summary>
    public BattlePhase CurrentPhase => m_currentPhase;

    /// <summary>
    /// 戦闘中のプレイヤー状態
    /// </summary>
    public BattleActorRuntime PlayerActor => m_playerActor;

    /// <summary>
    /// 戦闘中の敵状態
    /// </summary>
    public BattleActorRuntime EnemyActor => m_enemyActor;

    /// <summary>
    /// 現在の手札ダイスID
    /// </summary>
    public IReadOnlyList<string> CurrentHandDiceIds => m_currentHandDiceIds;

    /// <summary>
    /// 選択済みダイスID
    /// </summary>
    public IReadOnlyList<string> SelectedDiceIds => m_selectedDiceIds;

    /// <summary>
    /// 直近のダイスロール結果
    /// </summary>
    public DiceRollResult LatestDiceRollResult => m_latestDiceRollResult;

    /// <summary>
    /// 直近のターン結果
    /// </summary>
    public BattleTurnResult LatestTurnResult => m_latestTurnResult;

    /// <summary>
    /// 戦闘が終了しているかどうか
    /// </summary>
    public bool IsBattleFinished => m_isBattleFinished;

    /// <summary>
    /// プレイヤーが勝利したかどうか
    /// </summary>
    public bool IsPlayerWin => m_isPlayerWin;

    /// <summary>
    /// 戦闘状態を初期化します
    /// </summary>
    /// <param name="playerActor">プレイヤーRuntime状態</param>
    /// <param name="enemyActor">敵Runtime状態</param>
    public void Initialize(BattleActorRuntime playerActor, BattleActorRuntime enemyActor)
    {
        m_turnNumber = 0;
        m_currentPhase = BattlePhase.BattleStart;
        m_playerActor = playerActor;
        m_enemyActor = enemyActor;
        m_currentHandDiceIds.Clear();
        m_selectedDiceIds.Clear();
        m_latestDiceRollResult = default;
        m_latestTurnResult = default;
        m_isBattleFinished = false;
        m_isPlayerWin = false;
    }

    /// <summary>
    /// 現在フェーズを設定します
    /// </summary>
    /// <param name="battlePhase">設定するフェーズ</param>
    public void SetPhase(BattlePhase battlePhase)
    {
        m_currentPhase = battlePhase;
    }

    /// <summary>
    /// 次のターンへ進めます
    /// </summary>
    public void AdvanceTurn()
    {
        m_turnNumber++;
    }

    /// <summary>
    /// 現在の手札ダイスIDを設定します
    /// </summary>
    /// <param name="diceIds">手札ダイスIDリスト</param>
    public void SetCurrentHandDiceIds(IEnumerable<string> diceIds)
    {
        m_currentHandDiceIds.Clear();
        if (diceIds == null)
        {
            return;
        }

        m_currentHandDiceIds.AddRange(diceIds);
    }

    /// <summary>
    /// 選択済みダイスIDを設定します
    /// </summary>
    /// <param name="diceIds">選択済みダイスIDリスト</param>
    public void SetSelectedDiceIds(IEnumerable<string> diceIds)
    {
        m_selectedDiceIds.Clear();
        if (diceIds == null)
        {
            return;
        }

        m_selectedDiceIds.AddRange(diceIds);
    }

    /// <summary>
    /// 直近のダイスロール結果を設定します
    /// </summary>
    /// <param name="diceRollResult">ダイスロール結果</param>
    public void SetLatestDiceRollResult(DiceRollResult diceRollResult)
    {
        m_latestDiceRollResult = diceRollResult;
    }

    /// <summary>
    /// 直近のターン結果を設定します
    /// </summary>
    /// <param name="turnResult">ターン結果</param>
    public void SetLatestTurnResult(BattleTurnResult turnResult)
    {
        m_latestTurnResult = turnResult;
    }

    /// <summary>
    /// 戦闘終了状態を設定します
    /// </summary>
    /// <param name="isPlayerWin">プレイヤーが勝利したかどうか</param>
    public void SetBattleFinished(bool isPlayerWin)
    {
        m_isBattleFinished = true;
        m_isPlayerWin = isPlayerWin;
        m_currentPhase = isPlayerWin ? BattlePhase.BattleWin : BattlePhase.BattleLose;
    }
}
