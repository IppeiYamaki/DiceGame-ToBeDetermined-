using System;

/// <summary>
/// 戦闘中の敵1体のランタイム状態を保持する軽量クラス。
/// MonoBehaviourではなく純データコンテナとして使用します。
/// </summary>
public class EnemyUnitRuntime
{
    /// <summary>敵の定義データ。</summary>
    public EnemyDefinition Definition { get; private set; }

    /// <summary>現在HP。</summary>
    public int CurrentHp { get; private set; }

    /// <summary>現在のBlock値。</summary>
    public int Block { get; private set; }

    /// <summary>次回の行動候補。</summary>
    public EnemyWeightedActionGroup NextActionGroup { get; set; }

    public EnemyUnitRuntime(EnemyDefinition definition)
    {
        Definition = definition;
        CurrentHp = definition != null ? definition.MaxHp : 0;
        Block = 0;
        NextActionGroup = null;
    }

    /// <summary>
    /// ダメージを適用し、残HPを返します。
    /// Blockによる軽減を考慮してBlockを減らします。
    /// </summary>
    public int ApplyDamage(int damage)
    {
        int safe = Math.Max(0, damage);
        BlockDamageResult result = BattleCalculator.CalculateBlockDamage(safe, Block);
        Block = result.RemainingBlock;
        int actual = Math.Max(0, CurrentHp - result.Damage);
        CurrentHp = Math.Max(0, CurrentHp - result.Damage);
        return actual;
    }

    /// <summary>
    /// 指定量のBlockを追加する。
    /// </summary>
    public void AddBlock(int value)
    {
        Block += Math.Max(0, value);
    }

    /// <summary>
    /// 敵が生存しているか。
    /// </summary>
    public bool IsAlive => CurrentHp > 0;
}
