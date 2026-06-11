using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 戦闘中のダイスロール演出を担当するコンポーネント
/// 最前面 Canvas と演出カメラ・ダイスリグ（Miyahara DiceItem ×3）を使用して
/// ダイスを常時表示し、Roll 時のみスピン→確定出目表示の演出を再生します
/// 
/// 使い方:
/// 1. Inspector で演出カメラ・Canvas・RawImage・3つの DiceItem を登録します
/// 2. Play(DiceRollResult result, Action onComplete) を呼ぶと演出が開始されます
/// 3. 演出終了後に onComplete コールバックが呼ばれます
/// 4. 未設定時は演出をスキップして即座にコールバックを呼びます（安全フォールバック）
/// 
/// 注意:
/// - ダイスは常に画面上に表示され、Roll したときのみスピン演出が再生されます
/// - RawImage の raycastTarget と Canvas の GraphicRaycaster を自動で無効化するため、
///   下にあるボタン等のクリック操作を遮断しません
/// </summary>
public class BattleDiceRollAnimator : MonoBehaviour
{
    [SerializeField]
    [Header("演出カメラ")]
    [Tooltip("ダイスを映すための演出専用カメラ（RenderTexture に出力）")]
    private Camera m_rollCamera;

    [SerializeField]
    [Header("最前面 Canvas")]
    [Tooltip("ダイス演出を最前面に表示するための Canvas")]
    private Canvas m_frontCanvas;

    [SerializeField]
    [Header("Canvas の RawImage")]
    [Tooltip("演出カメラの RenderTexture を表示する RawImage")]
    private RawImage m_canvasRawImage;

    [SerializeField]
    [Header("ダイスリグ（3つ）")]
    [Tooltip("Miyahara の DiceItem ×3（スピン演出用）")]
    private List<DiceItem> m_diceItems = new List<DiceItem>();

    [SerializeField]
    [Header("スピン演出時間（秒）")]
    [Tooltip("ダイスが回転する演出の長さ")]
    [Min(0f)]
    private float m_spinDuration = 1.5f;

    [SerializeField]
    [Header("結果表示時間（秒）")]
    [Tooltip("確定出目を表示する時間")]
    [Min(0f)]
    private float m_resultDisplayDuration = 1.0f;

    [SerializeField]
    [Header("スピン中のランダム数字更新間隔（秒）")]
    [Tooltip("スピン中にダイスの数字をランダムに切り替える間隔")]
    [Min(0.01f)]
    private float m_randomUpdateInterval = 0.1f;

    private bool m_isPlaying = false;

    /// <summary>
    /// 起動時にダイス演出を常時表示で初期化し、
    /// RawImage / GraphicRaycaster が下のボタンのクリックを遮断しないようにします
    /// </summary>
    private void Awake()
    {
        // RawImage は映像表示専用のため、下にあるボタン等のクリックを遮断しない
        if (m_canvasRawImage != null)
        {
            m_canvasRawImage.raycastTarget = false;
        }

        // 最前面 Canvas の GraphicRaycaster を無効化し、Canvas 全体がクリックを横取りしないようにする
        if (m_frontCanvas != null)
        {
            GraphicRaycaster raycaster = m_frontCanvas.GetComponent<GraphicRaycaster>();
            if (raycaster != null)
            {
                raycaster.enabled = false;
            }
        }

        // ダイスは常に画面上に表示する（Roll 時のみスピン演出が再生される）
        SetAnimationVisible(true);
    }

    /// <summary>
    /// ダイスロール演出を再生します
    /// </summary>
    /// <param name="result">表示するダイスロール結果</param>
    /// <param name="onComplete">演出終了後に呼ばれるコールバック</param>
    public void Play(DiceRollResult result, Action onComplete = null)
    {
        if (m_isPlaying)
        {
            Debug.LogWarning("[BattleDiceRollAnimator] 演出がすでに再生中です。");
            return;
        }

        // 必須コンポーネントの検証（演出スキップのフォールバック）
        if (m_rollCamera == null || m_frontCanvas == null || m_canvasRawImage == null || m_diceItems.Count < 3)
        {
            Debug.LogWarning("[BattleDiceRollAnimator] 演出に必要なコンポーネントが不足しています。演出をスキップします。");
            onComplete?.Invoke();
            return;
        }

        // DiceRollResult の検証（RollData が 3 個未満なら演出スキップ）
        if (result.RollData == null || result.RollData.Length < 3)
        {
            Debug.LogWarning("[BattleDiceRollAnimator] DiceRollResult が不正です。演出をスキップします。");
            onComplete?.Invoke();
            return;
        }

        StartCoroutine(PlayRollAnimation(result, onComplete));
    }

    /// <summary>
    /// ダイスロール演出のコルーチン
    /// </summary>
    /// <param name="result">表示するダイスロール結果</param>
    /// <param name="onComplete">演出終了後のコールバック</param>
    /// <returns>コルーチン</returns>
    private IEnumerator PlayRollAnimation(DiceRollResult result, Action onComplete)
    {
        m_isPlaying = true;

        // 1. スピン演出開始（ダイスは常時表示のためそのままスピンする）
        float elapsed = 0f;
        while (elapsed < m_spinDuration)
        {
            // ダイスをランダム数字で更新しながら回転
            for (int i = 0; i < 3 && i < m_diceItems.Count; i++)
            {
                DiceItem diceItem = m_diceItems[i];
                if (diceItem != null && diceItem.numberText != null)
                {
                    // スピン中はランダムな数字を表示
                    int randomNumber = UnityEngine.Random.Range(1, 7);
                    diceItem.numberText.text = randomNumber.ToString();

                    // 回転（rollSpinSpeed を参照）
                    diceItem.transform.Rotate(Vector3.up * diceItem.rollSpinSpeed * Time.deltaTime);
                }
            }

            elapsed += m_randomUpdateInterval;
            yield return new WaitForSeconds(m_randomUpdateInterval);
        }

        // 2. 確定出目を表示
        for (int i = 0; i < 3 && i < m_diceItems.Count && i < result.RollData.Length; i++)
        {
            DiceItem diceItem = m_diceItems[i];
            int finalNumber = result.RollData[i].Number;

            if (diceItem != null && diceItem.numberText != null)
            {
                diceItem.numberText.text = finalNumber.ToString();
            }
        }

        // 3. 結果表示時間待機（確定出目はそのまま画面に残る）
        yield return new WaitForSeconds(m_resultDisplayDuration);

        m_isPlaying = false;

        // 4. コールバック呼び出し
        onComplete?.Invoke();
    }

    /// <summary>
    /// 演出一式（カメラ・Canvas・RawImage・ダイス）の表示/非表示を切り替えます
    /// 通常は Awake() からの常時表示初期化にのみ使用します
    /// </summary>
    /// <param name="visible">true で表示、false で非表示</param>
    private void SetAnimationVisible(bool visible)
    {
        if (m_rollCamera != null)
        {
            m_rollCamera.gameObject.SetActive(visible);
        }

        if (m_frontCanvas != null)
        {
            m_frontCanvas.gameObject.SetActive(visible);
        }

        if (m_canvasRawImage != null)
        {
            m_canvasRawImage.gameObject.SetActive(visible);
        }

        // ダイスアイテムの表示切り替え
        foreach (DiceItem diceItem in m_diceItems)
        {
            if (diceItem != null)
            {
                diceItem.gameObject.SetActive(visible);
            }
        }
    }

    /// <summary>
    /// 演出が再生中かどうかを取得します
    /// </summary>
    public bool IsPlaying => m_isPlaying;
}
