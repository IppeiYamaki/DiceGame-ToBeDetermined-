public class Enemy1_AI : EnemyBattleAI
{
    public override string GetIntentText()
    {
        int mod = TurnCount % 4;
        return mod switch
        {
            1 or 2 => "突進の構え (9)",
            3 => "エネルギーを溜めている！ (0)",
            0 => "大突進の予兆！ (15)",
            _ => ""
        };
    }

    public override ActionResult ExecuteAction()
    {
        int mod = TurnCount % 4;

        return mod switch
        {
            1 or 2 => new ActionResult { Damage = 5, IsAttack = true },
            3 => new ActionResult { Damage = 0, IsAttack = false },
            0 => new ActionResult { Damage = 10, IsAttack = true },
            _ => new ActionResult { Damage = 0, IsAttack = false }
        };
    }
}