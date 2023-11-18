using System.Collections.Generic;

namespace Boruvka;

public class Graph<Node> where Node: IEquatable<Node> {
    readonly List<Node> Nodes = new();

    // Invariant 1: ∀(a, b) ∈ edges, a ∈ nodes && b ∈ nodes
    // Invariant 2: ∀(a, b) ∈ edges, (b, a) ∈ edges
    readonly Dictionary<Node, HashSet<Node>> Edges = new();

    public Graph() {}

    public Graph(IEnumerable<Node> nodes) {
        this.Nodes = new List<Node>(nodes);
    }

    public void AddNode(Node node) {
        this.Nodes.Add(node);
    }

    /// <remarks>
    /// NOTE: this breaks invariant 2 if it is only called once.
    /// <br />
    /// NOTE: invariant 1 will only be upheld if <i>from ∈ nodes</i> and <i>to ∈ nodes</i>
    /// </remarks>
    private void AddDirectedEdge(Node from, Node to) {
        if (this.Edges.ContainsKey(from)) {
            var edges = this.Edges[from];
            edges.Add(to);
        } else {
            this.Edges.Add(from, new HashSet<Node> { to } );
        }
    }

    /// <summary>
    /// Adds the edge (fst, snd) to the graph; if `fst` or `snd` is missing from the graph, then they are automatically added.
    /// </summary>
    public void AddEdge(Node fst, Node snd) {
        if (!this.Nodes.Contains(fst)) {
            this.Nodes.Add(fst);
        }
        if (!this.Nodes.Contains(snd)) {
            this.Nodes.Add(snd);
        }

        this.AddDirectedEdge(fst, snd);
        this.AddDirectedEdge(snd, fst);
    }

    /// <remarks>
    /// NOTE: This breaks invariant 2 if it is only called once.
    /// </remarks>
    private void RemoveDirectedEdge(Node from, Node to) {
        if (this.Edges.ContainsKey(from)) {
            this.Edges[from].Remove(to);
        }
    }

    /// <summary>
    /// Removes the edge `(fst, snd)` from the graph. The nodes will still be in the graph.
    /// </summary>
    /// <param name="fst"></param>
    /// <param name="snd"></param>
    public void RemoveEdge(Node fst, Node snd) {
        this.RemoveDirectedEdge(fst, snd);
        this.RemoveDirectedEdge(snd, fst);
    }

    /// <summary>
    /// Removes all edges `(a, b)` for which `a == node` or `b == node`.
    /// </summary>
    public void RemoveEdgesWith(Node node) {
        this.Edges.Remove(node);

        foreach (var (_node, node_edges) in this.Edges) {
            node_edges.Remove(node);
        }
    }

    /// <summary>
    /// Removes the node `node` and its edges.
    /// </summary>
    public void RemoveNode(Node node) {
        this.RemoveEdgesWith(node);
        // We now have ∀(a,b) ∈ E, a != node && b != node

        this.Nodes.Remove(node);
    }

    public bool HasNode(Node node) {
        return this.Nodes.Contains(node);
    }

    public bool HasEdge(Node fst, Node snd) {
        return this.Edges.ContainsKey(fst) && this.Edges[fst].Contains(snd);
    }
}
