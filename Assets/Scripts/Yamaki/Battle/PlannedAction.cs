using System;
using UnityEngine;

/// <summary>
/// 予約された行動1件を表します。
/// </summary>
public class PlannedAction
{
    public enum PlannedActionKind
    {
        Attack,
        Defense,
        Skill,
        Item
    }

    public PlannedActionKind Kind { get; private set; }

    /// <summary>消費AP（予約時に確保される値）。</summary>
    public int Cost { get; private set; }

    /// <summary>対象タイプ（Self/SingleEnemy/AllEnemies/RandomEnemy）。</summary>
    public ActionTargetType TargetType { get; private set; }

    /// <summary>SingleEnemyのときに参照するターゲット。</summary>
    public EnemyUnitRuntime TargetEnemy { get; private set; }

    /// <summary>スキル参照（Kind==Skill のとき使用）。</summary>
    public SkillDefinition Skill { get; private set; }

    /// <summary>アイテム参照（Kind==Item のとき使用）。</summary>
    public ItemData Item { get; private set; }

    /// <summary>アイテム効果エントリ（Kind==Item のとき使用）。</summary>
    public ItemEffectEntry ItemEffect { get; private set; }

    /// <summary>表示ラベル（UI表示用）。</summary>
    public string Label { get; private set; }

    public PlannedAction(PlannedActionKind kind, int cost, ActionTargetType targetType, string label = "")
    {
        Kind = kind;
        Cost = cost;
        TargetType = targetType;
        Label = label;
    }

    public static PlannedAction CreateAttack(int cost, EnemyUnitRuntime target)
    {
        var pa = new PlannedAction(PlannedActionKind.Attack, cost, ActionTargetType.SingleEnemy, $"Attack {cost}");
        pa.TargetEnemy = target;
        return pa;
    }

    public static PlannedAction CreateDefense(int cost)
    {
        return new PlannedAction(PlannedActionKind.Defense, cost, ActionTargetType.Self, $"Defense {cost}");
    }

    public static PlannedAction CreateSkill(SkillDefinition skill, EnemyUnitRuntime target = null)
    {
        var pa = new PlannedAction(PlannedActionKind.Skill, skill != null ? skill.Cost : 0, skill != null ? skill.TargetType : ActionTargetType.Self, $"{(skill!=null?skill.SkillName:"Skill")} ({(skill!=null?skill.Cost:0)})");
        pa.Skill = skill;
        pa.TargetEnemy = target;
        return pa;
    }

    public static PlannedAction CreateItem(ItemData item, ItemEffectEntry effectEntry, EnemyUnitRuntime target = null)
    {
        ActionTargetType targetType = effectEntry != null ? effectEntry.TargetType : ActionTargetType.Self;
        string label = item != null ? item.itemName : "Item";
        var pa = new PlannedAction(PlannedActionKind.Item, 0, targetType, label);
        pa.Item = item;
        pa.ItemEffect = effectEntry;
        pa.TargetEnemy = target;
        return pa;
    }
}
