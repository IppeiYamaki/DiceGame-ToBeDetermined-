using UnityEngine;

public class ChestManager : MonoBehaviour
{
    // どれか1つでも宝箱が開かれたか
    private bool m_isAnyChestOpened = false;

    // 宝箱を開けられるか
    public bool CanOpenChest()
    {
        return !m_isAnyChestOpened;
    }

    // 宝箱が開かれたことを記録
    public void OpenedChest()
    {
        m_isAnyChestOpened = true;
    }
}