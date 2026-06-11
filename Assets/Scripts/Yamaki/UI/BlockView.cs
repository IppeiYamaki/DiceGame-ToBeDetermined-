using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Block（ブロック）値表示用の汎用View
/// Blockテキストとオプションのアイコン（盾等）を更新します
/// 
/// 使い方:
/// 1. Inspector で TMP_Text と Image（任意）を割り当てます
/// 2. UpdateBlock(blockValue) を呼んで表示を更新します
/// 
/// HpView のスタイルを踏襲しています
/// </summary>
public class BlockView : MonoBehaviour
{
    [SerializeField]
    [Header("Block表示テキスト")]
    [Tooltip("ブロック値を表示するテキスト\n例: 「Block: 5」")]
    private TMP_Text m_blockText;

    [SerializeField]
    [Header("Blockアイコン（任意）")]
    [Tooltip("ブロック値を視覚的に示すアイコン（例: 盾）\n未設定でも動作します")]
    private Image m_blockIcon;

    [SerializeField]
    [Header("Block表示フォーマット")]
    [Tooltip("Block表示の書式。{0}=現在Block値\n例: \"Block: {0}\"、\"防御 {0}\"")]
    private string m_blockFormat = "Block: {0}";

    [SerializeField]
    [Header("Block値が0の時に非表示にする")]
    [Tooltip("Block値が0の時にテキストとアイコンを非表示にします")]
    private bool m_hideWhenZero = false;

    /// <summary>
    /// Block表示を更新します
    /// </summary>
    /// <param name="blockValue">現在のBlock値</param>
    public void UpdateBlock(int blockValue)
    {
        blockValue = Mathf.Max(0, blockValue);

        bool isVisible = !m_hideWhenZero || blockValue > 0;

        if (m_blockText != null)
        {
            m_blockText.text = string.Format(m_blockFormat, blockValue);
            m_blockText.gameObject.SetActive(isVisible);
        }

        if (m_blockIcon != null)
        {
            m_blockIcon.gameObject.SetActive(isVisible);
        }
    }

    /// <summary>
    /// 表示フォーマットを変更します
    /// </summary>
    /// <param name="format">新しいフォーマット（例: "防御: {0}"）</param>
    public void SetBlockFormat(string format)
    {
        if (!string.IsNullOrEmpty(format))
        {
            m_blockFormat = format;
        }
    }

    /// <summary>
    /// Blockアイコンを変更します
    /// </summary>
    /// <param name="icon">新しいアイコンSprite</param>
    public void SetBlockIcon(Sprite icon)
    {
        if (m_blockIcon != null)
        {
            m_blockIcon.sprite = icon;
        }
    }
}
