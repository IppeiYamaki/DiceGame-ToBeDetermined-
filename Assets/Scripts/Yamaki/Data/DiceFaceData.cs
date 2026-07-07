using System;
using UnityEngine;

/// <summary>
/// ダイスの1面分のデータ
/// その面の目の値を保持します
/// </summary>
[Serializable]
public struct DiceFaceData
{
    // ─────────────────────────────────────────────────────────
    // フィールド
    // ─────────────────────────────────────────────────────────

    [SerializeField]
    [Header("目の値")]
    [Tooltip("この面の目の値\n役判定やロール結果に使用されます")]
    [Min(1)]
    private int m_number;

    // ─────────────────────────────────────────────────────────
    // 読み取り専用プロパティ
    // ─────────────────────────────────────────────────────────

    /// <summary>
    /// この面の目の値
    /// </summary>
    public int Number => m_number;

    // ─────────────────────────────────────────────────────────
    // コンストラクタ
    // ─────────────────────────────────────────────────────────

    /// <summary>
    /// DiceFaceData を生成します
    /// </summary>
    /// <param name="number">目の値</param>
    public DiceFaceData(int number)
    {
        m_number = Mathf.Max(1, number);
    }

    /// <summary>
    /// 親DiceDefinitionから出目を設定するための補助メソッドです
    /// </summary>
    /// <param name="number">設定する目の値</param>
    public void SetNumber(int number)
    {
        m_number = Mathf.Max(1, number);
    }
}
