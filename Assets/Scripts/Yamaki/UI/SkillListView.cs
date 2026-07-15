using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 所持スキルを一覧表示するView。
/// </summary>
public class SkillListView : MonoBehaviour
{
    [SerializeField]
    [Header("スキルボタンPrefab")]
    private SkillButtonView m_skillButtonPrefab;

    [SerializeField]
    [Header("配置先")]
    private Transform m_contentRoot;

    private readonly List<SkillButtonView> m_buttonViews = new List<SkillButtonView>();

    /// <summary>
    /// スキル一覧を再構築します。
    /// </summary>
    public void Refresh(IReadOnlyList<SkillDefinition> skills, int currentActionPoint, Action<SkillDefinition> clicked)
    {
        EnsureButtonCount(skills != null ? skills.Count : 0);

        for (int i = 0; i < m_buttonViews.Count; i++)
        {
            SkillButtonView buttonView = m_buttonViews[i];
            if (buttonView == null) continue;

            bool hasSkill = skills != null && i < skills.Count && skills[i] != null;
            buttonView.gameObject.SetActive(hasSkill);
            if (hasSkill)
            {
                buttonView.Setup(skills[i], currentActionPoint, clicked);
            }
        }
    }

    private void EnsureButtonCount(int requiredCount)
    {
        if (m_skillButtonPrefab == null) return;

        Transform root = m_contentRoot != null ? m_contentRoot : transform;
        while (m_buttonViews.Count < requiredCount)
        {
            SkillButtonView buttonView = Instantiate(m_skillButtonPrefab, root);
            m_buttonViews.Add(buttonView);
        }
    }
}
