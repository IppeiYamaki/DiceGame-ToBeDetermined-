using UnityEngine;

public class ClickManager : MonoBehaviour
{
    [SerializeField] private AudioSource seSource;
    [SerializeField] private AudioClip clickSound;

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            PlayClick();
        }
    }

    public void PlayClick()
    {
        if (seSource == null)
        {
            Debug.LogWarning("AudioSourceが設定されていません。", this);
            return;
        }

        if (clickSound == null)
        {
            Debug.LogWarning("クリック音が設定されていません。", this);
            return;
        }

        seSource.PlayOneShot(clickSound);
    }
}