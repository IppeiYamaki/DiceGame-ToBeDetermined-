using System;
using UnityEngine;

/// <summary>
/// ダイスの面が持つ要素（属性1つ分）
/// 1つの面に複数の属性を設定できるようにするための構造体です
/// 
/// 使用例:
/// - 面に「攻撃1、防御2」を設定したい場合
///   → Elements = [ { Attack }, { Defense }, { Defense } ]
/// 
/// Inspector 上でリスト形式で編集できます
/// </summary>
[Serializable]
public struct DiceFaceElement
{
    // ─────────────────────────────────────────────────────────
    // フィールド
    // ─────────────────────────────────────────────────────────

    [SerializeField]
    [Tooltip("この要素の属性タイプ（攻撃 / 防御 など）")]
    private DiceAttributeType m_attributeType;

    // ─────────────────────────────────────────────────────────
    // 読み取り専用プロパティ
    // ─────────────────────────────────────────────────────────

    /// <summary>
    /// この要素の属性タイプ
    /// </summary>
    public DiceAttributeType AttributeType => m_attributeType;

    // ─────────────────────────────────────────────────────────
    // コンストラクタ
    // ─────────────────────────────────────────────────────────

    /// <summary>
    /// DiceFaceElement を生成します
    /// </summary>
    /// <param name="attributeType">属性タイプ</param>
    public DiceFaceElement(DiceAttributeType attributeType)
    {
        m_attributeType = attributeType;
    }
}
