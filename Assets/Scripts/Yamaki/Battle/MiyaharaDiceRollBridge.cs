using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Miyahara の物理ダイス(RandomDice)を BattleScene の RollDice フェーズで駆動し、
/// 出目を集計して ActionPointBattleSceneController に渡すブリッジです。
/// - DiceRole コンポーネントがシーンにある場合は Awake 時に無効化して自動ループを止めます。
/// - DiceCamera(RenderTexture) と RawImage を割り当てると画面上にダイスだけを表示します。
/// </summary>
public class MiyaharaDiceRollBridge : MonoBehaviour
{
    [Header("Controller")]
    [SerializeField]
    private ActionPointBattleSceneController m_controller;

    [Header("Miyahara Dice")]
    [SerializeField]
    private DiceRole m_diceRole; // optional

    [SerializeField]
    private List<RandomDice> m_randomDice = new List<RandomDice>(); // 3 個想定

    [Header("Dice Camera / UI")]
    [SerializeField]
    private Camera m_diceCamera;

    [SerializeField]
    private RawImage m_diceRawImage;

    [Header("Settings")]
    [SerializeField]
    private string m_diceLayerName = "DiceRollFX";

    [SerializeField]
    [Header("結果表示後にダイスを消すまでの時間（秒）")]
    [Min(0f)]
    private float m_resultDisplayDuration = 1.5f;

    [SerializeField]
    [Header("Idle初期化からドロップ開始までの待機（秒）")]
    [Min(0f)]
    private float m_dropDelay = 0.6f;

    private Coroutine m_rollCoroutine;

    private void Awake()
    {
        if (m_diceRole != null)
        {
            // DiceRole の自動再ロールを止める
            m_diceRole.enabled = false;
        }

        if (m_diceCamera != null && m_diceRawImage != null)
        {
            if (m_diceCamera.targetTexture != null)
            {
                m_diceRawImage.texture = m_diceCamera.targetTexture;
            }

            // RawImage は映像専用なのでクリックを遮らない
            m_diceRawImage.raycastTarget = false;
        }

        // ロール中以外はダイス表示を隠す
        SetDiceVisible(false);
    }

    /// <summary>
    /// Canvas 上のダイス表示(RawImage)とダイスカメラの有効/無効を切り替えます。
    /// </summary>
    private void SetDiceVisible(bool visible)
    {
        if (m_diceRawImage != null)
        {
            m_diceRawImage.enabled = visible;
        }

        if (m_diceCamera != null)
        {
            m_diceCamera.enabled = visible;
        }
    }

    private void OnEnable()
    {
        if (m_controller != null)
        {
            m_controller.ExternalRollRequested += OnExternalRollRequested;
        }
    }

    private void OnDisable()
    {
        if (m_controller != null)
        {
            m_controller.ExternalRollRequested -= OnExternalRollRequested;
        }
    }

    private void OnExternalRollRequested(DiceDefinition[] defs)
    {
        // 外部からロール要求が来たら自動で物理ダイスを転がす
        if (m_rollCoroutine != null)
        {
            StopCoroutine(m_rollCoroutine);
        }

        m_rollCoroutine = StartCoroutine(RollAndSubmitCoroutine());
    }

    private IEnumerator RollAndSubmitCoroutine()
    {
        if (m_randomDice == null || m_randomDice.Count == 0)
        {
            Debug.LogWarning("[MiyaharaDiceRollBridge] RandomDice が割り当てられていません。");
            yield break;
        }

        // ダイス表示を出す
        SetDiceVisible(true);

        // 1) スペースキー相当の遷移を自動化: Idle に戻して初期化させる
        //    (Idle 内で位置リセット・isStopped=false・stopCount リセットが行われる)
        foreach (var rd in m_randomDice)
        {
            if (rd == null) continue;
            rd.state = RandomDice.DiceState.Idle;
        }

        // Idle の初期化(1フレーム以上)と落下前の待機
        yield return null;
        yield return new WaitForSeconds(m_dropDelay);

        // 2) 自動でドロップ開始
        foreach (var rd in m_randomDice)
        {
            if (rd == null) continue;
            rd.state = RandomDice.DiceState.Dropping;
        }

        // 3) 全ダイスが停止(state == Stopped)するまで待つ
        while (true)
        {
            bool allStopped = true;
            foreach (var rd in m_randomDice)
            {
                if (rd == null) continue;
                if (rd.state != RandomDice.DiceState.Stopped)
                {
                    allStopped = false;
                    break;
                }
            }

            if (allStopped) break;
            yield return null;
        }

        // collect values
        List<int> values = new List<int>();
        foreach (var rd in m_randomDice)
        {
            if (rd == null) continue;
            values.Add(rd.diceValue);
        }

        int total = 0;
        foreach (var v in values) total += v;

        // role / multiplier calculation: duplicate DiceRole logic
        Role role = GetRole(values);
        int multiplier = GetMultiplier(values);
        int totalPoint = total * multiplier;

        // submit to controller
        if (m_controller != null)
        {
            m_controller.SubmitExternalRollPoint(values.ToArray(), totalPoint, role.ToString());
        }
        else
        {
            Debug.LogWarning("[MiyaharaDiceRollBridge] Controller が未割り当てのため結果を送信できませんでした。");
        }

        // 4) Pt反映後、しばらく出目を表示してから隠す
        yield return new WaitForSeconds(m_resultDisplayDuration);

        // 次のロールに備えてフラグ類をリセット
        foreach (var rd in m_randomDice)
        {
            if (rd == null) continue;
            rd.state = RandomDice.DiceState.NextEvent;
        }

        SetDiceVisible(false);
        m_rollCoroutine = null;
    }

    // 以下は Miyahara/DiceRole と同等のロジックをローカルに実装
    public enum Role
    {
        None,
        Pinzoro,
        Arashi,
        Pair,
        Shigoro,
        Hifumi,
        Even,
        Odd
    }

    Role GetRole(List<int> diceValue)
    {
        if (diceValue == null || diceValue.Count != 3)
            return Role.None;

        List<int> s = new List<int>(diceValue);
        s.Sort();

        bool allSame = s[0] == s[1] && s[1] == s[2];

        if (allSame && s[0] == 1)
            return Role.Pinzoro;

        if (allSame && s[0] != 1)
            return Role.Arashi;

        bool hasPair = (s[0] == s[1]) || (s[1] == s[2]);
        if (hasPair)
            return Role.Pair;

        if (s[0] == 4 && s[1] == 5 && s[2] == 6)
            return Role.Shigoro;

        if (s[0] == 1 && s[1] == 2 && s[2] == 3)
            return Role.Hifumi;

        bool allDifferent = s[0] != s[1] && s[1] != s[2] && s[0] != s[2];
        bool allEven = s[0] % 2 == 0 && s[1] % 2 == 0 && s[2] % 2 == 0;

        if (allDifferent && allEven)
            return Role.Even;

        bool allOdd = s[0] % 2 != 0 && s[1] % 2 != 0 && s[2] % 2 != 0;

        if (allDifferent && allOdd)
            return Role.Odd;

        return Role.None;
    }

    int GetMultiplier(List<int> diceValue)
    {
        switch (GetRole(diceValue))
        {
            case Role.Pinzoro: return (m_diceRole != null) ? m_diceRole.Pinzoro : 10;
            case Role.Arashi: return (m_diceRole != null) ? m_diceRole.Arashii : 5;
            case Role.Pair: return (m_diceRole != null) ? m_diceRole.Pair : 2;
            case Role.Shigoro: return (m_diceRole != null) ? m_diceRole.Shigoro : 3;
            case Role.Hifumi: return (m_diceRole != null) ? m_diceRole.Hifumi : 3;
            case Role.Even: return (m_diceRole != null) ? m_diceRole.Even : 3;
            case Role.Odd: return (m_diceRole != null) ? m_diceRole.Odd : 3;
            default: return 1;
        }
    }
}
