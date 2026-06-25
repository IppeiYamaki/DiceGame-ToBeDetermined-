using UnityEngine;

public class SkillManager : MonoBehaviour
{
    [SerializeField]
    private Skill_Kaihuku m_kaihuku;
    [SerializeField]
    private Skill_Doku m_doku;
    [SerializeField]
    private Skill_Mahi m_mahi;
    [SerializeField]
    private Skill_Hissatuwaza m_hissatuwaza;
    [SerializeField]
    private Skill_Buff m_buff;

    [SerializeField]
    private BlackImage[] m_BlackImage;

    private int abcde = 5;

    

    public void ImageUpdate()
    {
        bool[] flags = new bool[abcde];
        flags[0] = m_kaihuku.CheckUseSkill();
        flags[1] = m_doku.CheckUseSkill();
        flags[2] = m_mahi.CheckUseSkill();
        flags[3] = m_hissatuwaza.CheckUseSkill();
        flags[4] = m_buff.CheckUseSkill();

        for(int i = 0; i < abcde; i++)
        {
            m_BlackImage[i].AlphaChangeImage(flags[i]);
            //Debug.Log(flags[i]);
        }
    }
}
