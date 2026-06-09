using System.IO;
using UnityEditor;
using UnityEngine;

public static class DiceRecordingAssetUtility
{
    // .json Ç©ÇÁ ScriptableObject Ç∆ÇµÇƒçƒçÏê¨
    public static bool RebuildDiceRecordingAsset(string jsonPath, string assetPath)
    {
        if (!File.Exists(jsonPath))
        {
            Debug.LogError("JSON file not found: " + jsonPath);
            return false;
        }

        string json = File.ReadAllText(jsonPath);
        DiceRecording so = ScriptableObject.CreateInstance<DiceRecording>();
        JsonUtility.FromJsonOverwrite(json, so);

        AssetDatabase.CreateAsset(so, assetPath);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log("DiceRecording Asset created: " + assetPath);
        return true;
    }
}