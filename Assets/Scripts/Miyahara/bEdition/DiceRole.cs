/*
--------------------------------------------------------
DiceRole の処理の流れ

① DrawRole()
    ↓
    現在のサイコロで作れる役だけを抽選する

② RoleValue()
    ↓
    抽選された役になる候補の出目を生成する

③ CanAssign()
    ↓
    実際に各サイコロで出せる組み合わせだけを残す

④ ランダムで1つ選び、
   roleValue に設定する

⑤ サイコロ停止後、
   GetRole() と GetMultiplier() により
   実際の役と倍率を判定する
--------------------------------------------------------
*/
using System.Collections.Generic;
using TMPro;
using UnityEngine;


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
    public List<GameObject> diceObjects = new List<GameObject>();// ダイスのオブジェクトを格納するリスト
    public TMP_Text totalValueText;// ダイスの合計値を表示するテキスト
    public Vector3 rolevalue; // ダイスの役の値を格納する変数
    public bool isPlaying = true; // 再生中かどうかを判定するフラグ



    public Role currentRole = Role.None;

    //各役の倍率
    [Header("ピンゾロ倍率")] public int Pinzoro = 10; // 1 が 3 つのときの倍率
    [Header("アラシ倍率")] public int Arashii = 5; // 1 以外が 3 つのときの倍率
    [Header("ペア倍率")] public int Pair = 2; // ペアのときの倍率
    [Header("シゴロ倍率")] public int Shigoro = 3; // 4,5,6 がそろったときの倍率
    [Header("ヒフミ倍率")] public int Hifumi = 3; // 1,2,3 がそろったときの倍率
    [Header("偶数倍率")] public int Even = 3; // 異なる偶数がそろったときの倍率
    [Header("奇数倍率")] public int Odd = 3; // 異なる奇数がそろったときの倍率


    public enum Role
    {
        None,
        Pinzoro,
        Arashi,
        Pair,
        Shigoro,
        Hifumi,
        Even,
        Odd
    }

    [System.Serializable]
    public class DicePattern
    {
        public int[] values;
        public Role role;
    }

    // 全216通りの出目
    private List<DicePattern> allPatterns = new List<DicePattern>();

    void Start()
    {
        BuildRolePatterns();
        RollDice();
        Debug.Log(currentRole);
    }
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
            RollDice();
            Debug.Log(currentRole);
        }
    }


    /// <summary>
    /// サイコロ3個の出目から役を判定する。
    /// ここが役判定の基準となるため、新しい役を追加する場合はこの関数を編集する。
    /// </summary>
    /// <param name="diceValue">サイコロ3個の出目</param>
    /// <returns>成立した役</returns>
    Role GetRole(List<int> diceValue)
    {
        if (diceValue == null || diceValue.Count != 3)
            return Role.None;

        // 判定しやすいように昇順に並べ替える
        List<int> s = new List<int>(diceValue);
        s.Sort();

        // 全て同じ数字か
        bool allSame = s[0] == s[1] && s[1] == s[2];

        // 1.1 1 が 3 つ
        if (allSame && s[0] == 1)
        {
            return Role.Pinzoro; // 1×10 など好きな倍率
        }

        // 1.2 1 以外が 3 つ（2,3,4,5,6 のどれか 3 つ同じ）
        if (allSame && s[0] != 1)
        {
            return Role.Arashi; // 1 以外が 3 つなら×5
        }
        // 2つだけ同じ数字か（ペア判定）
        bool hasPair = (s[0] == s[1]) || (s[1] == s[2]);

        if (hasPair)
        {
            return Role.Pair; // ペア：×2（好きな倍率に変更）
        }

        // 2. 4,5,6 がそろった場合（順番不要）
        if (s[0] == 4 && s[1] == 5 && s[2] == 6)
        {
            return Role.Shigoro;
        }

        // 3. 1,2,3 がそろった場合（順番不要）
        if (s[0] == 1 && s[1] == 2 && s[2] == 3)
        {
            return Role.Hifumi;
        }

        // 全て異なる数字か
        bool allDifferent = s[0] != s[1] && s[1] != s[2] && s[0] != s[2];
        bool allEven = s[0] % 2 == 0 && s[1] % 2 == 0 && s[2] % 2 == 0;

        if (allDifferent && allEven)
        {
            return Role.Even;
        }

        // 5. 異なる奇数がそろった場合（1,3,5 のみ）
        bool allOdd = s[0] % 2 != 0 && s[1] % 2 != 0 && s[2] % 2 != 0;

        if (allDifferent && allOdd)
        {
            return Role.Odd;
        }

        // 条件に合わない場合は倍率 1
        return Role.None;
    }
    /// <summary>
    /// 役に応じた倍率を取得する。
    /// 役の判定自体は GetRole() に任せ、この関数では倍率のみ返す。
    /// </summary>
    int GetMultiplier(List<int> diceValue)
    {
        switch (GetRole(diceValue))
        {
            case Role.Pinzoro:
                return Pinzoro;

            case Role.Arashi:
                return Arashii;

            case Role.Pair:
                return Pair;

            case Role.Shigoro:
                return Shigoro;

            case Role.Hifumi:
                return Hifumi;

            case Role.Even:
                return Even;

            case Role.Odd:
                return Odd;

            default:
                return 1;
        }
    }


    void BuildRolePatterns()
    {
        allPatterns.Clear();

        for (int i = 0; i < getDiceValue[0].DiceFace.Count; i++)
        {
            for (int j = 0; j < getDiceValue[1].DiceFace.Count; j++)
            {
                for (int k = 0; k < getDiceValue[2].DiceFace.Count; k++)
                {
                    List<int> dice = new List<int>()
                {
                    getDiceValue[0].DiceFace[i],
                    getDiceValue[1].DiceFace[j],
                    getDiceValue[2].DiceFace[k]
                };

                    DicePattern pattern = new DicePattern();

                    pattern.values = new int[]
                    {
                    dice[0],
                    dice[1],
                    dice[2]
                    };

                    pattern.role = GetRole(dice);

                    allPatterns.Add(pattern);
                }
            }
        }
    }

    public void RollDice()
    {
        int index = Random.Range(0, allPatterns.Count);

        DicePattern pattern = allPatterns[index];

        rolevalue = new Vector3(
            pattern.values[0],
            pattern.values[1],
            pattern.values[2]);

        currentRole = pattern.role;
    }

}
