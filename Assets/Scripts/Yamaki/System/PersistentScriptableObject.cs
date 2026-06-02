using System;
using UnityEngine;

/// <summary>
/// 永続 ID（GUID 文字列）を保持する ScriptableObject 基底クラス
/// セーブ/ロード時に SO 参照を直接シリアライズせず、ID 経由で復元するために使用します
/// 
/// 使用方法:
/// - マスターデータ ScriptableObject の基底クラスとしてこの型を継承します
/// - Editor 上では新規アセット作成時に自動で GUID を採番します
/// - 実行時には事前にシリアライズ済みの ID 値を読み出すだけです
/// 
/// 注意:
/// - ID は自動付与されるため、手動で編集しないでください
/// - Inspector 上では ReadOnly 属性により編集不可になっています
/// </summary>
public abstract class PersistentScriptableObject : ScriptableObject
{
    // ─────────────────────────────────────────────────────────
    // 永続 ID（GUID 文字列）
    // ─────────────────────────────────────────────────────────

    [SerializeField, ReadOnly]
    [Header("永続 ID（自動付与）")]
    [Tooltip("セーブ/ロード時にこの ScriptableObject アセットを一意に識別するための GUID 文字列\n" +
             "Editor で空のときに自動採番されます。手で書き換えないでください。")]
    private string m_persistentId = "";

    // ─────────────────────────────────────────────────────────
    // 読み取り専用プロパティ
    // ─────────────────────────────────────────────────────────

    /// <summary>
    /// このアセットの永続 ID（GUID 文字列）
    /// </summary>
    public string PersistentId => m_persistentId;

    // ─────────────────────────────────────────────────────────
    // ID 自動採番メソッド
    // ─────────────────────────────────────────────────────────

    /// <summary>
    /// 永続 ID が未設定なら新しく採番します
    /// 通常は Editor 経由で自動的に呼ばれるため、ランタイムから直接呼ぶ必要はありません
    /// </summary>
    public void EnsurePersistentId()
    {
        if (string.IsNullOrEmpty(m_persistentId))
        {
            m_persistentId = Guid.NewGuid().ToString("N");
#if UNITY_EDITOR
            UnityEditor.EditorUtility.SetDirty(this);
#endif
        }
    }

#if UNITY_EDITOR
    /// <summary>
    /// Editor 上でアセットが読み込まれた / 値が編集されたタイミングで ID を確保します
    /// ランタイムでは呼ばれません
    /// </summary>
    protected virtual void OnValidate()
    {
        EnsurePersistentId();
    }
#endif
}
