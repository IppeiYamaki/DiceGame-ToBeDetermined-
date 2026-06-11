using System.Collections.Generic;

public static class MapSession
{
    public static MapGraph Graph;
    public static int CurrentNodeId;
    public static HashSet<int> VisitedNodeIds = new();

    public static bool HasData => Graph != null;

    public static void Clear()
    {
        Graph = null;
        CurrentNodeId = 0;
        VisitedNodeIds.Clear();
    }
}