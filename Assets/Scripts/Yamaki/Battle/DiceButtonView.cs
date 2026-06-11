using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// ダイス選択画面に表示する1個分のUIボタン。
/// </summary>
public class DiceButtonView : MonoBehaviour
{
    [SerializeField]
    [Header("ダイス名テキスト")]
    private TMP_Text m_diceNameText;

    [SerializeField]
    [Header("出目テキスト")]
    private TMP_Text m_rollNumberText;

    [SerializeField]
    [Header("選択中表示")]
    private GameObject m_selectedMarker;

    [SerializeField]
    [Header("クリックボタン")]
    private Button m_button;

    private DiceDefinition m_diceDefinition;
    private int m_index = -1;
    private bool m_isSelected;
    private Action<DiceButtonView> m_clicked;

    public DiceDefinition DiceDefinition => m_diceDefinition;
    public int Index => m_index;
    public bool IsSelected => m_isSelected;

    private void Awake()
    {
        if (m_button == null)
        {
            m_button = GetComponent<Button>();
        }

        if (m_button != null)
        {
            m_button.onClick.AddListener(HandleClick);
        }
    }

    private void OnDestroy()
    {
        if (m_button != null)
        {
            m_button.onClick.RemoveListener(HandleClick);
        }
    }

    public void Setup(int index, DiceDefinition diceDefinition, Action<DiceButtonView> clicked)
    {
        m_index = index;
        m_diceDefinition = diceDefinition;
        m_clicked = clicked;
        SetSelected(false);
        SetRollNumber(0);

        if (m_diceNameText != null)
        {
            m_diceNameText.text = diceDefinition != null ? diceDefinition.DiceName : "No Dice";
        }

        gameObject.SetActive(diceDefinition != null);
    }

    public void SetInteractable(bool interactable)
    {
        if (m_button != null)
        {
            m_button.interactable = interactable;
        }
    }

    public void SetSelected(bool isSelected)
    {
        m_isSelected = isSelected;

        if (m_selectedMarker != null)
        {
            m_selectedMarker.SetActive(isSelected);
        }
    }

    public void SetRollNumber(int number)
    {
        if (m_rollNumberText != null)
        {
            m_rollNumberText.text = number > 0 ? number.ToString() : "-";
        }
    }

    private void HandleClick()
    {
        if (m_diceDefinition == null) return;
        m_clicked?.Invoke(this);
    }
}
