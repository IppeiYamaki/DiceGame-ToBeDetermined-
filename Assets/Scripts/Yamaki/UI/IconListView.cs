using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// アイコンリスト表示用の汎用View
/// 複数のSprite（バフ/デバフ/敵行動予測など）を動的に生成・更新します
/// 
/// 使い方:
/// 1. Inspector で IconPrefab（Image付きのPrefab）と IconContainer（親Transform）を割り当てます
/// 2. UpdateIcons(sprites) を呼んで表示を更新します
/// </summary>
public class IconListView : MonoBehaviour
{
    [SerializeField]
    [Header("アイコンPrefab")]
    [Tooltip("アイコン1個分のPrefab（Image Component必須）")]
    private GameObject m_iconPrefab;

    [SerializeField]
    [Header("アイコン配置先")]
    [Tooltip("アイコンを生成する親Transform（例: HorizontalLayoutGroup付きのパネル）")]
    private Transform m_iconContainer;

    [SerializeField]
    [Header("最大表示数")]
    [Tooltip("表示するアイコンの最大数。0で無制限")]
    [Min(0)]
    private int m_maxIconCount = 0;

    private readonly List<GameObject> m_iconInstances = new List<GameObject>();

    /// <summary>
    /// アイコンリストを更新します
    /// 既存のアイコンは削除し、新しいSpriteリストに基づいて再生成します
    /// </summary>
    /// <param name="sprites">表示するSpriteのリスト</param>
    public void UpdateIcons(IReadOnlyList<Sprite> sprites)
    {
        ClearIcons();

        if (sprites == null || sprites.Count == 0 || m_iconPrefab == null || m_iconContainer == null)
        {
            return;
        }

        int displayCount = m_maxIconCount > 0 ? Mathf.Min(sprites.Count, m_maxIconCount) : sprites.Count;

        for (int i = 0; i < displayCount; i++)
        {
            Sprite sprite = sprites[i];
            if (sprite == null) continue;

            GameObject iconInstance = Instantiate(m_iconPrefab, m_iconContainer);
            Image iconImage = iconInstance.GetComponent<Image>();
            if (iconImage != null)
            {
                iconImage.sprite = sprite;
            }

            m_iconInstances.Add(iconInstance);
        }
    }

    /// <summary>
    /// アイコンとラベルの組み合わせでリストを更新します
    /// </summary>
    /// <param name="displays">表示するアイコンデータのリスト</param>
    public void UpdateIconDisplays(IReadOnlyList<IconDisplayData> displays)
    {
        ClearIcons();

        if (displays == null || displays.Count == 0 || m_iconPrefab == null || m_iconContainer == null)
        {
            return;
        }

        int displayCount = m_maxIconCount > 0 ? Mathf.Min(displays.Count, m_maxIconCount) : displays.Count;

        for (int i = 0; i < displayCount; i++)
        {
            IconDisplayData display = displays[i];
            if (display == null || display.IconSprite == null) continue;

            GameObject iconInstance = Instantiate(m_iconPrefab, m_iconContainer);
            Image iconImage = iconInstance.GetComponent<Image>();
            if (iconImage != null)
            {
                iconImage.sprite = display.IconSprite;
                iconImage.raycastTarget = false;
            }

            TMP_Text labelText = iconInstance.GetComponentInChildren<TMP_Text>(true);
            if (labelText == null)
            {
                labelText = CreateLabelText(iconInstance.transform);
            }

            if (labelText != null)
            {
                labelText.text = display.LabelText;
                labelText.raycastTarget = false;
            }

            m_iconInstances.Add(iconInstance);
        }
    }

    private TMP_Text CreateLabelText(Transform parent)
    {
        GameObject labelObject = new GameObject("ValueText", typeof(RectTransform));
        labelObject.transform.SetParent(parent, false);

        RectTransform rectTransform = labelObject.GetComponent<RectTransform>();
        rectTransform.anchorMin = Vector2.zero;
        rectTransform.anchorMax = Vector2.one;
        rectTransform.offsetMin = Vector2.zero;
        rectTransform.offsetMax = Vector2.zero;

        TextMeshProUGUI labelText = labelObject.AddComponent<TextMeshProUGUI>();
        labelText.alignment = TextAlignmentOptions.Center;
        labelText.fontSize = 18f;
        labelText.color = Color.white;
        labelText.fontStyle = FontStyles.Bold;
        labelText.enableAutoSizing = true;
        labelText.fontSizeMin = 8f;
        labelText.fontSizeMax = 24f;
        labelText.raycastTarget = false;

        return labelText;
    }

    /// <summary>
    /// すべてのアイコンをクリアします
    /// </summary>
    public void ClearIcons()
    {
        foreach (GameObject iconInstance in m_iconInstances)
        {
            if (iconInstance != null)
            {
                Destroy(iconInstance);
            }
        }

        m_iconInstances.Clear();
    }

    /// <summary>
    /// 最大表示数を変更します
    /// </summary>
    /// <param name="maxCount">新しい最大表示数（0で無制限）</param>
    public void SetMaxIconCount(int maxCount)
    {
        m_maxIconCount = Mathf.Max(0, maxCount);
    }

    private void OnDestroy()
    {
        ClearIcons();
    }
}
