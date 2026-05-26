
using UnityEngine;
using UnityEngine.SceneManagement;

#if UNITY_EDITOR
using UnityEditor;
#endif
public class SceneChangeManager : MonoBehaviour
{
    #if UNITY_EDITOR
    [SerializeField]
    private SceneAsset nextScene;
#endif

    // 実行用にシーン名を保存
    [SerializeField]
    private string sceneName;

#if UNITY_EDITOR
    // Inspector変更時に自動更新
    private void OnValidate()
    {
        if (nextScene != null)
        {
            sceneName = nextScene.name;
        }
    }
#endif

    public void ChangeScene()
    {
        SceneManager.LoadScene(sceneName);
    }
}
