using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// 1個分の手札ダイスUI
/// クリック、ホバー、表示名更新、ピックアップ時の拡大/色変更/アウトライン風表示を担当します
/// </summary>
public class DiceHandButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField]
    [Header("Button")]
    [Tooltip("クリック操作を受け付けるButton")]
    private Button m_button;

    [SerializeField]
    [Header("表示Text")]
    [Tooltip("ダイス名またはIDを表示するText")]
    private Text m_labelText;

    [SerializeField]
    [Header("背景Image")]
    [Tooltip("ピックアップ状態の色変更に使用するImage")]
    private Image m_backgroundImage;

    [SerializeField]
    [Header("アウトライン表示")]
    [Tooltip("ピックアップ状態のアウトライン風表示に使用するGameObject")]
    private GameObject m_outlineObject;

    [SerializeField]
    [Header("通常色")]
    [Tooltip("未選択時の背景色")]
    private Color m_normalColor = Color.white;

    [SerializeField]
    [Header("ピックアップ色")]
    [Tooltip("ピックアップ時の背景色")]
    private Color m_pickedColor = new Color(1f, 0.9f, 0.3f, 1f);

    [SerializeField]
    [Header("通常スケール")]
    [Tooltip("未選択時の表示スケール")]
    private Vector3 m_normalScale = Vector3.one;

    [SerializeField]
    [Header("ピックアップスケール")]
    [Tooltip("ピックアップ時の表示スケール")]
    private Vector3 m_pickedScale = new Vector3(1.12f, 1.12f, 1f);

    private string m_diceId = "";
    private DiceDefinition m_diceDefinition;
    private bool m_isPicked = false;

    /// <summary>
    /// ダイスクリック時に通知されるイベント
    /// </summary>
    public event Action<DiceHandButton> OnClicked;

    /// <summary>
    /// ダイスホバー開始時に通知されるイベント
    /// </summary>
    public event Action<DiceHandButton> OnHoverEntered;

    /// <summary>
    /// ダイスホバー終了時に通知されるイベント
    /// </summary>
    public event Action<DiceHandButton> OnHoverExited;

    /// <summary>
    /// ダイスID
    /// </summary>
    public string DiceId => m_diceId;

    /// <summary>
    /// ダイス定義
    /// </summary>
    public DiceDefinition DiceDefinition => m_diceDefinition;

    /// <summary>
    /// ピックアップ中かどうか
    /// </summary>
    public bool IsPicked => m_isPicked;

    /// <summary>
    /// ボタンイベントを登録します
    /// </summary>
    private void Awake()
    {
        if (m_button != null)
        {
            m_button.onClick.AddListener(HandleButtonClicked);
        }
        else
        {
            Debug.LogWarning("[DiceHandButton] Button が設定されていないため、ダイスをクリックして選択できません。", this);
        }

        RefreshVisual();
    }

    /// <summary>
    /// オブジェクト破棄時にボタンイベントを解除します
    /// </summary>
    private void OnDestroy()
    {
        if (m_button != null)
        {
            m_button.onClick.RemoveListener(HandleButtonClicked);
        }
    }

    /// <summary>
    /// ダイス情報を設定します
    /// </summary>
    /// <param name="diceId">ダイスID</param>
    /// <param name="diceDefinition">ダイス定義</param>
    public void Setup(string diceId, DiceDefinition diceDefinition)
    {
        m_diceId = diceId;
        m_diceDefinition = diceDefinition;
        SetPicked(false);

        if (m_labelText != null)
        {
            m_labelText.text = diceDefinition != null && !string.IsNullOrEmpty(diceDefinition.DiceName)
                ? diceDefinition.DiceName
                : diceId;
        }
    }

    /// <summary>
    /// ピックアップ状態を設定します
    /// </summary>
    /// <param name="isPicked">ピックアップ中にするかどうか</param>
    public void SetPicked(bool isPicked)
    {
        m_isPicked = isPicked;
        RefreshVisual();
    }

    /// <summary>
    /// ホバー開始時に呼び出されます
    /// </summary>
    /// <param name="eventData">ポインターイベントデータ</param>
    public void OnPointerEnter(PointerEventData eventData)
    {
        OnHoverEntered?.Invoke(this);
    }

    /// <summary>
    /// ホバー終了時に呼び出されます
    /// </summary>
    /// <param name="eventData">ポインターイベントデータ</param>
    public void OnPointerExit(PointerEventData eventData)
    {
        OnHoverExited?.Invoke(this);
    }

    /// <summary>
    /// クリック時の処理
    /// </summary>
    private void HandleButtonClicked()
    {
        OnClicked?.Invoke(this);
    }

    /// <summary>
    /// ピックアップ状態に応じて見た目を更新します
    /// </summary>
    private void RefreshVisual()
    {
        transform.localScale = m_isPicked ? m_pickedScale : m_normalScale;

        if (m_backgroundImage != null)
        {
            m_backgroundImage.color = m_isPicked ? m_pickedColor : m_normalColor;
        }

        if (m_outlineObject != null)
        {
            m_outlineObject.SetActive(m_isPicked);
        }
    }
}
