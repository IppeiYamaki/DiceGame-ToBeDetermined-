using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class DiceButtonUI : MonoBehaviour
{
    public int diceId;

    public float selectedMoveY = 40f;

    private RectTransform rectTransform;
    private Button button;
    private DiceSelectManager manager;

    private Vector2 originalPosition;
    private Vector2 targetPosition;

    public void Init(DiceSelectManager diceSelectManager)
    {
        manager = diceSelectManager;

        rectTransform = GetComponent<RectTransform>();
        button = GetComponent<Button>();

        originalPosition = rectTransform.anchoredPosition;
        targetPosition = originalPosition;

        button.onClick.AddListener(OnClickDice);
    }

    void Update()
    {
        rectTransform.anchoredPosition = Vector2.Lerp(
            rectTransform.anchoredPosition,
            targetPosition,
            Time.deltaTime * 12f
        );
    }

    void OnClickDice()
    {
        manager.SelectDice(this);
    }

    public void SetSelected(bool selected)
    {
        if (selected)
        {
            targetPosition = originalPosition + new Vector2(0, selectedMoveY);
        }
        else
        {
            targetPosition = originalPosition;
        }
    }
}