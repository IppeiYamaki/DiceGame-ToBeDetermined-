using UnityEngine;

[CreateAssetMenu(fileName = "NewItem", menuName = "Items/Item")]
public class ItemData : ScriptableObject
{
    [SerializeField]
    private int itemID;//

    public int ItemID => itemID;

    public string itemName;

    [TextArea]
    public string description;

    public Texture icon;

    

    // Database‘¤‚¾‚¯‚ªŒÄ‚Ô
    public void SetID(int id)
    {
        itemID = id;
    }


}
