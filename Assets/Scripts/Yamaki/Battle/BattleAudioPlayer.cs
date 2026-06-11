using UnityEngine;

/// <summary>
/// 戦闘中のサウンド再生を担当するコンポーネント
/// AudioSource を参照し、Play(AudioClip, volume) でワンショット再生します
/// 
/// 使い方:
/// 1. BattleSceneController 等にアタッチまたは参照で渡します
/// 2. Play(clip, volume) を呼ぶと指定されたクリップを再生します
/// 3. AudioSource が未設定の場合は Awake で自動追加されます
/// </summary>
[RequireComponent(typeof(AudioSource))]
public class BattleAudioPlayer : MonoBehaviour
{
    [SerializeField]
    [Header("AudioSource（未設定時は自動追加）")]
    [Tooltip("サウンド再生用の AudioSource。未設定の場合は自動的に取得または追加されます")]
    private AudioSource m_audioSource;

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

        // ワンショット再生用に PlayOnAwake を OFF
        if (m_audioSource != null)
        {
            m_audioSource.playOnAwake = false;
        }
    }

    /// <summary>
    /// 指定されたクリップをワンショット再生します
    /// </summary>
    /// <param name="clip">再生する AudioClip（null の場合は再生しない）</param>
    /// <param name="volume">再生音量（0.0～1.0）</param>
    public void Play(AudioClip clip, float volume = 1.0f)
    {
        if (clip == null)
        {
            // null クリップは無視（エラーログなし）
            return;
        }

        if (m_audioSource == null)
        {
            Debug.LogWarning("[BattleAudioPlayer] AudioSource が設定されていません。");
            return;
        }

        volume = Mathf.Clamp01(volume);
        m_audioSource.PlayOneShot(clip, volume);
    }

    /// <summary>
    /// 現在再生中のサウンドを停止します
    /// </summary>
    public void Stop()
    {
        if (m_audioSource != null && m_audioSource.isPlaying)
        {
            m_audioSource.Stop();
        }
    }
}
