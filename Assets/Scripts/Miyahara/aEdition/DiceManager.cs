using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class DiceManager : MonoBehaviour
{
    public GameObject selectionPanel;
    [SerializeField]private List<SlotData> slotDataList; // SlotDataのリスト
    public TMP_Text GetRondomText (int index) => slotDataList[index].randomText; // SlotDataからrandomTextを取得するメソッド
    public GameObject GetSlotPos(int index) => slotDataList[index].slotPos; // SlotDataからslotPosを取得するメソッド
    public bool GetIsSelectedDice(int index) => slotDataList[index].isSelectedDice; // SlotDataからisSelectedDiceを取得するメソッド
    public GameObject GetAllDices(int index) => slotDataList[index].allDise; // SlotDataからallDicesを取得するメソッド

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
        for (int i = 0; i < slotDataList.Count; i++)
        {
            slotDataList[i].randomText.text = "0";
            this.transform.position = slotDataList[i].slotPos.transform.position + new Vector3(2.3f, 1, 0);
            slotDataList[i].isSelectedDice = false;
        }

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
        for (int i = 0; i < slotDataList.Count; i++)
        {
            if (slotDataList[i].isSelectedDice == true)
            {
                slotDataList[i].slotPos.SetActive(isDiceActive);
                slotDataList[i].randomText.enabled = isDiceActive;
            }
            else
            {
                slotDataList[i].slotPos.SetActive(false);
                slotDataList[i].randomText.enabled = false;
            }


        }
        selectionPanel.SetActive(!isDiceActive);
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

            for (int i = 0; i < slotDataList.Count; i++)
            {
                int randomNumber = Random.Range(1, 6);
                slotDataList[i].randomText.text = randomNumber.ToString();
                Debug.Log("Random Number: " + randomNumber);
            }

        }
    }
    public void OnConfirmButtonClick()
    {

        for (int i = 0; i < slotDataList.Count; i++)
        {
            if (slotDataList[i].allDise != null && slotDataList[i].allDise.GetComponent<DiceItem>().IsRaised)
            {
                slotDataList[i].isSelectedDice = true;
                diceState = DiceState.Rotating;
            }
            else
            {
                slotDataList[i].isSelectedDice = false;
                Debug.Log("ダイス選択されていません");
            }
        }



    }
}
