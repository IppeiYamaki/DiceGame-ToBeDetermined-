using System;
using UnityEngine;

/// <summary>
/// Yasuda版ItemData 1件に対して効果情報を紐付ける変換エントリ。
/// ItemData自体は効果データを持たないため、効果種別・効果量・対象・表示用Spriteをここで補完します。
/// 戦闘側コードはこのエントリ経由で効果種別・効果量・対象・アイコンを参照します。
/// </summary>
[Serializable]
public class ItemEffectEntry
{
    [SerializeField]
    [Header("対象アイテム(Yasuda版 ItemData)")]
    [Tooltip("効果を定義する対象の ItemData アセット")]
    private ItemData m_item;

    [SerializeField]
    [Header("効果種別")]
    private ItemEffectType m_effectType = ItemEffectType.Heal;

    [SerializeField]
    [Tooltip("効果種別が ApplyStatusEffect の場合に付与する状態異常")]
    private StatusEffectDefinition m_statusEffect;

    /// <summary>付与する状態異常。</summary>
    public StatusEffectDefinition StatusEffect => m_statusEffect;

    [SerializeField]
    [Header("効果量")]
    [Min(0)]
    private int m_effectValue = 1;

    [SerializeField]
    [Header("対象タイプ")]
    private ActionTargetType m_targetType = ActionTargetType.Self;

    [SerializeField]
    [Header("表示用アイコン(Sprite)")]
    [Tooltip("未設定の場合は ItemData の icon(Texture2D)から自動変換を試みます")]
    private Sprite m_iconOverride;

    [SerializeField]
    [Header("説明の上書き(任意)")]
    [Tooltip("空欄の場合は ItemData の description を使用します")]
    [TextArea(2, 4)]
    private string m_descriptionOverride = "";

    /// <summary>Texture2Dから生成したSpriteのキャッシュ。</summary>
    private Sprite m_cachedConvertedIcon;

    /// <summary>対象のItemData。</summary>
    public ItemData Item => m_item;

    /// <summary>アイテム名(ItemData由来)。</summary>
    public string ItemName => m_item != null ? m_item.itemName : "-";

    /// <summary>説明。上書きがあれば上書きを、なければItemDataの説明を返します。</summary>
    public string Description =>
        !string.IsNullOrEmpty(m_descriptionOverride)
            ? m_descriptionOverride
            : (m_item != null ? m_item.description : "");

    /// <summary>効果種別。</summary>
    public ItemEffectType EffectType => m_effectType;

    /// <summary>効果量。</summary>
    public int EffectValue => m_effectValue;

    /// <summary>行動の対象タイプ。</summary>
    public ActionTargetType TargetType => m_targetType;

    /// <summary>
    /// 表示用アイコン。Sprite上書き優先、未設定ならItemDataのTexture2Dから変換(キャッシュあり)。
    /// </summary>
    public Sprite Icon
    {
        get
        {
            if (m_iconOverride != null) return m_iconOverride;

            if (m_cachedConvertedIcon == null && m_item != null && m_item.icon is Texture2D texture2D)
            {
                m_cachedConvertedIcon = Sprite.Create(
                    texture2D,
                    new Rect(0f, 0f, texture2D.width, texture2D.height),
                    new Vector2(0.5f, 0.5f));
            }

            return m_cachedConvertedIcon;
        }
    }
}
