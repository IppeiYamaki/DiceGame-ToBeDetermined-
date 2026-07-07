using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

/// <summary>
/// 役倍率一覧パネルの表示切替用Controller
/// 役一覧の本格UIは保留し、後から常時表示/ボタン表示どちらにも対応できる土台です
/// </summary>
public class RoleListPanelController : MonoBehaviour
{
    [SerializeField]
    [Header("役一覧パネル")]
    private GameObject m_roleListPanel;

    [SerializeField]
    [Header("役一覧テキスト（仮表示）")]
    private TMP_Text m_roleListText;

    [SerializeField]
    [Header("開始時に表示する")]
    private bool m_showOnStart;

    private void Start()
    {
        SetVisible(m_showOnStart);
    }

    /// <summary>
    /// パネル表示を切り替えます
    /// ボタンのOnClickから呼び出せます
    /// </summary>
    public void ToggleVisible()
    {
        bool currentVisible = m_roleListPanel != null ? m_roleListPanel.activeSelf : gameObject.activeSelf;
        SetVisible(!currentVisible);
    }

    /// <summary>
    /// パネルの表示/非表示を設定します
    /// </summary>
    /// <param name="visible">表示する場合はtrue</param>
    public void SetVisible(bool visible)
    {
        if (m_roleListPanel != null)
        {
            m_roleListPanel.SetActive(visible);
        }
        else
        {
            gameObject.SetActive(visible);
        }
    }

    /// <summary>
    /// DiceRoleDefinition一覧から仮テキスト表示を更新します
    /// 将来的にPrefab行生成へ差し替える想定です
    /// </summary>
    /// <param name="roleDefinitions">表示する役定義リスト</param>
    public void UpdateRoleList(IReadOnlyList<DiceRoleDefinition> roleDefinitions)
    {
        if (m_roleListText == null) return;

        if (roleDefinitions == null || roleDefinitions.Count == 0)
        {
            m_roleListText.text = "役なし";
            return;
        }

        m_roleListText.text = string.Join("\n", roleDefinitions
            .Where(role => role != null)
            .Select(role => $"{role.RoleName}: x{role.Multiplier:0.##}"));
    }
}
