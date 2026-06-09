
using TMPro;
using UnityEngine;


public class DemeChusen : MonoBehaviour
{
    //[SerializeField]
    //private TMP_Text demeText1;
    //[SerializeField]
    //private TMP_Text demeText2;
    //[SerializeField]
    //private TMP_Text demeText3;

    [SerializeField]
    private DiceHyouzi dh;


    [SerializeField]
    private Test_DiceData diceData;

    private int[] test = new int[3];

    private bool Syori = false;

    public void DemeEnsyutu()
    {
        for (int i = 0; i < test.Length; i++)
        {
            int a = Random.Range(0, 6);
            test[i] = diceData.GetNumber(1, i, a);//0`5‚ð•Ô‚·
            dh.ShowDetail(i, test[i]);
            test[i] += 1;//1‘«‚·‚±‚Æ‚Å‚½‚¾‚µ‚¢”’l‚É‚È‚é
        }

        //demeText1.text = test[0].ToString();
        //demeText2.text = test[1].ToString();
        //demeText3.text = test[2].ToString();
    }


    public int[] GetDeme()
    {
        return test;
    }

    public void ChangeSyori()
    {
        Syori ^= true;
    }

    public bool GetSyori()
    {
        return Syori;
    }

    void Update()
    {
        if(Syori)
        {
            DemeEnsyutu();
        }
    }
}
