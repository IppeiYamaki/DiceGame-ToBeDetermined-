using UnityEngine;

/// 戦闘に参加するエンティティを表すインターフェース。<br />
public interface ICombatEntity { } // 戦闘に参加するエンティティを表すインターフェース。

/// ダイスアクションを表す抽象クラス。
public abstract class DiceAction : PersistentScriptableObject {
    public abstract void Execute(ICombatEntity user, ICombatEntity target, int diceValue);
}