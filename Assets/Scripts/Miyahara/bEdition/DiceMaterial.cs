using UnityEngine;

public class DiceMaterial : MonoBehaviour
{
    [SerializeField] private Sprite[] spriteFaces;   // 目1~6 に対応する Sprite[] （0~5）
    [SerializeField] private int faceIndex = 0;      // どの目を表示するか（0~5）
    [SerializeField] private MeshRenderer meshRenderer;

    void Start()
    {
        if (meshRenderer == null)
            meshRenderer = GetComponent<MeshRenderer>();

        ApplySpriteToMaterial(faceIndex);
    }

    void ApplySpriteToMaterial(int spriteIndex)
    {
        if (spriteIndex < 0 || spriteIndex >= spriteFaces.Length)
            return;

        Sprite sprite = spriteFaces[spriteIndex];
        Texture2D tex = sprite.texture;

        Vector2 offset = new Vector2(
            sprite.textureRect.x / (float)tex.width,
            sprite.textureRect.y / (float)tex.height
        );
        Vector2 scale = new Vector2(
            sprite.textureRect.width / (float)tex.width,
            sprite.textureRect.height / (float)tex.height
        );

        Material mat = meshRenderer.material;
        mat.mainTexture = tex;
        mat.mainTextureOffset = offset;
        mat.mainTextureScale = scale;
    }

}
