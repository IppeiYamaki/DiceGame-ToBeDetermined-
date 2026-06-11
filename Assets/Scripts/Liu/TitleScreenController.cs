using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleScreenController : MonoBehaviour
{
    public void OnClickNewGame()
    {
        SceneManager.LoadScene("NewScene");
    }

    public void OnClickHowTo()
    {
        Debug.Log("操作方法ボタンが押されました");
    }

    public void OnClickSetting()
    {
        Debug.Log("設定ボタンが押されました");
    }
}