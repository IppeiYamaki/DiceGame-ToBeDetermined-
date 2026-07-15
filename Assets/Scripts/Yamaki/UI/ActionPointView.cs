using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// ActionPoint表示用の汎用View。
/// 現在APとターン開始時の獲得APを表示します。
/// </summary>
public class ActionPointView : MonoBehaviour
{
    [SerializeField]
    [Header("AP表示テキスト")]
    private TMP_Text m_actionPointText;

    [SerializeField]
    [Header("APスライダー（任意）")]
    private Slider m_actionPointSlider;

    [SerializeField]
    [Header("AP表示フォーマット")]
    [Tooltip("{0}=現在AP、{1}=獲得AP")]
    private string m_actionPointFormat = "AP: {0} / {1}";

    /// <summary>
    /// AP表示を更新します。
    /// </summary>
    public void UpdateActionPoint(int currentActionPoint, int gainedActionPoint)
    {
        currentActionPoint = Mathf.Max(0, currentActionPoint);
        gainedActionPoint = Mathf.Max(0, gainedActionPoint);

        if (m_actionPointText != null)
        {
            m_actionPointText.text = string.Format(m_actionPointFormat, currentActionPoint, gainedActionPoint);
        }

        if (m_actionPointSlider != null)
        {
            m_actionPointSlider.maxValue = Mathf.Max(1, gainedActionPoint);
            m_actionPointSlider.value = currentActionPoint;
        }
    }

    /// <summary>
    /// 表示フォーマットを変更します。
    /// </summary>
    public void SetActionPointFormat(string format)
    {
        if (!string.IsNullOrEmpty(format))
        {
            m_actionPointFormat = format;
        }
    }
}
