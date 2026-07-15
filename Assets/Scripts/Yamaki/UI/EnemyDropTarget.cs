using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// 攻撃ドラッグを受け取り、ActionPointBattleSceneControllerへ攻撃実行を依頼するDropTarget。
/// </summary>
public class EnemyDropTarget : MonoBehaviour, IDropHandler
{
    [SerializeField]
    [Header("ActionPoint戦闘Controller")]
    private ActionPointBattleSceneController m_battleController;

    [SerializeField]
    [Header("敵インデックス（EnemyArea内の順番）")]
    private int m_enemyIndex = -1;

    public void OnDrop(PointerEventData eventData)
    {
        if (m_battleController == null)
        {
            m_battleController = FindObjectOfType<ActionPointBattleSceneController>();
        }

        if (m_battleController == null || AttackDragSource.Current == null)
        {
            return;
        }

        // ドロップ先の敵インデックスを指定して予約攻撃を行う
        m_battleController.ReserveAttack(AttackDragSource.CurrentAttackCost, m_enemyIndex);
    }

    /// <summary>
    /// エリア内のインデックスを設定します（EnemyAreaView から呼び出す想定）。
    /// </summary>
    public void InitIndex(int index)
    {
        m_enemyIndex = index;
    }
}
