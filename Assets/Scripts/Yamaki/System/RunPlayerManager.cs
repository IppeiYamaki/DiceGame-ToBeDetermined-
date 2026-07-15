using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ラン中のプレイヤー可変状態を管理する常駐Manager。
/// HP、所持ダイス、所持スキル、所持アイテムを全シーンから参照・更新します。
/// </summary>
public class RunPlayerManager : MonoBehaviour
{
    public const int DiceSlotCount = 6;

    [SerializeField]
    [Header("プレイヤー名")]
    private string m_playerName = "Player";

    [SerializeField]
    [Header("最大HP")]
    [Min(1)]
    private int m_maxHp = 100;

    [SerializeField]
    [Header("現在HP")]
    [Min(0)]
    private int m_currentHp = 100;

    [SerializeField]
    [Header("所持ダイス（6個）")]
    private List<DiceDefinition> m_ownedDice = new List<DiceDefinition>(DiceSlotCount);

    [SerializeField]
    [Header("最大スキル所持数")]
    [Min(0)]
    private int m_maxSkillCount = 4;

    [SerializeField]
    [Header("所持スキル")]
    private List<SkillDefinition> m_ownedSkills = new List<SkillDefinition>();

    [SerializeField]
    [Header("最大アイテム枠数(スタック数)")]
    [Min(0)]
    private int m_maxItemCount = 4;

    [SerializeField]
    [Header("所持アイテム(同一アイテムは1枠にスタック)")]
    private List<OwnedItemStack> m_ownedItems = new List<OwnedItemStack>();

    [SerializeField]
    [Header("アイテム効果テーブル")]
    [Tooltip("ItemData と効果の対応表(DiceGame/ItemEffectTable アセット)")]
    private ItemEffectTable m_itemEffectTable;

    private static RunPlayerManager s_instance;

    public event Action PlayerStateChanged;

    /// <summary>現在有効なPlayer Manager。</summary>
    public static RunPlayerManager Instance => s_instance;

    /// <summary>プレイヤー名。</summary>
    public string PlayerName => m_playerName;

    /// <summary>最大HP。</summary>
    public int MaxHp => m_maxHp;

    /// <summary>現在HP。</summary>
    public int CurrentHp => m_currentHp;

    /// <summary>生存しているか。</summary>
    public bool IsAlive => m_currentHp > 0;

    /// <summary>所持ダイス。</summary>
    public IReadOnlyList<DiceDefinition> OwnedDice => m_ownedDice;

    /// <summary>最大スキル所持数。</summary>
    public int MaxSkillCount => m_maxSkillCount;

    /// <summary>所持スキル。</summary>
    public IReadOnlyList<SkillDefinition> OwnedSkills => m_ownedSkills;

    /// <summary>最大アイテム枠数(スタック数)。</summary>
    public int MaxItemCount => m_maxItemCount;

    /// <summary>所持アイテム(スタック単位)。</summary>
    public IReadOnlyList<OwnedItemStack> OwnedItems => m_ownedItems;

    /// <summary>アイテム効果テーブル。</summary>
    public ItemEffectTable ItemEffectTable => m_itemEffectTable;

    private void Awake()
    {
        if (s_instance != null && s_instance != this)
        {
            Destroy(gameObject);
            return;
        }

        s_instance = this;
        DontDestroyOnLoad(gameObject);
        NormalizeState();
    }

    private void OnDestroy()
    {
        if (s_instance == this)
        {
            s_instance = null;
        }
    }

    /// <summary>
    /// 最大HPと現在HPを設定します。
    /// </summary>
    public void InitializeHp(int maxHp, int currentHp)
    {
        m_maxHp = Mathf.Max(1, maxHp);
        m_currentHp = Mathf.Clamp(currentHp, 0, m_maxHp);
        NotifyChanged();
    }

    /// <summary>
    /// 現在HPを最大HPまで回復します。
    /// </summary>
    public void FullHeal()
    {
        m_currentHp = m_maxHp;
        NotifyChanged();
    }

    /// <summary>
    /// ダメージを受け、実際に減少したHPを返します。
    /// </summary>
    public int ApplyDamage(int damageValue)
    {
        int damage = Mathf.Max(0, damageValue);
        int beforeHp = m_currentHp;
        m_currentHp = Mathf.Max(0, m_currentHp - damage);
        NotifyChanged();
        return beforeHp - m_currentHp;
    }

    /// <summary>
    /// HPを回復し、実際に回復したHPを返します。
    /// </summary>
    public int Heal(int healValue)
    {
        int heal = Mathf.Max(0, healValue);
        int beforeHp = m_currentHp;
        m_currentHp = Mathf.Min(m_maxHp, m_currentHp + heal);
        NotifyChanged();
        return m_currentHp - beforeHp;
    }

    /// <summary>
    /// 指定枠のダイスを交換します。
    /// </summary>
    public bool ReplaceDice(int slotIndex, DiceDefinition diceDefinition)
    {
        if (slotIndex < 0 || slotIndex >= DiceSlotCount)
        {
            Debug.LogWarning($"[RunPlayerManager] ダイス枠 index={slotIndex} は範囲外です。");
            return false;
        }

        EnsureDiceSlotCount();
        m_ownedDice[slotIndex] = diceDefinition;
        NotifyChanged();
        return true;
    }

    /// <summary>
    /// スキルを追加します。満杯の場合は false を返します。
    /// </summary>
    public bool TryAddSkill(SkillDefinition skillDefinition)
    {
        if (skillDefinition == null) return false;
        NormalizeSkillList();

        if (m_ownedSkills.Count >= m_maxSkillCount)
        {
            return false;
        }

        m_ownedSkills.Add(skillDefinition);
        NotifyChanged();
        return true;
    }

    /// <summary>
    /// 指定枠のスキルを交換します。
    /// </summary>
    public bool ReplaceSkill(int slotIndex, SkillDefinition skillDefinition)
    {
        if (skillDefinition == null) return false;
        NormalizeSkillList();

        if (slotIndex < 0 || slotIndex >= m_maxSkillCount)
        {
            Debug.LogWarning($"[RunPlayerManager] スキル枠 index={slotIndex} は範囲外です。");
            return false;
        }

        while (m_ownedSkills.Count <= slotIndex)
        {
            m_ownedSkills.Add(null);
        }

        m_ownedSkills[slotIndex] = skillDefinition;
        NormalizeSkillList();
        NotifyChanged();
        return true;
    }

    /// <summary>
    /// アイテムを1個追加します。既に所持している場合はスタック数を増やし、
    /// 未所持の場合は新しい枠を使用します。枠が満杯の場合は false を返します。
    /// </summary>
    public bool TryAddItem(ItemData itemData)
    {
        if (itemData == null) return false;
        NormalizeItemList();

        OwnedItemStack stack = FindStack(itemData);
        if (stack != null)
        {
            stack.Increment();
            NotifyChanged();
            return true;
        }

        if (m_ownedItems.Count >= m_maxItemCount)
        {
            return false;
        }

        m_ownedItems.Add(new OwnedItemStack(itemData));
        NotifyChanged();
        return true;
    }

    /// <summary>
    /// 指定したアイテムを1個消費します。スタック数が0になった枠は削除します。
    /// </summary>
    public bool TryRemoveItem(ItemData itemData)
    {
        if (itemData == null) return false;

        OwnedItemStack stack = FindStack(itemData);
        if (stack == null) return false;

        if (stack.Decrement() <= 0)
        {
            m_ownedItems.Remove(stack);
        }

        NotifyChanged();
        return true;
    }

    /// <summary>
    /// 指定アイテムの所持数を取得します(未所持なら0)。
    /// </summary>
    public int GetItemCount(ItemData itemData)
    {
        OwnedItemStack stack = FindStack(itemData);
        return stack != null ? stack.Count : 0;
    }

    /// <summary>
    /// ItemDataに対応する効果エントリを取得します(未登録の場合はnull)。
    /// </summary>
    public ItemEffectEntry ResolveItemEffect(ItemData itemData)
    {
        if (m_itemEffectTable == null)
        {
            Debug.LogWarning("[RunPlayerManager] ItemEffectTable が未設定です。Inspector で登録してください。");
            return null;
        }

        return m_itemEffectTable.GetEntry(itemData);
    }

    private OwnedItemStack FindStack(ItemData itemData)
    {
        if (itemData == null || m_ownedItems == null) return null;

        for (int i = 0; i < m_ownedItems.Count; i++)
        {
            if (m_ownedItems[i] != null && m_ownedItems[i].Item == itemData)
            {
                return m_ownedItems[i];
            }
        }

        return null;
    }

    private void NormalizeState()
    {
        m_maxHp = Mathf.Max(1, m_maxHp);
        m_currentHp = Mathf.Clamp(m_currentHp, 0, m_maxHp);
        m_maxSkillCount = Mathf.Max(0, m_maxSkillCount);
        m_maxItemCount = Mathf.Max(0, m_maxItemCount);
        EnsureDiceSlotCount();
        NormalizeSkillList();
        NormalizeItemList();
    }

    private void EnsureDiceSlotCount()
    {
        if (m_ownedDice == null)
        {
            m_ownedDice = new List<DiceDefinition>(DiceSlotCount);
        }

        while (m_ownedDice.Count < DiceSlotCount)
        {
            m_ownedDice.Add(null);
        }

        if (m_ownedDice.Count > DiceSlotCount)
        {
            m_ownedDice.RemoveRange(DiceSlotCount, m_ownedDice.Count - DiceSlotCount);
        }
    }

    private void NormalizeSkillList()
    {
        if (m_ownedSkills == null)
        {
            m_ownedSkills = new List<SkillDefinition>();
        }

        m_ownedSkills.RemoveAll(skill => skill == null);
        if (m_ownedSkills.Count > m_maxSkillCount)
        {
            m_ownedSkills.RemoveRange(m_maxSkillCount, m_ownedSkills.Count - m_maxSkillCount);
        }
    }

    private void NormalizeItemList()
    {
        if (m_ownedItems == null)
        {
            m_ownedItems = new List<OwnedItemStack>();
        }

        m_ownedItems.RemoveAll(stack => stack == null || stack.Item == null);
        foreach (OwnedItemStack stack in m_ownedItems)
        {
            stack.NormalizeCount();
        }

        if (m_ownedItems.Count > m_maxItemCount)
        {
            m_ownedItems.RemoveRange(m_maxItemCount, m_ownedItems.Count - m_maxItemCount);
        }
    }

    private void NotifyChanged()
    {
        PlayerStateChanged?.Invoke();
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        NormalizeState();
    }
#endif
}
