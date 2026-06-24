using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DiceEventManager : MonoBehaviour
{
    [Header("UI")]
    public GameObject eventPanel;
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI descriptionText;
    public TextMeshProUGUI diceResultText;
    public Button rollButton;
    public Button skipButton;

    [Header("Player")]
    public PlayerStatus playerStatus;

    [Header("Reward Dice")]
    public DiceDefinition rewardDice;

    private bool hasRolled = false;

    void Start()
    {
        titleText.text = " 運命のダイスイベント発生！";

        descriptionText.text =
            "手札のダイスを1回だけ振ります。\n" +
            "何かしらの数字の役が成立すれば新ダイスを獲得！\n" +
            "役に満たない場合は、わなが発動し【10ダメージ】を受けます！";

        diceResultText.text = "";

        rollButton.onClick.AddListener(RollDice);
        skipButton.onClick.AddListener(SkipEvent);
    }

    void RollDice()
    {
        if (hasRolled)
        {
            return;
        }

        hasRolled = true;
        rollButton.interactable = false;

        int dice1 = Random.Range(1, 7);
        int dice2 = Random.Range(1, 7);
        int dice3 = Random.Range(1, 7);

        diceResultText.text = $"結果：{dice1}・{dice2}・{dice3}";

        bool isSuccess = CheckRole(dice1, dice2, dice3);

        if (isSuccess)
        {
            SuccessEvent();
        }
        else
        {
            FailedEvent();
        }
    }

    bool CheckRole(int d1, int d2, int d3)
    {
        // 3つのダイスのうち、2つ以上同じ数字なら役成立
        if (d1 == d2 || d2 == d3 || d1 == d3)
        {
            return true;
        }

        return false;
    }

    void SuccessEvent()
    {
        diceResultText.text += "\n役成立！新ダイス獲得！";

        if (rewardDice != null)
        {
            diceResultText.text += "\n獲得ダイス：" + rewardDice.DiceName;
        }
    }

    void FailedEvent()
    {
        diceResultText.text += "\n役に満たない！10ダメージ！";

        if (playerStatus != null)
        {
            playerStatus.TakeDamage(10);
        }
    }

    void SkipEvent()
    {
        eventPanel.SetActive(false);
    }
}