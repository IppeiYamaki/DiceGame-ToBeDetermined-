using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// 攻撃・防御など、使用APを上下ボタンで調整するコマンドView。
/// </summary>
public class AdjustableCostCommandView : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [SerializeField]
    [Header("コマンド名")]
    private string m_commandName = "Command";

    [SerializeField]
    [Header("コマンド名テキスト")]
    private TMP_Text m_commandNameText;

    [SerializeField]
    [Header("コストテキスト")]
    private TMP_Text m_costText;

    [SerializeField]
    [Header("増加ボタン")]
    private Button m_increaseButton;

    [SerializeField]
    [Header("減少ボタン")]
    private Button m_decreaseButton;

    [SerializeField]
    [Header("実行ボタン")]
    private Button m_executeButton;

    [SerializeField]
    [Header("最小コスト")]
    [Min(0)]
    private int m_minCost = 0;

    [SerializeField]
    [Header("初期コスト")]
    [Min(0)]
    private int m_initialCost = 1;

    [SerializeField]
    [Header("コスト表示フォーマット")]
    private string m_costFormat = "Pt: {0}";

    private int m_currentCost;
    private int m_maxCost = int.MaxValue;
    private bool m_isInteractable = true;

    public event Action<int> CostChanged;
    public event Action<int> CommandExecuted;
    public event Action<int, PointerEventData> DragStarted;
    public event Action<int, PointerEventData> DragEnded;

    /// <summary>現在設定中のコスト。</summary>
    public int CurrentCost => m_currentCost;

    /// <summary>コマンド名。</summary>
    public string CommandName => m_commandName;

    private void Awake()
    {
        m_currentCost = Mathf.Max(m_minCost, m_initialCost);
        RegisterEvents();
        Refresh();
    }

    private void OnDestroy()
    {
        UnregisterEvents();
    }

    /// <summary>
    /// 操作可能状態を設定します。
    /// </summary>
    public void SetInteractable(bool isInteractable)
    {
        m_isInteractable = isInteractable;
        Refresh();
    }

    /// <summary>
    /// 現在APに応じた最大コストを設定します。
    /// </summary>
    public void SetMaxCost(int maxCost)
    {
        m_maxCost = Mathf.Max(m_minCost, maxCost);
        if (m_currentCost > m_maxCost)
        {
            SetCost(m_maxCost);
            return;
        }

        Refresh();
    }

    /// <summary>
    /// コストを直接設定します。
    /// </summary>
    public void SetCost(int cost)
    {
        int nextCost = Mathf.Clamp(cost, m_minCost, m_maxCost);
        if (m_currentCost == nextCost)
        {
            Refresh();
            return;
        }

        m_currentCost = nextCost;
        Refresh();
        CostChanged?.Invoke(m_currentCost);
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (!m_isInteractable) return;
        DragStarted?.Invoke(m_currentCost, eventData);
    }

    public void OnDrag(PointerEventData eventData)
    {
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (!m_isInteractable) return;
        DragEnded?.Invoke(m_currentCost, eventData);
    }

    private void IncreaseCost()
    {
        SetCost(m_currentCost + 1);
    }

    private void DecreaseCost()
    {
        SetCost(m_currentCost - 1);
    }

    private void ExecuteCommand()
    {
        if (!m_isInteractable) return;
        CommandExecuted?.Invoke(m_currentCost);
    }

    private void Refresh()
    {
        if (m_commandNameText != null)
        {
            m_commandNameText.text = m_commandName;
        }

        if (m_costText != null)
        {
            m_costText.text = string.Format(m_costFormat, m_currentCost);
        }

        if (m_increaseButton != null)
        {
            m_increaseButton.interactable = m_isInteractable && m_currentCost < m_maxCost;
        }

        if (m_decreaseButton != null)
        {
            m_decreaseButton.interactable = m_isInteractable && m_currentCost > m_minCost;
        }

        if (m_executeButton != null)
        {
            m_executeButton.interactable = m_isInteractable && m_currentCost <= m_maxCost;
        }
    }

    private void RegisterEvents()
    {
        if (m_increaseButton != null) m_increaseButton.onClick.AddListener(IncreaseCost);
        if (m_decreaseButton != null) m_decreaseButton.onClick.AddListener(DecreaseCost);
        if (m_executeButton != null) m_executeButton.onClick.AddListener(ExecuteCommand);
    }

    private void UnregisterEvents()
    {
        if (m_increaseButton != null) m_increaseButton.onClick.RemoveListener(IncreaseCost);
        if (m_decreaseButton != null) m_decreaseButton.onClick.RemoveListener(DecreaseCost);
        if (m_executeButton != null) m_executeButton.onClick.RemoveListener(ExecuteCommand);
    }
}
