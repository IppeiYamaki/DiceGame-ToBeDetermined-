using UnityEngine;
using static ItemDateBase;

public class ItemManager : MonoBehaviour
{
    [SerializeField]
    private ItemDateBase itemDatabase;

    private void Start()
    {
        ItemData item = itemDatabase.GetItemByID(0);

        if (item != null)
        {
            Debug.Log(item.itemName);
            Debug.Log(item.description);
        }
        else
        {
            Debug.LogError("A");
        }
    }
}
