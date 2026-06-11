using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// プレイヤーの戦闘用マスターデータ
/// 名前、最大HP、初期ダイスデッキ、ターンごとのドロー数と使用ダイス数を定義します
/// </summary>
[CreateAssetMenu(fileName = "PlayerDefinition_", menuName = "DiceGame/PlayerDefinition")]
public class PlayerDefinition : ScriptableObject
{
    [SerializeField]
    [Header("プレイヤー名")]
    [Tooltip("戦闘中に表示・参照するプレイヤー名")]
    private string m_playerName = "Player";

    [SerializeField]
    [Header("最大HP")]
    [Tooltip("戦闘開始時の最大HP")]
    private int m_maxHp = 100;

    [SerializeField]
    [Header("初期ダイスデッキID")]
    [Tooltip("プレイヤーが戦闘開始時に持つダイスの永続IDリスト")]
    private List<string> m_initialDiceDeckIds = new List<string>();

    [SerializeField]
    [Header("ドロー数")]
    [Tooltip("ターン開始時にデッキから引くダイス数")]
    private int m_drawDiceCount = 5;

    [SerializeField]
    [Header("使用ダイス数")]
    [Tooltip("1ターンに選択してロールするダイス数")]
    private int m_useDiceCount = 3;

    /// <summary>
    /// プレイヤー名
    /// </summary>
    public string PlayerName => m_playerName;

    /// <summary>
    /// 最大HP
    /// </summary>
    public int MaxHp => m_maxHp;

    /// <summary>
    /// 初期ダイスデッキIDリスト
    /// </summary>
    public IReadOnlyList<string> InitialDiceDeckIds => m_initialDiceDeckIds;

    /// <summary>
    /// ターン開始時に引くダイス数
    /// </summary>
    public int DrawDiceCount => m_drawDiceCount;

    /// <summary>
    /// 1ターンに使用するダイス数
    /// </summary>
    public int UseDiceCount => m_useDiceCount;

#if UNITY_EDITOR
    /// <summary>
    /// Inspector編集時に戦闘用数値を有効範囲へ補正します
    /// </summary>
    private void OnValidate()
    {
        m_maxHp = Mathf.Max(1, m_maxHp);
        m_drawDiceCount = Mathf.Max(1, m_drawDiceCount);
        m_useDiceCount = Mathf.Max(1, m_useDiceCount);
    }
#endif
}
