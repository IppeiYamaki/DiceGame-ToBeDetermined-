using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// 攻撃コマンドを敵へドラッグするためのSource。
/// Drag中の攻撃コストを共有し、EnemyDropTargetが受け取ります。
/// </summary>
public class AttackDragSource : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [SerializeField]
    [Header("攻撃コマンドView")]
    private AdjustableCostCommandView m_attackCommandView;

    private static AttackDragSource s_current;
    private static int s_currentAttackCost;

    /// <summary>現在ドラッグ中の攻撃Source。</summary>
    public static AttackDragSource Current => s_current;

    /// <summary>現在ドラッグ中の攻撃コスト。</summary>
    public static int CurrentAttackCost => s_currentAttackCost;

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (m_attackCommandView == null) return;

        s_current = this;
        s_currentAttackCost = m_attackCommandView.CurrentCost;
    }

    public void OnDrag(PointerEventData eventData)
    {
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (s_current == this)
        {
            s_current = null;
            s_currentAttackCost = 0;
        }
    }
}
