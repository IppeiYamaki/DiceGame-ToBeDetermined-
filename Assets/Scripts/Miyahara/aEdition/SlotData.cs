using System;
using TMPro;
using UnityEngine;

[System.Serializable]  // これが大事！Inspectorで表示可能に
public class SlotData
{
    public bool isSelectedDice; // ダイスが選択されているかどうかを示すフラグ
    public TMP_Text randomText;
    public GameObject slotPos;
    public GameObject allDise;

}