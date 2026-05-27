using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// マスタ ScriptableObject（DiceDefinition / DiceRoleDefinition）を
/// 永続 ID から実体参照へ解決するためのレジストリ
/// 
/// セーブデータには ScriptableObject 参照ではなく PersistentId（GUID 文字列）を保存し、
/// ロード時にこのレジストリを介して実体 SO を引き当てます
/// 
/// 作成方法:
/// Project ビュー右クリック → Create → DiceGame/DiceMasterRegistry
/// 
/// 使い方:
/// 1. すべての DiceDefinition と DiceRoleDefinition を Inspector で登録します
/// 2. シーン起動時などに `DiceMasterRegistry.SetActive(registry)` でアクティブインスタンスを設定します
/// 3. ロジッククラスから `DiceMasterRegistry.Active` 経由でアクセスします
/// </summary>
[CreateAssetMenu(fileName = "DiceMasterRegistry", menuName = "DiceGame/DiceMasterRegistry")]
public class DiceMasterRegistry : ScriptableObject
{
    // ─────────────────────────────────────────────────────────
    // 登録用リスト（Inspector から設定）
    // ─────────────────────────────────────────────────────────

    [SerializeField]
    [Header("ダイス定義（マスタ）")]
    [Tooltip("セーブ/ロードで参照させたい DiceDefinition アセットを登録します")]
    private List<DiceDefinition> m_diceDefinitions = new List<DiceDefinition>();

    [SerializeField]
    [Header("役定義（マスタ）")]
    [Tooltip("セーブ/ロードで参照させたい DiceRoleDefinition アセットを登録します")]
    private List<DiceRoleDefinition> m_roleDefinitions = new List<DiceRoleDefinition>();

    // ─────────────────────────────────────────────────────────
    // ID → 実体 のキャッシュ（最初のアクセス時に構築）
    // ─────────────────────────────────────────────────────────

    private Dictionary<string, DiceDefinition> m_definitionById;
    private Dictionary<string, DiceRoleDefinition> m_roleById;

    // ─────────────────────────────────────────────────────────
    // アクティブインスタンス（プロセス全体でひとつ）
    // ─────────────────────────────────────────────────────────

    private static DiceMasterRegistry s_active;

    /// <summary>
    /// 現在アクティブなレジストリ
    /// シーン起動時などに <see cref="SetActive"/> で設定してください
    /// </summary>
    public static DiceMasterRegistry Active => s_active;

    /// <summary>
    /// アクティブなレジストリを設定します（null で解除）
    /// </summary>
    /// <param name="registry">設定するレジストリ（null可）</param>
    public static void SetActive(DiceMasterRegistry registry)
    {
        s_active = registry;
    }

    // ─────────────────────────────────────────────────────────
    // 解決 API
    // ─────────────────────────────────────────────────────────

    /// <summary>
    /// 永続 ID から DiceDefinition を取得します（見つからなければ null）
    /// </summary>
    /// <param name="persistentId">永続 ID（GUID 文字列）</param>
    /// <returns>対応する DiceDefinition（見つからなければ null）</returns>
    public DiceDefinition ResolveDefinition(string persistentId)
    {
        EnsureCache();
        if (string.IsNullOrEmpty(persistentId)) return null;
        return m_definitionById.TryGetValue(persistentId, out DiceDefinition def) ? def : null;
    }

    /// <summary>
    /// 永続 ID から DiceRoleDefinition を取得します（見つからなければ null）
    /// </summary>
    /// <param name="persistentId">永続 ID（GUID 文字列）</param>
    /// <returns>対応する DiceRoleDefinition（見つからなければ null）</returns>
    public DiceRoleDefinition ResolveRole(string persistentId)
    {
        EnsureCache();
        if (string.IsNullOrEmpty(persistentId)) return null;
        return m_roleById.TryGetValue(persistentId, out DiceRoleDefinition role) ? role : null;
    }

    /// <summary>
    /// キャッシュを強制再構築します
    /// 実行中に Inspector で登録内容を変更したときに呼んでください
    /// </summary>
    public void RebuildCache()
    {
        m_definitionById = null;
        m_roleById = null;
        EnsureCache();
    }

    /// <summary>
    /// 登録されているすべての DiceDefinition を取得します（読み取り専用）
    /// </summary>
    public IReadOnlyList<DiceDefinition> AllDiceDefinitions => m_diceDefinitions;

    /// <summary>
    /// 登録されているすべての DiceRoleDefinition を取得します（読み取り専用）
    /// </summary>
    public IReadOnlyList<DiceRoleDefinition> AllRoleDefinitions => m_roleDefinitions;

    // ─────────────────────────────────────────────────────────
    // 内部実装
    // ─────────────────────────────────────────────────────────

    /// <summary>
    /// ID → 実体 の辞書をまだ作っていなければ構築する
    /// </summary>
    private void EnsureCache()
    {
        if (m_definitionById != null && m_roleById != null) return;

        // DiceDefinition のキャッシュ構築
        m_definitionById = new Dictionary<string, DiceDefinition>();
        foreach (DiceDefinition def in m_diceDefinitions)
        {
            if (def == null) continue;
            string id = def.PersistentId;
            if (string.IsNullOrEmpty(id))
            {
                Debug.LogWarning($"[DiceMasterRegistry] DiceDefinition '{def.name}' に PersistentId がありません。スキップします。");
                continue;
            }
            if (m_definitionById.TryGetValue(id, out DiceDefinition existing) && existing != def)
            {
                Debug.LogError($"[DiceMasterRegistry] DiceDefinition の PersistentId が重複しています: " +
                               $"'{existing.name}' と '{def.name}' (id={id})。後者を採用します。");
            }
            m_definitionById[id] = def;
        }

        // DiceRoleDefinition のキャッシュ構築
        m_roleById = new Dictionary<string, DiceRoleDefinition>();
        foreach (DiceRoleDefinition role in m_roleDefinitions)
        {
            if (role == null) continue;
            string id = role.PersistentId;
            if (string.IsNullOrEmpty(id))
            {
                Debug.LogWarning($"[DiceMasterRegistry] DiceRoleDefinition '{role.name}' に PersistentId がありません。スキップします。");
                continue;
            }
            if (m_roleById.TryGetValue(id, out DiceRoleDefinition existingRole) && existingRole != role)
            {
                Debug.LogError($"[DiceMasterRegistry] DiceRoleDefinition の PersistentId が重複しています: " +
                               $"'{existingRole.name}' と '{role.name}' (id={id})。後者を採用します。");
            }
            m_roleById[id] = role;
        }
    }
}
