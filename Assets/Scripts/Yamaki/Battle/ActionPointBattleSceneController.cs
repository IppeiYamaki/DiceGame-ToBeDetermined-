using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 新ルール用のActionPoint戦闘Controller。
/// ターン開始時に所持ダイスからランダムで3個を振り、獲得APを攻撃・防御・スキル・アイテムに消費します。
/// </summary>
public class ActionPointBattleSceneController : MonoBehaviour
{
    private const int RollDiceCount = 3;

    [SerializeField]
    [Header("System Manager")]
    private RunSystemManager m_runSystemManager;

    [SerializeField]
    [Header("Player Manager")]
    private RunPlayerManager m_runPlayerManager;

    [SerializeField]
    [Header("敵データ")]
    private EnemyDefinition m_enemyDefinition;

    [SerializeField]
    [Header("敵データ（複数対応）")]
    private List<EnemyDefinition> m_enemyDefinitions;

    [SerializeField]
    [Header("EnemyAreaView（動的生成用）")]
    private EnemyAreaView m_enemyAreaView;

    [SerializeField]
    [Header("行動予約レーンView")]
    private ActionReservationLaneView m_reservationLaneView;

    [SerializeField]
    [Header("アイテム一覧View")]
    private ItemListView m_itemListView;

    [SerializeField]
    [Header("ダイス役一覧View")]
    private DiceRoleListView m_diceRoleListView;

    [SerializeField]
    [Header("プレイヤー状態アイコンView（任意）")]
    private IconListView m_playerStatusIconListView;

    [SerializeField]
    [Header("敵行動の間隔（秒）")]
    [Min(0f)]
    private float m_enemyActionInterval = 0.6f;

    [SerializeField]
    [Header("外部ロール演出を待つ")]
    private bool m_waitExternalRollResult = false;

    [SerializeField]
    [Header("Player HP表示")]
    private HpView m_playerHpView;

    [SerializeField]
    [Header("Player Block表示")]
    private BlockView m_playerBlockView;

    [SerializeField]
    [Header("Enemy表示")]
    private EnemyView m_enemyView;

    [SerializeField]
    [Header("ActionPoint表示")]
    private ActionPointView m_actionPointView;

    [SerializeField]
    [Header("攻撃コマンド")]
    private AdjustableCostCommandView m_attackCommandView;

    [SerializeField]
    [Header("防御コマンド")]
    private AdjustableCostCommandView m_defenseCommandView;

    [SerializeField]
    [Header("スキル一覧")]
    private SkillListView m_skillListView;

    [SerializeField]
    [Header("ターン終了ボタン")]
    private Button m_endTurnButton;

    [SerializeField]
    [Header("ターン表示")]
    private TMP_Text m_turnText;

    [SerializeField]
    [Header("フェーズ表示")]
    private TMP_Text m_phaseText;

    [SerializeField]
    [Header("ロール結果表示")]
    private TMP_Text m_rollResultText;

    [SerializeField]
    [Header("戦闘ログ")]
    private TMP_Text m_battleLogText;

    public event Action<DiceDefinition[]> ExternalRollRequested;

    private BattlePhase m_currentPhase = BattlePhase.None;
    private int m_currentTurn = 1;
    private int m_currentActionPoint;
    private int m_gainedActionPoint;
    private int m_playerBlock;
    private int m_pendingDefensePoint;
    private int m_enemyHp;
    private int m_enemyBlock;
    private DiceDefinition[] m_currentRolledDice = Array.Empty<DiceDefinition>();
    private DiceRollResult m_latestRollResult;
    private EnemyWeightedActionGroup m_nextEnemyActionGroup;
    private Coroutine m_enemyActionCoroutine;

    // 複数敵ランタイム
    private readonly List<EnemyUnitRuntime> m_enemyRuntimes = new List<EnemyUnitRuntime>();

    // 予約済み行動キュー
    private readonly List<PlannedAction> m_reservations = new List<PlannedAction>();

    // 現在選択中の敵インデックス（-1 = 未選択）
    private int m_currentTargetIndex = -1;

    // 1戦镘1回制限スキルの使用済み記録（戦闘開始時にクリア）
    private readonly HashSet<SkillDefinition> m_usedOncePerBattleSkills = new HashSet<SkillDefinition>();

    // プレイヤーに付与中のバフ/デバフ
    private readonly List<ActiveStatusEffect> m_playerStatusEffects = new List<ActiveStatusEffect>();

    [SerializeField]
    [Header("SE再生用 AudioSource(任意)")]
    private AudioSource m_audioSource;

    /// <summary>現在のActionPoint。</summary>
    public int CurrentActionPoint => m_currentActionPoint;

    /// <summary>現在ターンのロールで獲得したActionPoint。</summary>
    public int GainedActionPoint => m_gainedActionPoint;

    /// <summary>今回ロール対象になったダイス。</summary>
    public IReadOnlyList<DiceDefinition> CurrentRolledDice => m_currentRolledDice;

    private void Awake()
    {
        RegisterEvents();
    }

    private void Start()
    {
        StartBattle();
    }

    private void OnDestroy()
    {
        UnregisterEvents();
    }

    /// <summary>
    /// 戦闘を初期化して開始します。
    /// </summary>
    public void StartBattle()
    {
        ResolveManagers();
        m_runSystemManager?.ApplyActiveRegistries();

        if (m_runPlayerManager == null)
        {
            Debug.LogError("[ActionPointBattleSceneController] RunPlayerManager が見つかりません。");
            return;
        }

        m_currentTurn = 1;
        m_currentActionPoint = 0;
        m_gainedActionPoint = 0;
        m_playerBlock = 0;
        m_pendingDefensePoint = 0;
        m_enemyHp = m_enemyDefinition != null ? m_enemyDefinition.MaxHp : 50;
        m_enemyBlock = 0;
        m_latestRollResult = default;
        m_usedOncePerBattleSkills.Clear();
        m_playerStatusEffects.Clear();

        // 複数敵が設定されていればランタイムを生成してEnemyAreaViewで表示する
        m_enemyRuntimes.Clear();
        List<EnemyDefinition> encounterEnemies = ResolveEncounterEnemies();
        if (encounterEnemies != null && encounterEnemies.Count > 0)
        {
            foreach (var def in encounterEnemies)
            {
                m_enemyRuntimes.Add(new EnemyUnitRuntime(def));
            }

            if (m_enemyAreaView != null)
            {
                m_enemyAreaView.SpawnEnemies(encounterEnemies);
                // 敵クリックのハンドラ登録
                var spawned = m_enemyAreaView.SpawnedEnemies;
                for (int i = 0; i < spawned.Count; i++)
                {
                    var sel = spawned[i].GetComponentInChildren<EnemySelectable>();
                    if (sel != null)
                    {
                        int idx = i;
                        sel.Clicked += (_) => OnEnemyClicked(idx);
                    }
                }
            }
        }

        AppendLog("戦闘開始");
        if (m_diceRoleListView != null)
        {
            m_diceRoleListView.Refresh();
        }

        UpdateAllViews();
        BeginPlayerTurn();
    }

    /// <summary>
    /// 攻撃コマンドを現在設定中のコストで実行します。
    /// </summary>
    public void ExecuteAttackCommand()
    {
        int cost = m_attackCommandView != null ? m_attackCommandView.CurrentCost : 0;
        // 旧来の即時実行ではなく予約方式へ（ターゲットは現在の選択を使用）
        ReserveAttack(cost, m_currentTargetIndex);
    }

    /// <summary>
    /// 指定APを消費して敵へ攻撃します。
    /// </summary>
    public bool ExecuteAttack(int actionPointCost, bool isReservationExecution = false)
    {
        if (m_currentPhase != BattlePhase.PlayerAction || IsBattleFinished()) return false;
        // 予約からの実行時は既にAPが確保されているためここでの消費は行わない
        if (!isReservationExecution)
        {
            if (!BattleCalculator.TrySpendActionPoint(ref m_currentActionPoint, actionPointCost))
            {
                AppendLog("APが足りません");
                UpdateAllViews();
                return false;
            }
        }

        int attackValue = BattleCalculator.CalculateAttackValue(actionPointCost);
        BlockDamageResult result = BattleCalculator.CalculateBlockDamage(attackValue, m_enemyBlock);
        m_enemyBlock = result.RemainingBlock;
        m_enemyHp = Mathf.Max(0, m_enemyHp - result.Damage);

        AppendLog($"攻撃: {attackValue}（Block {result.BlockedAmount} 軽減 → {result.Damage} ダメージ）");
        UpdateAllViews();
        CheckStatusAfterPlayerAction();
        return true;
    }

    /// <summary>
    /// 指定の敵インデックスを対象に攻撃を予約します。
    /// </summary>
    public bool ReserveAttack(int cost, int enemyIndex)
    {
        if (m_currentPhase != BattlePhase.PlayerAction || IsBattleFinished()) return false;

        if (!BattleCalculator.TrySpendActionPoint(ref m_currentActionPoint, cost))
        {
            AppendLog("APが足りません");
            UpdateAllViews();
            return false;
        }

        EnemyUnitRuntime target = null;
        if (enemyIndex >= 0 && enemyIndex < m_enemyRuntimes.Count)
        {
            target = m_enemyRuntimes[enemyIndex];
        }

        var pa = PlannedAction.CreateAttack(cost, target);
        m_reservations.Add(pa);
        AppendLog($"攻撃予約: {cost} -> {(target != null ? target.Definition.EnemyName : "(自動選択)")}");
        UpdateReservationView();
        UpdateAllViews();
        return true;
    }

    /// <summary>
    /// 防御コマンドを現在設定中のコストで予約します。
    /// </summary>
    public void ExecuteDefenseCommand()
    {
        int cost = m_defenseCommandView != null ? m_defenseCommandView.CurrentCost : 0;
        AddDefense(cost);
    }

    /// <summary>
    /// APを消費し、ターン終了時にBlockへ変換する防御値として予約します。
    /// </summary>
    public bool AddDefense(int actionPointCost)
    {
        if (m_currentPhase != BattlePhase.PlayerAction || IsBattleFinished()) return false;
        if (!BattleCalculator.TrySpendActionPoint(ref m_currentActionPoint, actionPointCost))
        {
            AppendLog("APが足りません");
            UpdateAllViews();
            return false;
        }

        int blockValue = BattleCalculator.CalculateBlockValue(actionPointCost);
        m_pendingDefensePoint += blockValue;
        AppendLog($"防御予約: +{blockValue}");
        UpdateAllViews();
        return true;
    }

    /// <summary>
    /// スキルを使用します。
    /// </summary>
    public bool UseSkill(SkillDefinition skillDefinition)
    {
        if (skillDefinition == null || m_currentPhase != BattlePhase.PlayerAction || IsBattleFinished()) return false;

        if (!CanUseOncePerBattleSkill(skillDefinition))
        {
            AppendLog($"スキル「{skillDefinition.SkillName}」はこの戦闘ではもう使用できません");
            return false;
        }

        if (!BattleCalculator.TrySpendActionPoint(ref m_currentActionPoint, skillDefinition.Cost))
        {
            AppendLog($"APが足りないためスキル「{skillDefinition.SkillName}」を使えません");
            UpdateAllViews();
            return false;
        }

        ApplySkillEffect(skillDefinition, null);
        MarkSkillUsed(skillDefinition);
        PlayUseSound(skillDefinition.UseSound);
        AppendLog($"スキル使用: {skillDefinition.SkillName}（AP -{skillDefinition.Cost}）");
        UpdateAllViews();
        CheckStatusAfterPlayerAction();
        return true;
    }

    /// <summary>
    /// アイテムを使用します。
    /// </summary>
    public bool UseItem(ItemData itemData)
    {
        if (itemData == null || m_currentPhase != BattlePhase.PlayerAction || IsBattleFinished()) return false;

        ItemEffectEntry effectEntry = m_runPlayerManager != null ? m_runPlayerManager.ResolveItemEffect(itemData) : null;
        if (effectEntry == null)
        {
            AppendLog($"アイテム「{itemData.itemName}」の効果が未登録のため使用できません");
            return false;
        }

        ApplyItemEffect(effectEntry);
        if (m_runPlayerManager != null)
        {
            m_runPlayerManager.TryRemoveItem(itemData);
        }

        AppendLog($"アイテム使用: {itemData.itemName}");
        UpdateAllViews();
        CheckStatusAfterPlayerAction();
        return true;
    }

    /// <summary>
    /// ターン終了ボタンから呼び出します。
    /// </summary>
    public void EndPlayerTurn()
    {
        if (m_currentPhase != BattlePhase.PlayerAction || IsBattleFinished()) return;

        if (m_pendingDefensePoint > 0)
        {
            m_playerBlock += m_pendingDefensePoint;
            AppendLog($"Block +{m_pendingDefensePoint}");
            m_pendingDefensePoint = 0;
        }

        m_currentActionPoint = 0;
        UpdateAllViews();

        if (m_enemyActionCoroutine != null)
        {
            StopCoroutine(m_enemyActionCoroutine);
        }

        m_enemyActionCoroutine = StartCoroutine(ExecuteEnemyActionCoroutine());
    }

    /// <summary>
    /// 外部ロール演出から確定した出目を受け取ります。
    /// </summary>
    public void SubmitExternalRollNumbers(int[] numbers)
    {
        if (m_currentPhase != BattlePhase.RollDice)
        {
            Debug.LogWarning("[ActionPointBattleSceneController] 現在は外部ロール結果を受け取れないフェーズです。");
            return;
        }

        if (m_currentRolledDice == null || m_currentRolledDice.Length != RollDiceCount)
        {
            Debug.LogWarning("[ActionPointBattleSceneController] ロール対象ダイスがありません。");
            return;
        }

        string[] diceIds = m_currentRolledDice.Select(dice => dice != null ? dice.PersistentId : string.Empty).ToArray();
        List<DiceRoleDefinition> roles = GetAvailableRoles();
        DiceRollResult rollResult = DiceRollCalculator.CalculateFromRolledNumbers(diceIds, numbers, roles);
        ApplyRollResult(rollResult);
    }

    private void BeginPlayerTurn()
    {
        if (IsBattleFinished()) return;

        SetPhase(BattlePhase.TurnStart);
        m_currentActionPoint = 0;
        m_gainedActionPoint = 0;
        m_pendingDefensePoint = 0;
        m_reservations.Clear();
        UpdateReservationView();

        if (m_enemyRuntimes.Count > 0)
        {
            foreach (EnemyUnitRuntime runtime in m_enemyRuntimes)
            {
                if (runtime != null && runtime.IsAlive)
                {
                    runtime.NextActionGroup = EnemyActionSelector.SelectActionGroup(runtime.Definition, runtime.CurrentHp);
                }
            }
        }
        else
        {
            m_nextEnemyActionGroup = EnemyActionSelector.SelectActionGroup(m_enemyDefinition, m_enemyHp);
        }

        UpdateTurnText();
        RollDiceForTurn();
    }

    private void RollDiceForTurn()
    {
        SetPhase(BattlePhase.RollDice);
        m_currentRolledDice = SelectRandomDiceFromPlayer();
        if (m_currentRolledDice.Length != RollDiceCount || m_currentRolledDice.Any(dice => dice == null))
        {
            AppendLog("ロール可能なダイスが3個未満です");
            return;
        }

        AppendLog("ターン開始: ランダム3ダイスをロール");

        if (m_waitExternalRollResult)
        {
            ExternalRollRequested?.Invoke(m_currentRolledDice);
            UpdateAllViews();
            return;
        }

        string[] diceIds = m_currentRolledDice.Select(dice => dice.PersistentId).ToArray();
        DiceRollResult rollResult = DiceRollCalculator.Roll(diceIds, GetAvailableRoles());
        ApplyRollResult(rollResult);
    }

    private void ApplyRollResult(DiceRollResult rollResult)
    {
        m_latestRollResult = rollResult;
        m_gainedActionPoint = BattleCalculator.CalculateActionPoint(rollResult);
        m_currentActionPoint = m_gainedActionPoint;

        string numbers = rollResult.RollData != null ? string.Join(", ", rollResult.RollData.Select(data => data.Number.ToString())) : "-";
        string roleName = rollResult.EvaluateResult.IsRoleMatched ? rollResult.EvaluateResult.Role.RoleName : "なし";
        AppendLog($"出目: {numbers} / 役: {roleName} / AP +{m_gainedActionPoint}");

        SetPhase(BattlePhase.PlayerAction);
        UpdateAllViews();
    }

    private DiceDefinition[] SelectRandomDiceFromPlayer()
    {
        if (m_runPlayerManager == null || m_runPlayerManager.OwnedDice == null)
        {
            return Array.Empty<DiceDefinition>();
        }

        List<DiceDefinition> dicePool = m_runPlayerManager.OwnedDice.Where(dice => dice != null).ToList();
        DiceDefinition[] selectedDice = new DiceDefinition[Mathf.Min(RollDiceCount, dicePool.Count)];
        for (int i = 0; i < selectedDice.Length; i++)
        {
            int randomIndex = UnityEngine.Random.Range(0, dicePool.Count);
            selectedDice[i] = dicePool[randomIndex];
            dicePool.RemoveAt(randomIndex);
        }

        return selectedDice;
    }

    private List<DiceRoleDefinition> GetAvailableRoles()
    {
        DiceMasterRegistry registry = DiceMasterRegistry.Active;
        if (registry == null && m_runSystemManager != null)
        {
            registry = m_runSystemManager.DiceMasterRegistry;
        }

        return registry != null ? registry.AllRoleDefinitions.ToList() : new List<DiceRoleDefinition>();
    }

    private IEnumerator ExecuteEnemyActionCoroutine()
    {
        SetPhase(BattlePhase.EnemyAction);

        if (m_enemyRuntimes.Count > 0)
        {
            foreach (EnemyUnitRuntime runtime in m_enemyRuntimes.ToArray())
            {
                if (runtime == null || !runtime.IsAlive) continue;

                EnemyWeightedActionGroup group = runtime.NextActionGroup;
                if (group == null || group.Actions == null || group.Actions.Count == 0)
                {
                    int fallbackPower = runtime.Definition != null ? runtime.Definition.AttackPower : 0;
                    ApplyEnemyAttack(fallbackPower);
                    UpdateAllViews();
                    if (m_runPlayerManager != null && !m_runPlayerManager.IsAlive) break;
                    yield return new WaitForSeconds(m_enemyActionInterval);
                    continue;
                }

                foreach (EnemyActionEntry entry in group.Actions)
                {
                    if (entry == null) continue;

                    ApplyEnemyActionEntryToRuntime(entry, runtime);
                    UpdateAllViews();
                    if (m_runPlayerManager != null && !m_runPlayerManager.IsAlive) break;
                    yield return new WaitForSeconds(m_enemyActionInterval);
                }

                if (m_runPlayerManager != null && !m_runPlayerManager.IsAlive) break;
            }

            CheckStatusAfterEnemyAction();
            yield break;
        }

        EnemyWeightedActionGroup actionGroup = m_nextEnemyActionGroup;
        if (actionGroup == null || actionGroup.Actions == null || actionGroup.Actions.Count == 0)
        {
            int fallbackPower = m_enemyDefinition != null ? m_enemyDefinition.AttackPower : 0;
            ApplyEnemyAttack(fallbackPower);
            AppendLog($"敵の行動: 攻撃 {fallbackPower}");
            UpdateAllViews();
            yield return new WaitForSeconds(m_enemyActionInterval);
            CheckStatusAfterEnemyAction();
            yield break;
        }

        foreach (EnemyActionEntry entry in actionGroup.Actions)
        {
            if (entry == null) continue;

            ApplyEnemyActionEntry(entry);
            if (m_enemyView != null)
            {
                m_enemyView.ShowActionVisual(m_enemyActionInterval);
            }

            UpdateAllViews();
            if (m_runPlayerManager != null && !m_runPlayerManager.IsAlive) break;
            yield return new WaitForSeconds(m_enemyActionInterval);
        }

        CheckStatusAfterEnemyAction();
    }

    private void ApplyEnemyActionEntryToRuntime(EnemyActionEntry entry, EnemyUnitRuntime runtime)
    {
        string enemyName = runtime.Definition != null ? runtime.Definition.EnemyName : "敵";
        switch (entry.ActionType)
        {
            case BattleActionType.Attack:
                ApplyEnemyAttack(entry.Power);
                break;

            case BattleActionType.Defense:
                runtime.AddBlock(entry.Power);
                AppendLog($"{enemyName} の行動: 防御 Block +{entry.Power}");
                break;

            case BattleActionType.Buff:
                AppendLog($"{enemyName} の行動: バフ（新Controllerでは効果適用未実装）");
                break;

            case BattleActionType.Debuff:
                AppendLog($"{enemyName} の行動: デバフ（新Controllerでは効果適用未実装）");
                break;
        }
    }

    private void ApplyEnemyActionEntry(EnemyActionEntry entry)
    {
        switch (entry.ActionType)
        {
            case BattleActionType.Attack:
                ApplyEnemyAttack(entry.Power);
                break;

            case BattleActionType.Defense:
                m_enemyBlock += Mathf.Max(0, entry.Power);
                AppendLog($"敵の行動: 防御 Block +{entry.Power}");
                break;

            case BattleActionType.Buff:
                AppendLog("敵の行動: バフ（新Controllerでは効果適用未実装）");
                break;

            case BattleActionType.Debuff:
                AppendLog("敵の行動: デバフ（新Controllerでは効果適用未実装）");
                break;
        }
    }

    private void ApplyEnemyAttack(int attackPower)
    {
        BlockDamageResult result = BattleCalculator.CalculateBlockDamage(attackPower, m_playerBlock);
        m_playerBlock = result.RemainingBlock;
        int actualDamage = m_runPlayerManager != null ? m_runPlayerManager.ApplyDamage(result.Damage) : result.Damage;
        AppendLog($"敵の攻撃: {attackPower}（Block {result.BlockedAmount} 軽減 → {actualDamage} ダメージ）");
    }

    private void ApplySkillEffect(SkillDefinition skillDefinition, EnemyUnitRuntime target)
    {
        switch (skillDefinition.EffectType)
        {
            case SkillEffectType.Heal:
                ApplyHealEffect(skillDefinition.EffectValue, $"スキル: {skillDefinition.SkillName}");
                break;

            case SkillEffectType.Damage:
                ApplyDamageEffect(skillDefinition.EffectValue, target, $"スキル: {skillDefinition.SkillName}");
                break;

            case SkillEffectType.Block:
                ApplyBlockEffect(skillDefinition.EffectValue, $"スキル: {skillDefinition.SkillName}");
                break;

            case SkillEffectType.GainActionPoint:
                ApplyGainActionPointEffect(skillDefinition.EffectValue, $"スキル: {skillDefinition.SkillName}");
                break;

            case SkillEffectType.ApplyStatusEffect:
                ApplyStatusEffectToPlayer(skillDefinition.StatusEffect, $"スキル: {skillDefinition.SkillName}");
                break;
        }
    }

    private void ApplyItemEffect(ItemEffectEntry effectEntry)
    {
        switch (effectEntry.EffectType)
        {
            case ItemEffectType.Heal:
                ApplyHealEffect(effectEntry.EffectValue, $"アイテム: {effectEntry.ItemName}");
                break;

            case ItemEffectType.Damage:
                ApplyDamageEffect(effectEntry.EffectValue, null, $"アイテム: {effectEntry.ItemName}");
                break;

            case ItemEffectType.Block:
                ApplyBlockEffect(effectEntry.EffectValue, $"アイテム: {effectEntry.ItemName}");
                break;

            case ItemEffectType.GainActionPoint:
                ApplyGainActionPointEffect(effectEntry.EffectValue, $"アイテム: {effectEntry.ItemName}");
                break;

            case ItemEffectType.ApplyStatusEffect:
                ApplyStatusEffectToPlayer(effectEntry.StatusEffect, $"アイテム: {effectEntry.ItemName}");
                break;
        }
    }

    // ---- Item/Skill 共通の効果適用ヘルパー ----

    private void ApplyHealEffect(int value, string sourceLabel)
    {
        int healed = m_runPlayerManager != null ? m_runPlayerManager.Heal(value) : 0;
        AppendLog($"{sourceLabel} HP +{healed}");
    }

    private void ApplyDamageEffect(int value, EnemyUnitRuntime target, string sourceLabel)
    {
        var resolvedTarget = target ?? m_enemyRuntimes.FirstOrDefault(e => e.IsAlive);
        if (resolvedTarget != null)
        {
            int beforeHp = resolvedTarget.CurrentHp;
            resolvedTarget.ApplyDamage(value);
            int damage = beforeHp - resolvedTarget.CurrentHp;
            AppendLog($"{sourceLabel} 敵に {damage} ダメージ");
            RemoveEnemyIfDead(resolvedTarget);
        }
        else
        {
            ApplyDirectDamageToEnemy(value);
        }
    }

    private void ApplyBlockEffect(int value, string sourceLabel)
    {
        m_playerBlock += BattleCalculator.CalculateBlockValue(value);
        AppendLog($"{sourceLabel} Block +{value}");
    }

    private void ApplyGainActionPointEffect(int value, string sourceLabel)
    {
        m_currentActionPoint = BattleCalculator.AddActionPoint(m_currentActionPoint, value);
        AppendLog($"{sourceLabel} AP +{value}");
    }

    private void ApplyStatusEffectToPlayer(StatusEffectDefinition statusEffect, string sourceLabel)
    {
        if (statusEffect == null)
        {
            AppendLog($"{sourceLabel} 効果なし（StatusEffect 未設定）");
            return;
        }

        m_playerStatusEffects.Add(new ActiveStatusEffect(statusEffect));
        AppendLog($"{sourceLabel} {statusEffect.EffectName} を付与");
    }

    // ---- OncePerBattle / SE ----

    private bool CanUseOncePerBattleSkill(SkillDefinition skillDefinition)
    {
        return skillDefinition == null
            || !skillDefinition.OncePerBattle
            || !m_usedOncePerBattleSkills.Contains(skillDefinition);
    }

    private void MarkSkillUsed(SkillDefinition skillDefinition)
    {
        if (skillDefinition != null && skillDefinition.OncePerBattle)
        {
            m_usedOncePerBattleSkills.Add(skillDefinition);
        }
    }

    private void PlayUseSound(AudioClip clip)
    {
        if (clip == null) return;

        if (m_audioSource != null)
        {
            m_audioSource.PlayOneShot(clip);
        }
        else
        {
            AudioSource.PlayClipAtPoint(clip, Camera.main != null ? Camera.main.transform.position : Vector3.zero);
        }
    }

    private void ApplyDirectDamageToEnemy(int damageValue)
    {
        int safeDamage = Mathf.Max(0, damageValue);
        BlockDamageResult result = BattleCalculator.CalculateBlockDamage(safeDamage, m_enemyBlock);
        m_enemyBlock = result.RemainingBlock;
        m_enemyHp = Mathf.Max(0, m_enemyHp - result.Damage);
        AppendLog($"敵に {result.Damage} ダメージ");
    }

    private void CheckStatusAfterPlayerAction()
    {
        if (AreAllEnemiesDefeated())
        {
            SetPhase(BattlePhase.Win);
            AppendLog("勝利");
            UpdateAllViews();
        }
    }

    private bool AreAllEnemiesDefeated()
    {
        if (m_enemyDefinitions != null && m_enemyDefinitions.Count > 0)
        {
            return m_enemyRuntimes.Count == 0 || m_enemyRuntimes.All(e => e == null || !e.IsAlive);
        }

        return m_enemyHp <= 0;
    }

    private void CheckStatusAfterEnemyAction()
    {
        SetPhase(BattlePhase.CheckStatus);
        if (m_runPlayerManager != null && !m_runPlayerManager.IsAlive)
        {
            SetPhase(BattlePhase.GameOver);
            AppendLog("ゲームオーバー");
            UpdateAllViews();
            return;
        }

        m_currentTurn++;
        BeginPlayerTurn();
    }

    private bool IsBattleFinished()
    {
        return m_currentPhase == BattlePhase.Win || m_currentPhase == BattlePhase.GameOver;
    }

    private void ResolveManagers()
    {
        if (m_runSystemManager == null)
        {
            m_runSystemManager = RunSystemManager.Instance;
        }

        if (m_runSystemManager == null)
        {
            m_runSystemManager = FindFirstObjectByType<RunSystemManager>();
        }

        if (m_runPlayerManager == null)
        {
            m_runPlayerManager = RunPlayerManager.Instance;
        }

        if (m_runPlayerManager == null)
        {
            m_runPlayerManager = FindFirstObjectByType<RunPlayerManager>();
        }
    }

    /// <summary>
    /// 出現させる敵編成を決定します。
    /// RunSystemManagerに抽選済みの次戦闘編成があればそれを優先し、
    /// 無ければInspector設定の敵データ（複数対応）を使用します（単独テスト起動用フォールバック）。
    /// </summary>
    private List<EnemyDefinition> ResolveEncounterEnemies()
    {
        if (m_runSystemManager != null && m_runSystemManager.HasNextBattleEnemies)
        {
            return m_runSystemManager.NextBattleEnemies.Where(enemy => enemy != null).ToList();
        }

        return m_enemyDefinitions;
    }

    private void SetPhase(BattlePhase phase)
    {
        m_currentPhase = phase;
        if (m_phaseText != null)
        {
            m_phaseText.text = $"Phase: {m_currentPhase}";
        }

        RefreshCommandViews();
    }

    private void UpdateAllViews()
    {
        if (m_runPlayerManager != null && m_playerHpView != null)
        {
            m_playerHpView.UpdateHp(m_runPlayerManager.CurrentHp, m_runPlayerManager.MaxHp);
        }

        if (m_playerBlockView != null)
        {
            m_playerBlockView.UpdateBlock(m_playerBlock + m_pendingDefensePoint);
        }

        if (m_enemyView != null)
        {
            m_enemyView.UpdateEnemy(m_enemyDefinition, m_enemyHp);
            m_enemyView.UpdateBlock(m_enemyBlock);
        }

        if (m_actionPointView != null)
        {
            m_actionPointView.UpdateActionPoint(m_currentActionPoint, m_gainedActionPoint);
        }

        if (m_enemyAreaView != null && m_enemyRuntimes.Count > 0)
        {
            IReadOnlyList<GameObject> spawned = m_enemyAreaView.SpawnedEnemies;
            for (int i = 0; i < spawned.Count && i < m_enemyRuntimes.Count; i++)
            {
                EnemyView view = spawned[i] != null ? spawned[i].GetComponent<EnemyView>() : null;
                if (view == null || m_enemyRuntimes[i] == null) continue;

                view.UpdateEnemy(m_enemyRuntimes[i].Definition, m_enemyRuntimes[i].CurrentHp);
                view.UpdateBlock(m_enemyRuntimes[i].Block);
            }
        }

        if (m_skillListView != null)
        {
            m_skillListView.Refresh(m_runPlayerManager != null ? m_runPlayerManager.OwnedSkills : null, m_currentActionPoint, HandleSkillClicked);
        }

        if (m_itemListView != null)
        {
            m_itemListView.Refresh(
                m_runPlayerManager != null ? m_runPlayerManager.OwnedItems : null,
                GetReservedItemCount,
                CanReserveItem,
                HandleItemClicked);
        }

        UpdateRollResultText();
        UpdateTurnText();
        RefreshCommandViews();
    }

    private bool CanReserveItem(ItemData itemData)
    {
        if (itemData == null || m_currentPhase != BattlePhase.PlayerAction || IsBattleFinished()) return false;

        int ownedCount = m_runPlayerManager != null ? m_runPlayerManager.GetItemCount(itemData) : 0;
        return GetReservedItemCount(itemData) < ownedCount;
    }

    /// <summary>
    /// 指定アイテムの予約済み数を返します。
    /// </summary>
    private int GetReservedItemCount(ItemData itemData)
    {
        return m_reservations.Count(r => r.Kind == PlannedAction.PlannedActionKind.Item && r.Item == itemData);
    }

    private void UpdateTurnText()
    {
        if (m_turnText != null)
        {
            m_turnText.text = $"Turn {m_currentTurn}";
        }
    }

    private void UpdateRollResultText()
    {
        if (m_rollResultText == null) return;

        if (m_latestRollResult.RollData == null)
        {
            m_rollResultText.text = "";
            return;
        }

        string diceNames = m_currentRolledDice != null ? string.Join(", ", m_currentRolledDice.Select(dice => dice != null ? dice.DiceName : "-")) : "-";
        string numbers = string.Join(", ", m_latestRollResult.RollData.Select(data => data.Number.ToString()));
        string roleName = m_latestRollResult.EvaluateResult.IsRoleMatched ? m_latestRollResult.EvaluateResult.Role.RoleName : "なし";
        m_rollResultText.text = $"Dice: {diceNames}\n出目: {numbers}\n合計: {m_latestRollResult.TotalNumber}\n役: {roleName} x{m_latestRollResult.EvaluateResult.Multiplier:0.##}\nAP: {m_gainedActionPoint}";
    }

    private void RefreshCommandViews()
    {
        bool canAct = m_currentPhase == BattlePhase.PlayerAction && !IsBattleFinished();

        if (m_endTurnButton != null)
        {
            m_endTurnButton.interactable = canAct;
        }

        if (m_attackCommandView != null)
        {
            m_attackCommandView.SetInteractable(canAct);
            m_attackCommandView.SetMaxCost(m_currentActionPoint);
        }

        if (m_defenseCommandView != null)
        {
            m_defenseCommandView.SetInteractable(canAct);
            m_defenseCommandView.SetMaxCost(m_currentActionPoint);
        }
    }

    private void AppendLog(string message)
    {
        Debug.Log($"[ActionPointBattle] {message}");
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

    private void RegisterEvents()
    {
        if (m_endTurnButton != null) m_endTurnButton.onClick.AddListener(ConfirmAndExecuteReservations);
        if (m_attackCommandView != null) m_attackCommandView.CommandExecuted += HandleAttackCommandExecuted;
        if (m_defenseCommandView != null) m_defenseCommandView.CommandExecuted += HandleDefenseCommandExecuted;
    }

    private void UnregisterEvents()
    {
        if (m_endTurnButton != null) m_endTurnButton.onClick.RemoveListener(ConfirmAndExecuteReservations);
        if (m_attackCommandView != null) m_attackCommandView.CommandExecuted -= HandleAttackCommandExecuted;
        if (m_defenseCommandView != null) m_defenseCommandView.CommandExecuted -= HandleDefenseCommandExecuted;
    }

    /// <summary>
    /// 行動決定ボタン（旧: ターン終了ボタン）を押したときに予約している行動を順次実行します。
    /// </summary>
    private void ConfirmAndExecuteReservations()
    {
        if (m_currentPhase != BattlePhase.PlayerAction || IsBattleFinished()) return;
        if (m_enemyActionCoroutine != null)
        {
            StopCoroutine(m_enemyActionCoroutine);
            m_enemyActionCoroutine = null;
        }

        StartCoroutine(ExecuteReservationsCoroutine());
    }

    private void UpdateReservationView()
    {
        if (m_reservationLaneView != null)
        {
            m_reservationLaneView.Refresh(m_reservations, CancelReservation);
        }
    }

    /// <summary>
    /// 敵が死亡していたらランタイムと表示から削除し、選択中ターゲットを同期します。
    /// </summary>
    private void RemoveEnemyIfDead(EnemyUnitRuntime enemy)
    {
        if (enemy == null || enemy.IsAlive) return;

        int idx = m_enemyRuntimes.IndexOf(enemy);
        if (idx < 0) return;

        m_enemyRuntimes.RemoveAt(idx);
        if (m_enemyAreaView != null) m_enemyAreaView.RemoveAt(idx);

        // 選択中のターゲットindexを再計算
        if (m_currentTargetIndex == idx)
        {
            m_currentTargetIndex = -1;
        }
        else if (m_currentTargetIndex > idx)
        {
            m_currentTargetIndex--;
        }

        AppendLog($"{(enemy.Definition != null ? enemy.Definition.EnemyName : "敵")} を撃破");
    }

    private void CancelReservation(int index)
    {
        if (index < 0 || index >= m_reservations.Count) return;
        var pa = m_reservations[index];
        m_reservations.RemoveAt(index);
        // 予約をキャンセルしたらAPを返却
        m_currentActionPoint = BattleCalculator.AddActionPoint(m_currentActionPoint, pa.Cost);
        AppendLog($"予約キャンセル: {pa.Label} (AP返却 {pa.Cost})");
        UpdateReservationView();
        UpdateAllViews();
    }

    private IEnumerator ExecuteReservationsCoroutine()
    {
        SetPhase(BattlePhase.PlayerAction);

        foreach (var pa in m_reservations.ToArray())
        {
            if (pa == null) continue;

            switch (pa.Kind)
            {
                case PlannedAction.PlannedActionKind.Attack:
                    // 予約実行: 既にAPは差し引かれているため isReservationExecution=true
                    if (pa.TargetEnemy != null && pa.TargetEnemy.IsAlive)
                    {
                        int before = pa.TargetEnemy.CurrentHp;
                        int attackValue = BattleCalculator.CalculateAttackValue(pa.Cost);
                        pa.TargetEnemy.ApplyDamage(attackValue);
                        int damage = before - pa.TargetEnemy.CurrentHp;
                        AppendLog($"攻撃(予約実行): {attackValue} -> {damage} ダメージ ({pa.TargetEnemy.Definition.EnemyName})");
                        RemoveEnemyIfDead(pa.TargetEnemy);
                    }
                    else
                    {
                        // 対象不在なら生存敵を自動選択
                        var target = m_enemyRuntimes.FirstOrDefault(e => e.IsAlive);
                        if (target != null)
                        {
                            int before = target.CurrentHp;
                            int attackValue = BattleCalculator.CalculateAttackValue(pa.Cost);
                            target.ApplyDamage(attackValue);
                            int damage = before - target.CurrentHp;
                            AppendLog($"攻撃(予約実行): {attackValue} -> {damage} ダメージ ({target.Definition.EnemyName})");
                            RemoveEnemyIfDead(target);
                        }
                        else
                        {
                            AppendLog("攻撃対象がいません（スキップ）");
                        }
                    }
                    break;

                case PlannedAction.PlannedActionKind.Defense:
                    int blockValue = BattleCalculator.CalculateBlockValue(pa.Cost);
                    m_pendingDefensePoint += blockValue;
                    AppendLog($"防御(予約実行): +{blockValue}");
                    break;

                case PlannedAction.PlannedActionKind.Skill:
                    if (pa.Skill != null)
                    {
                        ApplySkillEffect(pa.Skill, pa.TargetEnemy);
                        MarkSkillUsed(pa.Skill);
                        PlayUseSound(pa.Skill.UseSound);
                    }
                    break;

                case PlannedAction.PlannedActionKind.Item:
                    if (pa.Item != null && pa.ItemEffect != null)
                    {
                        // アイテム効果適用
                        switch (pa.ItemEffect.EffectType)
                        {
                            case ItemEffectType.Heal:
                                int healed = m_runPlayerManager != null ? m_runPlayerManager.Heal(pa.ItemEffect.EffectValue) : 0;
                                AppendLog($"アイテム: {pa.Item.itemName} HP +{healed}");
                                break;
                            case ItemEffectType.Damage:
                                var t2 = pa.TargetEnemy ?? m_enemyRuntimes.FirstOrDefault(e => e.IsAlive);
                                if (t2 != null)
                                {
                                    int beforeHp = t2.CurrentHp;
                                    t2.ApplyDamage(pa.ItemEffect.EffectValue);
                                    int dmg = beforeHp - t2.CurrentHp;
                                    AppendLog($"アイテム: {pa.Item.itemName} 敵に {dmg} ダメージ");
                                    RemoveEnemyIfDead(t2);
                                }
                                break;
                            case ItemEffectType.Block:
                                m_playerBlock += BattleCalculator.CalculateBlockValue(pa.ItemEffect.EffectValue);
                                AppendLog($"アイテム: {pa.Item.itemName} Block +{pa.ItemEffect.EffectValue}");
                                break;
                            case ItemEffectType.GainActionPoint:
                                m_currentActionPoint = BattleCalculator.AddActionPoint(m_currentActionPoint, pa.ItemEffect.EffectValue);
                                AppendLog($"アイテム: {pa.Item.itemName} AP +{pa.ItemEffect.EffectValue}");
                                break;
                        }

                        if (m_runPlayerManager != null && pa.Item != null)
                        {
                            m_runPlayerManager.TryRemoveItem(pa.Item);
                        }
                    }
                    break;
            }

            UpdateAllViews();
            yield return new WaitForSeconds(m_enemyActionInterval);
        }

        // 予約処理がすべて終わったら、防御予約値をBlockへ反映して敵行動へ
        if (m_pendingDefensePoint > 0)
        {
            m_playerBlock += m_pendingDefensePoint;
            AppendLog($"Block +{m_pendingDefensePoint}");
            m_pendingDefensePoint = 0;
        }

        m_reservations.Clear();
        UpdateReservationView();
        m_currentActionPoint = 0;
        UpdateAllViews();

        CheckStatusAfterPlayerAction();
        if (IsBattleFinished()) yield break;

        m_enemyActionCoroutine = StartCoroutine(ExecuteEnemyActionCoroutine());
    }

    private void HandleAttackCommandExecuted(int cost)
    {
        ReserveAttack(cost, m_currentTargetIndex);
    }

    private void HandleDefenseCommandExecuted(int cost)
    {
        AddDefense(cost);
    }

    private void HandleSkillClicked(SkillDefinition skillDefinition)
    {
        ReserveSkill(skillDefinition, m_currentTargetIndex);
    }

    private void HandleItemClicked(ItemData itemData)
    {
        ReserveItem(itemData, m_currentTargetIndex);
    }

    private void OnEnemyClicked(int index)
    {
        // toggle selection
        if (m_currentTargetIndex == index) m_currentTargetIndex = -1;
        else m_currentTargetIndex = index;

        if (m_enemyAreaView == null) return;
        var spawned = m_enemyAreaView.SpawnedEnemies;
        for (int i = 0; i < spawned.Count; i++)
        {
            var sel = spawned[i].GetComponentInChildren<EnemySelectable>();
            if (sel != null)
            {
                sel.SetTargeted(i == m_currentTargetIndex);
            }
        }
    }

    /// <summary>
    /// スキルを予約します（APを消費してキューへ追加）。
    /// </summary>
    public bool ReserveSkill(SkillDefinition skill, int enemyIndex = -1)
    {
        if (skill == null || m_currentPhase != BattlePhase.PlayerAction || IsBattleFinished()) return false;

        if (!CanUseOncePerBattleSkill(skill) ||
            m_reservations.Any(r => r.Kind == PlannedAction.PlannedActionKind.Skill && r.Skill == skill && skill.OncePerBattle))
        {
            AppendLog($"スキル「{skill.SkillName}」はこの戦闘ではもう予約できません");
            return false;
        }

        if (!BattleCalculator.TrySpendActionPoint(ref m_currentActionPoint, skill.Cost))
        {
            AppendLog($"APが足りないためスキル「{skill.SkillName}」を予約できません");
            UpdateAllViews();
            return false;
        }

        EnemyUnitRuntime target = null;
        if (enemyIndex >= 0 && enemyIndex < m_enemyRuntimes.Count) target = m_enemyRuntimes[enemyIndex];

        var pa = PlannedAction.CreateSkill(skill, target);
        m_reservations.Add(pa);
        AppendLog($"スキル予約: {skill.SkillName} (Cost {skill.Cost})");
        UpdateReservationView();
        UpdateAllViews();
        return true;
    }

    /// <summary>
    /// アイテム使用を予約します（消費は実行時）。
    /// </summary>
    public bool ReserveItem(ItemData item, int enemyIndex = -1)
    {
        if (item == null || m_currentPhase != BattlePhase.PlayerAction || IsBattleFinished()) return false;

        ItemEffectEntry effectEntry = m_runPlayerManager != null ? m_runPlayerManager.ResolveItemEffect(item) : null;
        if (effectEntry == null)
        {
            AppendLog($"アイテム「{item.itemName}」の効果が未登録のため予約できません");
            return false;
        }

        int ownedCount = m_runPlayerManager != null ? m_runPlayerManager.GetItemCount(item) : 0;
        if (GetReservedItemCount(item) >= ownedCount)
        {
            AppendLog($"アイテム「{item.itemName}」はこれ以上予約できません");
            return false;
        }

        EnemyUnitRuntime target = null;
        if (enemyIndex >= 0 && enemyIndex < m_enemyRuntimes.Count) target = m_enemyRuntimes[enemyIndex];

        var pa = PlannedAction.CreateItem(item, effectEntry, target);
        m_reservations.Add(pa);
        AppendLog($"アイテム予約: {item.itemName}");
        UpdateReservationView();
        UpdateAllViews();
        return true;
    }
}
