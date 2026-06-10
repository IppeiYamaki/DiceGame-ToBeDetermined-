public class Enemy2_AI : EnemyBattleAI
{
    public override string GetIntentText()
    {
        int mod = TurnCount % 3;
        return mod switch
        {
            1 => "•sˆÓ‘Å‚¿‚Ì\‚¦ (12)",
            2 => "“±‰Îü‚É‰Î‚ð‚Â‚¯‚½I (0)",
            0 => "”š’e“Š±‚Ì—\’›I (22)",
            _ => ""  
        };
    }

    public override ActionResult ExecuteAction()
    {
        int mod = TurnCount % 3;

        return mod switch
        {
            1  => new ActionResult { Damage = 12, IsAttack = true },
            2 => new ActionResult { Damage = 0, IsAttack = true },
            0 => new ActionResult { Damage = 22, IsAttack = false },
            _ => new ActionResult { Damage = 0, IsAttack = false }
        };
    }
}