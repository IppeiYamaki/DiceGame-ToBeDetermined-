using NUnit.Framework;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class DiceRole_2 : MonoBehaviour
{
    [SerializeField] private int totalValue = 0;
    private bool allStop = false;
    public List<RandomDice_2> getDiceValue = new List<RandomDice_2>();
    public List<int> diceValue = new List<int>();
    public TMP_Text totalValueText;

    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        allStop = true;
        for (int i = 0; i < getDiceValue.Count; i++)
        {
            if (!getDiceValue[i].isStopped)
            {
                allStop = false;
                break;
            }
        }
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
                getDiceValue[i].state = RandomDice_2.DiceState.NextEvent;
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
            return 10; // 1×10 など好きな倍率
        }

        // 1.2 1 以外が 3 つ（2,3,4,5,6 のどれか 3 つ同じ）
        if (allSame && s[0] != 1)
        {
            return 5; // 1 以外が 3 つなら×5
        }
        bool hasPair = (s[0] == s[1]) || (s[1] == s[2]);

        if (hasPair)
        {
            return 2; // ペア：×2（好きな倍率に変更）
        }

        // 2. 4,5,6 がそろった場合（順番不要）
        if (s[0] == 4 && s[1] == 5 && s[2] == 6)
        {
            return 3;
        }

        // 3. 1,2,3 がそろった場合（順番不要）
        if (s[0] == 1 && s[1] == 2 && s[2] == 3)
        {
            return 3;
        }

        // 4. 異なる偶数がそろった場合（2,4,6 のみ）
        bool allDifferent = s[0] != s[1] && s[1] != s[2] && s[0] != s[2];
        bool allEven = s[0] % 2 == 0 && s[1] % 2 == 0 && s[2] % 2 == 0;

        if (allDifferent && allEven)
        {
            return 3;
        }

        // 5. 異なる奇数がそろった場合（1,3,5 のみ）
        bool allOdd = s[0] % 2 != 0 && s[1] % 2 != 0 && s[2] % 2 != 0;

        if (allDifferent && allOdd)
        {
            return 3;
        }

        // 条件に合わない場合は倍率 1
        return 1;
    }



}
