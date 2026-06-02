#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

/// <summary>
/// ReadOnlyAttribute のカスタムプロパティドロワー
/// Inspector 上でフィールドをグレーアウト表示し、編集を無効化します
/// </summary>
[CustomPropertyDrawer(typeof(ReadOnlyAttribute))]
public class ReadOnlyAttributeDrawer : PropertyDrawer
{
    /// <summary>
    /// Inspector 上でプロパティを描画します（編集不可状態で）
    /// </summary>
    /// <param name="position">描画領域</param>
    /// <param name="property">対象プロパティ</param>
    /// <param name="label">ラベル</param>
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        // GUI を無効化（グレーアウト）して描画
        EditorGUI.BeginDisabledGroup(true);
        EditorGUI.PropertyField(position, property, label, true);
        EditorGUI.EndDisabledGroup();
    }

    /// <summary>
    /// プロパティの高さを取得します
    /// </summary>
    /// <param name="property">対象プロパティ</param>
    /// <param name="label">ラベル</param>
    /// <returns>プロパティの描画高さ</returns>
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return EditorGUI.GetPropertyHeight(property, label, true);
    }
}
#endif
