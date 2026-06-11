// 全敵AIの基底クラス
public abstract class EnemyBattleAI
{
    public int CurrentHp;
    public int MaxHp;
    public int Attack;
    public string EnemyName;

    public int TurnCount = 1;

    // EnemyDataから初期化
    public virtual void Initialize(EnemyData data)
    {
        EnemyName = data.EnemyName;
        MaxHp = data.MaxHp;
        CurrentHp = data.MaxHp;
        Attack = data.Attack;
    }

    // ターンの行動結果
    public struct ActionResult
    {
        public int Damage;      // 1回あたりのダメージ
        public int HitCount;    // 攻撃回数（デフォルト1）
        public string IntentText;   // 予兆テキスト（UI表示用）
        public bool IsAttack;     // 攻撃アニメを出すか

        // 合計ダメージの計算用プロパティ
        public int TotalDamage => Damage * (HitCount <= 0 ? 1 : HitCount);
    }

    // 次のターンの予兆テキストを返す（ダイス選択フェーズで呼ぶ）
    public abstract string GetIntentText();

    // 実際に行動を実行する（ターン処理フェーズで呼ぶ）
    public abstract ActionResult ExecuteAction();

    // ターン終了時に呼ぶ
    public void EndTurn() => TurnCount++;

    // ダメージを受ける
    public bool TakeDamage(int damage)
    {
        CurrentHp -= damage;
        return CurrentHp <= 0; // 死亡したらtrue
    }


}

