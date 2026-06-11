/// <summary>
/// 戦闘中にユニットへ付与されているバフ/デバフのランタイム状態
/// StatusEffectDefinition と残りターン数を保持します
/// 
/// 使い方:
/// - 付与時に new ActiveStatusEffect(definition) で生成します
/// - ターン終了時に RemainingTurns を減算し、0 以下になったらリストから除去します
/// - DurationTurns が 0 の定義は永続効果として扱います
/// </summary>
public class ActiveStatusEffect
{
    /// <summary>元になったバフ/デバフ定義</summary>
    public StatusEffectDefinition Definition { get; }

    /// <summary>残り持続ターン数（永続の場合は使用しない）</summary>
    public int RemainingTurns { get; set; }

    /// <summary>永続効果かどうか（DurationTurns が 0 の場合）</summary>
    public bool IsPermanent => Definition != null && Definition.DurationTurns <= 0;

    public ActiveStatusEffect(StatusEffectDefinition definition)
    {
        Definition = definition;
        RemainingTurns = definition != null ? definition.DurationTurns : 0;
    }
}
