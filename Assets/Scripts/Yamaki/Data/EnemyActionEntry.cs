using UnityEngine;

/// <summary>
/// 敵が実行する1個の行動を表す Serializable クラス
/// 行動区分（攻撃・防御・バフ・デバフ）、数値、効果、エフェクト、サウンドを定義します
/// 
/// 使い方:
/// - EnemyWeightedActionGroup のリスト内で使用します
/// - ActionType に応じて Power / StatusEffect を設定します
/// - エフェクトやサウンドは任意で有効化します
/// </summary>
[System.Serializable]
public class EnemyActionEntry
{
    // ─────────────────────────────────────────────────────────
    // 行動の基本設定
    // ─────────────────────────────────────────────────────────

    [SerializeField]
    [Header("行動区分")]
    [Tooltip("この行動の種別。予測アイコンの表示に使用されます\n" +
             "Attack: 攻撃\n" +
             "Defense: 防御（ブロック値獲得）\n" +
             "Buff: 自身へのバフ付与\n" +
             "Debuff: プレイヤーへのデバフ付与")]
    private BattleActionType m_actionType = BattleActionType.Attack;

    [SerializeField]
    [Header("パワー値")]
    [Tooltip("行動の数値\n" +
             "Attack: プレイヤーへのダメージ\n" +
             "Defense: 敵が得るブロック値\n" +
             "Buff/Debuff: StatusEffect の EffectValue を使用するため未使用")]
    [Min(0)]
    private int m_power = 1;

    [SerializeField]
    [Header("ステータス効果（Buff/Debuff時のみ）")]
    [Tooltip("Buff または Debuff 時に付与する StatusEffectDefinition\n" +
             "Attack/Defense では使用しません")]
    private StatusEffectDefinition m_statusEffect;

    // ─────────────────────────────────────────────────────────
    // エフェクト設定
    // ─────────────────────────────────────────────────────────

    [SerializeField]
    [Header("エフェクト使用")]
    [Tooltip("この行動でエフェクトを再生するかどうか")]
    private bool m_useEffect = false;

    [SerializeField]
    [Header("エフェクトPrefab")]
    [Tooltip("再生するエフェクトのPrefab（ParticleSystem等）\nUseEffectがtrueの場合に使用されます")]
    private GameObject m_effectPrefab;

    [SerializeField]
    [Header("エフェクト表示位置")]
    [Tooltip("エフェクトを表示する基準位置\n" +
             "Self: 敵自身\n" +
             "Opponent: プレイヤー\n" +
             "Center: 画面中央")]
    private EffectAnchorType m_effectAnchor = EffectAnchorType.Opponent;

    [SerializeField]
    [Header("エフェクト表示時間")]
    [Tooltip("エフェクトを自動削除するまでの秒数\n0で削除しない（エフェクト側で自動停止する場合）")]
    [Min(0f)]
    private float m_effectDuration = 1.0f;

    // ─────────────────────────────────────────────────────────
    // サウンド設定
    // ─────────────────────────────────────────────────────────

    [SerializeField]
    [Header("サウンド使用")]
    [Tooltip("この行動でサウンドを再生するかどうか")]
    private bool m_useSound = false;

    [SerializeField]
    [Header("再生するAudioClip")]
    [Tooltip("再生する効果音\nUseSoundがtrueの場合に使用されます")]
    private AudioClip m_audioClip;

    [SerializeField]
    [Header("サウンド再生タイミング")]
    [Tooltip("サウンドを再生するタイミング\n" +
             "ActionStart: 行動開始時\n" +
             "Hit: ダメージやバフ適用時")]
    private SoundTimingType m_soundTiming = SoundTimingType.ActionStart;

    [SerializeField]
    [Header("音量（0～1）")]
    [Tooltip("サウンドの再生音量")]
    [Range(0f, 1f)]
    private float m_volume = 1.0f;

    // ─────────────────────────────────────────────────────────
    // 読み取り専用プロパティ
    // ─────────────────────────────────────────────────────────

    /// <summary>行動区分（攻撃・防御・バフ・デバフ）</summary>
    public BattleActionType ActionType => m_actionType;

    /// <summary>パワー値（攻撃力 or ブロック値）</summary>
    public int Power => m_power;

    /// <summary>付与するステータス効果（Buff/Debuff時）</summary>
    public StatusEffectDefinition StatusEffect => m_statusEffect;

    /// <summary>エフェクトを使用するか</summary>
    public bool UseEffect => m_useEffect;

    /// <summary>エフェクトPrefab</summary>
    public GameObject EffectPrefab => m_effectPrefab;

    /// <summary>エフェクト表示位置</summary>
    public EffectAnchorType EffectAnchor => m_effectAnchor;

    /// <summary>エフェクト表示時間（秒）</summary>
    public float EffectDuration => m_effectDuration;

    /// <summary>サウンドを使用するか</summary>
    public bool UseSound => m_useSound;

    /// <summary>再生するAudioClip</summary>
    public AudioClip AudioClip => m_audioClip;

    /// <summary>サウンド再生タイミング</summary>
    public SoundTimingType SoundTiming => m_soundTiming;

    /// <summary>音量（0～1）</summary>
    public float Volume => m_volume;
}

/// <summary>
/// エフェクトの表示位置を指定する列挙型
/// </summary>
public enum EffectAnchorType
{
    /// <summary>敵自身</summary>
    Self,

    /// <summary>プレイヤー（相手）</summary>
    Opponent,

    /// <summary>画面中央</summary>
    Center
}

/// <summary>
/// サウンドの再生タイミングを指定する列挙型
/// </summary>
public enum SoundTimingType
{
    /// <summary>行動開始時</summary>
    ActionStart,

    /// <summary>ダメージやバフ適用時（ヒット時）</summary>
    Hit
}
