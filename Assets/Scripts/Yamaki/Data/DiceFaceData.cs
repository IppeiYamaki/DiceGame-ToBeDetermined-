using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ダイスの1面分のデータ
/// 目の数字（1～6）と、その面が持つ属性リストを保持します
/// 
/// 使用例:
/// - 目が「3」で「攻撃1、防御2」の面
///   → Number = 3, Elements = [ Attack, Defense, Defense ]
/// 
/// Inspector 上では、目の数字と属性リストの要素数が連動します
/// 例: Number を 3 にすると Elements は 3 個になり、増えた分は Attack で初期化されます
/// </summary>
[Serializable]
public struct DiceFaceData
{
    // ─────────────────────────────────────────────────────────
    // フィールド
    // ─────────────────────────────────────────────────────────

    [SerializeField]
    [Header("目の数字")]
    [Tooltip("この面の数字（1～6）\n役判定に使用されます")]
    [Range(1, 6)]
    private int m_number;

    [SerializeField]
    [Header("属性リスト")]
    [Tooltip("この面が持つ属性要素のリスト\n" +
             "要素数は目の数字と同じ数になるように自動調整されます\n" +
             "例: 目の数字が 3 の場合は3個の属性を設定できます\n" +
             "増えた分はデフォルトで Attack が設定されます")]
    private List<DiceFaceElement> m_elements;

    // ─────────────────────────────────────────────────────────
    // 読み取り専用プロパティ
    // ─────────────────────────────────────────────────────────

    /// <summary>
    /// この面の数字（1～6）
    /// </summary>
    public int Number => m_number;

    /// <summary>
    /// この面が持つ属性要素のリスト（読み取り専用）
    /// </summary>
    public IReadOnlyList<DiceFaceElement> Elements => m_elements;

    // ─────────────────────────────────────────────────────────
    // コンストラクタ
    // ─────────────────────────────────────────────────────────

    /// <summary>
    /// DiceFaceData を生成します
    /// </summary>
    /// <param name="number">目の数字（1～6）</param>
    /// <param name="elements">属性要素のリスト</param>
    public DiceFaceData(int number, List<DiceFaceElement> elements)
    {
        m_number = Mathf.Clamp(number, 1, 6);
        m_elements = elements ?? new List<DiceFaceElement>();
        SyncElementsWithNumber();
    }

    // ─────────────────────────────────────────────────────────
    // Inspector 同期用メソッド
    // ─────────────────────────────────────────────────────────

    /// <summary>
    /// 目の数字と属性リストの要素数を同期します
    /// m_number を 1～6 に補正し、m_elements の要素数を m_number と同じ数にします
    /// 要素が増える場合は、デフォルトで Attack を追加します
    /// </summary>
    public void SyncElementsWithNumber()
    {
        m_number = Mathf.Clamp(m_number, 1, 6);

        if (m_elements == null)
        {
            m_elements = new List<DiceFaceElement>();
        }

        while (m_elements.Count < m_number)
        {
            m_elements.Add(new DiceFaceElement(DiceAttributeType.Attack));
        }

        while (m_elements.Count > m_number)
        {
            m_elements.RemoveAt(m_elements.Count - 1);
        }
    }
}
