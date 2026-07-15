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
// データクラス
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
// マップ生成（固定マップ）
// =========================
// レイヤー構成：
// L0: Start(1)
// L1: Battle(1)
// L2: Battle(2) ← 分岐
// L3: Treasure(1) ← 収束
// L4: Battle(2) ← 分岐
// L5: Rest(1) / Event(1)
// L6: Treasure(1) ← 収束
// L7: Battle(2) ← 分岐
// L8: Event(1) / Rest(1) ← 交差接続
// L9: Boss(1) ← 収束
public static class MapGenerator
{
    public static MapGraph Generate(
        MapDirection direction = MapDirection.Horizontal,
        EnemyDatabase enemyDatabase = null
    )
    {
        var graph = new MapGraph();
        int nextId = 0;

        float layerSpacing = 130f;
        float laneSpacing = 400f;

        // -------------------------
        // レイヤー構成定義
        // (NodeType, laneIndex) のリスト
        // -------------------------
        var layerDefs = new List<List<(NodeType type, int lane)>>
        {
            // L0: Start
            new() { (NodeType.Start, 0) },
            // L1: Battle×1
            new() { (NodeType.Battle, 0) },
            // L2: Battle×2（分岐）
            new() { (NodeType.Battle, 0), (NodeType.Battle, 1) },
            // L3: Treasure×1（収束）
            new() { (NodeType.Treasure, 0) },
            // L4: Battle×2（分岐）
            new() { (NodeType.Battle, 0), (NodeType.Battle, 1) },
            // L5: Rest / Event
            new() { (NodeType.Rest, 0), (NodeType.Event, 1) },
            // L6: Treasure×1（収束）
            new() { (NodeType.Treasure, 0) },
            // L7: Battle×2（分岐）
            new() { (NodeType.Battle, 0), (NodeType.Battle, 1) },
            // L8: Event / Rest（交差接続）
            new() { (NodeType.Event, 0), (NodeType.Rest, 1) },
            // L9: Boss×1（収束）
            new() { (NodeType.Boss, 0) },
        };

        // -------------------------
        // ノード生成
        // -------------------------
        var layers = new List<List<MapNode>>();

        for (int layer = 0; layer < layerDefs.Count; layer++)
        {
            var layerNodes = new List<MapNode>();
            var defs = layerDefs[layer];
            int count = defs.Count;
            float centerOffset = (count - 1) / 2f;

            for (int i = 0; i < count; i++)
            {
                var (type, lane) = defs[i];

                float along = layer * layerSpacing;
                float cross = (i - centerOffset) * laneSpacing;

                float x = direction == MapDirection.Horizontal ? along : cross;
                float y = direction == MapDirection.Horizontal ? cross : along;

                var node = new MapNode
                {
                    Id = nextId++,
                    Layer = layer,
                    Lane = lane,
                    IndexInLayer = i,
                    Type = type,
                    Position = new Vector2(x, y)
                };

                if (type == NodeType.Battle && enemyDatabase != null)
                    node.Enemy = enemyDatabase.GetRandom();

                graph.Nodes[node.Id] = node;
                layerNodes.Add(node);
            }
            layers.Add(layerNodes);
        }

        // -------------------------
        // 接続定義
        // 基本：同インデックス or 収束は全員→0番
        // 交差：L8はL7の0番→L8の1番、L7の1番→L8の0番
        // -------------------------

        // L0(Start) → L1(Battle)
        Connect(layers[0][0], layers[1][0]);

        // L1(Battle) → L2(Battle×2) 分岐
        Connect(layers[1][0], layers[2][0]);
        Connect(layers[1][0], layers[2][1]);

        // L2(Battle×2) → L3(Treasure) 収束
        Connect(layers[2][0], layers[3][0]);
        Connect(layers[2][1], layers[3][0]);

        // L3(Treasure) → L4(Battle×2) 分岐
        Connect(layers[3][0], layers[4][0]);
        Connect(layers[3][0], layers[4][1]);

        // L4(Battle×2) → L5(Rest/Event) 各自対応
        // ①ルート(lane0) → Rest(0番)
        // ②ルート(lane1) → Event(1番)
        Connect(layers[4][0], layers[5][0]);
        Connect(layers[4][1], layers[5][1]);

        // L5(Rest/Event) → L6(Treasure) 収束
        Connect(layers[5][0], layers[6][0]);
        Connect(layers[5][1], layers[6][0]);

        // L6(Treasure) → L7(Battle×2) 分岐
        Connect(layers[6][0], layers[7][0]);
        Connect(layers[6][0], layers[7][1]);

        // L7(Battle×2) → L8(Event/Rest) 交差接続
        // ①ルート(lane0) → Rest(1番)  ※交差
        // ②ルート(lane1) → Event(0番) ※交差
        Connect(layers[7][0], layers[8][1]);
        Connect(layers[7][1], layers[8][0]);

        // L8(Event/Rest) → L9(Boss) 収束
        Connect(layers[8][0], layers[9][0]);
        Connect(layers[8][1], layers[9][0]);

        graph.StartNodeId = layers[0][0].Id;
        graph.BossNodeId = layers[9][0].Id;

        CenterGraph(graph);
        return graph;
    }

    private static void Connect(MapNode from, MapNode to)
    {
        if (!from.NextNodeIds.Contains(to.Id))
            from.NextNodeIds.Add(to.Id);
    }

    private static void CenterGraph(MapGraph graph)
    {
        if (graph.Nodes.Count == 0) return;

        float minX = float.MaxValue, maxX = float.MinValue;
        float minY = float.MaxValue, maxY = float.MinValue;

        foreach (var node in graph.Nodes.Values)
        {
            minX = Mathf.Min(minX, node.Position.x);
            maxX = Mathf.Max(maxX, node.Position.x);
            minY = Mathf.Min(minY, node.Position.y);
            maxY = Mathf.Max(maxY, node.Position.y);
        }

        Vector2 center = new Vector2((minX + maxX) * 0.5f, (minY + maxY) * 0.5f);
        foreach (var node in graph.Nodes.Values)
            node.Position -= center;
    }
}

// =========================
// 進行管理
// =========================
public class MapFlow : MonoBehaviour
{
    public int CurrentNodeId;
    public Dictionary<int, MapNode> Nodes;
    public MapGraph graph;

    HashSet<int> visitedNodeIds = new();
    Dictionary<int, MapNodeButton> nodeButtons = new();
    int selectableIndex = 0;

    [SerializeField] GameObject nodePrefab;
    [SerializeField] RectTransform parent;
    [SerializeField] EnemyDatabase enemyDatabase;

    void Awake()
    {
        if (!MapSession.HasData)
        {
            graph = MapGenerator.Generate(
                direction: MapDirection.Horizontal,
                enemyDatabase: enemyDatabase
            );
            MapSession.Graph = graph;
            MapSession.CurrentNodeId = graph.StartNodeId;
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

    void Start()
    {
        DebugAllNodes();
        DebugDrawConnections();

        foreach (Transform child in parent)
            Destroy(child.gameObject);
        nodeButtons.Clear();

        CreateUI();
        RefreshAllNodes();
    }

    public MapNode CurrentNode => Nodes[CurrentNodeId];

    public IEnumerable<MapNode> GetSelectableNodes()
    {
        foreach (var id in Nodes[CurrentNodeId].NextNodeIds)
            yield return Nodes[id];
    }

    public void SelectNextNode(int nextId)
    {
        if (!Nodes[CurrentNodeId].NextNodeIds.Contains(nextId))
            return;

        CurrentNodeId = nextId;
        visitedNodeIds.Add(nextId);
        MapSession.CurrentNodeId = CurrentNodeId;

        var node = Nodes[nextId];

        switch (node.Type)
        {
            case NodeType.Battle:
                {
                    string enemyName =
                        node.Enemy != null
                        ? node.Enemy.EnemyName
                        : "未設定";

                    Debug.Log(
                        $"Layer{node.Layer} の {node.IndexInLayer + 1}マス目に移動" +
                        $" | タイプ:Battle | 敵:{enemyName}"
                    );

                    LoadSceneSafe(battleSceneName, node.Type);
                    break;
                }

            case NodeType.Rest:
                {
                    Debug.Log(
                        $"Layer{node.Layer} の {node.IndexInLayer + 1}マス目に移動" +
                        $" | タイプ:Rest"
                    );

                    LoadSceneSafe(restSceneName, node.Type);
                    break;
                }

            case NodeType.Event:
                {
                    Debug.Log(
                        $"Layer{node.Layer} の {node.IndexInLayer + 1}マス目に移動" +
                        $" | タイプ:Event"
                    );

                    LoadSceneSafe(eventSceneName, node.Type);
                    break;
                }

            case NodeType.Treasure:
                {
                    Debug.Log(
                        $"Layer{node.Layer} の {node.IndexInLayer + 1}マス目に移動" +
                        $" | タイプ:Treasure"
                    );

                    LoadSceneSafe(treasureSceneName, node.Type);
                    break;
                }

            case NodeType.Item:
                {
                    Debug.Log(
                        $"Layer{node.Layer} の {node.IndexInLayer + 1}マス目に移動" +
                        $" | タイプ:Item"
                    );

                    LoadSceneSafe(itemSceneName, node.Type);
                    break;
                }

            case NodeType.Boss:
                {
                    Debug.Log(
                        $"Layer{node.Layer} の {node.IndexInLayer + 1}マス目に移動" +
                        $" | タイプ:Boss"
                    );

                    LoadSceneSafe(bossSceneName, node.Type);
                    break;
                }

            default:
                {
                    Debug.Log(
                        $"Layer{node.Layer} の {node.IndexInLayer + 1}マス目に移動" +
                        $" | タイプ:{node.Type}"
                    );

                    RefreshAllNodes();
                    return;
                }
        }

        RefreshAllNodes();
    }
    private void LoadSceneSafe(string sceneName, NodeType nodeType)
    {
        // シーン名が設定されていない場合
        if (string.IsNullOrWhiteSpace(sceneName))
        {
            Debug.LogError(
                $"{nodeType}用のシーンが設定されていません。\n" +
                "MapFlowのInspectorにある「シーン遷移設定」を確認してください。",
                this
            );

            return;
        }

        // Build Settings / Build Profiles に
        // シーンが登録されているか確認
        if (!Application.CanStreamedLevelBeLoaded(sceneName))
        {
            Debug.LogError(
                $"シーン「{sceneName}」を読み込めません。\n" +
                "Build Settings または Build Profiles のScene Listに" +
                "シーンが登録されているか確認してください。",
                this
            );

            return;
        }

        SceneManager.LoadScene(sceneName);
    }

    void RefreshAllNodes()
    {
        var selectables = GetSelectableNodes().ToList();
        var selectableIds = selectables.Select(n => n.Id).ToHashSet();

        if (selectableIndex >= selectables.Count)
            selectableIndex = 0;

        int focusedId = selectables.Count > 0 ? selectables[selectableIndex].Id : -1;

        foreach (var (id, btn) in nodeButtons)
        {
            NodeState state;

            if (id == CurrentNodeId)
                state = NodeState.Current;
            else if (selectableIds.Contains(id))
                state = NodeState.Selectable;
            else if (visitedNodeIds.Contains(id))
                state = NodeState.Visited;
            else
                state = NodeState.Locked;

            bool focused = (id == focusedId);
            btn.Refresh(state, Nodes[id].Type, focused);
        }
    }

    // =========================
    // デバッグ
    // =========================
    void DebugAllNodes()
    {
        foreach (var node in Nodes.Values.OrderBy(n => n.Layer))
        {
            Debug.Log(
                $"Node {node.Id} | Layer {node.Layer} | {node.Type} " +
                $"Pos:{node.Position} -> [{string.Join(",", node.NextNodeIds)}]"
            );
        }
    }

    void DebugDrawConnections()
    {
        foreach (var node in Nodes.Values)
        {
            foreach (var nextId in node.NextNodeIds)
            {
                var target = Nodes[nextId];
                Debug.DrawLine(node.Position, target.Position, Color.white, 100f);
            }
        }
    }

    void OnDrawGizmos()
    {
        if (Nodes == null) return;

        foreach (var node in Nodes.Values)
        {
            Gizmos.color = (node.Id == CurrentNodeId) ? Color.yellow : Color.white;
            Gizmos.DrawSphere(node.Position, 20f);

            foreach (var nextId in node.NextNodeIds)
                Gizmos.DrawLine(node.Position, Nodes[nextId].Position);
        }
    }

    // =========================
    // UI生成
    // =========================
    void CreateUI()
    {
        Dictionary<int, GameObject> nodeObjects = new();

        foreach (var node in Nodes.Values)
        {
            var obj = Instantiate(nodePrefab, parent);
            var rect = obj.GetComponent<RectTransform>();
            rect.localPosition = node.Position;

            var btn = obj.GetComponent<MapNodeButton>();
            btn.NodeId = node.Id;
            btn.mapFlow = this;

            nodeObjects[node.Id] = obj;
            nodeButtons[node.Id] = btn;

            Debug.Log($"Generating UI Node: {node.Id}");
        }

        CreateLines(nodeObjects);
    }

    void CreateLines(Dictionary<int, GameObject> nodeObjects)
    {
        foreach (var node in Nodes.Values)
        {
            foreach (var nextId in node.NextNodeIds)
            {
                if (!Nodes.ContainsKey(nextId)) continue;
                if (Nodes[nextId].Layer != node.Layer + 1) continue;

                var from = (Vector2)nodeObjects[node.Id].GetComponent<RectTransform>().localPosition;
                var to = (Vector2)nodeObjects[nextId].GetComponent<RectTransform>().localPosition;
                Vector2 diff = to - from;
                float length = diff.magnitude;
                float angle = Mathf.Atan2(diff.y, diff.x) * Mathf.Rad2Deg;
                Vector2 mid = (from + to) * 0.5f;

                var lineObj = new GameObject("Line", typeof(RectTransform), typeof(CanvasRenderer), typeof(UnityEngine.UI.Image));
                lineObj.transform.SetParent(parent, false);
                lineObj.transform.SetAsFirstSibling();

                var rect = lineObj.GetComponent<RectTransform>();
                rect.localPosition = new Vector3(mid.x, mid.y, 0);
                rect.sizeDelta = new Vector2(length, 6f);
                rect.localRotation = Quaternion.Euler(0, 0, angle);

                var img = lineObj.GetComponent<UnityEngine.UI.Image>();
                img.color = new Color(0.8f, 0.8f, 0.8f, 1f);
            }
        }
    }

    void Update()
    {
        var selectables = GetSelectableNodes().ToList();
        if (selectables.Count == 0) return;

        if (Keyboard.current.upArrowKey.wasPressedThisFrame)
        {
            selectableIndex = (selectableIndex - 1 + selectables.Count) % selectables.Count;
            RefreshAllNodes();
        }
        else if (Keyboard.current.downArrowKey.wasPressedThisFrame)
        {
            selectableIndex = (selectableIndex + 1) % selectables.Count;
            RefreshAllNodes();
        }
        else if (Keyboard.current.rightArrowKey.wasPressedThisFrame || Keyboard.current.enterKey.wasPressedThisFrame)
        {
            SelectNextNode(selectables[selectableIndex].Id);
            selectableIndex = 0;
        }
    }

    public static void ResetSession()
    {
        MapSession.Clear();
    }

    [Header("シーン遷移設定")]
#if UNITY_EDITOR
    [SerializeField] UnityEditor.SceneAsset battleScene;
    [SerializeField] UnityEditor.SceneAsset restScene;
    [SerializeField] UnityEditor.SceneAsset eventScene;
    [SerializeField] UnityEditor.SceneAsset treasureScene;
    [SerializeField] UnityEditor.SceneAsset itemScene;
    [SerializeField] UnityEditor.SceneAsset bossScene;
#endif

    [SerializeField, HideInInspector] string battleSceneName;
    [SerializeField, HideInInspector] string restSceneName;
    [SerializeField, HideInInspector] string eventSceneName;
    [SerializeField, HideInInspector] string treasureSceneName;
    [SerializeField, HideInInspector] string itemSceneName;
    [SerializeField, HideInInspector] string bossSceneName;

#if UNITY_EDITOR
    void OnValidate()
    {
        if (battleScene != null) battleSceneName = battleScene.name;
        if (restScene != null) restSceneName = restScene.name;
        if (eventScene != null) eventSceneName = eventScene.name;
        if (treasureScene != null) treasureSceneName = treasureScene.name;
        if (itemScene != null) itemSceneName = itemScene.name;
        if (bossScene != null) bossSceneName = bossScene.name;
    }
#endif
}