using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// 予約レーンの1エントリ表示。
/// 固定スロットとして配置し、ActionReservationLaneViewから内容を更新します。
/// 右クリックで予約をキャンセルします。
/// </summary>
public class ReservationEntryView : MonoBehaviour, IPointerClickHandler
{
    [SerializeField]
    [Header("対象アイコン")]
    private Image m_targetIcon;

    [SerializeField]
    [Header("行動アイコン")]
    private Image m_actionIcon;

    [SerializeField]
    [Header("ラベルテキスト")]
    private TMP_Text m_labelText;

    [SerializeField]
    [Header("キャンセルボタン（任意）")]
    private Button m_cancelButton;

    private int m_index = -1;
    private Action<int> m_onCancel;
    private bool m_hasAction;

    private void Awake()
    {
        if (m_cancelButton != null)
        {
            m_cancelButton.onClick.AddListener(HandleCancel);
        }
    }

    private void OnDestroy()
    {
        if (m_cancelButton != null)
        {
            m_cancelButton.onClick.RemoveListener(HandleCancel);
        }
    }

    public void Setup(int index, PlannedAction action, Action<int> onCancel)
    {
        m_index = index;
        m_onCancel = onCancel;
        m_hasAction = action != null;

        if (m_labelText != null)
        {
            m_labelText.text = action != null ? action.Label : "";
        }

        // アイコンは未設定時は無効化
        if (m_actionIcon != null)
        {
            m_actionIcon.enabled = false;
        }

        if (m_targetIcon != null)
        {
            m_targetIcon.enabled = false;
        }
    }

    /// <summary>
    /// 空スロット表示にします。
    /// </summary>
    public void SetupEmpty()
    {
        m_index = -1;
        m_onCancel = null;
        m_hasAction = false;

        if (m_labelText != null) m_labelText.text = "";
        if (m_actionIcon != null) m_actionIcon.enabled = false;
        if (m_targetIcon != null) m_targetIcon.enabled = false;
    }

    /// <summary>
    /// 右クリックで予約をキャンセルします。
    /// </summary>
    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Right) return;
        HandleCancel();
    }

    private void HandleCancel()
    {
        if (!m_hasAction) return;
        m_onCancel?.Invoke(m_index);
    }
}
