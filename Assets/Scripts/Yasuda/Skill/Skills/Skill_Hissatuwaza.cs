using UnityEngine;
using UnityEngine.EventSystems;

public class Skill_Hissatuwaza : MonoBehaviour, IPointerClickHandler
{
    [SerializeField]
    SkillPointBox m_point;
    [SerializeField]
    private int m_usePoint = 15;

    [SerializeField]//このバトルで使えるのは一度きりか設定。trueなら一度だけ
    private bool m_gameOneUse = false;

    //このターン使ったか。使ったらtrueにする
    private bool m_use = false;

    [SerializeField]
    private AudioClip se;
    private AudioSource audioSource;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }
    private void PlaySE()
    {
        audioSource.PlayOneShot(se);
    }

    public bool CheckUseSkill()
    {
        //スキルを使用できるかチェック
        //・共通
        //１このターンまだこのスキルを使ってないか
        //２残りポイントが消費ポイント以上あるか
        //
        //・その他
        //必要に応じて設定

        if (m_use == false)
        {
            int skillPoint = m_point.GetSkillPoint();
            if (skillPoint >= m_usePoint)
            {
                //ここにそれぞれの条件を見る
                return true;
            }
        }

        return false;
    }
    public void UseSkill()
    {
        //出た目の2倍のダメージを敵に与える
        PlaySE();
    }


    public void TurnStart()
    {
        //プレイヤーターン開始時に実行
        //1バトルで一度きりならm_useを戻さない
        if (m_gameOneUse == false)
        {
            m_use = false;
        }
    }


    public void OnPointerClick(PointerEventData eventData)//対象Imageをクリックすると実行
    {
        if (CheckUseSkill() == true)
        {
            m_use = true;
            m_point.UseSkillPoint(m_usePoint);
            UseSkill();//ここでスキル発動
           
        }
    }
}
