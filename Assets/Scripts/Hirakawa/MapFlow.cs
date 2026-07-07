using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

// =========================
// 型定義
// =========================


//ノード（マス）の種類を定義する列挙型。Start, Battle, Rest, Event, Bossの5種類。
//増やしたい場合はここに新しいノードの名前を追加し、MapGenerator.DecideNodeTypeメソッドで割り当てる条件を追加する必要があります。
//シーン遷移の処理もMapFlow.SelectNextNodeメソッドに追加する必要があります。
//MapNodeButtonクラスのTypeColors配列にも新しいノードタイプの色を追加する必要があります。
public enum NodeType
{
    Start,
    Battle,
    Rest,
    Event,
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


//



// =========================
// マップ生成
// =========================



// 生成アルゴリズムの概要
public static class MapGenerator
{

    // 生成パラメータ
    //maxNodesPerLayerは、各レイヤーに配置されるノードの最大数。増やせば選択肢が増える。
    //enemyDatabaseは、敵データベースを指定することで、Battleノード敵を割り当てる。


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

        var nodeCounts = new int[layerCount];
        for (int layer = 0; layer < layerCount; layer++)
        {
            if (layer == 0 || layer == 2 || layer == 4 || layer == layerCount - 1)
                nodeCounts[layer] = 1;
            else
                nodeCounts[layer] = Random.Range(minNodesPerLayer, maxNodesPerLayer + 1);
        }

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

                if (node.Type == NodeType.Battle && enemyDatabase != null)
                    node.Enemy = enemyDatabase.GetRandom();

                graph.Nodes[node.Id] = node;
                layerNodes.Add(node);
            }
            layers.Add(layerNodes);
        }

        for (int layer = 0; layer < layerCount - 1; layer++)
        {
            var cur = layers[layer];
            var next = layers[layer + 1];

            if (layer == layerCount - 2)
            {
                foreach (var node in cur)
                {
                    node.NextNodeIds.Clear();
                    node.NextNodeIds.Add(next[0].Id);
                }
                continue;
            }

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


    // ノード(マス)タイプを決定するメソッド。レイヤー番号と最大レイヤー数に基づいて、ノードの種類を決定します。
    //今現在は、レイヤー0はStart、レイヤー2と4はRest、最終レイヤーはBoss、それ以外のレイヤーでは70%の確率でBattle、30%の確率でEventとなるように設定されています。
    private static NodeType DecideNodeType(int layer, int maxLayer)
    {
        if (layer == 0) return NodeType.Start;
        if (layer == 2 || layer == 4) return NodeType.Rest;
        if (layer == maxLayer - 1) return NodeType.Boss;

        return Random.value < 0.7f ? NodeType.Battle : NodeType.Event;
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

            // 初回生成時のみ、マップを生成してセッションに保存
            // 生成パラメータは必要に応じて調整可能にしてます  
            //layerCountの数字を増やすと、マップのレイヤー数が増えます。ボスまでの道が長くなります。
            graph = MapGenerator.Generate(
                layerCount: 6,
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

        // 修正①：UIを生成する前にparentの子を全削除して二重生成を防ぐ
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

    //ノード選択時の処理。Battle, Rest, Event, Boss以外にも対応する場合は、ここに追加する。
    public void SelectNextNode(int nextId)
    {
        if (!Nodes[CurrentNodeId].NextNodeIds.Contains(nextId)) return;

        CurrentNodeId = nextId;
        visitedNodeIds.Add(nextId);
        MapSession.CurrentNodeId = CurrentNodeId;

        var node = Nodes[nextId];

        switch (node.Type)
        {
            case NodeType.Battle:
                string enemyName = node.Enemy != null ? node.Enemy.EnemyName : "未設定";
                Debug.Log($"Layer{node.Layer} の {node.IndexInLayer + 1}マス目に移動 | タイプ:Battle | 敵:{enemyName}");

                
                UnityEngine.SceneManagement.SceneManager.LoadScene(battleSceneName);
                break;

            case NodeType.Rest:
                Debug.Log($"Layer{node.Layer} の {node.IndexInLayer + 1}マス目に移動 | タイプ:Rest");
                UnityEngine.SceneManagement.SceneManager.LoadScene(restSceneName);
                break;

            case NodeType.Event:
                Debug.Log($"Layer{node.Layer} の {node.IndexInLayer + 1}マス目に移動 | タイプ:Event");
                UnityEngine.SceneManagement.SceneManager.LoadScene(eventSceneName);
                break;

            case NodeType.Boss:
                Debug.Log($"Layer{node.Layer} の {node.IndexInLayer + 1}マス目に移動 | タイプ:Boss");
                UnityEngine.SceneManagement.SceneManager.LoadScene(bossSceneName);
                break;

            default:
                Debug.Log($"Layer{node.Layer} の {node.IndexInLayer + 1}マス目に移動 | タイプ:{node.Type}");
                RefreshAllNodes();
                return;
        }
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
            // 変更後
            bool dimIcon = (state == NodeState.Current || state == NodeState.Visited);

            btn.Refresh(state, Nodes[id].Type, focused, Nodes[id].Enemy, showIcon, dimIcon);
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


    //ラインを生成するメソッド。ノード間の接続を視覚的に表現するために使用されます。
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
    [SerializeField] UnityEditor.SceneAsset bossScene;
#endif

    [SerializeField, HideInInspector] string battleSceneName;
    [SerializeField, HideInInspector] string restSceneName;
    [SerializeField, HideInInspector] string eventSceneName;
    [SerializeField, HideInInspector] string bossSceneName;

#if UNITY_EDITOR
    void OnValidate()
    {
        if (battleScene != null) battleSceneName = battleScene.name;
        if (restScene != null) restSceneName = restScene.name;
        if (eventScene != null) eventSceneName = eventScene.name;
        if (bossScene != null) bossSceneName = bossScene.name;
    }
#endif
}