using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ダイスIDをカードゲームのデッキのように管理するRuntimeクラス
/// 山札、手札、使用済み、捨て札、シャッフル、ドロー、使用ダイス確定処理を提供します
/// </summary>
[Serializable]
public class DiceDeckRuntime
{
    [SerializeField]
    [Header("山札")]
    [Tooltip("現在ドロー可能なダイスIDリスト")]
    private List<string> m_drawPile = new List<string>();

    [SerializeField]
    [Header("手札")]
    [Tooltip("現在手札にあるダイスIDリスト")]
    private List<string> m_hand = new List<string>();

    [SerializeField]
    [Header("使用済み")]
    [Tooltip("このターンに使用したダイスIDリスト")]
    private List<string> m_usedDice = new List<string>();

    [SerializeField]
    [Header("捨て札")]
    [Tooltip("山札が尽きたときに再シャッフルされるダイスIDリスト")]
    private List<string> m_discardPile = new List<string>();

    /// <summary>
    /// 山札
    /// </summary>
    public IReadOnlyList<string> DrawPile => m_drawPile;

    /// <summary>
    /// 手札
    /// </summary>
    public IReadOnlyList<string> Hand => m_hand;

    /// <summary>
    /// 使用済みダイス
    /// </summary>
    public IReadOnlyList<string> UsedDice => m_usedDice;

    /// <summary>
    /// 捨て札
    /// </summary>
    public IReadOnlyList<string> DiscardPile => m_discardPile;

    /// <summary>
    /// デッキを初期化します
    /// </summary>
    /// <param name="diceIds">初期デッキのダイスIDリスト</param>
    public void Initialize(IEnumerable<string> diceIds)
    {
        m_drawPile.Clear();
        m_hand.Clear();
        m_usedDice.Clear();
        m_discardPile.Clear();

        if (diceIds != null)
        {
            foreach (string diceId in diceIds)
            {
                if (!string.IsNullOrEmpty(diceId))
                {
                    m_drawPile.Add(diceId);
                }
            }
        }

        Shuffle(m_drawPile);
    }

    /// <summary>
    /// 指定数のダイスをドローします
    /// </summary>
    /// <param name="drawCount">ドロー数</param>
    /// <returns>今回ドローしたダイスIDリスト</returns>
    public List<string> Draw(int drawCount)
    {
        int clampedDrawCount = Mathf.Max(0, drawCount);
        List<string> drawnDice = new List<string>();

        for (int i = 0; i < clampedDrawCount; i++)
        {
            if (m_drawPile.Count <= 0)
            {
                RefillDrawPileFromDiscard();
            }

            if (m_drawPile.Count <= 0)
            {
                break;
            }

            string diceId = m_drawPile[0];
            m_drawPile.RemoveAt(0);
            m_hand.Add(diceId);
            drawnDice.Add(diceId);
        }

        return drawnDice;
    }

    /// <summary>
    /// 手札から使用するダイスを確定します
    /// </summary>
    /// <param name="selectedDiceIds">選択したダイスIDリスト</param>
    /// <returns>選択が成功したかどうか</returns>
    public bool TryUseDice(IReadOnlyList<string> selectedDiceIds)
    {
        if (selectedDiceIds == null || selectedDiceIds.Count <= 0)
        {
            return false;
        }

        List<string> handCopy = new List<string>(m_hand);
        for (int i = 0; i < selectedDiceIds.Count; i++)
        {
            string diceId = selectedDiceIds[i];
            if (!handCopy.Remove(diceId))
            {
                return false;
            }
        }

        m_usedDice.Clear();
        for (int i = 0; i < selectedDiceIds.Count; i++)
        {
            string diceId = selectedDiceIds[i];
            m_hand.Remove(diceId);
            m_usedDice.Add(diceId);
        }

        return true;
    }

    /// <summary>
    /// ターン終了時に手札と使用済みダイスを捨て札へ移動します
    /// </summary>
    public void DiscardTurnDice()
    {
        m_discardPile.AddRange(m_usedDice);
        m_discardPile.AddRange(m_hand);
        m_usedDice.Clear();
        m_hand.Clear();
    }

    /// <summary>
    /// 捨て札を山札へ戻してシャッフルします
    /// </summary>
    private void RefillDrawPileFromDiscard()
    {
        if (m_discardPile.Count <= 0)
        {
            return;
        }

        m_drawPile.AddRange(m_discardPile);
        m_discardPile.Clear();
        Shuffle(m_drawPile);
    }

    /// <summary>
    /// リストをシャッフルします
    /// </summary>
    /// <param name="list">シャッフル対象リスト</param>
    private void Shuffle(List<string> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            int randomIndex = UnityEngine.Random.Range(i, list.Count);
            string temporary = list[i];
            list[i] = list[randomIndex];
            list[randomIndex] = temporary;
        }
    }
}
