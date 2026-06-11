using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// バフ/デバフ定義（StatusEffectDefinition）を一元管理するレジストリ
/// DiceMasterRegistry と同様に、永続 ID から定義への解決を行います
/// 
/// 作成方法:
/// Project ビュー右クリック → Create → DiceGame/StatusEffectRegistry
/// 
/// 使い方:
/// 1. すべての StatusEffectDefinition を Inspector で登録します
/// 2. シーン起動時などに `StatusEffectRegistry.SetActive(registry)` でアクティブインスタンスを設定します
/// 3. ロジッククラスから `StatusEffectRegistry.Active` 経由でアクセスします
/// </summary>
[CreateAssetMenu(fileName = "StatusEffectRegistry", menuName = "DiceGame/StatusEffectRegistry")]
public class StatusEffectRegistry : ScriptableObject
{
    // ─────────────────────────────────────────────────────────
    // 登録用リスト（Inspector から設定）
    // ─────────────────────────────────────────────────────────

    [SerializeField]
    [Header("バフ/デバフ定義（マスタ）")]
    [Tooltip("セーブ/ロードで参照させたい StatusEffectDefinition アセットを登録します")]
    private List<StatusEffectDefinition> m_effectDefinitions = new List<StatusEffectDefinition>();

    // ─────────────────────────────────────────────────────────
    // ID → 実体 のキャッシュ（最初のアクセス時に構築）
    // ─────────────────────────────────────────────────────────

    private Dictionary<string, StatusEffectDefinition> m_effectById;

    // ─────────────────────────────────────────────────────────
    // アクティブインスタンス（プロセス全体でひとつ）
    // ─────────────────────────────────────────────────────────

    private static StatusEffectRegistry s_active;

    /// <summary>
    /// 現在アクティブなレジストリ
    /// シーン起動時などに <see cref="SetActive"/> で設定してください
    /// </summary>
    public static StatusEffectRegistry Active => s_active;

    /// <summary>
    /// アクティブなレジストリを設定します（null で解除）
    /// </summary>
    /// <param name="registry">設定するレジストリ（null可）</param>
    public static void SetActive(StatusEffectRegistry registry)
    {
        s_active = registry;
    }

    // ─────────────────────────────────────────────────────────
    // 解決 API
    // ─────────────────────────────────────────────────────────

    /// <summary>
    /// 永続 ID から StatusEffectDefinition を取得します（見つからなければ null）
    /// </summary>
    /// <param name="persistentId">永続 ID（GUID 文字列）</param>
    /// <returns>対応する StatusEffectDefinition（見つからなければ null）</returns>
    public StatusEffectDefinition ResolveEffect(string persistentId)
    {
        EnsureCache();
        if (string.IsNullOrEmpty(persistentId)) return null;
        return m_effectById.TryGetValue(persistentId, out StatusEffectDefinition effect) ? effect : null;
    }

    /// <summary>
    /// キャッシュを強制再構築します
    /// 実行中に Inspector で登録内容を変更したときに呼んでください
    /// </summary>
    public void RebuildCache()
    {
        m_effectById = null;
        EnsureCache();
    }

    /// <summary>
    /// 登録されているすべての StatusEffectDefinition を取得します（読み取り専用）
    /// </summary>
    public IReadOnlyList<StatusEffectDefinition> AllEffectDefinitions => m_effectDefinitions;

    // ─────────────────────────────────────────────────────────
    // 内部実装
    // ─────────────────────────────────────────────────────────

    /// <summary>
    /// ID → 実体 の辞書をまだ作っていなければ構築する
    /// </summary>
    private void EnsureCache()
    {
        if (m_effectById != null) return;

        m_effectById = new Dictionary<string, StatusEffectDefinition>();
        foreach (StatusEffectDefinition effect in m_effectDefinitions)
        {
            if (effect == null) continue;
            string id = effect.PersistentId;
            if (string.IsNullOrEmpty(id))
            {
                Debug.LogWarning($"[StatusEffectRegistry] StatusEffectDefinition '{effect.name}' に PersistentId がありません。スキップします。");
                continue;
            }
            if (m_effectById.TryGetValue(id, out StatusEffectDefinition existing) && existing != effect)
            {
                Debug.LogError($"[StatusEffectRegistry] StatusEffectDefinition の PersistentId が重複しています: " +
                               $"'{existing.name}' と '{effect.name}' (id={id})。後者を採用します。");
            }
            m_effectById[id] = effect;
        }
    }
}
