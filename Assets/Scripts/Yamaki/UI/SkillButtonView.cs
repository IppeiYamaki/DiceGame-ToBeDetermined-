using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 1つのスキルを表示・発動するボタンView。
/// </summary>
public class SkillButtonView : MonoBehaviour
{
    [SerializeField]
    [Header("スキル名テキスト")]
    private TMP_Text m_skillNameText;

    [SerializeField]
    [Header("コストテキスト")]
    private TMP_Text m_costText;

    [SerializeField]
    [Header("説明テキスト")]
    private TMP_Text m_descriptionText;

    [SerializeField]
    [Header("アイコン")]
    private Image m_iconImage;

    [SerializeField]
    [Header("発動ボタン")]
    private Button m_button;

    private SkillDefinition m_skillDefinition;
    private Action<SkillDefinition> m_clicked;

    private void Awake()
    {
        if (m_button != null)
        {
            m_button.onClick.AddListener(HandleClicked);
        }
    }

    private void OnDestroy()
    {
        if (m_button != null)
        {
            m_button.onClick.RemoveListener(HandleClicked);
        }
    }

    /// <summary>
    /// スキル表示を設定します。
    /// </summary>
    public void Setup(SkillDefinition skillDefinition, int currentActionPoint, Action<SkillDefinition> clicked)
    {
        m_skillDefinition = skillDefinition;
        m_clicked = clicked;

        string skillName = skillDefinition != null ? skillDefinition.SkillName : "-";
        int cost = skillDefinition != null ? skillDefinition.Cost : 0;
        string description = skillDefinition != null ? skillDefinition.Description : "";
        Sprite icon = skillDefinition != null ? skillDefinition.Icon : null;

        if (m_skillNameText != null)
        {
            m_skillNameText.text = skillName;
        }

        if (m_costText != null)
        {
            m_costText.text = $"Cost: {cost}";
        }

        if (m_descriptionText != null)
        {
            m_descriptionText.text = description;
        }

        if (m_iconImage != null)
        {
            m_iconImage.sprite = icon;
            m_iconImage.enabled = icon != null;
        }

        if (m_button != null)
        {
            m_button.interactable = skillDefinition != null && currentActionPoint >= cost;
        }
    }

    private void HandleClicked()
    {
        if (m_skillDefinition == null) return;
        m_clicked?.Invoke(m_skillDefinition);
    }
}
