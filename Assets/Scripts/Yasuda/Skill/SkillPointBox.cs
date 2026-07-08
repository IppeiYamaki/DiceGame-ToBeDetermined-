using System.Drawing;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
public class SkillPointBox : MonoBehaviour, IPointerClickHandler
{
    //ポイント管理するコード
    //



    [SerializeField]//テスト用。本番環境は出目から取るので不要になるはず
    private int m_SetSkillPoint = 30;

    private int m_SkillPoint = 0;
    [SerializeField]
    private TMP_Text m_NokoriSkillPoint;

    [SerializeField]
    private SkillManager m_manager;

    void ImageUpdater()
    {
        m_manager.ImageUpdate();
    }



    void Start()
    {
        //SetSkillPoint(m_SetSkillPoint);//テスト用。本番は不要
    }

    public void SetSkillPoint(int skillPoint)
    {
        //ここで出目の合計を受け取る
        m_SkillPoint = skillPoint;
        m_NokoriSkillPoint.text = m_SkillPoint.ToString();

        //明るくするか確認
        ImageUpdater();
    }
    public void AddSkillPoint(int skillPoint)
    {
        //ポイントを増やすならこっち
        m_SkillPoint += skillPoint;
        m_NokoriSkillPoint.text = m_SkillPoint.ToString();

        //明るくするか確認
        ImageUpdater();
    }
    public void UseSkillPoint(int skillPoint)
    {
        //スキルを使う時に消費
        m_SkillPoint -= skillPoint;
        m_NokoriSkillPoint.text = m_SkillPoint.ToString();

        //暗くするか確認
        ImageUpdater();
    }
    public int GetSkillPoint()
    {
        //スキル使える分のポイント余っているか調べる時などに使う
        return m_SkillPoint;
    }

    public void OnPointerClick(PointerEventData eventData)//対象Imageをクリックすると実行
    {
        //テスト（クリックしたらスキルポイントセット）
        SetSkillPoint(m_SetSkillPoint);

        
    }
}
