using UnityEngine;

public class DiceFaceView : MonoBehaviour
{
    [Header("1～6のダイス画像")]
    public Sprite face1;
    public Sprite face2;
    public Sprite face3;
    public Sprite face4;
    public Sprite face5;
    public Sprite face6;

    private SpriteRenderer[] spriteRenderers;

    void Awake()
    {
        spriteRenderers = GetComponentsInChildren<SpriteRenderer>(true);
    }

    public void SetFace(int value)
    {
        Debug.Log("SetFace called : " + value);

        Sprite targetSprite = GetSprite(value);

        if (targetSprite == null)
        {
            Debug.LogWarning("ダイス画像が設定されていません：" + value);
            return;
        }

        if (spriteRenderers == null || spriteRenderers.Length == 0)
        {
            spriteRenderers = GetComponentsInChildren<SpriteRenderer>(true);
        }

        foreach (SpriteRenderer renderer in spriteRenderers)
        {
            renderer.gameObject.SetActive(true);
            renderer.sprite = targetSprite;
        }
    }

    Sprite GetSprite(int value)
    {
        switch (value)
        {
            case 1:
                return face1;
            case 2:
                return face2;
            case 3:
                return face3;
            case 4:
                return face4;
            case 5:
                return face5;
            case 6:
                return face6;
            default:
                return null;
        }
    }
}