using UnityEngine;

public class LootChest
{
    //宝箱用アイテムIDの取得するところ
    //仮で手打ちしておく
    public enum ItemId
    {
        Item_001,
        Item_002,
        Item_005,
        Item_006,
        Item_010
    }

    //アイテムIDの再代入を禁止
    private readonly ItemId[] m_itemTable =
    {
        ItemId.Item_001,
        ItemId.Item_002,
        ItemId.Item_005,
        ItemId.Item_006,
        ItemId.Item_010
    };


    private bool m_isOpened = false;


    //宝箱を開封したらランダムで1つ排出する
    public ItemId Open()
    {
        if (m_isOpened)
        {
            Debug.LogWarning("この宝箱は既に開封済みです。");
            return default;
        }

        m_isOpened = true;

        int randomIndex = Random.Range(0, m_itemTable.Length);
        ItemId lootRewards = m_itemTable[randomIndex];

        Debug.Log($"獲得アイテム : {lootRewards}");

        return lootRewards;
    }
}