using UnityEngine;

/// <summary>
/// バフ/デバフの定義を表す ScriptableObject
/// 名前、説明、種別、効果量、持続ターン、個別アイコンを定義します
/// 
/// 作成方法:
/// Project ビュー右クリック → Create → DiceGame/StatusEffectDefinition
/// 
/// 使用例:
/// - 攻撃力アップ
///   - EffectName: "攻撃力強化"
///   - EffectType: Buff
///   - EffectValue: 2
///   - DurationTurns: 3
///   - CustomIcon: 剣アイコンSprite（任意）
/// 
/// - 防御力ダウン
///   - EffectName: "防御力低下"
///   - EffectType: Debuff
///   - EffectValue: -1
///   - DurationTurns: 2
///   - CustomIcon: 盾アイコンSprite（任意）
/// </summary>
[CreateAssetMenu(fileName = "StatusEffectDefinition_", menuName = "DiceGame/StatusEffectDefinition")]
public class StatusEffectDefinition : PersistentScriptableObject
{
    [SerializeField]
    [Header("効果名")]
    [Tooltip("このバフ/デバフの表示名\n例: 「攻撃力強化」「防御力低下」")]
    private string m_effectName = "";

    [SerializeField]
    [Header("説明")]
    [Tooltip("この効果の詳細説明\n例: 「攻撃力が2増加する」")]
    [TextArea(2, 4)]
    private string m_description = "";

    [SerializeField]
    [Header("効果種別")]
    [Tooltip("Buff（有利） または Debuff（不利）")]
    private StatusEffectType m_effectType = StatusEffectType.Buff;

    [SerializeField]
    [Header("効果量")]
    [Tooltip("この効果が与える数値\n例: 攻撃力+2 なら 2、防御力-1 なら -1")]
    private int m_effectValue = 1;

    [SerializeField]
    [Header("持続ターン数")]
    [Tooltip("この効果が持続するターン数\n0 で永続、1 で次のターン終了まで")]
    [Min(0)]
    private int m_durationTurns = 1;

    [SerializeField]
    [Header("個別アイコン（任意）")]
    [Tooltip("この効果専用のアイコンSprite\n未設定の場合は BattleIconSettings の汎用Buff/Debuffアイコンが使用されます")]
    private Sprite m_customIcon;

    /// <summary>効果名</summary>
    public string EffectName => m_effectName;

    /// <summary>効果の説明</summary>
    public string Description => m_description;

    /// <summary>効果種別（Buff / Debuff）</summary>
    public StatusEffectType EffectType => m_effectType;

    /// <summary>効果量</summary>
    public int EffectValue => m_effectValue;

    /// <summary>持続ターン数（0で永続）</summary>
    public int DurationTurns => m_durationTurns;

    /// <summary>個別アイコン（未設定の場合は null）</summary>
    public Sprite CustomIcon => m_customIcon;

#if UNITY_EDITOR
    /// <summary>
    /// Inspector編集時に数値を有効範囲へ補正します
    /// </summary>
    protected override void OnValidate()
    {
        base.OnValidate();
        m_durationTurns = Mathf.Max(0, m_durationTurns);
    }
#endif
}
