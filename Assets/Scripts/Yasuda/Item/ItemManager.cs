using UnityEngine;
using static ItemDataBase;

public class ItemManager : MonoBehaviour
{
    [SerializeField]
    private ItemDataBase itemDatabase;

    private void Start()
    {
        int test = 0;
        int check = 0;
        while (check<11)
        {
            ItemData item = itemDatabase.GetItemByID(check);

            if (item != null)
            {
                Debug.Log(item.itemName +" : "+ item.description);
                test++;
            }
            else
            {
               
                //Debug.Log("“o˜^‚³‚ê‚Ä‚¢‚Ü‚¹‚ñ");
                //break;
            }
            check++;
        }
        Debug.Log("“o˜^‚³‚ê‚½ƒAƒCƒeƒ€”F" + test);


    }
}
