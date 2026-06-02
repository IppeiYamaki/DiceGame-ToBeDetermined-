using UnityEngine;

public class BattleFlow2 : MonoBehaviour
{
    [SerializeField]
    Player player;
    [SerializeField]
    Enemy enemy;
    [SerializeField]
    DemeChusen test;

    private int[] deme = new int[3];//新しいサイコロ

    void Start()
    {
        for (int i = 0; i < deme.Length; i++)
        { deme[i] = 0; }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if(test.GetSyori())
            {
                deme = test.GetDeme();
                Debug.Log("出目：" + deme[0] + "、" + deme[1] + "、" + deme[2]);
                enemy.SetDamege(deme[0] + deme[1] + deme[2]);
                test.ChangeSyori();
            }
            else
            {
                test.ChangeSyori();
            }


               

            


            
        }
        if (Input.GetKeyDown(KeyCode.V))
        {
            player.SetDamege(10);
        }
    }
}
