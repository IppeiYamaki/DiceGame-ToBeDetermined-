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
    public class RoleCard
    {
        public Role role;
        public int count;
    }
    public List<RoleCard> roleCards;
    void Start()
    {
        currentRole = DrawRole();
        rolevalue = RoleValue(currentRole);
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
            currentRole = DrawRole();
            rolevalue = RoleValue(currentRole);
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
    /// <summary>
    /// 現在のサイコロ構成で実際に作ることができる役のみを抽選する。
    /// 作れない役は抽選対象から除外される。
    /// </summary>
    public Role DrawRole()
    {
        // 現在のサイコロで実現可能な役だけを保存
        List<RoleCard> available = new List<RoleCard>();

        foreach (RoleCard card in roleCards)
        {
            if (CanMakeRole(card.role))
            {
                available.Add(card);
            }
        }

        if (available.Count == 0)
            return Role.None;

        int total = 0;

        foreach (RoleCard card in available)
        {
            total += card.count;
        }

        int rand = Random.Range(0, total);

        foreach (RoleCard card in available)
        {
            if (rand < card.count)
                return card.role;

            rand -= card.count;
        }

        return Role.None;
    }

    /// <summary>
    /// 指定した役になる出目を生成する。
    /// 実際に各サイコロで出すことのできる組み合わせのみ返す。
    /// </summary>
    public Vector3 RoleValue(Role role)
    {
        // 指定した役になる代表パターンを取得
        List<int[]> patterns = GeneratePatterns(role);

        // 現在のサイコロで実現可能な出目だけ保存する
        List<int[]> success = new List<int[]>();

        foreach (int[] pattern in patterns)
        {
            int[] assign;

            if (CanAssign(pattern, out assign))
            {
                success.Add(assign);
            }
        }

        // 実現できる組み合わせが1つも無い
        if (success.Count == 0)
        {
            return Vector3.zero;
        }

        // 実現可能な組み合わせの中からランダムに選ぶ
        int[] result = success[Random.Range(0, success.Count)];

        return new Vector3(
            result[0],
            result[1],
            result[2]
        );
    }


    /// <summary>
    /// 指定した役が現在の3つのサイコロで作れるか判定する。
    /// DrawRole()で抽選可能か判断するために使用する。
    /// </summary>
    bool CanMakeRole(Role role)
    {
        List<int[]> patterns = GeneratePatterns(role);

        foreach (int[] p in patterns)
        {
            int[] assign;

            if (CanAssign(p, out assign))
                return true;
        }

        return false;
    }


    /// <summary>
    /// 指定した出目を現在の3つのサイコロで実現できるか判定する。
    /// 実現できる場合は各サイコロに割り当てる数字(assign)も返す。
    /// </summary>
    /// <param name="target">実現したい出目</param>
    /// <param name="assign">
    /// 実際に各サイコロへ割り当てる数字
    /// 例：{6,5,4}
    /// </param>
    /// <returns>実現可能ならtrue</returns>
    bool CanAssign(int[] target, out int[] assign)
    {
        assign = null;
        // サイコロ3個への数字の割り当て順（3! = 6通り）
        int[][] orders =
        {
        new[]{0,1,2},
        new[]{0,2,1},
        new[]{1,0,2},
        new[]{1,2,0},
        new[]{2,0,1},
        new[]{2,1,0}
    };
        // 全ての並び順を試す
        foreach (var order in orders)
        {
            if (getDiceValue[0].DiceFace.Contains(target[order[0]]) &&
               getDiceValue[1].DiceFace.Contains(target[order[1]]) &&
               getDiceValue[2].DiceFace.Contains(target[order[2]]))
            {
                assign = new int[]
                {
                target[order[0]],
                target[order[1]],
                target[order[2]]
                };

                return true;
            }
        }

        return false;
    }
    /// <summary>
    /// 指定した役になる代表パターンを全て生成する。
    /// 並び順は生成せず、CanAssign()で実際の並び替えを行う。
    /// 例：
    /// Pair → 112
    /// Shigoro → 456
    /// Hifumi → 123
    /// </summary>
    List<int[]> GeneratePatterns(Role role)
    {
        List<int[]> result = new List<int[]>();

        for (int a = 1; a <= 6; a++)
        {
            for (int b = a; b <= 6; b++)
            {
                for (int c = b; c <= 6; c++)
                {
                    List<int> dice = new List<int>()
                {
                    a,b,c
                };

                    if (GetRole(dice) == role)
                    {
                        result.Add(new int[]
                        {
                        a,b,c
                        });
                    }
                }
            }
        }

        return result;
    }

}
