using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Button のホバー音/クリック音を一括管理する汎用SEプレイヤー
/// 
/// 使い方:
/// 1. Canvas や UIルートにアタッチします
/// 2. Inspector でホバーSE・クリックSE・音量を設定します
/// 3. 「起動時に子Buttonへ自動登録」をONにすると、子階層のすべての Button に
///    ButtonSoundHandler が自動付与されます
/// 4. 実行中に動的生成した Button には RegisterButtonsInChildren() を再度呼ぶか、
///    手動で ButtonSoundHandler を付けて Setup() してください
/// </summary>
public class ButtonSoundPlayer : MonoBehaviour
{
    [SerializeField]
    [Header("AudioSource（未設定時は自動追加）")]
    [Tooltip("SE再生用の AudioSource。未設定の場合は自動的に取得または追加されます")]
    private AudioSource m_audioSource;

    [SerializeField]
    [Header("ホバーSE")]
    [Tooltip("カーソルがボタンに重なった時に再生する効果音")]
    private AudioClip m_hoverClip;

    [SerializeField]
    [Header("クリックSE")]
    [Tooltip("ボタンをクリックした時に再生する効果音")]
    private AudioClip m_clickClip;

    [SerializeField]
    [Header("音量（0～1）")]
    [Tooltip("ホバーSE/クリックSEの再生音量")]
    [Range(0f, 1f)]
    private float m_volume = 1.0f;

    [SerializeField]
    [Header("起動時に子Buttonへ自動登録")]
    [Tooltip("ONの場合、Awake時に子階層のすべてのButtonへ ButtonSoundHandler を自動付与します")]
    private bool m_registerOnAwake = true;

    private void Awake()
    {
        // AudioSource が未設定の場合は自動的に取得または追加
        if (m_audioSource == null)
        {
            m_audioSource = GetComponent<AudioSource>();
            if (m_audioSource == null)
            {
                m_audioSource = gameObject.AddComponent<AudioSource>();
            }
        }

        m_audioSource.playOnAwake = false;

        if (m_registerOnAwake)
        {
            RegisterButtonsInChildren();
        }
    }

    /// <summary>
    /// 子階層のすべての Button に ButtonSoundHandler を付与します
    /// すでに付与済みの Button は再利用され、重複追加されません
    /// 実行中に動的生成した Button がある場合はこのメソッドを再度呼んでください
    /// </summary>
    public void RegisterButtonsInChildren()
    {
        Button[] buttons = GetComponentsInChildren<Button>(true);
        foreach (Button button in buttons)
        {
            if (button == null) continue;

            ButtonSoundHandler handler = button.GetComponent<ButtonSoundHandler>();
            if (handler == null)
            {
                handler = button.gameObject.AddComponent<ButtonSoundHandler>();
            }

            handler.Setup(this);
        }
    }

    /// <summary>
    /// ホバー音を再生します
    /// </summary>
    public void PlayHover()
    {
        PlayClip(m_hoverClip);
    }

    /// <summary>
    /// クリック音を再生します
    /// </summary>
    public void PlayClick()
    {
        PlayClip(m_clickClip);
    }

    private void PlayClip(AudioClip clip)
    {
        if (clip == null || m_audioSource == null) return;

        m_audioSource.PlayOneShot(clip, Mathf.Clamp01(m_volume));
    }
}
