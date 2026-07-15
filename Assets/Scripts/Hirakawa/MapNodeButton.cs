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
    [SerializeField] Button button;
    [SerializeField] Sprite[] nodeTypeSprites;

    // NodeTypeに対応する色を定義します。インデックスはNodeTypeの順序に対応しています。
    //上から順に画像を差し込む感じですね。
    //まあ画像差し込むので見えないかもしれませんがね

    //嘘です。画像データそのままの色にするため全部白にしときます
    static readonly Color[] TypeColors = new Color[]
      {
            new Color(1.0f, 1.0f, 1.0f), // Start
            new Color(1.0f, 1.0f, 1.0f), // Battle
            new Color(1.0f, 1.0f, 1.0f), // Rest  
            new Color(1.0f, 1.0f, 1.0f), // Event
            new Color(1.0f, 1.0f, 1.0f), // Treasure
            new Color(1.0f, 1.0f, 1.0f), // Item   → 灰
            new Color(1.0f, 1.0f, 1.0f), // Boss   → オレンジ

      };

    static readonly float MultVisited = 0.4f;
    static readonly float MultLocked = 0.25f;
    static readonly float MultTwoStepAhead = 0.6f; // 2マス先の明るさ
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
    public void Refresh(
     NodeState state,
     NodeType nodeType,
     bool focused = false,
     bool isClickable = false
 )
    {
        currentState = state;
        isFocused = focused;

        if (cursorImage != null)
        {
            cursorImage.gameObject.SetActive(
                state == NodeState.Current
            );
        }

        if (button != null)
        {
            button.interactable = isClickable;
        }

        int typeIndex = (int)nodeType;

        if (
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

        nodeImage.color = state switch
        {
            NodeState.Current => baseColor,
            NodeState.Selectable => isClickable ? baseColor : baseColor * MultTwoStepAhead,
            NodeState.Visited => baseColor * MultVisited,
            _ => baseColor * MultLocked
        };

        if (nodeType == NodeType.Boss)
        {
            transform.localScale = Vector3.one * 3f;
        }
        else
        {
            transform.localScale =
                isFocused
                ? Vector3.one * focusedScale
                : Vector3.one;
        }
    }
}