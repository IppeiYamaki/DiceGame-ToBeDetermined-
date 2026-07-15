using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.EventSystems;

public class PS_SkillNumber : MonoBehaviour, IPointerClickHandler
{
    public int m_SkillNumber = -1;

    [SerializeField]
    PS_BIManager m_ps_BIManager;

    [SerializeField]
    PS_SkillText m_skillText;

    [SerializeField]
    private AudioClip m_se;
    private AudioSource audioSource;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void OnPointerClick(PointerEventData eventData)//対象Imageをクリックすると実行
    { 
        Debug.Log(m_SkillNumber);
        PS_Manager.Instance.SetPassiveSkill(m_SkillNumber);
        m_ps_BIManager.SetAlpha(m_SkillNumber);
        m_skillText.SetText(m_SkillNumber);
        audioSource.PlayOneShot(m_se);
    }
}
