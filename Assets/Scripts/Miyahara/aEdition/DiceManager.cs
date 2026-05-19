using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class DiceManager : MonoBehaviour
{
    public TMP_Text randomText;
    public GameObject slotPos;
    private int tapCount = 0;       // タップ回数をカウントする変数

    public bool isRotating = false; // 回転中かどうかを示すフラグ

    // ダイスの状態を表す列挙型
    public enum DiceState
    {
        Idle,
        Rotating,
        NextEvent
    }
    public DiceState diceState;

    private void Start()
    {
        randomText.text = "0";
        this.transform.position = slotPos.transform.position + new Vector3(2.3f, 1, 0);
        ActiveDice(false);
    }
    void Update()
    {
        // ダイスの状態に応じた処理を行う
        switch (diceState)
        {
            case DiceState.Idle:
                ActiveDice(false);
                break;
            case DiceState.Rotating:
                ActiveDice(true);
                DiceInput();
                DiceText();
                break;
            case DiceState.NextEvent:
                // 次のイベントに移る処理をここに記述
                tapCount = 0;
                ActiveDice(false);
                break;
        }
    }
    // ダイスの表示を切り替えるメソッド
    void ActiveDice(bool isDiceActive)
    {
        slotPos.SetActive(isDiceActive);
        randomText.enabled = isDiceActive;
    }
    // ダイスの入力処理を行うメソッド
    void DiceInput()
    {
        if (Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            tapCount++;
            if (tapCount <= 2)
            {
                // タップ回数が2回以下の場合、回転状態を切り替える
                isRotating = !isRotating;
            }
            else
            {
                diceState = DiceState.NextEvent;
            }

        }
    }
    // ダイスのテキストを更新するメソッド
    void DiceText()
    {
        if (isRotating)
        {
            int randomNumber = Random.Range(1, 6);
            randomText.text = randomNumber.ToString();
            Debug.Log("Random Number: " + randomNumber);
        }
    }

}
