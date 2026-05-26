using UnityEngine;
using static ItemDataBase;

public class ItemManager : MonoBehaviour
{
    [SerializeField]
    private ItemDataBase itemDatabase;

    private void Start()
    {
        int test = 0;
        while (test<10)
        {
            ItemData item = itemDatabase.GetItemByID(test);

            if (item != null)
            {
                Debug.Log(item.itemName);
                Debug.Log(item.description);
            }
            else
            {
                Debug.Log("“o˜^‚³‚ê‚½ƒAƒCƒeƒ€”F" + test);
                //Debug.Log("“o˜^‚³‚ê‚Ä‚¢‚Ü‚¹‚ñ");
                break;
            }
            test++;
        }
      

        
    }
}
