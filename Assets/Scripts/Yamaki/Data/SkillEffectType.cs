/// <summary>
/// スキル効果の種別。
/// </summary>
public enum SkillEffectType
{
    /// <summary>効果なし。</summary>
    None,

    /// <summary>プレイヤーを回復する。</summary>
    Heal,

    /// <summary>敵へダメージを与える。</summary>
    Damage,

    /// <summary>プレイヤーのBlockを増やす。</summary>
    Block,

    /// <summary>ActionPointを増やす。</summary>
    GainActionPoint,

    /// <summary>バフ/デバフ（StatusEffectDefinition）を付与する。</summary>
    ApplyStatusEffect
}
