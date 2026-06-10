using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

// =========================
// 型定義
// =========================
public enum NodeType
{
    Start,   // ① スタート
    Battle,  // ② 敵
    Rest,    // ③ 休憩
    Item,    // ④ アイテム
    Boss     // ⑤ ボス
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

    public EnemyData Enemy;
}

public class MapGraph
{
    public Dictionary<int, MapNode> Nodes = new();
    public int StartNodeId;
    public int BossNodeId;
}

// =========================
// マップ生成
// =========================
public static class MapGenerator
{
    public static MapGraph Generate(
        int layerCount = 6,
        int minNodesPerLayer = 2,
        int maxNodesPerLayer = 4,
        MapDirection direction = MapDirection.Horizontal,
        EnemyDatabase enemyDatabase = null
    )
    {
        var graph = new MapGraph();
        int nextId = 0;
        List<List<MapNode>> layers = new();

        float layerSpacing = 170f;
        float laneSpacing = 110f;

        // ノード数を各レイヤーで決める
        var nodeCounts = new int[layerCount];
        for (int layer = 0; layer < layerCount; layer++)
        {
            // スタート・休憩固定レイヤー・ボスは1個
            if (layer == 0 || layer == 2 || layer == 4 || layer == layerCount - 1)
                nodeCounts[layer] = 1;
            else
                nodeCounts[layer] = Random.Range(minNodesPerLayer, maxNodesPerLayer + 1);
        }

        // ノード生成
        for (int layer = 0; layer < layerCount; layer++)
        {
            var layerNodes = new List<MapNode>();
            int count = nodeCounts[layer];
            float centerOffset = (count - 1) / 2f;

            for (int i = 0; i < count; i++)
            {
                float along = layer * layerSpacing;
                float cross = (i - centerOffset) * laneSpacing;

                var node = new MapNode
                {
                    Id = nextId++,
                    Layer = layer,
                    Lane = i,
                    IndexInLayer = i,
                    Type = DecideNodeType(layer, layerCount),
                    Position = direction == MapDirection.Horizontal
                                    ? new Vector2(along, cross)
                                    : new Vector2(cross, along)
                };

                // Battleノードに敵をランダム割り当て
                if (node.Type == NodeType.Battle && enemyDatabase != null)
                    node.Enemy = enemyDatabase.GetRandom();

                graph.Nodes[node.Id] = node;
                layerNodes.Add(node);
            }
            layers.Add(layerNodes);
        }

        // 接続
        for (int layer = 0; layer < layerCount - 1; layer++)
        {
            var cur = layers[layer];
            var next = layers[layer + 1];

            // ボス直前は全員ボスへ収束
            if (layer == layerCount - 2)
            {
                foreach (var node in cur)
                {
                    node.NextNodeIds.Clear();
                    node.NextNodeIds.Add(next[0].Id);
                }
                continue;
            }

            // 次レイヤーが1個（休憩固定）なら全員そこへ収束
            if (next.Count == 1)
            {
                foreach (var node in cur)
                {
                    if (!node.NextNodeIds.Contains(next[0].Id))
                        node.NextNodeIds.Add(next[0].Id);
                }
                continue;
            }

            int curCount = cur.Count;
            int nextCount = next.Count;

            foreach (var node in cur)
            {
                float t = (curCount <= 1) ? 0.5f : (float)node.IndexInLayer / (curCount - 1);
                float centerF = t * (nextCount - 1);
                int center = Mathf.RoundToInt(centerF);

                var candidates = Enumerable.Range(0, nextCount).ToList();
                candidates.Sort((a, b) =>
                    Mathf.Abs(a - center).CompareTo(Mathf.Abs(b - center)));

                int connected = 0;
                foreach (int ni in candidates)
                {
                    if (connected >= 2) break;

                    bool crosses = false;
                    for (int pi = 0; pi < curCount; pi++)
                    {
                        var other = cur[pi];
                        foreach (var cid in other.NextNodeIds)
                        {
                            int otherNi = next.FindIndex(n => n.Id == cid);
                            if (otherNi < 0) continue;

                            int fi = node.IndexInLayer;
                            if ((fi < pi && ni > otherNi) ||
                                (fi > pi && ni < otherNi))
                            {
                                crosses = true;
                                break;
                            }
                        }
                        if (crosses) break;
                    }

                    if (!crosses)
                    {
                        node.NextNodeIds.Add(next[ni].Id);
                        connected++;
                    }
                }

                if (node.NextNodeIds.Count == 0)
                    node.NextNodeIds.Add(next[center].Id);
            }

            // 孤立した次レイヤーノードへの保証接続
            for (int ni = 0; ni < nextCount; ni++)
            {
                var child = next[ni];
                if (cur.Any(p => p.NextNodeIds.Contains(child.Id))) continue;

                var bestParent = cur
                    .OrderBy(p => Mathf.Abs(GetNearestNextIndex(p, curCount, nextCount) - ni))
                    .First();

                if (!bestParent.NextNodeIds.Contains(child.Id))
                    bestParent.NextNodeIds.Add(child.Id);
            }
        }

        graph.StartNodeId = layers[0][0].Id;
        graph.BossNodeId = layers[^1][0].Id;

        CenterGraph(graph);
        return graph;
    }

    private static int GetNearestNextIndex(MapNode node, int curCount, int nextCount)
    {
        float t = (curCount <= 1) ? 0.5f : (float)node.IndexInLayer / (curCount - 1);
        return Mathf.RoundToInt(t * (nextCount - 1));
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

    private static NodeType DecideNodeType(int layer, int maxLayer)
    {
        // 固定レイヤー
        if (layer == 0) return NodeType.Start;
        if (layer == 2 || layer == 4) return NodeType.Rest;
        if (layer == maxLayer - 1) return NodeType.Boss;

        // 残りはBattleかItemをランダム
        return Random.value < 0.7f ? NodeType.Battle : NodeType.Item;
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
        graph = MapGenerator.Generate(
            layerCount: 6,
            direction: MapDirection.Horizontal,
            enemyDatabase: enemyDatabase
        );
        Nodes = graph.Nodes;
        CurrentNodeId = graph.StartNodeId;
        visitedNodeIds.Add(CurrentNodeId);
    }

    void Start()
    {
        DebugAllNodes();
        DebugDrawConnections();
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
        if (!Nodes[CurrentNodeId].NextNodeIds.Contains(nextId)) return;

        CurrentNodeId = nextId;
        visitedNodeIds.Add(nextId);

        var node = Nodes[nextId];

        if (node.Type == NodeType.Battle)
        {
            string enemyId = node.Enemy != null ? node.Enemy.InternalId : "未設定";
            Debug.Log($"Layer{node.Layer} の {node.IndexInLayer + 1}マス目に移動 | タイプ:Battle | 敵ID:{enemyId}");

            // TODO: 戦闘シーンへ遷移
            // 1. BattleSessionなどstaticクラスに node.Enemy を渡す
            // 2. SceneManager.LoadScene("BattleScene") を呼ぶ
        }
        else if (node.Type == NodeType.Rest)
        {
            Debug.Log($"Layer{node.Layer} の {node.IndexInLayer + 1}マス目に移動 | タイプ:Rest");

            // TODO: 休憩処理
            // HP回復などの処理をここで呼ぶ
        }
        else if (node.Type == NodeType.Item)
        {
            Debug.Log($"Layer{node.Layer} の {node.IndexInLayer + 1}マス目に移動 | タイプ:Item");

            // TODO: アイテム取得処理
            // アイテムシーンへ遷移、またはその場でアイテム選択UIを出す
        }
        else if (node.Type == NodeType.Boss)
        {
            Debug.Log($"Layer{node.Layer} の {node.IndexInLayer + 1}マス目に移動 | タイプ:Boss");

            // TODO: ボス戦闘シーンへ遷移
            // SceneManager.LoadScene("BossScene") を呼ぶ
        }
        else
        {
            Debug.Log($"Layer{node.Layer} の {node.IndexInLayer + 1}マス目に移動 | タイプ:{node.Type}");
        }

        RefreshAllNodes();
    }

    void RefreshAllNodes()
    {
        var selectables = GetSelectableNodes().ToList();
        var selectableIds = selectables.Select(n => n.Id).ToHashSet();

        if (selectableIndex >= selectables.Count)
            selectableIndex = 0;

        int focusedId = selectables.Count > 0 ? selectables[selectableIndex].Id : -1;
        int currentLayer = Nodes[CurrentNodeId].Layer;

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
            int nodeLayer = Nodes[id].Layer;
            bool showIcon = nodeLayer <= currentLayer + 1
                            && (state == NodeState.Selectable || state == NodeState.Current || state == NodeState.Visited);

            btn.Refresh(state, Nodes[id].Type, focused, Nodes[id].Enemy, showIcon);
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
}