using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 戦闘中に6個のダイスから3個を選択するUI。
/// </summary>
public class DiceSelectionView : MonoBehaviour
{
    private const int RequiredSelectionCount = 3;

    public event Action SelectionChanged;

    [SerializeField]
    [Header("選択パネル")]
    private GameObject m_selectionPanel;

    [SerializeField]
    [Header("ダイススロット")]
    private List<DiceButtonView> m_slotViews = new List<DiceButtonView>();

    [SerializeField]
    [Header("決定ボタン")]
    private Button m_confirmButton;

    [SerializeField]
    [Header("選択数テキスト")]
    private TMP_Text m_selectionCountText;

    private readonly List<DiceButtonView> m_selectedSlots = new List<DiceButtonView>();
    private readonly List<DiceButtonView> m_lockedRollSlots = new List<DiceButtonView>();
    private Action<DiceDefinition[]> m_confirmed;
    private bool m_isSelecting;

    public bool IsSelecting => m_isSelecting;
    public int SelectedCount => m_selectedSlots.Count;
    public bool CanConfirmSelection => m_isSelecting && m_selectedSlots.Count == RequiredSelectionCount;

    private void Awake()
    {
        if (m_confirmButton != null)
        {
            m_confirmButton.onClick.AddListener(HandleConfirm);
        }

        SetPanelActive(false);
    }

    private void OnDestroy()
    {
        if (m_confirmButton != null)
        {
            m_confirmButton.onClick.RemoveListener(HandleConfirm);
        }
    }

    public void BeginSelection(IReadOnlyList<DiceDefinition> diceDefinitions, Action<DiceDefinition[]> confirmed)
    {
        m_confirmed = confirmed;
        m_isSelecting = true;
        m_selectedSlots.Clear();
        m_lockedRollSlots.Clear();

        for (int i = 0; i < m_slotViews.Count; i++)
        {
            DiceDefinition diceDefinition = diceDefinitions != null && i < diceDefinitions.Count ? diceDefinitions[i] : null;
            DiceButtonView slotView = m_slotViews[i];
            if (slotView == null) continue;

            slotView.Setup(i, diceDefinition, HandleSlotClicked);
            slotView.SetInteractable(diceDefinition != null);
        }

        SetPanelActive(true);
        RefreshState();
    }

    public void ClearSelection()
    {
        foreach (DiceButtonView slotView in m_selectedSlots)
        {
            if (slotView != null)
            {
                slotView.SetSelected(false);
            }
        }

        m_selectedSlots.Clear();
        RefreshState();
    }

    public DiceDefinition[] GetSelectedDiceDefinitions()
    {
        DiceDefinition[] selectedDice = new DiceDefinition[m_selectedSlots.Count];
        for (int i = 0; i < m_selectedSlots.Count; i++)
        {
            selectedDice[i] = m_selectedSlots[i] != null ? m_selectedSlots[i].DiceDefinition : null;
        }

        return selectedDice;
    }

    public string[] GetSelectedDiceIds()
    {
        DiceDefinition[] selectedDice = GetSelectedDiceDefinitions();
        string[] selectedDiceIds = new string[selectedDice.Length];
        for (int i = 0; i < selectedDice.Length; i++)
        {
            selectedDiceIds[i] = selectedDice[i] != null ? selectedDice[i].PersistentId : string.Empty;
        }

        return selectedDiceIds;
    }

    public bool TryGetRequiredSelectedDice(out DiceDefinition[] selectedDice)
    {
        selectedDice = GetSelectedDiceDefinitions();
        return selectedDice.Length == RequiredSelectionCount && Array.TrueForAll(selectedDice, dice => dice != null);
    }

    public void LockCurrentSelectionForRoll()
    {
        if (m_selectedSlots.Count == 0) return;

        m_lockedRollSlots.Clear();
        foreach (DiceButtonView slotView in m_selectedSlots)
        {
            if (slotView != null)
            {
                m_lockedRollSlots.Add(slotView);
            }
        }
    }

    public void EndSelection()
    {
        m_isSelecting = false;
        m_selectedSlots.Clear();
        m_confirmed = null;
        SetPanelActive(false);
        RefreshState();
    }

    public void ShowRollNumbers(DiceRollResult rollResult)
    {
        for (int i = 0; i < m_slotViews.Count; i++)
        {
            DiceButtonView slotView = m_slotViews[i];
            if (slotView == null) continue;

            slotView.SetRollNumber(0);
        }

        if (rollResult.RollData == null)
        {
            return;
        }

        if (m_lockedRollSlots.Count > 0)
        {
            int displayCount = Mathf.Min(m_lockedRollSlots.Count, rollResult.RollData.Length);
            for (int i = 0; i < displayCount; i++)
            {
                DiceButtonView slotView = m_lockedRollSlots[i];
                if (slotView != null)
                {
                    slotView.SetRollNumber(rollResult.RollData[i].Number);
                }
            }

            return;
        }

        for (int i = 0; i < m_slotViews.Count; i++)
        {
            DiceButtonView slotView = m_slotViews[i];
            if (slotView == null) continue;

            int number = 0;
            for (int resultIndex = 0; resultIndex < rollResult.RollData.Length; resultIndex++)
            {
                if (slotView.DiceDefinition != null && rollResult.RollData[resultIndex].DiceId == slotView.DiceDefinition.PersistentId)
                {
                    number = rollResult.RollData[resultIndex].Number;
                    break;
                }
            }

            slotView.SetRollNumber(number);
        }
    }

    private void HandleSlotClicked(DiceButtonView slotView)
    {
        if (!m_isSelecting || slotView == null) return;

        if (slotView.IsSelected)
        {
            slotView.SetSelected(false);
            m_selectedSlots.Remove(slotView);
        }
        else
        {
            if (m_selectedSlots.Count >= RequiredSelectionCount) return;

            slotView.SetSelected(true);
            m_selectedSlots.Add(slotView);
        }

        RefreshState();
    }

    private void HandleConfirm()
    {
        if (!TryGetRequiredSelectedDice(out DiceDefinition[] selectedDice)) return;

        LockCurrentSelectionForRoll();
        Action<DiceDefinition[]> confirmed = m_confirmed;
        EndSelection();
        confirmed?.Invoke(selectedDice);
    }

    private void RefreshState()
    {
        if (m_confirmButton != null)
        {
            m_confirmButton.interactable = CanConfirmSelection;
        }

        if (m_selectionCountText != null)
        {
            m_selectionCountText.text = $"選択数: {m_selectedSlots.Count}/{RequiredSelectionCount}";
        }

        SelectionChanged?.Invoke();
    }

    private void SetPanelActive(bool isActive)
    {
        if (m_selectionPanel != null)
        {
            m_selectionPanel.SetActive(isActive);
        }
        else
        {
            gameObject.SetActive(isActive);
        }
    }
}
