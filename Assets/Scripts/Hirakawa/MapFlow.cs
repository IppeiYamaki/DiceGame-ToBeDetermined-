using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

// =========================
// 型定義
// =========================
public enum NodeType
{
    Start,
    Battle,
    Rest,
    Event,
    Treasure,
    Item,
    Boss
}

public enum MapDirection
{
    Horizontal,
    Vertical
}

// =========================
// レイヤー別敵データベース
// =========================
[System.Serializable]
public class LayerEnemyDatabase
{
    [Tooltip("このデータベースを使用し始めるレイヤー")]
    public int minLayer;

    [Tooltip("このデータベースを使用する最後のレイヤー")]
    public int maxLayer;

    [Tooltip("この範囲で使用する敵データベース")]
    public EnemyDatabase enemyDatabase;
}

// =========================
// マップデータ
// =========================
[System.Serializable]
public class MapNode
{
    public int Id;
    public int Layer;
    public int IndexInLayer;
    public NodeType Type;
    public Vector2 Position;
    public int Lane;
    public List<int> NextNodeIds = new();

    public EnemyDefinition Enemy;
}

public class MapGraph
{
    public Dictionary<int, MapNode> Nodes = new();
    public int StartNodeId;
    public int BossNodeId;
}

// =========================
// 固定マップ生成
// =========================
public static class MapGenerator
{
    public static MapGraph Generate(
        MapDirection direction = MapDirection.Horizontal,
        EnemyDatabase enemyDatabase = null,
        List<LayerEnemyDatabase> layerEnemyDatabases = null
    )
    {
        var graph = new MapGraph();
        int nextId = 0;

        float layerSpacing = 130f;
        float laneSpacing = 400f;

        // 固定マップ構成
        var layerDefs = new List<List<(NodeType type, int lane)>>
        {
            // L0
            new()
            {
                (NodeType.Start, 0)
            },

            // L1
            new()
            {
                (NodeType.Battle, 0)
            },

            // L2
            new()
            {
                (NodeType.Battle, 0),
                (NodeType.Battle, 1)
            },

            // L3
            new()
            {
                (NodeType.Treasure, 0)
            },

            // L4
            new()
            {
                (NodeType.Battle, 0),
                (NodeType.Battle, 1)
            },

            // L5
            new()
            {
                (NodeType.Rest, 0),
                (NodeType.Event, 1)
            },

            // L6
            new()
            {
                (NodeType.Treasure, 0)
            },

            // L7
            new()
            {
                (NodeType.Battle, 0),
                (NodeType.Battle, 1)
            },

            // L8
            new()
            {
                (NodeType.Event, 0),
                (NodeType.Rest, 1)
            },

            // L9
            new()
            {
                (NodeType.Boss, 0)
            }
        };

        var layers = new List<List<MapNode>>();

        // =========================
        // ノード生成
        // =========================
        for (int layer = 0; layer < layerDefs.Count; layer++)
        {
            var layerNodes = new List<MapNode>();
            var definitions = layerDefs[layer];

            int count = definitions.Count;
            float centerOffset = (count - 1) / 2f;

            for (int i = 0; i < count; i++)
            {
                var (type, lane) = definitions[i];

                float along = layer * layerSpacing;
                float cross = (i - centerOffset) * laneSpacing;

                float x =
                    direction == MapDirection.Horizontal
                        ? along
                        : cross;

                float y =
                    direction == MapDirection.Horizontal
                        ? cross
                        : along;

                var node = new MapNode
                {
                    Id = nextId++,
                    Layer = layer,
                    Lane = lane,
                    IndexInLayer = i,
                    Type = type,
                    Position = new Vector2(x, y)
                };

                // Battleノードの敵を抽選
                if (type == NodeType.Battle)
                {
                    EnemyDatabase selectedDatabase =
                        GetEnemyDatabaseForLayer(
                            layer,
                            enemyDatabase,
                            layerEnemyDatabases
                        );

                    if (selectedDatabase != null)
                    {
                        node.Enemy = selectedDatabase.GetRandom();
                    }
                    else
                    {
                        Debug.LogWarning(
                            $"Layer {layer} に使用できるEnemyDatabaseがありません。"
                        );
                    }
                }

                graph.Nodes[node.Id] = node;
                layerNodes.Add(node);
            }

            layers.Add(layerNodes);
        }

        // =========================
        // 接続
        // =========================

        // L0 → L1
        Connect(layers[0][0], layers[1][0]);

        // L1 → L2
        Connect(layers[1][0], layers[2][0]);
        Connect(layers[1][0], layers[2][1]);

        // L2 → L3
        Connect(layers[2][0], layers[3][0]);
        Connect(layers[2][1], layers[3][0]);

        // L3 → L4
        Connect(layers[3][0], layers[4][0]);
        Connect(layers[3][0], layers[4][1]);

        // L4 → L5
        Connect(layers[4][0], layers[5][0]);
        Connect(layers[4][1], layers[5][1]);

        // L5 → L6
        Connect(layers[5][0], layers[6][0]);
        Connect(layers[5][1], layers[6][0]);

        // L6 → L7
        Connect(layers[6][0], layers[7][0]);
        Connect(layers[6][0], layers[7][1]);

        // L7 → L8（交差）
        Connect(layers[7][0], layers[8][1]);
        Connect(layers[7][1], layers[8][0]);

        // L8 → L9
        Connect(layers[8][0], layers[9][0]);
        Connect(layers[8][1], layers[9][0]);

        graph.StartNodeId = layers[0][0].Id;
        graph.BossNodeId = layers[9][0].Id;

        CenterGraph(graph);

        return graph;
    }

    private static EnemyDatabase GetEnemyDatabaseForLayer(
        int layer,
        EnemyDatabase defaultDatabase,
        List<LayerEnemyDatabase> layerEnemyDatabases
    )
    {
        if (layerEnemyDatabases != null)
        {
            foreach (var setting in layerEnemyDatabases)
            {
                if (setting == null)
                    continue;

                if (setting.enemyDatabase == null)
                    continue;

                if (
                    layer >= setting.minLayer &&
                    layer <= setting.maxLayer
                )
                {
                    return setting.enemyDatabase;
                }
            }
        }

        return defaultDatabase;
    }

    private static void Connect(MapNode from, MapNode to)
    {
        if (!from.NextNodeIds.Contains(to.Id))
        {
            from.NextNodeIds.Add(to.Id);
        }
    }

    private static void CenterGraph(MapGraph graph)
    {
        if (graph.Nodes.Count == 0)
            return;

        float minX = float.MaxValue;
        float maxX = float.MinValue;
        float minY = float.MaxValue;
        float maxY = float.MinValue;

        foreach (var node in graph.Nodes.Values)
        {
            minX = Mathf.Min(minX, node.Position.x);
            maxX = Mathf.Max(maxX, node.Position.x);
            minY = Mathf.Min(minY, node.Position.y);
            maxY = Mathf.Max(maxY, node.Position.y);
        }

        Vector2 center = new Vector2(
            (minX + maxX) * 0.5f,
            (minY + maxY) * 0.5f
        );

        foreach (var node in graph.Nodes.Values)
        {
            node.Position -= center;
        }
    }
}

// =========================
// マップ進行管理
// =========================
public class MapFlow : MonoBehaviour
{
    public int CurrentNodeId;
    public Dictionary<int, MapNode> Nodes;
    public MapGraph graph;

    private HashSet<int> visitedNodeIds = new();

    private readonly Dictionary<int, MapNodeButton> nodeButtons = new();
    private readonly Dictionary<int, RectTransform> nodeRects = new();

    private int selectableIndex;
    private bool isPlayerMoving;

    // =========================
    // マップUI
    // =========================
    [Header("マップUI")]

    [SerializeField]
    private GameObject nodePrefab;

    [SerializeField]
    private RectTransform parent;

    // =========================
    // プレイヤー画像
    // =========================
    [Header("プレイヤー表示")]

    [Tooltip("マップ上に表示するプレイヤー画像")]
    [SerializeField]
    private RectTransform playerImage;

    [Tooltip("プレイヤー画像が次のノードへ移動する時間")]
    [SerializeField]
    private float playerMoveDuration = 0.35f;

    [Tooltip("ノード位置からプレイヤー画像をずらす量")]
    [SerializeField]
    private Vector2 playerPositionOffset = Vector2.zero;

    // =========================
    // 敵抽選
    // =========================
    [Header("敵抽選設定")]

    [Tooltip("レイヤー別設定がない場合に使用する敵データベース")]
    [SerializeField]
    private EnemyDatabase enemyDatabase;

    [Tooltip("レイヤーごとに使用する敵データベース")]
    [SerializeField]
    private List<LayerEnemyDatabase> layerEnemyDatabases = new();

    // =========================
    // 初期化
    // =========================
    private void Awake()
    {
        if (!MapSession.HasData)
        {
            graph = MapGenerator.Generate(
                direction: MapDirection.Horizontal,
                enemyDatabase: enemyDatabase,
                layerEnemyDatabases: layerEnemyDatabases
            );

            MapSession.Graph = graph;
            MapSession.CurrentNodeId = graph.StartNodeId;

            MapSession.VisitedNodeIds.Clear();
            MapSession.VisitedNodeIds.Add(graph.StartNodeId);
        }
        else
        {
            graph = MapSession.Graph;
        }

        Nodes = graph.Nodes;
        CurrentNodeId = MapSession.CurrentNodeId;
        visitedNodeIds = MapSession.VisitedNodeIds;
    }

    private void Start()
    {
        DebugAllNodes();
        DebugDrawConnections();

        ClearGeneratedMapObjects();

        nodeButtons.Clear();
        nodeRects.Clear();

        CreateUI();
        RefreshAllNodes();

        // 保存されている現在地へプレイヤー画像を配置
        MovePlayerImmediately(CurrentNodeId);
    }

    private void ClearGeneratedMapObjects()
    {
        foreach (Transform child in parent)
        {
            // プレイヤー画像は削除しない
            if (
                playerImage != null &&
                child == playerImage.transform
            )
            {
                continue;
            }

            Destroy(child.gameObject);
        }
    }

    public MapNode CurrentNode
    {
        get
        {
            return Nodes[CurrentNodeId];
        }
    }

    public IEnumerable<MapNode> GetSelectableNodes()
    {
        foreach (int id in Nodes[CurrentNodeId].NextNodeIds)
        {
            yield return Nodes[id];
        }
    }

    // =========================
    // ノード選択
    // =========================
    public void SelectNextNode(int nextId)
    {
        if (isPlayerMoving)
            return;

        if (!Nodes.ContainsKey(nextId))
            return;

        if (!Nodes[CurrentNodeId].NextNodeIds.Contains(nextId))
            return;

        CurrentNodeId = nextId;
        visitedNodeIds.Add(nextId);

        MapSession.CurrentNodeId = CurrentNodeId;

        MapNode node = Nodes[nextId];

        // 現在地と視認範囲を先に更新
        RefreshAllNodes();

        string destinationSceneName = null;

        switch (node.Type)
        {
            case NodeType.Battle:
                {
                    string enemyName =
                        node.Enemy != null
                            ? node.Enemy.EnemyName
                            : "未設定";

                    Debug.Log(
                        $"Layer{node.Layer} の " +
                        $"{node.IndexInLayer + 1}マス目に移動" +
                        $" | タイプ:Battle" +
                        $" | 敵:{enemyName}"
                    );

                    destinationSceneName = battleSceneName;
                    break;
                }

            case NodeType.Rest:
                {
                    Debug.Log(
                        $"Layer{node.Layer} の " +
                        $"{node.IndexInLayer + 1}マス目に移動" +
                        " | タイプ:Rest"
                    );

                    destinationSceneName = restSceneName;
                    break;
                }

            case NodeType.Event:
                {
                    Debug.Log(
                        $"Layer{node.Layer} の " +
                        $"{node.IndexInLayer + 1}マス目に移動" +
                        " | タイプ:Event"
                    );

                    destinationSceneName = eventSceneName;
                    break;
                }

            case NodeType.Treasure:
                {
                    Debug.Log(
                        $"Layer{node.Layer} の " +
                        $"{node.IndexInLayer + 1}マス目に移動" +
                        " | タイプ:Treasure"
                    );

                    destinationSceneName = treasureSceneName;
                    break;
                }

            case NodeType.Item:
                {
                    Debug.Log(
                        $"Layer{node.Layer} の " +
                        $"{node.IndexInLayer + 1}マス目に移動" +
                        " | タイプ:Item"
                    );

                    destinationSceneName = itemSceneName;
                    break;
                }

            case NodeType.Boss:
                {
                    Debug.Log(
                        $"Layer{node.Layer} の " +
                        $"{node.IndexInLayer + 1}マス目に移動" +
                        " | タイプ:Boss"
                    );

                    destinationSceneName = bossSceneName;
                    break;
                }

            default:
                {
                    Debug.Log(
                        $"Layer{node.Layer} の " +
                        $"{node.IndexInLayer + 1}マス目に移動" +
                        $" | タイプ:{node.Type}"
                    );

                    break;
                }
        }

        StartCoroutine(
            MovePlayerRoutine(
                nextId,
                destinationSceneName,
                node.Type
            )
        );
    }

    // =========================
    // プレイヤー画像
    // =========================
    private void MovePlayerImmediately(int nodeId)
    {
        if (playerImage == null)
        {
            Debug.LogWarning(
                "MapFlowのPlayer Imageが設定されていません。",
                this
            );

            return;
        }

        if (!nodeRects.TryGetValue(nodeId, out RectTransform targetNode))
        {
            Debug.LogWarning(
                $"Node {nodeId} のRectTransformが見つかりません。",
                this
            );

            return;
        }

        playerImage.anchoredPosition =
            targetNode.anchoredPosition +
            playerPositionOffset;

        playerImage.SetAsLastSibling();
    }

    private IEnumerator MovePlayerRoutine(
        int destinationNodeId,
        string sceneName,
        NodeType nodeType
    )
    {
        isPlayerMoving = true;

        // プレイヤー画像が未設定でも進行不能にはしない
        if (
            playerImage == null ||
            !nodeRects.TryGetValue(
                destinationNodeId,
                out RectTransform targetNode
            )
        )
        {
            isPlayerMoving = false;

            if (!string.IsNullOrWhiteSpace(sceneName))
            {
                LoadSceneSafe(sceneName, nodeType);
            }

            yield break;
        }

        playerImage.SetAsLastSibling();

        Vector2 startPosition =
            playerImage.anchoredPosition;

        Vector2 targetPosition =
            targetNode.anchoredPosition +
            playerPositionOffset;

        float duration = Mathf.Max(
            0f,
            playerMoveDuration
        );

        if (duration <= 0f)
        {
            playerImage.anchoredPosition =
                targetPosition;
        }
        else
        {
            float elapsedTime = 0f;

            while (elapsedTime < duration)
            {
                elapsedTime += Time.unscaledDeltaTime;

                float t = Mathf.Clamp01(
                    elapsedTime / duration
                );

                // なめらかな加速と減速
                float smoothT =
                    t * t * (3f - 2f * t);

                playerImage.anchoredPosition =
                    Vector2.Lerp(
                        startPosition,
                        targetPosition,
                        smoothT
                    );

                yield return null;
            }

            playerImage.anchoredPosition =
                targetPosition;
        }

        isPlayerMoving = false;

        if (!string.IsNullOrWhiteSpace(sceneName))
        {
            LoadSceneSafe(
                sceneName,
                nodeType
            );
        }
    }

    // =========================
    // シーン遷移
    // =========================
    private void LoadSceneSafe(
        string sceneName,
        NodeType nodeType
    )
    {
        if (string.IsNullOrWhiteSpace(sceneName))
        {
            Debug.LogError(
                $"{nodeType}用のシーンが設定されていません。\n" +
                "MapFlowのInspectorにある「シーン遷移設定」を確認してください。",
                this
            );

            return;
        }

        if (!Application.CanStreamedLevelBeLoaded(sceneName))
        {
            Debug.LogError(
                $"シーン「{sceneName}」を読み込めません。\n" +
                "Build SettingsまたはBuild ProfilesのScene Listに、" +
                "シーンが登録されているか確認してください。",
                this
            );

            return;
        }

        SceneManager.LoadScene(sceneName);
    }

    // =========================
    // ノード表示更新
    // =========================
    private void RefreshAllNodes()
    {
        var selectables =
            GetSelectableNodes().ToList();

        var selectableIds =
            selectables
                .Select(node => node.Id)
                .ToHashSet();

        // 現在地から2マス先にあるノード
        var twoStepAheadIds =
            new HashSet<int>();

        foreach (MapNode selectable in selectables)
        {
            foreach (int nextId in selectable.NextNodeIds)
            {
                twoStepAheadIds.Add(nextId);
            }
        }

        if (selectableIndex >= selectables.Count)
        {
            selectableIndex = 0;
        }

        int focusedId =
            selectables.Count > 0
                ? selectables[selectableIndex].Id
                : -1;

        foreach (var pair in nodeButtons)
        {
            int id = pair.Key;
            MapNodeButton mapNodeButton = pair.Value;

            NodeState state;
            bool isClickable;

            if (id == CurrentNodeId)
            {
                state = NodeState.Current;
                isClickable = false;
            }
            else if (selectableIds.Contains(id))
            {
                // 1マス先
                state = NodeState.Selectable;
                isClickable = true;
            }
            else if (visitedNodeIds.Contains(id))
            {
                state = NodeState.Visited;
                isClickable = false;
            }
            else if (twoStepAheadIds.Contains(id))
            {
                // 2マス先は見えるが選択不可
                state = NodeState.Selectable;
                isClickable = false;
            }
            else
            {
                state = NodeState.Locked;
                isClickable = false;
            }

            bool focused =
                id == focusedId;

            mapNodeButton.Refresh(
                state,
                Nodes[id].Type,
                focused,
                isClickable
            );
        }
    }

    // =========================
    // UI生成
    // =========================
    private void CreateUI()
    {
        var nodeObjects =
            new Dictionary<int, GameObject>();

        foreach (MapNode node in Nodes.Values)
        {
            GameObject nodeObject =
                Instantiate(
                    nodePrefab,
                    parent
                );

            RectTransform rect =
                nodeObject.GetComponent<RectTransform>();

            rect.anchoredPosition =
                node.Position;

            MapNodeButton mapNodeButton =
                nodeObject.GetComponent<MapNodeButton>();

            mapNodeButton.NodeId =
                node.Id;

            mapNodeButton.mapFlow =
                this;

            nodeObjects[node.Id] =
                nodeObject;

            nodeButtons[node.Id] =
                mapNodeButton;

            nodeRects[node.Id] =
                rect;
        }

        CreateLines(nodeObjects);

        if (playerImage != null)
        {
            playerImage.SetAsLastSibling();
        }
    }

    private void CreateLines(
        Dictionary<int, GameObject> nodeObjects
    )
    {
        foreach (MapNode node in Nodes.Values)
        {
            foreach (int nextId in node.NextNodeIds)
            {
                if (!Nodes.ContainsKey(nextId))
                    continue;

                if (Nodes[nextId].Layer != node.Layer + 1)
                    continue;

                Vector2 from =
                    nodeObjects[node.Id]
                        .GetComponent<RectTransform>()
                        .anchoredPosition;

                Vector2 to =
                    nodeObjects[nextId]
                        .GetComponent<RectTransform>()
                        .anchoredPosition;

                Vector2 difference =
                    to - from;

                float length =
                    difference.magnitude;

                float angle =
                    Mathf.Atan2(
                        difference.y,
                        difference.x
                    ) * Mathf.Rad2Deg;

                Vector2 middle =
                    (from + to) * 0.5f;

                GameObject lineObject =
                    new GameObject(
                        "Line",
                        typeof(RectTransform),
                        typeof(CanvasRenderer),
                        typeof(UnityEngine.UI.Image)
                    );

                lineObject.transform.SetParent(
                    parent,
                    false
                );

                // ノードより背面に置く
                lineObject.transform.SetAsFirstSibling();

                RectTransform rect =
                    lineObject.GetComponent<RectTransform>();

                rect.anchorMin =
                    new Vector2(0.5f, 0.5f);

                rect.anchorMax =
                    new Vector2(0.5f, 0.5f);

                rect.pivot =
                    new Vector2(0.5f, 0.5f);

                rect.anchoredPosition =
                    middle;

                rect.sizeDelta =
                    new Vector2(
                        length,
                        6f
                    );

                rect.localRotation =
                    Quaternion.Euler(
                        0f,
                        0f,
                        angle
                    );

                UnityEngine.UI.Image image =
                    lineObject.GetComponent<UnityEngine.UI.Image>();

                image.color =
                    new Color(
                        0.8f,
                        0.8f,
                        0.8f,
                        1f
                    );

                image.raycastTarget = false;
            }
        }

        if (playerImage != null)
        {
            playerImage.SetAsLastSibling();
        }
    }

    // =========================
    // 入力
    // =========================
    private void Update()
    {
        if (isPlayerMoving)
            return;

        if (Keyboard.current == null)
            return;

        var selectables =
            GetSelectableNodes().ToList();

        if (selectables.Count == 0)
            return;

        if (
            Keyboard.current
                .upArrowKey
                .wasPressedThisFrame
        )
        {
            selectableIndex =
                (
                    selectableIndex -
                    1 +
                    selectables.Count
                ) %
                selectables.Count;

            RefreshAllNodes();
        }
        else if (
            Keyboard.current
                .downArrowKey
                .wasPressedThisFrame
        )
        {
            selectableIndex =
                (
                    selectableIndex +
                    1
                ) %
                selectables.Count;

            RefreshAllNodes();
        }
        else if (
            Keyboard.current
                .rightArrowKey
                .wasPressedThisFrame ||
            Keyboard.current
                .enterKey
                .wasPressedThisFrame
        )
        {
            SelectNextNode(
                selectables[selectableIndex].Id
            );

            selectableIndex = 0;
        }
    }

    // =========================
    // デバッグ
    // =========================
    private void DebugAllNodes()
    {
        foreach (
            MapNode node in
            Nodes.Values.OrderBy(node => node.Layer)
        )
        {
            string enemyName =
                node.Enemy != null
                    ? node.Enemy.EnemyName
                    : "なし";

            Debug.Log(
                $"Node {node.Id}" +
                $" | Layer {node.Layer}" +
                $" | {node.Type}" +
                $" | Enemy:{enemyName}" +
                $" | Pos:{node.Position}" +
                $" -> [{string.Join(",", node.NextNodeIds)}]"
            );
        }
    }

    private void DebugDrawConnections()
    {
        foreach (MapNode node in Nodes.Values)
        {
            foreach (int nextId in node.NextNodeIds)
            {
                MapNode target =
                    Nodes[nextId];

                Debug.DrawLine(
                    node.Position,
                    target.Position,
                    Color.white,
                    100f
                );
            }
        }
    }

    private void OnDrawGizmos()
    {
        if (Nodes == null)
            return;

        foreach (MapNode node in Nodes.Values)
        {
            Gizmos.color =
                node.Id == CurrentNodeId
                    ? Color.yellow
                    : Color.white;

            Gizmos.DrawSphere(
                node.Position,
                20f
            );

            foreach (int nextId in node.NextNodeIds)
            {
                Gizmos.DrawLine(
                    node.Position,
                    Nodes[nextId].Position
                );
            }
        }
    }

    public static void ResetSession()
    {
        MapSession.Clear();
    }

    // =========================
    // シーン設定
    // =========================
    [Header("シーン遷移設定")]

#if UNITY_EDITOR
    [SerializeField]
    private UnityEditor.SceneAsset battleScene;

    [SerializeField]
    private UnityEditor.SceneAsset restScene;

    [SerializeField]
    private UnityEditor.SceneAsset eventScene;

    [SerializeField]
    private UnityEditor.SceneAsset treasureScene;

    [SerializeField]
    private UnityEditor.SceneAsset itemScene;

    [SerializeField]
    private UnityEditor.SceneAsset bossScene;
#endif

    [SerializeField, HideInInspector]
    private string battleSceneName;

    [SerializeField, HideInInspector]
    private string restSceneName;

    [SerializeField, HideInInspector]
    private string eventSceneName;

    [SerializeField, HideInInspector]
    private string treasureSceneName;

    [SerializeField, HideInInspector]
    private string itemSceneName;

    [SerializeField, HideInInspector]
    private string bossSceneName;

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (battleScene != null)
        {
            battleSceneName =
                battleScene.name;
        }

        if (restScene != null)
        {
            restSceneName =
                restScene.name;
        }

        if (eventScene != null)
        {
            eventSceneName =
                eventScene.name;
        }

        if (treasureScene != null)
        {
            treasureSceneName =
                treasureScene.name;
        }

        if (itemScene != null)
        {
            itemSceneName =
                itemScene.name;
        }

        if (bossScene != null)
        {
            bossSceneName =
                bossScene.name;
        }
    }
#endif
}