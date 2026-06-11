using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 戦闘ログを画面上に表示するUIパネル
/// 最大ログ数や表示先TextをUnity Editor上で設定できます
/// </summary>
public class BattleLogPanel : MonoBehaviour
{
    [SerializeField]
    [Header("ログ表示Text")]
    [Tooltip("戦闘ログ本文を表示するTextコンポーネント")]
    private Text m_logText;

    [SerializeField]
    [Header("ScrollRect")]
    [Tooltip("ログ表示に使用するScrollRect。未設定でも動作します")]
    private ScrollRect m_scrollRect;

    [SerializeField]
    [Header("最大ログ数")]
    [Tooltip("画面上に保持する最大ログ行数")]
    private int m_maxLogCount = 30;

    private readonly List<string> m_logMessages = new List<string>();

    /// <summary>
    /// ログを1行追加します
    /// </summary>
    /// <param name="message">追加するログメッセージ</param>
    public void AddLog(string message)
    {
        if (string.IsNullOrEmpty(message))
        {
            return;
        }

        m_logMessages.Add(message);
        while (m_logMessages.Count > m_maxLogCount)
        {
            m_logMessages.RemoveAt(0);
        }

        RefreshLogText();
    }

    /// <summary>
    /// すべてのログを削除します
    /// </summary>
    public void ClearLogs()
    {
        m_logMessages.Clear();
        RefreshLogText();
    }

    /// <summary>
    /// Textへ現在のログ内容を反映します
    /// </summary>
    private void RefreshLogText()
    {
        if (m_logText != null)
        {
            m_logText.text = string.Join("\n", m_logMessages);
        }

        if (m_scrollRect != null)
        {
            Canvas.ForceUpdateCanvases();
            m_scrollRect.verticalNormalizedPosition = 0f;
        }
    }

#if UNITY_EDITOR
    /// <summary>
    /// Inspector編集時に設定値を有効範囲へ補正します
    /// </summary>
    private void OnValidate()
    {
        m_maxLogCount = Mathf.Max(1, m_maxLogCount);
    }
#endif
}
