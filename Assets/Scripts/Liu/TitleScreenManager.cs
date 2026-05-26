using UnityEngine;

public class TitleScreenManager : MonoBehaviour
{
    [Header("Rooms Configuration")]
    public GameObject titleScreenPanel; 
    public GameObject gamePlayPanel;    
    void Start()
    {
        if (titleScreenPanel != null) titleScreenPanel.SetActive(true);
        if (gamePlayPanel != null) gamePlayPanel.SetActive(false);
    }

   
    public void OnNewGameButtonClick()
    {
        if (titleScreenPanel != null) titleScreenPanel.SetActive(false); 
        if (gamePlayPanel != null) gamePlayPanel.SetActive(true);      

        Debug.Log("ダイス画面に飛びなした");
    }
}