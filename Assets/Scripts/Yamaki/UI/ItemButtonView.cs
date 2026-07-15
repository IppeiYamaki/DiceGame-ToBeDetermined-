using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 1種類のアイテムスタックを表示・使用するボタンView。
/// 同一アイテムは1ボタンにまとめ、所持数を「×N」で表示します。
/// </summary>
public class ItemButtonView : MonoBehaviour
{
    [SerializeField]
    [Header("アイテム名テキスト")]
    private TMP_Text m_itemNameText;

    [SerializeField]
    [Header("説明テキスト")]
    private TMP_Text m_descriptionText;

    [SerializeField]
    [Header("所持数テキスト(任意)")]
    [Tooltip("未設定の場合はアイテム名に「×N」を連結します")]
    private TMP_Text m_countText;

    [SerializeField]
    [Header("アイコン")]
    private Image m_iconImage;

    [SerializeField]
    [Header("使用ボタン")]
    private Button m_button;

    private ItemData m_itemData;
    private Action<ItemData> m_clicked;

    private void Awake()
    {
        if (m_button != null)
        {
            m_button.onClick.AddListener(HandleClicked);
        }
    }

    private void OnDestroy()
    {
        if (m_button != null)
        {
            m_button.onClick.RemoveListener(HandleClicked);
        }
    }

    public void Setup(ItemData itemData, int count, Func<ItemData, bool> canUse, Action<ItemData> clicked)
    {
        m_itemData = itemData;
        m_clicked = clicked;

        ItemEffectEntry effectEntry = ResolveEffectEntry(itemData);

        string name = itemData != null ? itemData.itemName : "-";
        string desc = effectEntry != null ? effectEntry.Description : (itemData != null ? itemData.description : "");
        Sprite icon = effectEntry != null ? effectEntry.Icon : null;
        string countLabel = count > 1 ? $"×{count}" : "";

        if (m_countText != null)
        {
            m_countText.text = countLabel;
            if (m_itemNameText != null) m_itemNameText.text = name;
        }
        else if (m_itemNameText != null)
        {
            m_itemNameText.text = string.IsNullOrEmpty(countLabel) ? name : $"{name} {countLabel}";
        }

        if (m_descriptionText != null) m_descriptionText.text = desc;
        if (m_iconImage != null) { m_iconImage.sprite = icon; m_iconImage.enabled = icon != null; }
        if (m_button != null) m_button.interactable = itemData != null && count > 0 && (canUse == null || canUse(itemData));
    }

    private static ItemEffectEntry ResolveEffectEntry(ItemData itemData)
    {
        if (itemData == null) return null;

        RunPlayerManager manager = RunPlayerManager.Instance;
        if (manager == null || manager.ItemEffectTable == null) return null;

        return manager.ItemEffectTable.GetEntry(itemData);
    }

    private void HandleClicked()
    {
        if (m_itemData == null) return;
        m_clicked?.Invoke(m_itemData);
    }
}
