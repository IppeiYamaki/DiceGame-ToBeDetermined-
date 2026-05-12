using UnityEngine;
using System.Collections.Generic;
using System.Threading.Tasks;

public class BattleFlow : MonoBehaviour
{
    public enum BattleStep { Draw, Select, Roll, Result, EnemyTurn, EnemyAction, CheckStatus }
    private BattleStep _currentStep;

   
    // [①] ダイスデータ/デッキ
    // [②] ダイス選択UI
    // [③] スロット演出
    // [④] バトル計算ロジック

    int PlayerHP = 100; // 仮のHP値。実際はプレイヤーデータから取得
    int EnemyHP = 50; // 仮のHP値。実際は敵データから取得

    async void Start()
    {
        await BattleLoop();
    }

    private async Task BattleLoop()
    {
        bool isBattleActive = true;

        while (isBattleActive)
        {
            // ⓪ ドローフェーズ (6つから4つ抽出)
            // ここで [①] のデッキデータからランダムに4つ取得するコード
            Debug.Log("デッキから4つのダイスをドロー");

            // ① 行動選択 (4つから3つ選択)
            _currentStep = BattleStep.Select;
            // [②] のUIを表示し、3つ選ばれるまで待機
            // await DiceSelectionUI.Instance.WaitForSelect(3); 
            Debug.Log("3つのダイスを選択中...");
            //await Task.Delay(2000); // 仮の待機

            // ② ロール演出
            _currentStep = BattleStep.Roll;
            // [③] のスロットアニメーション演出を呼び出し、終わるまで待つ
            // await SlotAnimation.Instance.PlayRollAnimation();
            Debug.Log("ダイスロール中...");
            // await Task.Delay(1000);

            // ③ プレイヤー結果表示・処理
            _currentStep = BattleStep.Result;
            // [④] の計算ロジックから最終ダメージを取得
            // int damage = BattleCalculator.Calculate(selectedDice);
            int damage = 20;//一旦固定値としてDamegeを使用。実際は[④]のロジックから取得


            await ShowActionEffects($"プレイヤーの攻撃！ {damage}のダメージ！");

            EnemyHP -= damage;


            if(EnemyHP < 0) EnemyHP = 0;


            if (IsEnemyDead())
            {
                TransitionToWin();
                break;
            }



            // ④ 敵のターン
            _currentStep = BattleStep.EnemyTurn;
            Debug.Log("敵のターン思考中...");
            //await Task.Delay(1000);

            // ⑤ 敵行動結果の表示
            _currentStep = BattleStep.EnemyAction;


            int EnemyDamage = 15; //仮のダメージ値。実際は敵データから取得

            // [④] 敵の計算ロジック

            await ShowActionEffects($"敵の攻撃！ {EnemyDamage}のダメージ！");


            PlayerHP -= EnemyDamage;
            if (PlayerHP < 0) PlayerHP = 0;
            Debug.Log($"プレイヤーの残りHP: {PlayerHP}");




            if (IsPlayerDead())
            {
                TransitionToGameOver();
                break;
            }
            
        }
    }

    private async Task ShowActionEffects(string message)
    {
        Debug.Log(message);
        // ここで演出時間分待機。一応いろんなところに付けておきますけど必要ない場合削除
        //await Task.Delay(1500);
    }

    private bool IsEnemyDead() => EnemyHP <= 0;
    private bool IsPlayerDead() => PlayerHP <= 0;
    private void TransitionToWin() => Debug.Log("勝利画面へ");
    private void TransitionToGameOver() => Debug.Log("ゲームオーバー画面へ");
}

