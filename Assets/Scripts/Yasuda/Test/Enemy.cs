using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField]
    private int HP = 50;

    [SerializeField]
    SceneChangeManager manager;

    public void SetDamege(int damege)
    {
        HP -= damege;
        Debug.Log("“GHP " + HP);

        if (HP <= 0)
        {
            Debug.Log("Œ‚”j");

            Invoke("ChangeSceneManager", 4.0f);
            
        }
    }

    private void ChangeSceneManager()
    {
        manager.ChangeScene();
    }
}
