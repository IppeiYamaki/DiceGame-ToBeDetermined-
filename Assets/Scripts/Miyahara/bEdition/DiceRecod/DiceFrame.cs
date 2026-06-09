using System;
using System.Collections.Generic;
using UnityEngine;
[Serializable]
public struct DiceFrame
{
    public Vector3 position;
    public Quaternion rotation;
    public float time;

    public DiceFrame(Vector3 pos, Quaternion rot, float t)
    {
        position = pos;
        rotation = rot;
        time = t;
    }
}

// 1ìäÇ≤Ç∆ÇÃò^âÊÉfÅ[É^
[CreateAssetMenu(fileName = "DiceRecording", menuName = "Recording/DiceRecording")]
public class DiceRecording : ScriptableObject
{
    public List<DiceFrame> frames = new List<DiceFrame>();
}