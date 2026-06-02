using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ダイス本体を表す ScriptableObject
/// 6面分の DiceFaceData を持ち、各面の数字と属性を定義します
/// 
/// 作成方法:
/// Project ビュー右クリック → Create → DiceGame/DiceDefinition
/// 
/// 使用方法:
/// - Inspector でダイス名を設定します
/// - 6面分のデータ（目の数字と属性リスト）を設定します
/// - プレイヤーは PersistentId（自動付与される GUID）のみを保持します
/// </summary>
[CreateAssetMenu(fileName = "DiceDefinition_", menuName = "DiceGame/DiceDefinition")]
public class DiceDefinition : PersistentScriptableObject
{
    // ─────────────────────────────────────────────────────────
    // ダイス名
    // ─────────────────────────────────────────────────────────

    [SerializeField]
    [Header("ダイス名")]
    [Tooltip("このダイスの識別名\n例: 「スターターダイス」「攻撃型ダイス」")]
    private string m_diceName = "";

    // ─────────────────────────────────────────────────────────
    // 面リスト（6面固定）
    // ─────────────────────────────────────────────────────────

    [SerializeField]
    [Header("面リスト（6面）")]
    [Tooltip("ダイスの6面分のデータ\n" +
             "各面に目の数字（1～6）と属性リスト（攻撃、防御など）を設定します")]
    private DiceFaceData[] m_faces = new DiceFaceData[6];

    // ─────────────────────────────────────────────────────────
    // 読み取り専用プロパティ
    // ─────────────────────────────────────────────────────────

    /// <summary>
    /// ダイスの識別名
    /// </summary>
    public string DiceName => m_diceName;

    /// <summary>
    /// ダイスが持つ面の一覧（読み取り専用）
    /// </summary>
    public IReadOnlyList<DiceFaceData> Faces => m_faces;

    /// <summary>
    /// ダイスの面数（常に6）
    /// </summary>
    public int FaceCount => m_faces.Length;

    // ─────────────────────────────────────────────────────────
    // 初期化（Inspector で作成時に呼ばれる）
    // ─────────────────────────────────────────────────────────

#if UNITY_EDITOR
    /// <summary>
    /// 新規作成時に配列を初期化し、各面の目の数字と属性リスト数を同期します
    /// </summary>
    protected override void OnValidate()
    {
        base.OnValidate();

        // 配列が未初期化なら6面分確保
        if (m_faces == null || m_faces.Length != 6)
        {
            m_faces = new DiceFaceData[6];
        }

        // 各面の Number と Elements.Count を同期します
        // DiceFaceData はネストされた Serializable 構造体のため、親 ScriptableObject 側で同期します
        for (int i = 0; i < m_faces.Length; i++)
        {
            m_faces[i].SyncElementsWithNumber();
        }
    }
#endif
}
