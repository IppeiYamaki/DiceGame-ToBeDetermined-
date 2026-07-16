using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleScreenController : MonoBehaviour
{
    [SerializeField] UnityEditor.SceneAsset battleScene;

    public void OnClickNewGame()
    {
        if (battleScene != null)
        {
            string scenePath = UnityEditor.AssetDatabase.GetAssetPath(battleScene);
            string sceneName = System.IO.Path.GetFileNameWithoutExtension(scenePath);
            SceneManager.LoadScene(sceneName);
        }
        else
        {
            Debug.LogError("Battle scene is not assigned.");
        }
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