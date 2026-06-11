using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

/// <summary>
/// 画面上部の帯UIを管理するパネル
/// PlayerHP表示、OptionButton、MapButtonの参照とイベントをUnity Editor上で設定できます
/// </summary>
public class BattleTopBarPanel : MonoBehaviour
{
    [SerializeField]
    [Header("プレイヤーHP Text")]
    [Tooltip("画面上部に表示するプレイヤーHP Text")]
    private Text m_playerHpText;

    [SerializeField]
    [Header("オプションボタン")]
    [Tooltip("オプション画面用のボタン。機能本体はα版では未実装です")]
    private Button m_optionButton;

    [SerializeField]
    [Header("マップボタン")]
    [Tooltip("マップ確認用のボタン。機能本体はα版では未実装です")]
    private Button m_mapButton;

    [SerializeField]
    [Header("HP表示フォーマット")]
    [Tooltip("プレイヤーHP表示のフォーマット。{0}=現在HP、{1}=最大HP")]
    private string m_playerHpFormat = "Player HP: {0}/{1}";

    [SerializeField]
    [Header("オプションボタンイベント")]
    [Tooltip("OptionButtonクリック時に呼び出すUnityEvent")]
    private UnityEvent m_onOptionButtonClicked;

    [SerializeField]
    [Header("マップボタンイベント")]
    [Tooltip("MapButtonクリック時に呼び出すUnityEvent")]
    private UnityEvent m_onMapButtonClicked;

    /// <summary>
    /// プレイヤーHP表示を更新します
    /// </summary>
    /// <param name="currentHp">現在HP</param>
    /// <param name="maxHp">最大HP</param>
    public void SetPlayerHp(int currentHp, int maxHp)
    {
        if (m_playerHpText != null)
        {
            if (string.IsNullOrEmpty(m_playerHpFormat) || !m_playerHpFormat.Contains("{0}") || !m_playerHpFormat.Contains("{1}"))
            {
                m_playerHpText.text = $"Player HP: {currentHp}/{maxHp}";
                return;
            }

            m_playerHpText.text = string.Format(m_playerHpFormat, currentHp, maxHp);
        }
    }

    /// <summary>
    /// ボタンイベントを登録します
    /// </summary>
    private void Awake()
    {
        if (m_optionButton != null)
        {
            m_optionButton.onClick.AddListener(HandleOptionButtonClicked);
        }

        if (m_mapButton != null)
        {
            m_mapButton.onClick.AddListener(HandleMapButtonClicked);
        }
    }

    /// <summary>
    /// オブジェクト破棄時にボタンイベントを解除します
    /// </summary>
    private void OnDestroy()
    {
        if (m_optionButton != null)
        {
            m_optionButton.onClick.RemoveListener(HandleOptionButtonClicked);
        }

        if (m_mapButton != null)
        {
            m_mapButton.onClick.RemoveListener(HandleMapButtonClicked);
        }
    }

    /// <summary>
    /// オプションボタンクリック時の処理
    /// </summary>
    private void HandleOptionButtonClicked()
    {
        m_onOptionButtonClicked?.Invoke();
    }

    /// <summary>
    /// マップボタンクリック時の処理
    /// </summary>
    private void HandleMapButtonClicked()
    {
        m_onMapButtonClicked?.Invoke();
    }
}
