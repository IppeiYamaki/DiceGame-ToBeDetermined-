using Unity.VisualScripting;
using UnityEngine;

public class Test_DiceData : MonoBehaviour
{
    //テストで使うサイコロの面情報を記録する

    private int[,] n_diceData = new int[3, 6];//新しいサイコロ
    private int[,] o_diceData = new int[6, 6];//持ってるサイコロ

    void Start()
    {
        for(int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 6; j++)
            {
                n_diceData[i, j] = Random.Range(0, 6);
                Debug.Log(n_diceData[i, j] + "AA" + i + j);
            }
           
        }
        for (int i = 0; i < 6; i++)
        {
            for (int j = 0; j < 6; j++)
            {
                o_diceData[i, j] = Random.Range(0, 6);
                Debug.Log(o_diceData[i, j] + "AA" + i + j);
            }

        }
    }

    //面情報取得
    public int GetNumber(int g, int id, int n)
    {
        if (g == 0)
        {
            return n_diceData[id, n];
        }
        return o_diceData[id, n];
    }
   
    //持っているサイコロを新しいサイコロに書き換え
    public void SetNumber_Test(int nid, int oid)
    {
        for(int i = 0;i < 6;i++)
        {
            o_diceData[oid, i] = n_diceData[nid, i];
            //Debug.Log($"{o_diceData[oid, i]}");
        }
        
    }
}
