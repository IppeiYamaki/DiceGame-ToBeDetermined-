using UnityEngine;

public class BattleFlow2 : MonoBehaviour
{
    [SerializeField]
    Player player;
    [SerializeField]
    Enemy enemy;
    [SerializeField]
    DemeChusen test;

    [SerializeField]
    DiceRole dr;

    [SerializeField]
    RandomDice rd1;
    [SerializeField]
    RandomDice rd2;
    [SerializeField]
    RandomDice rd3;

    private int[] deme = new int[3];//新しいサイコロ
    private bool turn = false;

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
            turn = true;

            //if(test.GetSyori())
            //{
            //    deme = test.GetDeme();
            //    Debug.Log("出目：" + deme[0] + "、" + deme[1] + "、" + deme[2]);
            //    enemy.SetDamege(deme[0] + deme[1] + deme[2]);
            //    test.ChangeSyori();
            //}
            //else
            //{
            //    test.ChangeSyori();
            //}

            rd1.ChangeState();
            rd2.ChangeState();
            rd3.ChangeState();
        }

        //if (Input.GetKeyDown(KeyCode.W))
        //{
        //    enemy.SetDamege(dr.GetValue());
        //}

        if(dr.GetAllStop()&&turn)
        {
            enemy.SetDamege(dr.GetValue());
            turn = false;

            if(!enemy.GetDie())
            {
                player.SetDamege(10);
            }
           
        }



    }
}
