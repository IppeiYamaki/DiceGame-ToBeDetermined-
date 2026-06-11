public class Enemy3_AI : EnemyBattleAI
{
    public override string GetIntentText()
    {
        return "˜A‘±a‚è‚Ì\‚¦ (8~2)";
    }

    public override ActionResult ExecuteAction()
    {
        // 2‰ñ˜A‘±UŒ‚‚È‚Ì‚ÅHitCount‚Å“`‚¦‚é
        return new ActionResult
        {
            Damage = 8,
            HitCount = 2,
            IsAttack = true
        };
    }
}