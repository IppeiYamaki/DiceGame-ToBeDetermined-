using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 手札ダイスをボタンとして表示し、ピックアップ/解除とホバー時6面展開図表示を管理するUIパネル
/// 最大選択数やボタンPrefab、配置先をUnity Editor上で設定できます
/// </summary>
public class DiceHandUIPanel : MonoBehaviour
{
    [SerializeField]
    [Header("ボタン配置先")]
    [Tooltip("生成した手札ダイスボタンを配置するTransform")]
    private Transform m_buttonRoot;

    [SerializeField]
    [Header("ダイスボタンPrefab")]
    [Tooltip("手札ダイス1個分のDiceHandButton Prefab")]
    private DiceHandButton m_diceHandButtonPrefab;

    [SerializeField]
    [Header("6面展開パネル")]
    [Tooltip("ホバー時に表示する6面展開パネル")]
    private DiceFaceExpandPanel m_faceExpandPanel;

    [SerializeField]
    [Header("最大ピックアップ数")]
    [Tooltip("ロールに使用する最大ピックアップ数。通常はPlayerDefinition.UseDiceCountと同期します")]
    private int m_maxPickCount = 3;

    private readonly List<DiceHandButton> m_handButtons = new List<DiceHandButton>();
    private readonly List<string> m_pickedDiceIds = new List<string>();

    /// <summary>
    /// ピックアップ中のダイスIDが変化した時に通知されるイベント
    /// </summary>
    public event Action<IReadOnlyList<string>> OnPickedDiceChanged;

    /// <summary>
    /// ピックアップ中のダイスIDリスト
    /// </summary>
    public IReadOnlyList<string> PickedDiceIds => m_pickedDiceIds;

    /// <summary>
    /// 最大ピックアップ数を設定します
    /// </summary>
    /// <param name="maxPickCount">最大ピックアップ数</param>
    public void SetMaxPickCount(int maxPickCount)
    {
        m_maxPickCount = Mathf.Max(1, maxPickCount);
        TrimPickedDiceIfNeeded();
        RefreshPickedVisuals();
    }

    /// <summary>
    /// 手札ダイス表示を更新します
    /// </summary>
    /// <param name="diceIds">手札ダイスIDリスト</param>
    public void SetHandDice(IReadOnlyList<string> diceIds)
    {
        ClearHandButtons();
        m_pickedDiceIds.Clear();

        if (diceIds == null)
        {
            Debug.LogWarning("[DiceHandUIPanel] diceIds が null のため、手札ダイスを表示できません。", this);
            NotifyPickedChanged();
            return;
        }

        Debug.Log($"[DiceHandUIPanel] 手札ダイス表示開始: {diceIds.Count} 個", this);

        for (int i = 0; i < diceIds.Count; i++)
        {
            CreateDiceButton(diceIds[i]);
        }

        Debug.Log($"[DiceHandUIPanel] 手札ダイス生成完了: {m_handButtons.Count} 個 / ButtonRoot: {(m_buttonRoot != null ? m_buttonRoot.name : "None")}", this);

        NotifyPickedChanged();
    }

    /// <summary>
    /// ピックアップ状態をすべて解除します
    /// </summary>
    public void ClearPickedDice()
    {
        m_pickedDiceIds.Clear();
        RefreshPickedVisuals();
        NotifyPickedChanged();
    }

    /// <summary>
    /// ダイスボタンを生成します
    /// </summary>
    /// <param name="diceId">ダイスID</param>
    private void CreateDiceButton(string diceId)
    {
        if (m_diceHandButtonPrefab == null)
        {
            Debug.LogWarning("[DiceHandUIPanel] DiceHandButton Prefab が設定されていないため、手札ダイスを表示できません。");
            return;
        }

        if (m_buttonRoot == null)
        {
            Debug.LogWarning("[DiceHandUIPanel] Button Root が設定されていないため、手札ダイスを表示できません。");
            return;
        }

        DiceHandButton button = Instantiate(m_diceHandButtonPrefab, m_buttonRoot);
        DiceDefinition diceDefinition = DiceMasterRegistry.Active != null ? DiceMasterRegistry.Active.ResolveDefinition(diceId) : null;
        button.Setup(diceId, diceDefinition);
        button.OnClicked += HandleDiceButtonClicked;
        button.OnHoverEntered += HandleDiceButtonHoverEntered;
        button.OnHoverExited += HandleDiceButtonHoverExited;
        m_handButtons.Add(button);
        Debug.Log($"[DiceHandUIPanel] DiceHandButton生成: {diceId} / Parent: {m_buttonRoot.name} / Active: {button.gameObject.activeInHierarchy}", button);
    }

    /// <summary>
    /// ダイスボタンクリック時の処理
    /// </summary>
    /// <param name="diceHandButton">クリックされたボタン</param>
    private void HandleDiceButtonClicked(DiceHandButton diceHandButton)
    {
        if (diceHandButton == null || string.IsNullOrEmpty(diceHandButton.DiceId))
        {
            return;
        }

        if (m_pickedDiceIds.Contains(diceHandButton.DiceId))
        {
            m_pickedDiceIds.Remove(diceHandButton.DiceId);
        }
        else
        {
            if (m_pickedDiceIds.Count >= m_maxPickCount)
            {
                return;
            }

            m_pickedDiceIds.Add(diceHandButton.DiceId);
        }

        RefreshPickedVisuals();
        NotifyPickedChanged();
    }

    /// <summary>
    /// ダイスボタンホバー開始時の処理
    /// </summary>
    /// <param name="diceHandButton">ホバーされたボタン</param>
    private void HandleDiceButtonHoverEntered(DiceHandButton diceHandButton)
    {
        if (m_faceExpandPanel != null && diceHandButton != null)
        {
            m_faceExpandPanel.Show(diceHandButton.DiceDefinition);
        }
    }

    /// <summary>
    /// ダイスボタンホバー終了時の処理
    /// </summary>
    /// <param name="diceHandButton">ホバー終了したボタン</param>
    private void HandleDiceButtonHoverExited(DiceHandButton diceHandButton)
    {
        if (m_faceExpandPanel != null)
        {
            m_faceExpandPanel.Hide();
        }
    }

    /// <summary>
    /// 生成済みボタンをすべて削除します
    /// </summary>
    private void ClearHandButtons()
    {
        for (int i = 0; i < m_handButtons.Count; i++)
        {
            DiceHandButton button = m_handButtons[i];
            if (button == null)
            {
                continue;
            }

            button.OnClicked -= HandleDiceButtonClicked;
            button.OnHoverEntered -= HandleDiceButtonHoverEntered;
            button.OnHoverExited -= HandleDiceButtonHoverExited;
            Destroy(button.gameObject);
        }

        m_handButtons.Clear();
    }

    /// <summary>
    /// ピックアップ中ダイス数が最大数を超えている場合に切り詰めます
    /// </summary>
    private void TrimPickedDiceIfNeeded()
    {
        while (m_pickedDiceIds.Count > m_maxPickCount)
        {
            m_pickedDiceIds.RemoveAt(m_pickedDiceIds.Count - 1);
        }
    }

    /// <summary>
    /// ピックアップ状態を各ボタンの見た目に反映します
    /// </summary>
    private void RefreshPickedVisuals()
    {
        for (int i = 0; i < m_handButtons.Count; i++)
        {
            DiceHandButton button = m_handButtons[i];
            if (button != null)
            {
                button.SetPicked(m_pickedDiceIds.Contains(button.DiceId));
            }
        }
    }

    /// <summary>
    /// ピックアップ状態変更イベントを通知します
    /// </summary>
    private void NotifyPickedChanged()
    {
        OnPickedDiceChanged?.Invoke(m_pickedDiceIds);
    }

#if UNITY_EDITOR
    /// <summary>
    /// Inspector編集時に設定値を有効範囲へ補正します
    /// </summary>
    private void OnValidate()
    {
        m_maxPickCount = Mathf.Max(1, m_maxPickCount);
    }
#endif
}
