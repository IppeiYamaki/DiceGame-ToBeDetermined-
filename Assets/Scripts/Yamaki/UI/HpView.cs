using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// HP表示用の汎用View
/// HPテキストとオプションのスライダーを更新します
/// 
/// 使い方:
/// 1. Inspector で TMP_Text と Slider（任意）を割り当てます
/// 2. UpdateHp(currentHp, maxHp) を呼んで表示を更新します
/// </summary>
public class HpView : MonoBehaviour
{
    [SerializeField]
    [Header("HP表示テキスト")]
    [Tooltip("「100 / 150」のようにHP表示するテキスト")]
    private TMP_Text m_hpText;

    [SerializeField]
    [Header("HPスライダー（任意）")]
    [Tooltip("HPをバー表示するスライダー。未設定でも動作します")]
    private Slider m_hpSlider;

    [SerializeField]
    [Header("HP表示フォーマット")]
    [Tooltip("HP表示の書式。{0}=現在HP、{1}=最大HP\n例: \"{0} / {1}\"、\"HP: {0}\"")]
    private string m_hpFormat = "{0} / {1}";

    /// <summary>
    /// HP表示を更新します
    /// </summary>
    /// <param name="currentHp">現在HP</param>
    /// <param name="maxHp">最大HP</param>
    public void UpdateHp(int currentHp, int maxHp)
    {
        currentHp = Mathf.Max(0, currentHp);
        maxHp = Mathf.Max(1, maxHp);

        if (m_hpText != null)
        {
            m_hpText.text = string.Format(m_hpFormat, currentHp, maxHp);
        }

        if (m_hpSlider != null)
        {
            m_hpSlider.maxValue = maxHp;
            m_hpSlider.value = currentHp;
        }
    }

    /// <summary>
    /// 表示フォーマットを変更します
    /// </summary>
    /// <param name="format">新しいフォーマット（例: "HP: {0}/{1}"）</param>
    public void SetHpFormat(string format)
    {
        if (!string.IsNullOrEmpty(format))
        {
            m_hpFormat = format;
        }
    }
}
