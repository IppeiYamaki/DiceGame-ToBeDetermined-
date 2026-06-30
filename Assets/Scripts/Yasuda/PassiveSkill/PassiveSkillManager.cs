using UnityEngine;
using UnityEngine.EventSystems;

public class PassiveSkillManager : MonoBehaviour, IPointerClickHandler
{
    public int m_SkillNumber = -1;

    [SerializeField]
    PS_BIManager m_ps_BIManager;

    public void OnPointerClick(PointerEventData eventData)//対象Imageをクリックすると実行
    { 
        Debug.Log(m_SkillNumber);
        PS_Manager.Instance.SetPassiveSkill(m_SkillNumber);
        m_ps_BIManager.SetAlpha(m_SkillNumber);
    }
}
