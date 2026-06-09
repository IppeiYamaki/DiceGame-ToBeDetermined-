using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField]
    private int HP = 50;

    [SerializeField]
    SceneChangeManager manager;

    private bool die = false;

    private void Start()
    {
        die = false;
    }

    public void SetDamege(int damege)
    {
        HP -= damege;
        Debug.Log("“GHP " + HP);

        if (HP <= 0)
        {
            Debug.Log("Œ‚”j");
            die = true;
            Invoke("ChangeSceneManager", 4.0f);
            
        }
    }

    private void ChangeSceneManager()
    {
        manager.ChangeScene();
    }
    public bool GetDie()
    {
        return die;
    }
}
