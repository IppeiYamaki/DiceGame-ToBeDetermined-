using UnityEngine;

/// <summary>
/// Inspector 上でフィールドを読み取り専用（編集不可）にする属性
/// SerializeField と併用することで、値を表示しつつ編集を防ぎます
/// 
/// 使用例:
/// [SerializeField, ReadOnly]
/// private string m_generatedId;
/// </summary>
public class ReadOnlyAttribute : PropertyAttribute
{
}
