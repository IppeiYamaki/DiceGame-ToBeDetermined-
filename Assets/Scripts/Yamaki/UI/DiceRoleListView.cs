using System.Collections.Generic;
using TMPro;
using UnityEngine;

/// <summary>
/// ダイスの役一覧を表示する簡易View。
/// DiceMasterRegistry から役定義を取得して行Prefabを生成します。
/// </summary>
public class DiceRoleListView : MonoBehaviour
{
    [SerializeField]
    [Header("行Prefab（TextMeshPro - Text）")]
    private GameObject m_rowPrefab;

    [SerializeField]
    [Header("配置先Transform")]
    private Transform m_container;

    public void Refresh()
    {
        foreach (Transform child in m_container)
        {
            Destroy(child.gameObject);
        }

        DiceMasterRegistry registry = DiceMasterRegistry.Active;
        if (registry == null && RunSystemManager.Instance != null)
        {
            registry = RunSystemManager.Instance.DiceMasterRegistry;
        }

        if (registry == null || registry.AllRoleDefinitions == null) return;

        foreach (var role in registry.AllRoleDefinitions)
        {
            if (m_rowPrefab == null) break;
            var go = Instantiate(m_rowPrefab, m_container);
            var text = go.GetComponent<TMP_Text>();
            if (text != null)
            {
                text.text = $"{role.RoleName} x{role.Multiplier:0.##}";
            }
        }
    }
}
