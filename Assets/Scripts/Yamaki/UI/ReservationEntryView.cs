using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// 予約レーンの1エントリ表示。
/// 固定スロットとして配置し、ActionReservationLaneViewから内容を更新します。
/// 右クリックで予約をキャンセルします。
/// </summary>
public class ReservationEntryView : MonoBehaviour, IPointerClickHandler
{
    [SerializeField]
    [Header("対象アイコン")]
    private Image m_targetIcon;

    [SerializeField]
    [Header("行動アイコン")]
    private Image m_actionIcon;

    [SerializeField]
    [Header("ラベルテキスト")]
    private TMP_Text m_labelText;

    [SerializeField]
    [Header("キャンセルボタン（任意）")]
    private Button m_cancelButton;

    private int m_index = -1;
    private Action<int> m_onCancel;
    private bool m_hasAction;

    // Texture -> Sprite キャッシュ（生成コスト回避）
    private static readonly Dictionary<Texture2D, Sprite> s_textureSpriteCache = new Dictionary<Texture2D, Sprite>();

    private void Awake()
    {
        if (m_cancelButton != null)
        {
            m_cancelButton.onClick.AddListener(HandleCancel);
        }
    }

    private void OnDestroy()
    {
        if (m_cancelButton != null)
        {
            m_cancelButton.onClick.RemoveListener(HandleCancel);
        }
    }

    public void Setup(int index, PlannedAction action, Action<int> onCancel, BattleIconSettings iconSettings)
    {
        m_index = index;
        m_onCancel = onCancel;
        m_hasAction = action != null;

        if (m_labelText != null)
        {
            m_labelText.text = action != null ? action.Label : "";
        }

        // アイコンの適用
        ApplyActionIcon(action, iconSettings);
        ApplyTargetIcon(action, iconSettings);
    }

    /// <summary>
    /// 空スロット表示にします。
    /// </summary>
    public void SetupEmpty()
    {
        m_index = -1;
        m_onCancel = null;
        m_hasAction = false;

        if (m_labelText != null) m_labelText.text = "";
        if (m_actionIcon != null) m_actionIcon.enabled = false;
        if (m_targetIcon != null) m_targetIcon.enabled = false;
    }

    private void ApplyActionIcon(PlannedAction action, BattleIconSettings iconSettings)
    {
        if (m_actionIcon == null)
            return;

        Sprite sprite = null;

        if (action == null)
        {
            m_actionIcon.enabled = false;
            return;
        }

        switch (action.Kind)
        {
            case PlannedAction.PlannedActionKind.Attack:
                sprite = iconSettings != null ? iconSettings.AttackIcon : null;
                break;
            case PlannedAction.PlannedActionKind.Defense:
                sprite = iconSettings != null ? iconSettings.DefenseIcon : null;
                break;
            case PlannedAction.PlannedActionKind.Skill:
                sprite = action.Skill != null ? action.Skill.Icon : null;
                if (sprite == null && iconSettings != null)
                {
                    sprite = iconSettings.UnknownActionIcon;
                }
                break;
            case PlannedAction.PlannedActionKind.Item:
                if (action.Item != null)
                {
                    // ItemData.icon は Texture の場合がある
                    var texField = GetItemTexture(action.Item);
                    sprite = GetOrCreateSpriteFromTexture(texField);
                }
                break;
            default:
                sprite = iconSettings != null ? iconSettings.UnknownActionIcon : null;
                break;
        }

        m_actionIcon.sprite = sprite;
        m_actionIcon.enabled = sprite != null;
    }

    private void ApplyTargetIcon(PlannedAction action, BattleIconSettings iconSettings)
    {
        if (m_targetIcon == null)
            return;

        if (action == null)
        {
            m_targetIcon.enabled = false;
            return;
        }

        // SingleEnemy のとき TargetEnemy が設定されていれば表示
        if (action.TargetType == ActionTargetType.SingleEnemy && action.TargetEnemy != null)
        {
            var def = action.TargetEnemy.Definition;
            Sprite sprite = def != null ? def.VisualSprite : null;
            if (sprite == null && def != null && def.VisualTexture != null)
            {
                sprite = GetOrCreateSpriteFromTexture(def.VisualTexture);
            }

            m_targetIcon.sprite = sprite;
            m_targetIcon.enabled = sprite != null;
            return;
        }

        // Self や AllEnemies など、単一の敵アバターを表すべきでない場合は非表示
        m_targetIcon.enabled = false;
    }

    private static Sprite GetOrCreateSpriteFromTexture(Texture tex)
    {
        if (tex == null) return null;
        var tex2d = tex as Texture2D;
        if (tex2d == null) return null;

        if (s_textureSpriteCache.TryGetValue(tex2d, out var cached))
        {
            return cached;
        }

        var rect = new Rect(0, 0, tex2d.width, tex2d.height);
        var sprite = Sprite.Create(tex2d, rect, new Vector2(0.5f, 0.5f), 100f);
        s_textureSpriteCache[tex2d] = sprite;
        return sprite;
    }

    private static Texture GetItemTexture(ItemData item)
    {
        if (item == null) return null;
        // ItemData.icon は Texture のフィールド名として存在している
        return item.icon;
    }

    /// <summary>
    /// 右クリックで予約をキャンセルします。
    /// </summary>
    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Right) return;
        HandleCancel();
    }

    private void HandleCancel()
    {
        if (!m_hasAction) return;
        m_onCancel?.Invoke(m_index);
    }
}
