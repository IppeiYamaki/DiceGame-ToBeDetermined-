using System;
using UnityEngine;

/// <summary>
/// 戦闘中のキャラクター状態を管理するRuntimeデータ
/// ScriptableObject本体を書き換えず、戦闘中に変化するHPや防御値を保持します
/// </summary>
[Serializable]
public class BattleActorRuntime
{
    [SerializeField]
    [Header("名前")]
    [Tooltip("戦闘中に参照する名前")]
    private string m_actorName = "";

    [SerializeField]
    [Header("最大HP")]
    [Tooltip("戦闘中の最大HP")]
    private int m_maxHp = 1;

    [SerializeField]
    [Header("現在HP")]
    [Tooltip("戦闘中の現在HP")]
    private int m_currentHp = 1;

    [SerializeField]
    [Header("防御値")]
    [Tooltip("現在ターンに使用する防御値")]
    private int m_defenseValue = 0;

    /// <summary>
    /// 戦闘中の名前
    /// </summary>
    public string ActorName => m_actorName;

    /// <summary>
    /// 最大HP
    /// </summary>
    public int MaxHp => m_maxHp;

    /// <summary>
    /// 現在HP
    /// </summary>
    public int CurrentHp => m_currentHp;

    /// <summary>
    /// 現在の防御値
    /// </summary>
    public int DefenseValue => m_defenseValue;

    /// <summary>
    /// 生存しているかどうか
    /// </summary>
    public bool IsAlive => m_currentHp > 0;

    /// <summary>
    /// BattleActorRuntime を生成します
    /// </summary>
    /// <param name="actorName">戦闘中の名前</param>
    /// <param name="maxHp">最大HP</param>
    public BattleActorRuntime(string actorName, int maxHp)
    {
        m_actorName = actorName;
        m_maxHp = Mathf.Max(1, maxHp);
        m_currentHp = m_maxHp;
        m_defenseValue = 0;
    }

    /// <summary>
    /// 防御値を設定します
    /// </summary>
    /// <param name="defenseValue">設定する防御値</param>
    public void SetDefenseValue(int defenseValue)
    {
        m_defenseValue = Mathf.Max(0, defenseValue);
    }

    /// <summary>
    /// 防御値を0に戻します
    /// </summary>
    public void ClearDefenseValue()
    {
        m_defenseValue = 0;
    }

    /// <summary>
    /// ダメージを受けて現在HPを減らします
    /// </summary>
    /// <param name="damageValue">受けるダメージ値</param>
    /// <returns>実際に減少したHP</returns>
    public int ApplyDamage(int damageValue)
    {
        int clampedDamage = Mathf.Max(0, damageValue);
        int beforeHp = m_currentHp;
        m_currentHp = Mathf.Max(0, m_currentHp - clampedDamage);
        return beforeHp - m_currentHp;
    }

    /// <summary>
    /// HPを回復します
    /// </summary>
    /// <param name="healValue">回復量</param>
    /// <returns>実際に回復したHP</returns>
    public int Heal(int healValue)
    {
        int clampedHeal = Mathf.Max(0, healValue);
        int beforeHp = m_currentHp;
        m_currentHp = Mathf.Min(m_maxHp, m_currentHp + clampedHeal);
        return m_currentHp - beforeHp;
    }
}
