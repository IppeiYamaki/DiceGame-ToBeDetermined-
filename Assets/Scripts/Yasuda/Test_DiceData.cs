using Unity.VisualScripting;
using UnityEngine;

public class Test_DiceData : MonoBehaviour
{
 
    

    private int[,] n_diceData = new int[3, 6];
    private int[,] o_diceData = new int[6, 6];

    void Start()
    {
        for(int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 6; j++)
            {
                n_diceData[i, j] = Random.Range(0, 6);
                Debug.Log(n_diceData[i, j]+"AA"+i+j);
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

    public int GetNumber(int g, int id, int n)
    {
        if (g == 0)
        {
            return n_diceData[id, n];
        }
        return o_diceData[id, n];
    }
   
}
