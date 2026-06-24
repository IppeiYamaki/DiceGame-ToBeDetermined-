using UnityEngine;
using UnityEngine.UI;

public enum NodeState
{
    Current,
    Selectable,
    Visited,
    Locked
}

public class MapNodeButton : MonoBehaviour
{
    public int NodeId;
    public MapFlow mapFlow;

    [SerializeField] Image nodeImage;
    [SerializeField] Image cursorImage;
    [SerializeField] Image enemyIconImage; // 追加：敵アイコン表示用
    [SerializeField] Button button;
    [SerializeField] Sprite[] nodeTypeSprites;

    // NodeTypeに対応する色を定義します。インデックスはNodeTypeの順序に対応しています。
    //上から順に画像を差し込む感じですね。
    //まあ画像差し込むので見えないかもしれませんがね
    static readonly Color[] TypeColors = new Color[]
      {
            new Color(0.3f, 0.8f, 0.3f), // Start  → 緑
            new Color(0.9f, 0.3f, 0.3f), // Battle → 赤
            new Color(0.3f, 0.6f, 0.9f), // Rest   → 青
            new Color(0.9f, 0.8f, 0.2f), // Event   → 黄
            new Color(1.0f, 0.5f, 0.0f), // Boss   → オレンジ

      };

    static readonly float MultVisited = 0.4f;
    static readonly float MultLocked = 0.25f;

    [SerializeField] float blinkSpeed = 3f;
    [SerializeField] float blinkMinAlpha = 0.3f;
    [SerializeField] float focusedScale = 1.3f;

    NodeState currentState;
    bool isFocused;

    void Awake()
    {
        button.onClick.AddListener(OnClick);

        if (cursorImage != null)
            cursorImage.color = Color.white;


        // 起動時はアイコンを非表示にしておく
        if (enemyIconImage != null)
            enemyIconImage.gameObject.SetActive(false);
    }

    void OnClick()
    {
        mapFlow.SelectNextNode(NodeId);
    }

    void Update()
    {
        if (currentState == NodeState.Current && cursorImage != null && cursorImage.gameObject.activeSelf)
        {
            float alpha = Mathf.Lerp(blinkMinAlpha, 1f, (Mathf.Sin(Time.time * blinkSpeed) + 1f) * 0.5f);
            cursorImage.color = new Color(1f, 1f, 1f, alpha);
        }
    }

    // enemyDataを受け取るようにRefreshを変更
    public void Refresh(NodeState state, NodeType nodeType, bool focused = false, EnemyDefinition enemyData = null, bool showEnemyIcon = false, bool dimIcon = false)
    {
        currentState = state;
        isFocused = focused;

        cursorImage.gameObject.SetActive(state == NodeState.Current);
        button.interactable = (state == NodeState.Selectable);

        int typeIndex = (int)nodeType;
        if (nodeTypeSprites != null && typeIndex < nodeTypeSprites.Length && nodeTypeSprites[typeIndex] != null)
            nodeImage.sprite = nodeTypeSprites[typeIndex];

        Color baseColor = TypeColors[typeIndex];
        nodeImage.color = state switch
        {
            NodeState.Current => baseColor,
            NodeState.Selectable => baseColor,
            NodeState.Visited => baseColor * MultVisited,
            _ => baseColor * MultLocked
        };

        transform.localScale = isFocused ? Vector3.one * focusedScale : Vector3.one;

        // アイコン表示
        if (enemyIconImage != null)
        {
            if (showEnemyIcon && enemyData != null)
            {
                Sprite iconSprite = enemyData.VisualSprite;

                // VisualSpriteが無ければVisualTextureから動的生成
                if (iconSprite == null && enemyData.VisualTexture != null)
                {
                    var tex = enemyData.VisualTexture;
                    iconSprite = Sprite.Create(
                        tex,
                        new Rect(0, 0, tex.width, tex.height),
                        new Vector2(0.5f, 0.5f)
                    );
                }

                if (iconSprite != null)
                {
                    enemyIconImage.gameObject.SetActive(true);
                    enemyIconImage.sprite = iconSprite;
                    enemyIconImage.color = dimIcon
                        ? new Color(0.3f, 0.3f, 0.3f, 1f)
                        : Color.white;
                }
                else
                {
                    enemyIconImage.gameObject.SetActive(false);
                }
            }
            else
            {
                enemyIconImage.gameObject.SetActive(false);
            }
        }
    }
}