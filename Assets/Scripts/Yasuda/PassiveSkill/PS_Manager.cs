using UnityEngine;

public class PS_Manager : MonoBehaviour
{
    public static PS_Manager Instance;

    public enum PassiveSkill
    {
        None,
        Gyakkyo,
        Haisui,
        Yoigosi
    }

    private PassiveSkill m_passiveSkill;
    //外部から参照する用
    public PassiveSkill m_passiveSkillType => m_passiveSkill;




    private void Awake()
    {
        // すでに存在する場合は重複生成を防ぐ
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    //シーン開始時に他のスクリプトから呼び出してリセットする
    public void ResetPassiveSkill()
    {
        m_passiveSkill = PassiveSkill.None;
    }


    public void SetPassiveSkill(int number)
    {
        if(number == 0)
        {
            m_passiveSkill = PassiveSkill.Gyakkyo;
        }
        else if (number == 1)
        {
            m_passiveSkill = PassiveSkill.Haisui;
        }
        else
        {
            m_passiveSkill = PassiveSkill.Yoigosi;
        }

        Debug.Log(m_passiveSkill.ToString());
    }

    public PassiveSkill GetPassiveSkill()
    {
        return m_passiveSkill;
    }

    //呼び出し例
    //if(PS_Manager.Instance.GetPassiveSkill() == PassiveSkill.Gyakkyo)
    //{
    //  Debug.Log(PS_Manager.Instance.GetPassiveSkill());
    //}
      


}
