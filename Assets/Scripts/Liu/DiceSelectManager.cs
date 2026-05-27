using UnityEngine;
using UnityEngine.UI;

public class DiceSelectManager : MonoBehaviour
{
    public DiceButtonUI[] diceButtons;

    public Button decideButton;

    private DiceButtonUI selectedDice;

    void Start()
    {
        for (int i = 0; i < diceButtons.Length; i++)
        {
            diceButtons[i].Init(this);
        }

        decideButton.onClick.AddListener(OnDecide);
        decideButton.interactable = false;
    }

    public void SelectDice(DiceButtonUI dice)
    {
        if (selectedDice != null)
        {
            selectedDice.SetSelected(false);
        }

        selectedDice = dice;
        selectedDice.SetSelected(true);

        decideButton.interactable = true;
    }

    void OnDecide()
    {
        if (selectedDice == null)
        {
            Debug.Log("まだダイスを選択していません");
            return;
        }

        Debug.Log("選択したダイスID：" + selectedDice.diceId);
        Debug.Log("回転フェーズへ移行します");
    }
}