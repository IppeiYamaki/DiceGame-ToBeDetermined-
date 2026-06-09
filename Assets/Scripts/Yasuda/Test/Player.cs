using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField]
    private int HP = 100;
   


    public void SetDamege(int damege)
    {
        HP -= damege;
        Debug.Log("プレイヤーHP " + HP);

        if(HP <= 0 )
        {
            Debug.Log("プレイヤーは死んだ");
        }
    }


    void Update()
    {
        
    }
}
