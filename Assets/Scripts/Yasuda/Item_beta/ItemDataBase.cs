
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemDatabase", menuName = "Items/Item Database")]
public class ItemDataBase : MonoBehaviour
{
    public List<ItemData> items = new List<ItemData>();


    public void AddItem(ItemData item)
    {
        if (items.Contains(item))
            return;

        int newID = GetSmallestAvailableID();

        item.SetID(newID);

        items.Add(item);

#if UNITY_EDITOR
        UnityEditor.EditorUtility.SetDirty(this);
        UnityEditor.EditorUtility.SetDirty(item);
#endif
    }

    public void UseItem(ItemData item)
    {

    }



    public ItemData GetItemByID(int id)
    {
        foreach (var item in items)
        {
            if (item != null && item.ItemID == id)
            {
                return item;
            }
        }

        return null;
    }


    private int GetSmallestAvailableID()
    {
        HashSet<int> usedIDs = new HashSet<int>();

        foreach (var item in items)
        {
            if (item != null)
            {
                usedIDs.Add(item.ItemID);
            }
        }

        int id = 0;

        while (usedIDs.Contains(id))
        {
            id++;
        }

        return id;
    }
}
