using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public enum NodeState
{
    Current,
    Selectable,
    Visited,
    Locked
}

public class MapNodeButton :
    MonoBehaviour,
    IPointerEnterHandler,
    IPointerExitHandler
{
    public int NodeId;
    public MapFlow mapFlow;

    [Header("UI")]
    [SerializeField]
    private Image nodeImage;

    [SerializeField]
    private Image cursorImage;

    [SerializeField]
    private Button button;

    [SerializeField]
    private Sprite[] nodeTypeSprites;

    static readonly Color[] TypeColors =
    {
        Color.white, // Start
        Color.white, // Battle
        Color.white, // Rest
        Color.white, // Event
        Color.white, // Treasure
        Color.white, // Item
        Color.white  // Boss
    };

    private const float MultVisited = 0.4f;
    private const float MultLocked = 0.25f;

    [Header("カーソル点滅")]
    [SerializeField]
    private float blinkSpeed = 3f;

    [SerializeField]
    private float blinkMinAlpha = 0.3f;

    [Header("拡大設定")]
    [Tooltip("キーボード選択時の拡大率")]
    [SerializeField]
    private float focusedScale = 1.3f;

    [Tooltip("マウスオン時の拡大率")]
    [SerializeField]
    private float hoverScale = 1.3f;

    [Tooltip("拡大・縮小の速さ")]
    [SerializeField]
    private float scaleSpeed = 12f;

    [Tooltip("Bossノードの通常倍率")]
    [SerializeField]
    private float bossScale = 3f;

    private NodeState currentState;
    private bool isFocused;
    private bool isHovered;
    private bool isClickable;

    private Vector3 targetScale = Vector3.one;

    private void Awake()
    {
        if (button == null)
        {
            button = GetComponent<Button>();
        }

        if (button != null)
        {
            button.onClick.RemoveListener(OnClick);
            button.onClick.AddListener(OnClick);
        }

        if (cursorImage != null)
        {
            cursorImage.color = Color.white;

            // カーソル画像がクリックを吸わないようにする
            cursorImage.raycastTarget = false;
        }

        if (nodeImage != null)
        {
            // Button本体でクリック判定するなら、
            // nodeImageはRaycast Target ONのままで構いません
            nodeImage.raycastTarget = true;
        }
    }

    private void OnDestroy()
    {
        if (button != null)
        {
            button.onClick.RemoveListener(OnClick);
        }
    }

    private void OnClick()
    {
        if (!isClickable)
        {
            return;
        }

        if (mapFlow == null)
        {
            Debug.LogWarning(
                $"Node {NodeId} のMapFlowが設定されていません。",
                this
            );

            return;
        }

        mapFlow.SelectNextNode(NodeId);
    }

    private void Update()
    {
        UpdateCursorBlink();
        UpdateScale();
    }

    private void UpdateCursorBlink()
    {
        if (
            currentState == NodeState.Current &&
            cursorImage != null &&
            cursorImage.gameObject.activeSelf
        )
        {
            float alpha = Mathf.Lerp(
                blinkMinAlpha,
                1f,
                (
                    Mathf.Sin(Time.unscaledTime * blinkSpeed) +
                    1f
                ) * 0.5f
            );

            cursorImage.color = new Color(
                1f,
                1f,
                1f,
                alpha
            );
        }
    }

    private void UpdateScale()
    {
        transform.localScale = Vector3.Lerp(
            transform.localScale,
            targetScale,
            Time.unscaledDeltaTime * scaleSpeed
        );
    }

    public void Refresh(
        NodeState state,
        NodeType nodeType,
        bool focused = false,
        bool clickable = false
    )
    {
        currentState = state;
        isFocused = focused;
        isClickable = clickable;

        if (cursorImage != null)
        {
            cursorImage.gameObject.SetActive(
                state == NodeState.Current
            );
        }

        if (button != null)
        {
            button.interactable = clickable;
        }

        int typeIndex = (int)nodeType;

        if (
            nodeImage != null &&
            nodeTypeSprites != null &&
            typeIndex >= 0 &&
            typeIndex < nodeTypeSprites.Length &&
            nodeTypeSprites[typeIndex] != null
        )
        {
            nodeImage.sprite = nodeTypeSprites[typeIndex];
        }

        Color baseColor =
            typeIndex >= 0 &&
            typeIndex < TypeColors.Length
                ? TypeColors[typeIndex]
                : Color.white;

        if (nodeImage != null)
        {
            nodeImage.color = state switch
            {
                NodeState.Current =>
                    baseColor,

                NodeState.Selectable =>
                    baseColor,

                NodeState.Visited =>
                    baseColor * MultVisited,

                _ =>
                    baseColor * MultLocked
            };
        }

        UpdateTargetScale(nodeType);
    }

    public void OnPointerEnter(
        PointerEventData eventData
    )
    {
        isHovered = true;

        UpdateTargetScale(
            GetCurrentNodeType()
        );
    }

    public void OnPointerExit(
        PointerEventData eventData
    )
    {
        isHovered = false;

        UpdateTargetScale(
            GetCurrentNodeType()
        );
    }

    private NodeType GetCurrentNodeType()
    {
        if (
            mapFlow != null &&
            mapFlow.Nodes != null &&
            mapFlow.Nodes.TryGetValue(
                NodeId,
                out MapNode node
            )
        )
        {
            return node.Type;
        }

        return NodeType.Start;
    }

    private void UpdateTargetScale(
        NodeType nodeType
    )
    {
        float baseScale =
            nodeType == NodeType.Boss
                ? bossScale
                : 1f;

        float additionalScale = 1f;

        if (isHovered)
        {
            additionalScale = hoverScale;
        }
        else if (isFocused)
        {
            additionalScale = focusedScale;
        }

        targetScale =
            Vector3.one *
            baseScale *
            additionalScale;
    }
}