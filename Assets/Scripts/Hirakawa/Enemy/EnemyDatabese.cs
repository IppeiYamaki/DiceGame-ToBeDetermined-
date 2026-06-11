using UnityEngine;

[CreateAssetMenu(fileName = "EnemyDatabase", menuName = "Game/EnemyDatabase")]
public class EnemyDatabase : ScriptableObject
{
    public EnemyData[] Enemies;

    public EnemyData GetRandom()
    {
        if (Enemies == null || Enemies.Length == 0) return null;
        return Enemies[Random.Range(0, Enemies.Length)];
    }
}