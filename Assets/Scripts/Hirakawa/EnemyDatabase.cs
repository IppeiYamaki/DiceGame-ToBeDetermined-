using UnityEngine;

[CreateAssetMenu(fileName = "EnemyDatabase", menuName = "DiceGame/EnemyDatabase")]
public class EnemyDatabase : ScriptableObject
{
    public EnemyDefinition[] Enemies;

    public EnemyDefinition GetRandom()
    {
        if (Enemies == null || Enemies.Length == 0) return null;
        return Enemies[Random.Range(0, Enemies.Length)];
    }
}