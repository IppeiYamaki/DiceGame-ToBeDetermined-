using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ダイスの役（コンボ）定義を表す ScriptableObject
/// 役の成立条件、倍率、名前、説明を定義します
/// 
/// 作成方法:
/// Project ビュー右クリック → Create → DiceGame/DiceRoleDefinition
/// 
/// 使用例:
/// 
/// ピンゾロ（1,1,1）の場合:
/// - RoleName: "ピンゾロ"
/// - Multiplier: 3.0f
/// - ConditionType: ExactNumbers
/// - AllowedNumbers: [1, 1, 1]
/// 
/// ゾロ目（2～6）の場合:
/// - RoleName: "ゾロ目"
/// - Multiplier: 2.0f
/// - ConditionType: AllSame
/// - AllowedNumbers: [2, 3, 4, 5, 6]
/// 
/// 偶数の場合:
/// - RoleName: "偶数"
/// - Multiplier: 1.5f
/// - ConditionType: AllInAllowedSet
/// - AllowedNumbers: [2, 4, 6]
/// 
/// ワンペアの場合:
/// - RoleName: "ワンペア"
/// - Multiplier: 1.2f
/// - ConditionType: SameNumberCount
/// - RequiredSameCount: 2
/// </summary>
[CreateAssetMenu(fileName = "DiceRoleDefinition_", menuName = "DiceGame/DiceRoleDefinition")]
public class DiceRoleDefinition : PersistentScriptableObject
{
    // ─────────────────────────────────────────────────────────
    // 基本情報
    // ─────────────────────────────────────────────────────────

    [SerializeField]
    [Header("役名")]
    [Tooltip("この役の名前\n例: 「ピンゾロ」「シゴロ」「ゾロ目」")]
    private string m_roleName = "";

    [SerializeField]
    [Header("役の説明")]
    [Tooltip("この役の説明文\n例: 「1のゾロ目で最強の役」")]
    [TextArea(2, 4)]
    private string m_description = "";

    [SerializeField]
    [Header("倍率")]
    [Tooltip("この役が成立したときに適用される倍率\n例: 2.0 で2倍、3.0 で3倍")]
    [Range(1.0f, 10.0f)]
    private float m_multiplier = 1.0f;

    // ─────────────────────────────────────────────────────────
    // 成立条件
    // ─────────────────────────────────────────────────────────

    [SerializeField]
    [Header("条件タイプ")]
    [Tooltip("役の成立条件タイプ\n" +
             "ExactNumbers: 指定数字と完全一致\n" +
             "AllSame: すべて同じ数字\n" +
             "AllInAllowedSet: 指定数字セット内のみ\n" +
             "SameNumberCount: 同じ数字が指定数以上")]
    private DiceRoleConditionType m_conditionType = DiceRoleConditionType.ExactNumbers;

    [SerializeField]
    [Header("判定用数字リスト")]
    [Tooltip("判定に使用する数字のリスト\n" +
             "ExactNumbers: 完全一致させたい数字（例: [1,1,1] / [4,5,6]）\n" +
             "AllSame: 許可する数字（例: [2,3,4,5,6] でピンゾロ除外）\n" +
             "AllInAllowedSet: 許可する数字セット（例: [2,4,6] で偶数のみ）\n" +
             "SameNumberCount: 全数字（例: [1,2,3,4,5,6]）")]
    private List<int> m_allowedNumbers = new List<int>();

    [SerializeField]
    [Header("必要一致数（SameNumberCount 用）")]
    [Tooltip("SameNumberCount の場合のみ使用\n同じ数字が何個あれば成立するか（例: 2 でワンペア）")]
    [Range(2, 3)]
    private int m_requiredSameCount = 2;

    // ─────────────────────────────────────────────────────────
    // 読み取り専用プロパティ
    // ─────────────────────────────────────────────────────────

    /// <summary>役名</summary>
    public string RoleName => m_roleName;

    /// <summary>役の説明</summary>
    public string Description => m_description;

    /// <summary>倍率</summary>
    public float Multiplier => m_multiplier;

    /// <summary>条件タイプ</summary>
    public DiceRoleConditionType ConditionType => m_conditionType;

    /// <summary>判定用数字リスト（読み取り専用）</summary>
    public IReadOnlyList<int> AllowedNumbers => m_allowedNumbers;

    /// <summary>必要一致数（SameNumberCount 用）</summary>
    public int RequiredSameCount => m_requiredSameCount;
}
