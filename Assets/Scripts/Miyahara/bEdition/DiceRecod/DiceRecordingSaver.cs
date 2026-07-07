using System.IO;
using UnityEngine;

public static class DiceRecordingSaver
{
    // 1つのDiceRecordingを.jsonファイルに保存
    public static bool SaveToJSON(string filePath, DiceRecording recording)
    {
        // 保存先ディレクトリを自動作成
        string dir = Path.GetDirectoryName(filePath);
        if (!Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);
        }

        string json = JsonUtility.ToJson(recording, true);  // true で整形（可読性高め）

        try
        {
            File.WriteAllText(filePath, json);
            Debug.Log("DiceRecording saved to: " + filePath);
            return true;
        }
        catch (System.Exception e)
        {
            Debug.LogError("Failed to save DiceRecording to JSON: " + e.Message);
            return false;
        }
    }


    public static DiceRecording LoadFromJSONFile(string filePath)
    {
        if (!File.Exists(filePath))
        {
            Debug.LogError("DiceRecordingSaver: ファイルが見つかりません: " + filePath);
            return null;
        }

        string json = File.ReadAllText(filePath);
        DiceRecording rec = ScriptableObject.CreateInstance<DiceRecording>();
        JsonUtility.FromJsonOverwrite(json, rec);
        return rec;
    }
}
