using UnityEngine;
using UnityEngine.EventSystems;

public class SelectButton : MonoBehaviour, IPointerClickHandler
{
    [SerializeField]
    SceneChangeManager m_scm;

    public void OnPointerClick(PointerEventData eventData)//対象Imageをクリックすると実行
    {
        if (PS_Manager.Instance.m_passiveSkillType != PS_Manager.PassiveSkill.None)
        {
            //シーン遷移
            //Debug.Log("シーン遷移");
            m_scm.ChangeScene();
        }
        else
        {
            Debug.Log("スキルを選択してください");
        }
    }
}
