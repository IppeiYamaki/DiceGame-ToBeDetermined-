using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// BattleScene全体を制御するController
/// 6個のダイスから3個を選択し、既存のDiceRollCalculatorでロール結果を戦闘へ反映します
/// HP帯別敵行動システム、ブロック値、ダイスロール演出、エフェクト/サウンドに対応しています
/// </summary>
public class BattleSceneController : MonoBehaviour
{
    [SerializeField]
    [Header("ダイスマスタ")]
    private DiceMasterRegistry m_diceMasterRegistry;

    [SerializeField]
    [Header("バフ/デバフマスタ（任意）")]
    private StatusEffectRegistry m_statusEffectRegistry;

    [SerializeField]
    [Header("プレイヤー所持ダイス（6個）")]
    private List<DiceDefinition> m_playerDiceDefinitions = new List<DiceDefinition>();

    [SerializeField]
    [Header("敵データ")]
    private EnemyDefinition m_enemyDefinition;

    [SerializeField]
    [Header("プレイヤー最大HP")]
    [Min(1)]
    private int m_playerMaxHp = 100;

    [SerializeField]
    [Header("ダイス選択View")]
    private DiceSelectionView m_diceSelectionView;

    [SerializeField]
    [Header("Player HP表示")]
    private HpView m_playerHpView;

    [SerializeField]
    [Header("Player Block表示")]
    [Tooltip("プレイヤーのBlock値を表示するView（任意）")]
    private BlockView m_playerBlockView;

    [SerializeField]
    [Header("Playerバフ/デバフ表示")]
    private IconListView m_playerStatusIconListView;

    [SerializeField]
    [Header("Player初期バフ/デバフ（表示用）")]
    private List<StatusEffectDefinition> m_playerInitialStatusEffects = new List<StatusEffectDefinition>();

    [SerializeField]
    [Header("アイコン設定")]
    private BattleIconSettings m_iconSettings;

    [SerializeField]
    [Header("Enemy表示")]
    private EnemyView m_enemyView;

    [SerializeField]
    [Header("サウンド再生")]
    [Tooltip("戦闘中のサウンド再生を担当するコンポーネント（任意）")]
    private BattleAudioPlayer m_audioPlayer;

    [SerializeField]
    [Header("エフェクト再生")]
    [Tooltip("戦闘中のエフェクト再生を担当するコンポーネント（任意）")]
    private BattleEffectPlayer m_effectPlayer;


    [SerializeField]
    [Header("ターン数表示")]
    private TMP_Text m_turnText;

    [SerializeField]
    [Header("フェーズ表示（任意）")]
    private TMP_Text m_phaseText;

    [SerializeField]
    [Header("ロール結果表示（任意）")]
    private TMP_Text m_rollResultText;

    [SerializeField]
    [Header("戦闘ログ（任意）")]
    private TMP_Text m_battleLogText;

    [SerializeField]
    [Header("ボタン")]
    private Button m_clearButton;

    [SerializeField]
    private Button m_rollButton;

    [SerializeField]
    private Button m_mapButton;

    [SerializeField]
    private Button m_optionButton;

    [SerializeField]
    private Button m_deckButton;

    [SerializeField]
    private Button m_discardButton;

    [SerializeField]
    [Header("敵行動の間隔（秒）")]
    [Tooltip("敵が複数行動を行う際の行動間の待機時間")]
    [Min(0f)]
    private float m_enemyActionInterval = 0.6f;

    [SerializeField]
    [Header("ダイスロールSE")]
    [Tooltip("ダイスをロールした時に再生する効果音（任意）")]
    private AudioClip m_diceRollClip;

    [SerializeField]
    [Header("ダイスロールSE音量（0～1）")]
    [Range(0f, 1f)]
    private float m_diceRollVolume = 1.0f;

    [SerializeField]
    [Header("Player攻撃SE")]
    [Tooltip("プレイヤーの攻撃が発生した時に再生する効果音（任意）")]
    private AudioClip m_playerAttackClip;

    [SerializeField]
    [Header("Player防御SE")]
    [Tooltip("プレイヤーがブロックを獲得した時に再生する効果音（任意）")]
    private AudioClip m_playerDefenseClip;

    [SerializeField]
    [Header("Player行動SE音量（0～1）")]
    [Range(0f, 1f)]
    private float m_playerActionVolume = 1.0f;

    private BattlePhase m_currentPhase = BattlePhase.None;
    private int m_currentTurn = 1;
    private int m_playerHp;
    private int m_enemyHp;
    private int m_playerBlock; // プレイヤーのBlock値
    private int m_enemyBlock;  // 敵のBlock値

    // 敵の次行動（ターン開始時に事前抽選）
    private EnemyWeightedActionGroup m_nextEnemyActionGroup;

    // ランタイムバフ/デバフリスト（ユニット毎）
    private List<ActiveStatusEffect> m_playerActiveStatusEffects = new List<ActiveStatusEffect>();
    private List<ActiveStatusEffect> m_enemyActiveStatusEffects = new List<ActiveStatusEffect>();

    private void Awake()
    {
        RegisterButtonEvents();

        if (m_diceSelectionView != null)
        {
            m_diceSelectionView.SelectionChanged += RefreshButtonStates;
        }
    }

    private void Start()
    {
        StartBattle();
    }

    private void OnDestroy()
    {
        if (m_diceSelectionView != null)
        {
            m_diceSelectionView.SelectionChanged -= RefreshButtonStates;
        }

        UnregisterButtonEvents();
    }

    /// <summary>
    /// 戦闘を初期化して開始します
    /// </summary>
    public void StartBattle()
    {
        if (m_diceMasterRegistry != null)
        {
            DiceMasterRegistry.SetActive(m_diceMasterRegistry);
        }

        if (m_statusEffectRegistry != null)
        {
            StatusEffectRegistry.SetActive(m_statusEffectRegistry);
        }

        m_currentTurn = 1;
        m_playerHp = m_playerMaxHp;
        m_enemyHp = m_enemyDefinition != null ? m_enemyDefinition.MaxHp : 50;
        m_playerBlock = 0;
        m_enemyBlock = 0;

        m_playerActiveStatusEffects.Clear();
        m_enemyActiveStatusEffects.Clear();

        UpdateTurnText();
        UpdateStatusViews();
        SetRollResultText("");
        AppendLog("戦闘開始");
        BeginPlayerSelectPhase();
    }

    /// <summary>
    /// 3個選択されている場合のみロールを実行します
    /// Rollボタンから呼び出してください
    /// </summary>
    public void RollSelectedDice()
    {
        if (m_diceSelectionView == null || !m_diceSelectionView.TryGetRequiredSelectedDice(out DiceDefinition[] selectedDice))
        {
            AppendLog("ダイスを3個選択してください");
            RefreshButtonStates();
            return;
        }

        RollSelectedDice(selectedDice);
    }

    /// <summary>
    /// 選択中のダイスをすべて解除します
    /// Clearボタンから呼び出してください
    /// </summary>
    public void ClearSelectedDice()
    {
        if (m_diceSelectionView != null)
        {
            m_diceSelectionView.ClearSelection();
        }

        RefreshButtonStates();
    }

    /// <summary>
    /// マップ確認ボタン用の拡張口
    /// 現時点では未実装です
    /// </summary>
    public void OpenMap()
    {
        AppendLog("マップ確認は未実装です");
    }

    /// <summary>
    /// オプションボタン用の拡張口
    /// 現時点では未実装です
    /// </summary>
    public void OpenOptions()
    {
        AppendLog("オプションは未実装です");
    }

    /// <summary>
    /// 山札確認ボタン用の拡張口
    /// 現時点では未実装です
    /// </summary>
    public void OpenDeck()
    {
        AppendLog("山札確認は未実装です");
    }

    /// <summary>
    /// 捨て札確認ボタン用の拡張口
    /// 現時点では未実装です
    /// </summary>
    public void OpenDiscardPile()
    {
        AppendLog("捨て札確認は未実装です");
    }

    private void BeginPlayerSelectPhase()
    {
        if (IsBattleFinished()) return;

        SetPhase(BattlePhase.PlayerSelect);

        // ターン開始時に敵の次行動を事前抽選
        m_nextEnemyActionGroup = EnemyActionSelector.SelectActionGroup(m_enemyDefinition, m_enemyHp);
        UpdateEnemyIntentIcons();

        AppendLog("6個のダイスから3個を選択してください");

        if (m_diceSelectionView == null)
        {
            Debug.LogError("[BattleSceneController] DiceSelectionView が設定されていません。");
            return;
        }

        m_diceSelectionView.BeginSelection(m_playerDiceDefinitions, OnDiceSelectionConfirmed);
        RefreshButtonStates();
    }

    private void OnDiceSelectionConfirmed(DiceDefinition[] selectedDice)
    {
        if (selectedDice == null || selectedDice.Length != 3 || selectedDice.Any(dice => dice == null))
        {
            AppendLog("ダイスは3個選択してください");
            BeginPlayerSelectPhase();
            return;
        }

        RollSelectedDice(selectedDice);
    }

    private void RollSelectedDice(DiceDefinition[] selectedDice)
    {
        SetPhase(BattlePhase.PlayerRoll);
        AppendLog("ダイスロール");
        RefreshButtonStates();

        // ダイスロールSEを再生
        if (m_audioPlayer != null && m_diceRollClip != null)
        {
            m_audioPlayer.Play(m_diceRollClip, m_diceRollVolume);
        }

        if (m_diceSelectionView != null)
        {
            m_diceSelectionView.LockCurrentSelectionForRoll();
        }

        if (DiceMasterRegistry.Active == null)
        {
            Debug.LogError("[BattleSceneController] DiceMasterRegistry.Active が設定されていません。");
            return;
        }

        string[] diceIds = selectedDice.Select(dice => dice.PersistentId).ToArray();
        List<DiceRoleDefinition> roles = DiceMasterRegistry.Active.AllRoleDefinitions.ToList();
        DiceRollResult rollResult = DiceRollCalculator.Roll(diceIds, roles);


    }

    private void OnRollAnimationComplete(DiceRollResult rollResult)
    {
        // 演出完了後にダイス選択Viewに結果を表示
        if (m_diceSelectionView != null)
        {
            m_diceSelectionView.ShowRollNumbers(rollResult);
        }

        // ロール結果を適用
        ApplyPlayerResult(rollResult);
    }

    private void ApplyPlayerResult(DiceRollResult rollResult)
    {
        SetPhase(BattlePhase.PlayerResult);

        int baseAttack = rollResult.FinalValue;
        int baseDefense = 0;

        // バフ/デバフによる攻撃補正を適用
        int finalAttack = Mathf.Max(0, baseAttack + GetAttackModifier(m_playerActiveStatusEffects));

        // 敵ブロックを貫通してダメージを与える
        int blockedAmount = Mathf.Min(m_enemyBlock, finalAttack);
        int damageToEnemy = Mathf.Max(0, finalAttack - blockedAmount);
        m_enemyBlock = Mathf.Max(0, m_enemyBlock - blockedAmount);
        m_enemyHp = Mathf.Max(0, m_enemyHp - damageToEnemy);

        // プレイヤーの防御値は自身のブロックへ加算
        m_playerBlock += baseDefense;

        // プレイヤー行動SEを再生
        PlayPlayerActionSounds(finalAttack, baseDefense);

        SetRollResultText(CreateRollResultText(rollResult, finalAttack, baseDefense));
        AppendLog($"プレイヤーの攻撃: {finalAttack}（Block {blockedAmount} 軽減 → {damageToEnemy} ダメージ） / Block +{baseDefense}");
        UpdateStatusViews();
        CheckStatusAfterPlayerAction();
    }

    /// <summary>
    /// プレイヤーの行動結果に応じてSEを再生します
    /// 攻撃値があれば攻撃SE、防御値があれば防御SEを再生します（両方ある場合は両方再生）
    /// </summary>
    /// <param name="attackValue">攻撃の最終値</param>
    /// <param name="defenseValue">防御（ブロック獲得）値</param>
    private void PlayPlayerActionSounds(int attackValue, int defenseValue)
    {
        if (m_audioPlayer == null) return;

        if (attackValue > 0 && m_playerAttackClip != null)
        {
            m_audioPlayer.Play(m_playerAttackClip, m_playerActionVolume);
        }

        if (defenseValue > 0 && m_playerDefenseClip != null)
        {
            m_audioPlayer.Play(m_playerDefenseClip, m_playerActionVolume);
        }
    }

    private void CheckStatusAfterPlayerAction()
    {
        SetPhase(BattlePhase.CheckStatus);

        if (m_enemyHp <= 0)
        {
            SetPhase(BattlePhase.Win);
            AppendLog("勝利");
            RefreshButtonStates();
            return;
        }

        StartCoroutine(ExecuteEnemyActionCoroutine());
    }

    private IEnumerator ExecuteEnemyActionCoroutine()
    {
        SetPhase(BattlePhase.EnemyAction);

        EnemyWeightedActionGroup actionGroup = m_nextEnemyActionGroup;
        if (actionGroup == null || actionGroup.Actions == null || actionGroup.Actions.Count == 0)
        {
            // 行動未設定時は基本攻撃力でフォールバック
            int fallbackPower = m_enemyDefinition != null ? m_enemyDefinition.AttackPower : 0;
            ApplyEnemyAttack(fallbackPower);
            AppendLog($"敵の行動: 攻撃 {fallbackPower}");
            UpdateStatusViews();
            yield return new WaitForSeconds(m_enemyActionInterval);
            CheckStatusAfterEnemyAction();
            yield break;
        }

        foreach (EnemyActionEntry entry in actionGroup.Actions)
        {
            if (entry == null) continue;

            // 行動開始時サウンド
            if (entry.UseSound && entry.SoundTiming == SoundTimingType.ActionStart && m_audioPlayer != null)
            {
                m_audioPlayer.Play(entry.AudioClip, entry.Volume);
            }

            // エフェクト再生
            if (entry.UseEffect && m_effectPlayer != null)
            {
                m_effectPlayer.PlayEffect(entry.EffectPrefab, entry.EffectAnchor, entry.EffectDuration);
            }

            // 種別別の効果適用
            ApplyEnemyActionEntry(entry);

            // 行動時画像（行動Texture）を表示
            if (m_enemyView != null)
            {
                m_enemyView.ShowActionVisual(m_enemyActionInterval);
            }

            // 適用時サウンド
            if (entry.UseSound && entry.SoundTiming == SoundTimingType.Hit && m_audioPlayer != null)
            {
                m_audioPlayer.Play(entry.AudioClip, entry.Volume);
            }

            UpdateStatusViews();

            if (m_playerHp <= 0) break;

            yield return new WaitForSeconds(m_enemyActionInterval);
        }

        CheckStatusAfterEnemyAction();
    }

    private void ApplyEnemyActionEntry(EnemyActionEntry entry)
    {
        switch (entry.ActionType)
        {
            case BattleActionType.Attack:
                int finalAttack = Mathf.Max(0, entry.Power + GetAttackModifier(m_enemyActiveStatusEffects));
                ApplyEnemyAttack(finalAttack);
                break;

            case BattleActionType.Defense:
                m_enemyBlock += entry.Power;
                AppendLog($"敵の行動: 防御 Block +{entry.Power}");
                break;

            case BattleActionType.Buff:
                if (entry.StatusEffect != null)
                {
                    m_enemyActiveStatusEffects.Add(new ActiveStatusEffect(entry.StatusEffect));
                    AppendLog($"敵の行動: バフ「{entry.StatusEffect.EffectName}」を自身に付与");
                }
                break;

            case BattleActionType.Debuff:
                if (entry.StatusEffect != null)
                {
                    m_playerActiveStatusEffects.Add(new ActiveStatusEffect(entry.StatusEffect));
                    AppendLog($"敵の行動: デバフ「{entry.StatusEffect.EffectName}」をプレイヤーに付与");
                }
                break;
        }
    }

    private void ApplyEnemyAttack(int attackPower)
    {
        // プレイヤーブロックを貫通してダメージを与える
        int blockedAmount = Mathf.Min(m_playerBlock, attackPower);
        int damage = Mathf.Max(0, attackPower - blockedAmount);
        m_playerBlock = Mathf.Max(0, m_playerBlock - blockedAmount);
        m_playerHp = Mathf.Max(0, m_playerHp - damage);

        AppendLog($"敵の行動: 攻撃 {attackPower}（Block {blockedAmount} 軽減 → {damage} ダメージ）");
    }

    private void CheckStatusAfterEnemyAction()
    {
        SetPhase(BattlePhase.CheckStatus);

        if (m_playerHp <= 0)
        {
            SetPhase(BattlePhase.GameOver);
            AppendLog("ゲームオーバー");
            RefreshButtonStates();
            return;
        }

        // ターン終了処理: バフ/デバフの持続ターン減算
        TickStatusEffects(m_playerActiveStatusEffects);
        TickStatusEffects(m_enemyActiveStatusEffects);

        m_currentTurn++;
        UpdateTurnText();
        UpdateStatusViews();
        BeginPlayerSelectPhase();
    }

    /// <summary>
    /// バフ/デバフの残りターンを減算し、切れた効果を除去します
    /// </summary>
    private void TickStatusEffects(List<ActiveStatusEffect> statusEffects)
    {
        for (int i = statusEffects.Count - 1; i >= 0; i--)
        {
            ActiveStatusEffect effect = statusEffects[i];
            if (effect == null || effect.Definition == null)
            {
                statusEffects.RemoveAt(i);
                continue;
            }

            if (effect.IsPermanent) continue;

            effect.RemainingTurns--;
            if (effect.RemainingTurns <= 0)
            {
                AppendLog($"効果「{effect.Definition.EffectName}」が切れた");
                statusEffects.RemoveAt(i);
            }
        }
    }

    /// <summary>
    /// バフ/デバフによる攻撃力補正の合計を取得します
    /// </summary>
    private int GetAttackModifier(List<ActiveStatusEffect> statusEffects)
    {
        int modifier = 0;
        foreach (ActiveStatusEffect effect in statusEffects)
        {
            if (effect == null || effect.Definition == null) continue;
            modifier += effect.Definition.EffectValue;
        }

        return modifier;
    }

    private string CreateRollResultText(DiceRollResult rollResult, int attackValue, int defenseValue)
    {
        string numbers = rollResult.RollData != null
            ? string.Join(", ", rollResult.RollData.Select(data => data.Number.ToString()))
            : "-";
        string roleName = rollResult.EvaluateResult.IsRoleMatched ? rollResult.EvaluateResult.Role.RoleName : "なし";

        return $"出目: {numbers}\n合計: {rollResult.TotalNumber}\n役: {roleName} x{rollResult.EvaluateResult.Multiplier:0.##}\nAttack: {attackValue}";
    }

    private void SetPhase(BattlePhase phase)
    {
        m_currentPhase = phase;
        if (m_phaseText != null)
        {
            m_phaseText.text = $"Phase: {m_currentPhase}";
        }

        RefreshButtonStates();
    }

    private void UpdateStatusViews()
    {
        if (m_playerHpView != null)
        {
            m_playerHpView.UpdateHp(m_playerHp, m_playerMaxHp);
        }

        if (m_playerBlockView != null)
        {
            m_playerBlockView.UpdateBlock(m_playerBlock);
        }

        UpdatePlayerStatusIcons();

        if (m_enemyView != null)
        {
            m_enemyView.UpdateEnemy(m_enemyDefinition, m_enemyHp);
            m_enemyView.UpdateBlock(m_enemyBlock);
            m_enemyView.UpdateStatusIcons(CreateStatusEffectSprites(m_enemyActiveStatusEffects));
        }
    }

    /// <summary>
    /// 事前抽選した敵の次行動から予測アイコンを更新します
    /// </summary>
    private void UpdateEnemyIntentIcons()
    {
        if (m_enemyView == null) return;

        List<IconDisplayData> displays = new List<IconDisplayData>();

        if (m_nextEnemyActionGroup == null || m_nextEnemyActionGroup.Actions == null || m_nextEnemyActionGroup.Actions.Count == 0)
        {
            // 行動未設定時はフォールバックとして攻撃アイコンを表示
            int fallbackPower = m_enemyDefinition != null ? m_enemyDefinition.AttackPower : 0;
            int predictedAttack = Mathf.Max(0, fallbackPower + GetAttackModifier(m_enemyActiveStatusEffects));
            Sprite fallbackSprite = m_iconSettings != null ? m_iconSettings.GetIconForActionType(BattleActionType.Attack) : null;
            if (fallbackSprite != null)
            {
                displays.Add(new IconDisplayData(fallbackSprite, predictedAttack.ToString()));
            }

            m_enemyView.UpdateIntentIconDisplays(displays);
            return;
        }

        foreach (EnemyActionEntry entry in m_nextEnemyActionGroup.Actions)
        {
            if (entry == null) continue;

            Sprite sprite = m_iconSettings != null ? m_iconSettings.GetIconForActionType(entry.ActionType) : null;
            if (sprite == null) continue;

            displays.Add(new IconDisplayData(sprite, GetPredictedEnemyActionValueText(entry)));
        }

        m_enemyView.UpdateIntentIconDisplays(displays);
    }

    private string GetPredictedEnemyActionValueText(EnemyActionEntry entry)
    {
        if (entry == null) return "";

        switch (entry.ActionType)
        {
            case BattleActionType.Attack:
                return Mathf.Max(0, entry.Power + GetAttackModifier(m_enemyActiveStatusEffects)).ToString();

            case BattleActionType.Defense:
                return entry.Power.ToString();

            case BattleActionType.Buff:
            case BattleActionType.Debuff:
                return entry.StatusEffect != null ? entry.StatusEffect.EffectValue.ToString() : "0";

            case BattleActionType.Unknown:
            default:
                return "";
        }
    }

    /// <summary>
    /// ランタイムバフ/デバフリストからアイコンSpriteリストを作成します
    /// </summary>
    private List<Sprite> CreateStatusEffectSprites(List<ActiveStatusEffect> statusEffects)
    {
        List<Sprite> sprites = new List<Sprite>();
        if (m_iconSettings == null) return sprites;

        foreach (ActiveStatusEffect effect in statusEffects)
        {
            if (effect == null || effect.Definition == null) continue;

            Sprite sprite = m_iconSettings.GetIconForStatusEffect(effect.Definition);
            if (sprite != null)
            {
                sprites.Add(sprite);
            }
        }

        return sprites;
    }

    private void UpdatePlayerStatusIcons()
    {
        if (m_playerStatusIconListView == null) return;

        List<Sprite> sprites = new List<Sprite>();
        if (m_iconSettings != null)
        {
            // 初期設定のバフ/デバフ（表示用）
            foreach (StatusEffectDefinition effectDefinition in m_playerInitialStatusEffects)
            {
                Sprite sprite = m_iconSettings.GetIconForStatusEffect(effectDefinition);
                if (sprite != null)
                {
                    sprites.Add(sprite);
                }
            }

            // ランタイムで付与されたバフ/デバフ
            sprites.AddRange(CreateStatusEffectSprites(m_playerActiveStatusEffects));
        }

        m_playerStatusIconListView.UpdateIcons(sprites);
    }

    private void UpdateTurnText()
    {
        if (m_turnText != null)
        {
            m_turnText.text = $"Turn {m_currentTurn}";
        }
    }

    private void SetRollResultText(string message)
    {
        if (m_rollResultText != null)
        {
            m_rollResultText.text = message;
        }
    }

    private void AppendLog(string message)
    {
        Debug.Log($"[BattleScene] {message}");

        if (m_battleLogText == null) return;

        if (string.IsNullOrEmpty(m_battleLogText.text))
        {
            m_battleLogText.text = message;
        }
        else
        {
            m_battleLogText.text = $"{m_battleLogText.text}\n{message}";
        }
    }

    private bool IsBattleFinished()
    {
        return m_currentPhase == BattlePhase.Win || m_currentPhase == BattlePhase.GameOver;
    }

    private void RegisterButtonEvents()
    {
        if (m_clearButton != null) m_clearButton.onClick.AddListener(ClearSelectedDice);
        if (m_rollButton != null) m_rollButton.onClick.AddListener(RollSelectedDice);
        if (m_mapButton != null) m_mapButton.onClick.AddListener(OpenMap);
        if (m_optionButton != null) m_optionButton.onClick.AddListener(OpenOptions);
        if (m_deckButton != null) m_deckButton.onClick.AddListener(OpenDeck);
        if (m_discardButton != null) m_discardButton.onClick.AddListener(OpenDiscardPile);
    }

    private void UnregisterButtonEvents()
    {
        if (m_clearButton != null) m_clearButton.onClick.RemoveListener(ClearSelectedDice);
        if (m_rollButton != null) m_rollButton.onClick.RemoveListener(RollSelectedDice);
        if (m_mapButton != null) m_mapButton.onClick.RemoveListener(OpenMap);
        if (m_optionButton != null) m_optionButton.onClick.RemoveListener(OpenOptions);
        if (m_deckButton != null) m_deckButton.onClick.RemoveListener(OpenDeck);
        if (m_discardButton != null) m_discardButton.onClick.RemoveListener(OpenDiscardPile);
    }

    private void RefreshButtonStates()
    {
        bool canSelect = m_currentPhase == BattlePhase.PlayerSelect && !IsBattleFinished();

        if (m_clearButton != null)
        {
            m_clearButton.interactable = canSelect && m_diceSelectionView != null && m_diceSelectionView.SelectedCount > 0;
        }

        if (m_rollButton != null)
        {
            m_rollButton.interactable = canSelect && m_diceSelectionView != null && m_diceSelectionView.CanConfirmSelection;
        }
    }
}
