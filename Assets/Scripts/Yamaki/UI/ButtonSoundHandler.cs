using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// Button にアタッチして、カーソルホバー時とクリック時にSEを再生するハンドラー
/// 
/// 通常は ButtonSoundPlayer.RegisterButtonsInChildren() により自動付与されるため、
/// 手動でアタッチする必要はありません
/// 個別に付ける場合は、Setup() で ButtonSoundPlayer を渡してください
/// </summary>
[RequireComponent(typeof(Button))]
public class ButtonSoundHandler : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler
{
    // SE再生を委譲する ButtonSoundPlayer
    private ButtonSoundPlayer m_soundPlayer;

    // 対象の Button（interactable 判定に使用）
    private Button m_button;

    private void Awake()
    {
        m_button = GetComponent<Button>();
    }

    /// <summary>
    /// SE再生に使用する ButtonSoundPlayer を設定します
    /// </summary>
    /// <param name="soundPlayer">SE再生を担当する ButtonSoundPlayer</param>
    public void Setup(ButtonSoundPlayer soundPlayer)
    {
        m_soundPlayer = soundPlayer;
    }

    /// <summary>
    /// カーソルがボタンに重なった時にホバー音を再生します
    /// ボタンが非interactableの場合は再生しません
    /// </summary>
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!IsButtonInteractable()) return;

        m_soundPlayer?.PlayHover();
    }

    /// <summary>
    /// ボタンをクリックした時にクリック音を再生します
    /// ボタンが非interactableの場合は再生しません
    /// </summary>
    public void OnPointerClick(PointerEventData eventData)
    {
        if (!IsButtonInteractable()) return;

        m_soundPlayer?.PlayClick();
    }

    private bool IsButtonInteractable()
    {
        return m_button != null && m_button.IsInteractable();
    }
}
