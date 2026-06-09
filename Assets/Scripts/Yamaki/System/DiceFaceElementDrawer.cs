#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

/// <summary>
/// DiceFaceElement を Inspector 上で1行の属性選択ドロップダウンとして表示するための PropertyDrawer
/// 
/// 通常の Serializable 構造体は Element 0 / Element 1 のように折りたたみ表示されますが、
/// この Drawer によりタブを開かずに Attack / Defense などの属性を直接確認・変更できます
/// </summary>
[CustomPropertyDrawer(typeof(DiceFaceElement))]
public class DiceFaceElementDrawer : PropertyDrawer
{
    /// <summary>
    /// DiceFaceElement を1行で描画します
    /// </summary>
    /// <param name="position">描画領域</param>
    /// <param name="property">対象プロパティ</param>
    /// <param name="label">ラベル</param>
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        SerializedProperty attributeTypeProperty = property.FindPropertyRelative("m_attributeType");

        if (attributeTypeProperty == null)
        {
            EditorGUI.LabelField(position, label.text, "m_attributeType が見つかりません");
            return;
        }

        EditorGUI.PropertyField(position, attributeTypeProperty, label);
    }

    /// <summary>
    /// 1行分の高さを返します
    /// </summary>
    /// <param name="property">対象プロパティ</param>
    /// <param name="label">ラベル</param>
    /// <returns>1行分の描画高さ</returns>
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return EditorGUIUtility.singleLineHeight;
    }
}
#endif
