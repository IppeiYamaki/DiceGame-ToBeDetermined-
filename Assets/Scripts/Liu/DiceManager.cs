//using UnityEngine;
//using System.Collections.Generic;

//public class DiceManager : MonoBehaviour
//{

//    public List<DiceItem> allDices;


//    public void StartGame()
//    {
//        foreach (DiceItem dice in allDices)
//        {
//            if (dice != null) dice.ResetDice(); 
//        }
//    }

using UnityEngine;
using System.Collections.Generic;

public class DiceManager : MonoBehaviour
{
    public GameObject selectionPanel;
    public GameObject lotteryPanel;
    public List<DiceItem> allDices;
    public List<GameObject> lotteryUIImages;

    public void OnConfirmButtonClick()
    {
        int selectedIndex = -1;
        
        for (int i = 0; i < allDices.Count; i++)
        {
            if (allDices[i] != null && allDices[i].IsRaised)
            {
                selectedIndex = i;
                break;
            }
        }

  
        if (selectedIndex != -1)
        {
            selectionPanel.SetActive(false);
            lotteryPanel.SetActive(true);
            for (int i = 0; i < lotteryUIImages.Count; i++)
            {
                lotteryUIImages[i].SetActive(i == selectedIndex);
            }
            allDices[selectedIndex].PrepareForLottery();
        }
        else
        {
            Debug.Log("ダイス選択されていません");
        }
    }
} 