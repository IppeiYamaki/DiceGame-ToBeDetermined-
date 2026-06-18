using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ダイス本体を表す ScriptableObject
/// DiceFaceData を持ち、各面の目の値を定義します
/// 
/// 作成方法:
/// Project ビュー右クリック → Create → DiceGame/DiceDefinition
/// 
/// 使用方法:
/// - Inspector でダイス名を設定します
/// - 面ごとの目の値を設定します
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
    // 面リスト
    // ─────────────────────────────────────────────────────────

    [SerializeField]
    [Header("面リスト")]
    [Tooltip("ダイスの面ごとのデータ\n各面に目の値を設定します")]
    private DiceFaceData[] m_faces = new DiceFaceData[6];

    [SerializeField]
    [Header("標準出目を使用")]
    [Tooltip("ON の場合、面の目を 1 から順番に自動設定します。特殊な出目配列にしたい場合はOFFにしてください。")]
    private bool m_useStandardFaceNumbers = true;

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
    /// ダイスの面数
    /// </summary>
    public int FaceCount => m_faces != null ? m_faces.Length : 0;

    /// <summary>
    /// 標準出目（1～6）を自動使用するかどうか
    /// </summary>
    public bool UseStandardFaceNumbers => m_useStandardFaceNumbers;

    // ─────────────────────────────────────────────────────────
    // 初期化（Inspector で作成時に呼ばれる）
    // ─────────────────────────────────────────────────────────

#if UNITY_EDITOR
    /// <summary>
    /// 新規作成時に配列を初期化し、必要に応じて各面の目の値を同期します
    /// </summary>
    protected override void OnValidate()
    {
        base.OnValidate();

        // 配列が未初期化なら標準的な6面分を確保
        if (m_faces == null)
        {
            m_faces = new DiceFaceData[6];
        }

        // DiceFaceData はネストされた Serializable 構造体のため、親 ScriptableObject 側で補正します
        for (int i = 0; i < m_faces.Length; i++)
        {
            if (m_useStandardFaceNumbers)
            {
                m_faces[i].SetNumber(i + 1);
            }
        }
    }
#endif
}
