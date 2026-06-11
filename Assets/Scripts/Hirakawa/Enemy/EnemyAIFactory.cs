public static class EnemyAIFactory
{
    public static EnemyBattleAI Create(EnemyData data)
    {
        EnemyBattleAI ai = data.AIClassName switch
        {
            "Enemy1_AI" => new Enemy1_AI(),
            // “G‚ª‘‚¦‚½‚ç‚±‚±‚É’Ç‰Á
            "Enemy2_AI" => new Enemy2_AI(),
            "Enemy3_AI" => new Enemy3_AI(),
            _ => null
        };

        ai?.Initialize(data);
        return ai;
    }
}