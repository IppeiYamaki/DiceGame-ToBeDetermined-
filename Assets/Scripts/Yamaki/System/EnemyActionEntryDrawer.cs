#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

/// <summary>
/// EnemyActionEntry を Inspector 上で条件付き表示するための PropertyDrawer
/// ActionType に応じて Power / StatusEffect を出し分け、
/// UseEffect / UseSound が ON のときのみ詳細フィールドを表示します
/// 
/// 各フィールドの描画高さは EditorGUI.GetPropertyHeight で動的に取得します
/// （[Header] 等の装飾がある場合、固定行高では描画が重なるため）
/// </summary>
[CustomPropertyDrawer(typeof(EnemyActionEntry))]
public class EnemyActionEntryDrawer : PropertyDrawer
{
    private const float LineHeight = 18f;
    private const float Spacing = 2f;

    /// <summary>
    /// EnemyActionEntry を条件付きで描画します
    /// </summary>
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        SerializedProperty actionTypeProp = property.FindPropertyRelative("m_actionType");
        SerializedProperty powerProp = property.FindPropertyRelative("m_power");
        SerializedProperty statusEffectProp = property.FindPropertyRelative("m_statusEffect");
        SerializedProperty useEffectProp = property.FindPropertyRelative("m_useEffect");
        SerializedProperty effectPrefabProp = property.FindPropertyRelative("m_effectPrefab");
        SerializedProperty effectAnchorProp = property.FindPropertyRelative("m_effectAnchor");
        SerializedProperty effectDurationProp = property.FindPropertyRelative("m_effectDuration");
        SerializedProperty useSoundProp = property.FindPropertyRelative("m_useSound");
        SerializedProperty audioClipProp = property.FindPropertyRelative("m_audioClip");
        SerializedProperty soundTimingProp = property.FindPropertyRelative("m_soundTiming");
        SerializedProperty volumeProp = property.FindPropertyRelative("m_volume");

        // 折りたたみヘッダー
        Rect foldoutRect = new Rect(position.x, position.y, position.width, LineHeight);
        property.isExpanded = EditorGUI.Foldout(foldoutRect, property.isExpanded, label, true);

        if (!property.isExpanded)
        {
            EditorGUI.EndProperty();
            return;
        }

        EditorGUI.indentLevel++;
        float y = position.y + LineHeight + Spacing;

        // 行動区分
        DrawField(position, ref y, actionTypeProp);

        // ActionType に応じて Power / StatusEffect を出し分け
        BattleActionType actionType = (BattleActionType)actionTypeProp.enumValueIndex;

        if (actionType == BattleActionType.Attack || actionType == BattleActionType.Defense)
        {
            DrawField(position, ref y, powerProp);
        }
        else if (actionType == BattleActionType.Buff || actionType == BattleActionType.Debuff)
        {
            DrawField(position, ref y, statusEffectProp);
        }

        // エフェクト使用
        DrawField(position, ref y, useEffectProp);
        if (useEffectProp.boolValue)
        {
            DrawField(position, ref y, effectPrefabProp);
            DrawField(position, ref y, effectAnchorProp);
            DrawField(position, ref y, effectDurationProp);
        }

        // サウンド使用
        DrawField(position, ref y, useSoundProp);
        if (useSoundProp.boolValue)
        {
            DrawField(position, ref y, audioClipProp);
            DrawField(position, ref y, soundTimingProp);
            DrawField(position, ref y, volumeProp);
        }

        EditorGUI.indentLevel--;
        EditorGUI.EndProperty();
    }

    /// <summary>
    /// プロパティを実際の高さ（[Header] 等の装飾込み）で描画し、y を進めます
    /// 固定行高で描画すると装飾分だけ後続フィールドと重なるため、
    /// 必ず EditorGUI.GetPropertyHeight を使用します
    /// </summary>
    private static void DrawField(Rect position, ref float y, SerializedProperty property)
    {
        float height = EditorGUI.GetPropertyHeight(property, true);
        Rect rect = new Rect(position.x, y, position.width, height);
        EditorGUI.PropertyField(rect, property, true);
        y += height + Spacing;
    }

    /// <summary>
    /// プロパティ1個分の高さ（装飾込み＋間隔）を取得します
    /// </summary>
    private static float GetFieldHeight(SerializedProperty property)
    {
        return EditorGUI.GetPropertyHeight(property, true) + Spacing;
    }

    /// <summary>
    /// 描画高さを動的に計算します
    /// OnGUI と同じ式で合計し、描画と高さのズレ（＝重なり）を防ぎます
    /// </summary>
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        if (!property.isExpanded)
        {
            return LineHeight;
        }

        float height = LineHeight + Spacing; // Foldout

        SerializedProperty actionTypeProp = property.FindPropertyRelative("m_actionType");
        height += GetFieldHeight(actionTypeProp);

        BattleActionType actionType = (BattleActionType)actionTypeProp.enumValueIndex;

        if (actionType == BattleActionType.Attack || actionType == BattleActionType.Defense)
        {
            height += GetFieldHeight(property.FindPropertyRelative("m_power"));
        }
        else if (actionType == BattleActionType.Buff || actionType == BattleActionType.Debuff)
        {
            height += GetFieldHeight(property.FindPropertyRelative("m_statusEffect"));
        }

        SerializedProperty useEffectProp = property.FindPropertyRelative("m_useEffect");
        height += GetFieldHeight(useEffectProp);
        if (useEffectProp.boolValue)
        {
            height += GetFieldHeight(property.FindPropertyRelative("m_effectPrefab"));
            height += GetFieldHeight(property.FindPropertyRelative("m_effectAnchor"));
            height += GetFieldHeight(property.FindPropertyRelative("m_effectDuration"));
        }

        SerializedProperty useSoundProp = property.FindPropertyRelative("m_useSound");
        height += GetFieldHeight(useSoundProp);
        if (useSoundProp.boolValue)
        {
            height += GetFieldHeight(property.FindPropertyRelative("m_audioClip"));
            height += GetFieldHeight(property.FindPropertyRelative("m_soundTiming"));
            height += GetFieldHeight(property.FindPropertyRelative("m_volume"));
        }

        return height;
    }
}
#endif
