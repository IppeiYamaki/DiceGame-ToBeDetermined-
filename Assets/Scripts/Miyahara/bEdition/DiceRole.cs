using NUnit.Framework;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

[System.Serializable]
public class DiceRecorderEvent
{
    public Vector3 DiceValue;
    public string recordingId1;
    public string recordingId2;
    public string recordingId3;
}

//ダイスの役を判定するクラス
public class DiceRole : MonoBehaviour
{
    [SerializeField] private int totalValue = 0;// ダイスの合計値
    private bool allStop = false;// ダイスが全て止まったかどうか
    public List<DiceRecorderEvent> recordingId = new List<DiceRecorderEvent>(); // 録画IDのリスト（Inspectorで設定）

    public List<RandomDice> getDiceValue = new List<RandomDice>();// ダイスの値を取得するためのリスト
    public List<int> diceValue = new List<int>();// ダイスの値を格納するリスト
    public TMP_Text totalValueText;// ダイスの合計値を表示するテキスト


    //各役の倍率
    [Header("ピンゾロ倍率")] public int Pinzoro = 10; // 1 が 3 つのときの倍率
    [Header("アラシイ倍率")] public int Arashii = 5; // 1 以外が 3 つのときの倍率
    [Header("ペア倍率")] public int Pair = 2; // ペアのときの倍率
    [Header("シゴロ倍率")] public int Shigoro = 3; // 4,5,6 がそろったときの倍率
    [Header("ヒフミ倍率")] public int Hifumi = 3; // 1,2,3 がそろったときの倍率
    [Header("偶数倍率")] public int Even = 3; // 異なる偶数がそろったときの倍率
    [Header("奇数倍率")] public int Odd = 3; // 異なる奇数がそろったときの倍率



    void Update()
    {
        
        allStop = true;
        // ダイスが全て止まっているかどうかを確認
        for (int i = 0; i < getDiceValue.Count; i++)
        {
            if (!getDiceValue[i].isStopped)
            {
                allStop = false;
                break;
            }
        }
        // ダイスが全て止まっている場合、合計値を計算して表示する
        if (allStop)
        {
            totalValue = 0;


            for (int i = 0; i < getDiceValue.Count; i++)
            {
                if (diceValue.Count < getDiceValue.Count)
                    diceValue.Add(getDiceValue[i].diceValue);
                else
                    diceValue[i] = getDiceValue[i].diceValue;

                totalValue += diceValue[i];
                getDiceValue[i].state = RandomDice.DiceState.NextEvent;
            }

            int multiplier = GetMultiplier(diceValue);
            totalValue *= multiplier;
            totalValueText.text = totalValue.ToString();
        }
    }
    int GetMultiplier(List<int> diceValue)
    {
        if (diceValue == null || diceValue.Count != 3)
            return 1;

        // ソートしたリストを用意（順番を気にしない判定用）
        List<int> s = new List<int>(diceValue);
        s.Sort();

        // 1. 全部同じか
        bool allSame = s[0] == s[1] && s[1] == s[2];

        // 1.1 1 が 3 つ
        if (allSame && s[0] == 1)
        {
            return Pinzoro; // 1×10 など好きな倍率
        }

        // 1.2 1 以外が 3 つ（2,3,4,5,6 のどれか 3 つ同じ）
        if (allSame && s[0] != 1)
        {
            return Arashii; // 1 以外が 3 つなら×5
        }
        bool hasPair = (s[0] == s[1]) || (s[1] == s[2]);

        if (hasPair)
        {
            return Pair; // ペア：×2（好きな倍率に変更）
        }

        // 2. 4,5,6 がそろった場合（順番不要）
        if (s[0] == 4 && s[1] == 5 && s[2] == 6)
        {
            return Shigoro;
        }

        // 3. 1,2,3 がそろった場合（順番不要）
        if (s[0] == 1 && s[1] == 2 && s[2] == 3)
        {
            return Hifumi;
        }

        // 4. 異なる偶数がそろった場合（2,4,6 のみ）
        bool allDifferent = s[0] != s[1] && s[1] != s[2] && s[0] != s[2];
        bool allEven = s[0] % 2 == 0 && s[1] % 2 == 0 && s[2] % 2 == 0;

        if (allDifferent && allEven)
        {
            return Even;
        }

        // 5. 異なる奇数がそろった場合（1,3,5 のみ）
        bool allOdd = s[0] % 2 != 0 && s[1] % 2 != 0 && s[2] % 2 != 0;

        if (allDifferent && allOdd)
        {
            return Odd;
        }

        // 条件に合わない場合は倍率 1
        return 1;
    }


}
