using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// 敵表示エリア。EnemyView Prefab をコンテナに生成し自動整列します。
/// </summary>
public class EnemyAreaView : MonoBehaviour
{
    [SerializeField]
    [Header("EnemyView Prefab")]
    private GameObject m_enemyViewPrefab;

    [SerializeField]
    [Header("生成コンテナ（HorizontalLayoutGroupを想定）")]
    private Transform m_container;

    private readonly List<GameObject> m_spawned = new List<GameObject>();

    /// <summary>
    /// 生成した EnemyView オブジェクトの参照
    /// </summary>
    public IReadOnlyList<GameObject> SpawnedEnemies => m_spawned;

    public void SpawnEnemies(IReadOnlyList<EnemyDefinition> defs)
    {
        Clear();
        if (defs == null || m_enemyViewPrefab == null || m_container == null) return;

        for (int i = 0; i < defs.Count; i++)
        {
            var go = Instantiate(m_enemyViewPrefab, m_container);
            // EnemyView があれば初期表示を行う
            var ev = go.GetComponent<EnemyView>();
            if (ev != null)
            {
                ev.UpdateEnemy(defs[i], defs[i].MaxHp);
            }

            // EnemyDropTarget があればインデックスをセット
            var drop = go.GetComponentInChildren<EnemyDropTarget>();
            if (drop != null)
            {
                drop.InitIndex(i);
            }

            m_spawned.Add(go);
        }
    }

    public void Clear()
    {
        foreach (var go in m_spawned.ToArray())
        {
            if (go != null) Destroy(go);
        }
        m_spawned.Clear();
    }

    public void RemoveAt(int index)
    {
        if (index < 0 || index >= m_spawned.Count) return;
        var go = m_spawned[index];
        m_spawned.RemoveAt(index);
        if (go != null) Destroy(go);

        // 残りのEnemyDropTargetのインデックスを更新
        for (int i = 0; i < m_spawned.Count; i++)
        {
            var drop = m_spawned[i].GetComponentInChildren<EnemyDropTarget>();
            if (drop != null) drop.InitIndex(i);
        }
    }
}
