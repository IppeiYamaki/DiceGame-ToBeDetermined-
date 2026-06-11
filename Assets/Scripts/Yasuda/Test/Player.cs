using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField]
    private int HP = 100;
   


    public void SetDamege(int damege)
    {
        HP -= damege;
        Debug.Log("ƒvƒŒƒCƒ„[HP " + HP);
    }


    void Update()
    {
        
    }
}
