using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 戦闘中のHP、ターン数、フェーズを表示するUIパネル
/// Text参照と表示フォーマットをUnity Editor上で設定できます
/// </summary>
public class BattleStatusPanel : MonoBehaviour
{
    [SerializeField]
    [Header("プレイヤーHP Text")]
    [Tooltip("プレイヤーHPを表示するText")]
    private Text m_playerHpText;

    [SerializeField]
    [Header("敵HP Text")]
    [Tooltip("敵HPを表示するText")]
    private Text m_enemyHpText;

    [SerializeField]
    [Header("ターン Text")]
    [Tooltip("現在ターン数を表示するText")]
    private Text m_turnText;

    [SerializeField]
    [Header("フェーズ Text")]
    [Tooltip("現在フェーズを表示するText")]
    private Text m_phaseText;

    [SerializeField]
    [Header("プレイヤーHPフォーマット")]
    [Tooltip("プレイヤーHP表示フォーマット。{0}=現在HP、{1}=最大HP")]
    private string m_playerHpFormat = "Player HP: {0}/{1}";

    [SerializeField]
    [Header("敵HPフォーマット")]
    [Tooltip("敵HP表示フォーマット。{0}=現在HP、{1}=最大HP")]
    private string m_enemyHpFormat = "Enemy HP: {0}/{1}";

    [SerializeField]
    [Header("ターンフォーマット")]
    [Tooltip("ターン表示フォーマット。{0}=ターン数")]
    private string m_turnFormat = "Turn: {0}";

    [SerializeField]
    [Header("フェーズフォーマット")]
    [Tooltip("フェーズ表示フォーマット。{0}=フェーズ名")]
    private string m_phaseFormat = "Phase: {0}";

    /// <summary>
    /// プレイヤーHP表示を更新します
    /// </summary>
    /// <param name="currentHp">現在HP</param>
    /// <param name="maxHp">最大HP</param>
    public void SetPlayerHp(int currentHp, int maxHp)
    {
        if (m_playerHpText != null)
        {
            m_playerHpText.text = BuildTwoValueText(m_playerHpFormat, "Player HP: {0}/{1}", currentHp, maxHp);
        }
    }

    /// <summary>
    /// 敵HP表示を更新します
    /// </summary>
    /// <param name="currentHp">現在HP</param>
    /// <param name="maxHp">最大HP</param>
    public void SetEnemyHp(int currentHp, int maxHp)
    {
        if (m_enemyHpText != null)
        {
            m_enemyHpText.text = BuildTwoValueText(m_enemyHpFormat, "Enemy HP: {0}/{1}", currentHp, maxHp);
        }
    }

    /// <summary>
    /// ターン表示を更新します
    /// </summary>
    /// <param name="turnNumber">ターン数</param>
    public void SetTurn(int turnNumber)
    {
        if (m_turnText != null)
        {
            m_turnText.text = BuildOneValueText(m_turnFormat, "Turn: {0}", turnNumber);
        }
    }

    /// <summary>
    /// フェーズ表示を更新します
    /// </summary>
    /// <param name="battlePhase">戦闘フェーズ</param>
    public void SetPhase(BattlePhase battlePhase)
    {
        if (m_phaseText != null)
        {
            m_phaseText.text = BuildOneValueText(m_phaseFormat, "Phase: {0}", battlePhase);
        }
    }

    /// <summary>
    /// 2つの値を使う表示文字列を安全に生成します
    /// </summary>
    /// <param name="format">Inspector設定フォーマット</param>
    /// <param name="fallbackFormat">既定フォーマット</param>
    /// <param name="currentValue">現在値</param>
    /// <param name="maxValue">最大値</param>
    /// <returns>表示文字列</returns>
    private string BuildTwoValueText(string format, string fallbackFormat, int currentValue, int maxValue)
    {
        if (string.IsNullOrEmpty(format) || !format.Contains("{0}") || !format.Contains("{1}"))
        {
            return string.Format(fallbackFormat, currentValue, maxValue);
        }

        return string.Format(format, currentValue, maxValue);
    }

    /// <summary>
    /// 1つの値を使う表示文字列を安全に生成します
    /// </summary>
    /// <param name="format">Inspector設定フォーマット</param>
    /// <param name="fallbackFormat">既定フォーマット</param>
    /// <param name="value">表示値</param>
    /// <returns>表示文字列</returns>
    private string BuildOneValueText(string format, string fallbackFormat, object value)
    {
        if (string.IsNullOrEmpty(format) || !format.Contains("{0}"))
        {
            return string.Format(fallbackFormat, value);
        }

        return string.Format(format, value);
    }

    /// <summary>
    /// BattleStateの内容をまとめて反映します
    /// </summary>
    /// <param name="battleState">戦闘状態</param>
    public void Refresh(BattleState battleState)
    {
        if (battleState == null)
        {
            return;
        }

        if (battleState.PlayerActor != null)
        {
            SetPlayerHp(battleState.PlayerActor.CurrentHp, battleState.PlayerActor.MaxHp);
        }

        if (battleState.EnemyActor != null)
        {
            SetEnemyHp(battleState.EnemyActor.CurrentHp, battleState.EnemyActor.MaxHp);
        }

        SetTurn(battleState.TurnNumber);
        SetPhase(battleState.CurrentPhase);
    }
}
