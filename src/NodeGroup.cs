namespace Boruvka;

public class NodeGroup<Node> where Node: IEquatable<Node>, IComparable {
    public readonly HashSet<Node> Nodes;
    public readonly List<Node> Edges;

    public NodeGroup(Graph<Node> graph, Node node) {
        this.Nodes = new HashSet<Node> {node};
        // Note: this .Equals is using IEquatable's Equals
        this.Edges = graph.GetEdgesOf(node).Where(edge => !edge.Equals(node)).ToList();
        this.ShuffleEdges();
    }

    /// <summary>
    /// Returns a new NodeGroup, corresponding to the union of `groupsToMerge`
    /// </summary>
    public NodeGroup(IEnumerable<NodeGroup<Node>> groupsToMerge) {
        this.Nodes = groupsToMerge
            .Select(group => group.Nodes)
            .Aggregate((a, b) => new HashSet<Node>(a.Union(b)));
        this.Edges = groupsToMerge
            .Select(group => new HashSet<Node>(group.Edges))
            .Aggregate((a, b) => new HashSet<Node>(a.Union(b)))
            .Where(node => !this.Nodes.Contains(node))
            .ToList();
        this.ShuffleEdges();
    }

    private void ShuffleEdges() {
        Random rng = new();
        for (int i = 0; i < Edges.Count - 1; i++) {
            int other = rng.Next(Edges.Count - i) + i;

            (Edges[i], Edges[other]) = (Edges[other], Edges[i]);
        }
    }

    public int FindPair(List<NodeGroup<Node>> otherGroups) {
        int thisIndex = otherGroups.IndexOf(this);
        foreach (var edgeCandidate in this.Edges) {
            var otherGroup = otherGroups.FindIndex(group => group.Nodes.Contains(edgeCandidate));
            if (otherGroup == -1) continue;
            if (otherGroup == thisIndex) continue;
            return otherGroup;
        }
        return -1;
    }
}
