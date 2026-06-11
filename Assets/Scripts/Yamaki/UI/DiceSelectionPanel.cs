using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// ピックアップ済みダイス表示とロール実行ボタンを管理するUIパネル
/// 指定数選択時のみRollButtonを有効化し、ClearButtonで選択を解除できます
/// </summary>
public class DiceSelectionPanel : MonoBehaviour
{
    [SerializeField]
    [Header("選択済みText一覧")]
    [Tooltip("選択済みダイスを表示するTextリスト")]
    private List<Text> m_selectedDiceTexts = new List<Text>();

    [SerializeField]
    [Header("ロールボタン")]
    [Tooltip("指定数選択時に有効化するロール実行ボタン")]
    private Button m_rollButton;

    [SerializeField]
    [Header("クリアボタン")]
    [Tooltip("選択状態を解除するボタン")]
    private Button m_clearButton;

    [SerializeField]
    [Header("必要選択数")]
    [Tooltip("ロールに必要な選択ダイス数。通常はPlayerDefinition.UseDiceCountと同期します")]
    private int m_requiredPickCount = 3;

    [SerializeField]
    [Header("空欄表示")]
    [Tooltip("未選択スロットに表示する文字")]
    private string m_emptySlotText = "-";

    private readonly List<string> m_pickedDiceIds = new List<string>();

    /// <summary>
    /// ロールボタンが押された時に通知されるイベント
    /// </summary>
    public event Action<IReadOnlyList<string>> OnRollRequested;

    /// <summary>
    /// クリアボタンが押された時に通知されるイベント
    /// </summary>
    public event Action OnClearRequested;

    /// <summary>
    /// ボタンイベントを登録します
    /// </summary>
    private void Awake()
    {
        if (m_rollButton != null)
        {
            m_rollButton.onClick.AddListener(HandleRollButtonClicked);
        }
        else
        {
            Debug.LogWarning("[DiceSelectionPanel] Roll Button が設定されていないため、ダイス選択後に進行できません。", this);
        }

        if (m_clearButton != null)
        {
            m_clearButton.onClick.AddListener(HandleClearButtonClicked);
        }
        else
        {
            Debug.LogWarning("[DiceSelectionPanel] Clear Button が設定されていません。", this);
        }

        Refresh();
    }

    /// <summary>
    /// オブジェクト破棄時にボタンイベントを解除します
    /// </summary>
    private void OnDestroy()
    {
        if (m_rollButton != null)
        {
            m_rollButton.onClick.RemoveListener(HandleRollButtonClicked);
        }

        if (m_clearButton != null)
        {
            m_clearButton.onClick.RemoveListener(HandleClearButtonClicked);
        }
    }

    /// <summary>
    /// 必要選択数を設定します
    /// </summary>
    /// <param name="requiredPickCount">必要選択数</param>
    public void SetRequiredPickCount(int requiredPickCount)
    {
        m_requiredPickCount = Mathf.Max(1, requiredPickCount);
        Refresh();
    }

    /// <summary>
    /// 選択済みダイスIDリストを設定します
    /// </summary>
    /// <param name="pickedDiceIds">選択済みダイスIDリスト</param>
    public void SetPickedDice(IReadOnlyList<string> pickedDiceIds)
    {
        m_pickedDiceIds.Clear();
        if (pickedDiceIds != null)
        {
            for (int i = 0; i < pickedDiceIds.Count; i++)
            {
                m_pickedDiceIds.Add(pickedDiceIds[i]);
            }
        }

        Refresh();
    }

    /// <summary>
    /// 選択済み表示をクリアします
    /// </summary>
    public void ClearPickedDice()
    {
        m_pickedDiceIds.Clear();
        Refresh();
    }

    /// <summary>
    /// ロールボタンクリック時の処理
    /// </summary>
    private void HandleRollButtonClicked()
    {
        if (m_pickedDiceIds.Count != m_requiredPickCount)
        {
            return;
        }

        OnRollRequested?.Invoke(m_pickedDiceIds);
    }

    /// <summary>
    /// クリアボタンクリック時の処理
    /// </summary>
    private void HandleClearButtonClicked()
    {
        ClearPickedDice();
        OnClearRequested?.Invoke();
    }

    /// <summary>
    /// UI表示を更新します
    /// </summary>
    private void Refresh()
    {
        for (int i = 0; i < m_selectedDiceTexts.Count; i++)
        {
            Text selectedDiceText = m_selectedDiceTexts[i];
            if (selectedDiceText == null)
            {
                continue;
            }

            selectedDiceText.text = i < m_pickedDiceIds.Count ? m_pickedDiceIds[i] : m_emptySlotText;
        }

        if (m_rollButton != null)
        {
            m_rollButton.interactable = m_pickedDiceIds.Count == m_requiredPickCount;
        }
    }

#if UNITY_EDITOR
    /// <summary>
    /// Inspector編集時に設定値を有効範囲へ補正します
    /// </summary>
    private void OnValidate()
    {
        m_requiredPickCount = Mathf.Max(1, m_requiredPickCount);
    }
#endif
}
