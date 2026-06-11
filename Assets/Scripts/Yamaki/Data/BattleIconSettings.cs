using UnityEngine;

/// <summary>
/// 戦闘UI用の汎用アイコン設定を保持する ScriptableObject
/// 攻撃、防御、Buff、Debuff、敵行動予測などのアイコンを一元管理します
/// 
/// 作成方法:
/// Project ビュー右クリック → Create → DiceGame/BattleIconSettings
/// 
/// 使い方:
/// 1. Inspector で各種アイコン Sprite を設定します
/// 2. BattleSceneController などから参照して UI へ適用します
/// 3. 個別アイコンが未設定の StatusEffectDefinition などは、ここの汎用アイコンを使用します
/// </summary>
[CreateAssetMenu(fileName = "BattleIconSettings", menuName = "DiceGame/BattleIconSettings")]
public class BattleIconSettings : ScriptableObject
{
    [SerializeField]
    [Header("攻撃アイコン")]
    [Tooltip("攻撃属性や敵の攻撃行動予測に使用する汎用アイコン")]
    private Sprite m_attackIcon;

    [SerializeField]
    [Header("防御アイコン")]
    [Tooltip("防御属性や敵の防御行動予測に使用する汎用アイコン")]
    private Sprite m_defenseIcon;

    [SerializeField]
    [Header("汎用Buffアイコン")]
    [Tooltip("個別アイコン未設定のBuff効果に使用する汎用アイコン（例: 上向き矢印）")]
    private Sprite m_genericBuffIcon;

    [SerializeField]
    [Header("汎用Debuffアイコン")]
    [Tooltip("個別アイコン未設定のDebuff効果に使用する汎用アイコン（例: 下向き矢印）")]
    private Sprite m_genericDebuffIcon;

    [SerializeField]
    [Header("不明行動アイコン")]
    [Tooltip("敵の行動が不明な場合に使用する汎用アイコン（例: はてなマーク）")]
    private Sprite m_unknownActionIcon;

    /// <summary>攻撃アイコン</summary>
    public Sprite AttackIcon => m_attackIcon;

    /// <summary>防御アイコン</summary>
    public Sprite DefenseIcon => m_defenseIcon;

    /// <summary>汎用Buffアイコン</summary>
    public Sprite GenericBuffIcon => m_genericBuffIcon;

    /// <summary>汎用Debuffアイコン</summary>
    public Sprite GenericDebuffIcon => m_genericDebuffIcon;

    /// <summary>不明行動アイコン</summary>
    public Sprite UnknownActionIcon => m_unknownActionIcon;

    /// <summary>
    /// BattleActionType に対応する汎用アイコンを取得します
    /// </summary>
    /// <param name="actionType">行動種別</param>
    /// <returns>対応するアイコン（設定されていない場合は null）</returns>
    public Sprite GetIconForActionType(BattleActionType actionType)
    {
        switch (actionType)
        {
            case BattleActionType.Attack:
                return m_attackIcon;
            case BattleActionType.Defense:
                return m_defenseIcon;
            case BattleActionType.Buff:
                return m_genericBuffIcon;
            case BattleActionType.Debuff:
                return m_genericDebuffIcon;
            case BattleActionType.Unknown:
            default:
                return m_unknownActionIcon;
        }
    }

    /// <summary>
    /// StatusEffectDefinition の表示用アイコンを取得します
    /// CustomIcon が設定されていればそちらを、未設定なら汎用Buff/Debuffアイコンを返します
    /// </summary>
    /// <param name="effectDefinition">バフ/デバフ定義</param>
    /// <returns>表示用アイコン（設定されていない場合は null）</returns>
    public Sprite GetIconForStatusEffect(StatusEffectDefinition effectDefinition)
    {
        if (effectDefinition == null) return null;

        if (effectDefinition.CustomIcon != null)
        {
            return effectDefinition.CustomIcon;
        }

        return effectDefinition.EffectType == StatusEffectType.Buff ? m_genericBuffIcon : m_genericDebuffIcon;
    }
}
