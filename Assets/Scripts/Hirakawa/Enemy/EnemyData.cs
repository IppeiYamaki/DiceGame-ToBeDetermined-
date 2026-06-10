using UnityEngine;

[CreateAssetMenu(fileName = "EnemyData", menuName = "Game/EnemyData")]
public class EnemyData : ScriptableObject
{
    public string InternalId;   // "ENEMYのID"
    public string EnemyName;    // "ENEMYの名前"
    public Sprite Icon;         // アイコン画像
    public int MaxHp;           // 最大HP
    public int Attack;          // 攻撃力

    // このEnemyDataに対応するAIクラス名
    // 例："Enemy1AI"
    public string AIClassName;   // 対応するAIクラス名
}