using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// 1回の戦闘で同時に出現する敵の編成(エンカウントグループ)。
/// 例: [Enemy00, Enemy00] や [Enemy02, Enemy03] のような組み合わせを定義します。
/// </summary>
[System.Serializable]
public class EnemyEncounterGroup
{
    [SerializeField]
    [Header("出現する敵一覧")]
    [Tooltip("この編成が選ばれたときに同時に出現する敵のEnemyDefinition。同じ敵を複数登録可能です")]
    private List<EnemyDefinition> m_enemies = new List<EnemyDefinition>();

    /// <summary>この編成で出現する敵一覧。</summary>
    public IReadOnlyList<EnemyDefinition> Enemies => m_enemies;

    /// <summary>null要素を除いた有効な敵が1体以上いるか。</summary>
    public bool HasValidEnemy => m_enemies != null && m_enemies.Any(enemy => enemy != null);
}

/// <summary>
/// Battleノード用の敵エンカウントプール。
/// 複数のエンカウントグループを登録し、戦闘開始時にそこから1グループをランダム抽選します。
/// Pool1 / Pool2 のようにアセットを分けて作成し、RunSystemManagerに登録して使用します。
/// </summary>
[CreateAssetMenu(fileName = "EnemyEncounterPool_", menuName = "DiceGame/EnemyEncounterPool")]
public class EnemyEncounterPool : ScriptableObject
{
    [SerializeField]
    [Header("エンカウントグループ一覧")]
    [Tooltip("このプールから抽選される敵編成のリスト\n" +
             "例: 「Enemy00×2」「Enemy01」「Enemy02+Enemy03」の3グループを登録すると、その中から1つがランダムに選ばれます")]
    private List<EnemyEncounterGroup> m_encounterGroups = new List<EnemyEncounterGroup>();

    /// <summary>登録済みのエンカウントグループ一覧。</summary>
    public IReadOnlyList<EnemyEncounterGroup> EncounterGroups => m_encounterGroups;

    /// <summary>
    /// 有効なエンカウントグループから1つをランダムに抽選します。
    /// 有効なグループが存在しない場合は null を返します。
    /// </summary>
    public EnemyEncounterGroup GetRandomGroup()
    {
        if (m_encounterGroups == null || m_encounterGroups.Count == 0)
        {
            return null;
        }

        List<EnemyEncounterGroup> validGroups = m_encounterGroups
            .Where(group => group != null && group.HasValidEnemy)
            .ToList();
        if (validGroups.Count == 0)
        {
            return null;
        }

        int randomIndex = Random.Range(0, validGroups.Count);
        return validGroups[randomIndex];
    }
}
