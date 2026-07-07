using UnityEngine;

/// <summary>
/// アイコン表示に使用する Sprite とラベル文字列の組み合わせです。
/// </summary>
public class IconDisplayData
{
    public Sprite IconSprite { get; }
    public string LabelText { get; }

    public IconDisplayData(Sprite iconSprite, string labelText)
    {
        IconSprite = iconSprite;
        LabelText = labelText;
    }
}
