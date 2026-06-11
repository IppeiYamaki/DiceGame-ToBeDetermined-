#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

/// <summary>
/// EnemyDefinition の Inspector 表示をカスタマイズする CustomEditor
/// HP帯の境界を IntSlider と数値入力で編集可能にし、隣接帯と連動補正します
/// 「帯を追加（末尾を分割）」「この帯を削除（隣へ結合）」ボタンを提供します
/// </summary>
[CustomEditor(typeof(EnemyDefinition))]
public class EnemyDefinitionEditor : Editor
{
    private SerializedProperty m_enemyNameProp;
    private SerializedProperty m_maxHpProp;
    private SerializedProperty m_attackPowerProp;
    private SerializedProperty m_visualSpriteProp;
    private SerializedProperty m_visualTextureProp;
    private SerializedProperty m_idleTexturesProp;
    private SerializedProperty m_actionTextureProp;
    private SerializedProperty m_idleFrameIntervalProp;
    private SerializedProperty m_hpRangeActionSetsProp;

    private void OnEnable()
    {
        m_enemyNameProp = serializedObject.FindProperty("m_enemyName");
        m_maxHpProp = serializedObject.FindProperty("m_maxHp");
        m_attackPowerProp = serializedObject.FindProperty("m_attackPower");
        m_visualSpriteProp = serializedObject.FindProperty("m_visualSprite");
        m_visualTextureProp = serializedObject.FindProperty("m_visualTexture");
        m_idleTexturesProp = serializedObject.FindProperty("m_idleTextures");
        m_actionTextureProp = serializedObject.FindProperty("m_actionTexture");
        m_idleFrameIntervalProp = serializedObject.FindProperty("m_idleFrameInterval");
        m_hpRangeActionSetsProp = serializedObject.FindProperty("m_hpRangeActionSets");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        // 基本情報
        EditorGUILayout.LabelField("基本情報", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(m_enemyNameProp);
        EditorGUILayout.PropertyField(m_maxHpProp);
        EditorGUILayout.PropertyField(m_attackPowerProp);
        EditorGUILayout.PropertyField(m_visualSpriteProp);
        EditorGUILayout.PropertyField(m_visualTextureProp);

        EditorGUILayout.Space();

        // Enemy複数Visual
        EditorGUILayout.LabelField("Enemy複数Visual", EditorStyles.boldLabel);
        EditorGUILayout.HelpBox(
            "アイドルアニメ用Textureに複数枚設定すると、1→2→3→2→1... の往復ループで表示されます。\n" +
            "行動時Textureは敵が攻撃・防御などの行動をした瞬間に表示されます。\n" +
            "アイドルアニメ用Textureが空の場合は、従来どおり表示Sprite/表示Textureを使用します。",
            MessageType.Info);
        EditorGUILayout.PropertyField(m_idleTexturesProp, new GUIContent("アイドルアニメ用Texture（複数）"), true);
        EditorGUILayout.PropertyField(m_actionTextureProp);
        EditorGUILayout.PropertyField(m_idleFrameIntervalProp);

        EditorGUILayout.Space();

        // HP帯別行動パターン
        EditorGUILayout.LabelField("HP帯別行動パターン", EditorStyles.boldLabel);
        EditorGUILayout.HelpBox(
            "HP割合に応じた行動セットを定義します。\n" +
            "帯の境界を調整すると隣接帯も連動します（行動が存在しないHP帯を作らないため）。",
            MessageType.Info);

        if (m_hpRangeActionSetsProp == null || m_hpRangeActionSetsProp.arraySize == 0)
        {
            EditorGUILayout.HelpBox("HP帯が設定されていません。「帯を追加」ボタンで追加してください。", MessageType.Warning);
        }

        // 各HP帯を描画
        for (int i = 0; i < m_hpRangeActionSetsProp.arraySize; i++)
        {
            DrawHpRangeActionSet(i);
        }

        EditorGUILayout.Space();

        // 帯を追加ボタン（末尾を中点分割）
        if (GUILayout.Button("帯を追加（末尾を分割）"))
        {
            AddHpRangeActionSet();
        }

        serializedObject.ApplyModifiedProperties();
    }

    /// <summary>
    /// 個別のHP帯を描画します
    /// </summary>
    /// <param name="index">帯のインデックス</param>
    private void DrawHpRangeActionSet(int index)
    {
        SerializedProperty rangeSetProp = m_hpRangeActionSetsProp.GetArrayElementAtIndex(index);
        SerializedProperty upperPercentProp = rangeSetProp.FindPropertyRelative("m_upperPercent");
        SerializedProperty lowerPercentProp = rangeSetProp.FindPropertyRelative("m_lowerPercent");
        SerializedProperty actionGroupsProp = rangeSetProp.FindPropertyRelative("m_actionGroups");

        EditorGUILayout.BeginVertical(EditorStyles.helpBox);

        // ヘッダー: HP帯の範囲表示と削除ボタン
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField($"HP帯 {index + 1}: {upperPercentProp.intValue}% ～ {lowerPercentProp.intValue}%", EditorStyles.boldLabel);
        if (GUILayout.Button("この帯を削除", GUILayout.Width(100)))
        {
            RemoveHpRangeActionSet(index);
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndVertical();
            return;
        }
        EditorGUILayout.EndHorizontal();

        // 上限の編集（先頭以外）
        if (index > 0)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("HP上限（%）", GUILayout.Width(100));
            int newUpper = EditorGUILayout.IntSlider(upperPercentProp.intValue, 0, 100);
            if (newUpper != upperPercentProp.intValue)
            {
                upperPercentProp.intValue = newUpper;
                // 前の帯の Lower を連動
                if (index > 0)
                {
                    SerializedProperty prevRangeSetProp = m_hpRangeActionSetsProp.GetArrayElementAtIndex(index - 1);
                    SerializedProperty prevLowerPercentProp = prevRangeSetProp.FindPropertyRelative("m_lowerPercent");
                    prevLowerPercentProp.intValue = newUpper + 1;
                }
            }
            EditorGUILayout.EndHorizontal();
        }
        else
        {
            EditorGUILayout.LabelField($"HP上限（%）: {upperPercentProp.intValue}（固定：先頭は100%）");
        }

        // 下限の編集（末尾以外）
        if (index < m_hpRangeActionSetsProp.arraySize - 1)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("HP下限（%）", GUILayout.Width(100));
            int newLower = EditorGUILayout.IntSlider(lowerPercentProp.intValue, 0, 100);
            if (newLower != lowerPercentProp.intValue)
            {
                lowerPercentProp.intValue = newLower;
                // 次の帯の Upper を連動
                if (index < m_hpRangeActionSetsProp.arraySize - 1)
                {
                    SerializedProperty nextRangeSetProp = m_hpRangeActionSetsProp.GetArrayElementAtIndex(index + 1);
                    SerializedProperty nextUpperPercentProp = nextRangeSetProp.FindPropertyRelative("m_upperPercent");
                    nextUpperPercentProp.intValue = newLower - 1;
                }
            }
            EditorGUILayout.EndHorizontal();
        }
        else
        {
            EditorGUILayout.LabelField($"HP下限（%）: {lowerPercentProp.intValue}（固定：末尾は0%）");
        }

        EditorGUILayout.Space();

        // 行動グループリスト（SerializedProperty 標準描画 → Drawer が適用される）
        EditorGUILayout.PropertyField(actionGroupsProp, new GUIContent("行動グループリスト"), true);

        EditorGUILayout.EndVertical();
        EditorGUILayout.Space();
    }

    /// <summary>
    /// HP帯を追加します（末尾を中点分割）
    /// </summary>
    private void AddHpRangeActionSet()
    {
        serializedObject.ApplyModifiedProperties();

        EnemyDefinition enemyDefinition = target as EnemyDefinition;
        if (enemyDefinition == null) return;

        // 末尾を取得
        int lastIndex = m_hpRangeActionSetsProp.arraySize - 1;
        if (lastIndex < 0)
        {
            // 空の場合は 100～0 を追加
            m_hpRangeActionSetsProp.InsertArrayElementAtIndex(0);
            SerializedProperty newSetProp = m_hpRangeActionSetsProp.GetArrayElementAtIndex(0);
            newSetProp.FindPropertyRelative("m_upperPercent").intValue = 100;
            newSetProp.FindPropertyRelative("m_lowerPercent").intValue = 0;
        }
        else
        {
            // 末尾を中点分割
            SerializedProperty lastSetProp = m_hpRangeActionSetsProp.GetArrayElementAtIndex(lastIndex);
            int upper = lastSetProp.FindPropertyRelative("m_upperPercent").intValue;
            int lower = lastSetProp.FindPropertyRelative("m_lowerPercent").intValue;
            int mid = (upper + lower) / 2;

            // 末尾の Lower を mid + 1 に変更
            lastSetProp.FindPropertyRelative("m_lowerPercent").intValue = mid + 1;

            // 新しい帯を追加
            m_hpRangeActionSetsProp.InsertArrayElementAtIndex(lastIndex + 1);
            SerializedProperty newSetProp = m_hpRangeActionSetsProp.GetArrayElementAtIndex(lastIndex + 1);
            newSetProp.FindPropertyRelative("m_upperPercent").intValue = mid;
            newSetProp.FindPropertyRelative("m_lowerPercent").intValue = 0;
        }

        serializedObject.ApplyModifiedProperties();
        EditorUtility.SetDirty(target);
    }

    /// <summary>
    /// HP帯を削除します（隣接帯へ結合）
    /// </summary>
    /// <param name="index">削除する帯のインデックス</param>
    private void RemoveHpRangeActionSet(int index)
    {
        if (m_hpRangeActionSetsProp.arraySize <= 1)
        {
            EditorUtility.DisplayDialog("削除不可", "最後の1つの帯は削除できません。", "OK");
            return;
        }

        serializedObject.ApplyModifiedProperties();

        // 前の帯の Lower を現在の帯の Lower へ拡張（末尾の場合）、または次の帯の Upper を現在の帯の Upper へ拡張（先頭の場合）
        if (index == m_hpRangeActionSetsProp.arraySize - 1)
        {
            // 末尾を削除 → 前の帯の Lower を 0 に
            if (index > 0)
            {
                SerializedProperty prevSetProp = m_hpRangeActionSetsProp.GetArrayElementAtIndex(index - 1);
                prevSetProp.FindPropertyRelative("m_lowerPercent").intValue = 0;
            }
        }
        else if (index == 0)
        {
            // 先頭を削除 → 次の帯の Upper を 100 に
            if (index < m_hpRangeActionSetsProp.arraySize - 1)
            {
                SerializedProperty nextSetProp = m_hpRangeActionSetsProp.GetArrayElementAtIndex(index + 1);
                nextSetProp.FindPropertyRelative("m_upperPercent").intValue = 100;
            }
        }
        else
        {
            // 中間を削除 → 次の帯の Upper を現在の帯の Upper に
            SerializedProperty currentSetProp = m_hpRangeActionSetsProp.GetArrayElementAtIndex(index);
            int currentUpper = currentSetProp.FindPropertyRelative("m_upperPercent").intValue;

            SerializedProperty nextSetProp = m_hpRangeActionSetsProp.GetArrayElementAtIndex(index + 1);
            nextSetProp.FindPropertyRelative("m_upperPercent").intValue = currentUpper;
        }

        m_hpRangeActionSetsProp.DeleteArrayElementAtIndex(index);
        serializedObject.ApplyModifiedProperties();
        EditorUtility.SetDirty(target);
    }
}
#endif
