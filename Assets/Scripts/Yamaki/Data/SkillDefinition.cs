using UnityEngine;

/// <summary>
/// プレイヤーが所持・使用できるスキルのマスターデータ。
/// ActionPointをコストとして消費し、効果種別に応じた処理を行います。
/// </summary>
[CreateAssetMenu(fileName = "SkillDefinition_", menuName = "DiceGame/SkillDefinition")]
public class SkillDefinition : PersistentScriptableObject
{
    [SerializeField]
    [Header("スキル名")]
    private string m_skillName = "";

    [SerializeField]
    [Header("説明")]
    [TextArea(2, 4)]
    private string m_description = "";

    [SerializeField]
    [Header("コスト")]
    [Min(0)]
    private int m_cost = 1;

    [SerializeField]
    [Header("効果種別")]
    private SkillEffectType m_effectType = SkillEffectType.Heal;

    [SerializeField]
    [Header("効果量")]
    [Min(0)]
    private int m_effectValue = 1;

    [SerializeField]
    [Header("アイコン")]
    private Sprite m_icon;

    [SerializeField]
    [Header("対象タイプ")]
    private ActionTargetType m_targetType = ActionTargetType.Self;

    [SerializeField]
    [Header("付与するバフ/デバフ(ApplyStatusEffect時)")]
    [Tooltip("EffectType が ApplyStatusEffect のときに付与する StatusEffectDefinition")]
    private StatusEffectDefinition m_statusEffect;

    [SerializeField]
    [Header("1戦镘1回のみ使用可能")]
    private bool m_oncePerBattle = false;

    [SerializeField]
    [Header("使用時SE(任意)")]
    private AudioClip m_useSound;

    /// <summary>スキル名。</summary>
    public string SkillName => m_skillName;

    /// <summary>説明。</summary>
    public string Description => m_description;

    /// <summary>ActionPointコスト。</summary>
    public int Cost => m_cost;

    /// <summary>効果種別。</summary>
    public SkillEffectType EffectType => m_effectType;

    /// <summary>効果量。</summary>
    public int EffectValue => m_effectValue;

    /// <summary>表示用アイコン。</summary>
    public Sprite Icon => m_icon;

    /// <summary>行動の対象タイプ。</summary>
    public ActionTargetType TargetType => m_targetType;

    /// <summary>付与するバフ/デバフ（ApplyStatusEffect時のみ使用）。</summary>
    public StatusEffectDefinition StatusEffect => m_statusEffect;

    /// <summary>1戦镘1回のみ使用可能か。</summary>
    public bool OncePerBattle => m_oncePerBattle;

    /// <summary>使用時SE（未設定の場合はnull）。</summary>
    public AudioClip UseSound => m_useSound;

#if UNITY_EDITOR
    protected override void OnValidate()
    {
        base.OnValidate();
        m_cost = Mathf.Max(0, m_cost);
        m_effectValue = Mathf.Max(0, m_effectValue);
    }
#endif
}
