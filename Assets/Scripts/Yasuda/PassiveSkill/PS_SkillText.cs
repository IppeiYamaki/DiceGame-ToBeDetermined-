using TMPro;
using UnityEngine;


public class PS_SkillText : MonoBehaviour
{
   
    [SerializeField]
    private TMP_Text m_skillText;

    void Start()
    {
        m_skillText.text = "";
    }

    public void SetText(int i)
    {
        if (i == 0)
        {
            m_skillText.text = "ダイスの出目の合計値が「4～6」だった場合、そのダイスから得られるポイント「+5」にして算出する。（発動条件：基本的に合計値が6以下の場合発動可能　役があっても発動する場合は（123・ペア）";
        }
        else if (i == 1)
        {
            m_skillText.text = "DEFに振ったポイントが「0」のターン、ATKの数値が「+10」になる。";
        }
        else if (i == 2)
        {
            m_skillText.text = "ターン終了時、ATKにもDEFにもスキルにも使わず「余らせたポイント」を、次のターンに最大5ポイントまで持ち越すことができる。";
        }
        else
        {
            m_skillText.text = "無効";
        }
    }
}
