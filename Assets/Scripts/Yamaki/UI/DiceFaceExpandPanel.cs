using System.Text;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// ダイスにカーソルを合わせた時に6面情報を表示するUIパネル
/// 表示先TextやPanelの表示/非表示をUnity Editor上で設定できます
/// </summary>
public class DiceFaceExpandPanel : MonoBehaviour
{
    [SerializeField]
    [Header("ルートオブジェクト")]
    [Tooltip("表示/非表示を切り替えるルートGameObject。未設定の場合は自身のGameObjectを使用します")]
    private GameObject m_rootObject;

    [SerializeField]
    [Header("面情報 Text")]
    [Tooltip("ダイス6面情報を表示するText")]
    private Text m_faceInfoText;

    [SerializeField]
    [Header("タイトルフォーマット")]
    [Tooltip("展開図タイトルのフォーマット。{0}=ダイス名")]
    private string m_titleFormat = "【{0} の面】";

    [SerializeField]
    [Header("面フォーマット")]
    [Tooltip("1面分の表示フォーマット。{0}=面番号、{1}=出目")]
    private string m_faceFormat = "[面{0}] 出目:{1}";

    /// <summary>
    /// 初期状態では非表示にします
    /// </summary>
    private void Awake()
    {
        Hide();
    }

    /// <summary>
    /// ダイス定義をもとに面情報を表示します
    /// </summary>
    /// <param name="diceDefinition">表示するダイス定義</param>
    public void Show(DiceDefinition diceDefinition)
    {
        if (diceDefinition == null)
        {
            Hide();
            return;
        }

        if (m_faceInfoText != null)
        {
            m_faceInfoText.text = BuildFaceInfoText(diceDefinition);
        }

        SetActive(true);
    }

    /// <summary>
    /// 展開図を非表示にします
    /// </summary>
    public void Hide()
    {
        SetActive(false);
    }

    /// <summary>
    /// ダイス面情報の表示テキストを生成します
    /// </summary>
    /// <param name="diceDefinition">ダイス定義</param>
    /// <returns>表示用テキスト</returns>
    private string BuildFaceInfoText(DiceDefinition diceDefinition)
    {
        StringBuilder stringBuilder = new StringBuilder();
        stringBuilder.AppendLine(string.Format(m_titleFormat, diceDefinition.DiceName));

        for (int i = 0; i < diceDefinition.Faces.Count; i++)
        {
            DiceFaceData faceData = diceDefinition.Faces[i];
            stringBuilder.AppendLine(string.Format(m_faceFormat, i + 1, faceData.Number));
        }

        return stringBuilder.ToString();
    }

    /// <summary>
    /// 表示状態を切り替えます
    /// </summary>
    /// <param name="isActive">表示するかどうか</param>
    private void SetActive(bool isActive)
    {
        GameObject targetObject = m_rootObject != null ? m_rootObject : gameObject;
        targetObject.SetActive(isActive);
    }
}
