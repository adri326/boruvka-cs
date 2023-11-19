namespace Boruvka;

public class Boruvka<Node>: GraphAutomaton<Node>
where
    Node: IEquatable<Node>, IComparable
{
    Graph<Node> Graph = new();
    List<NodeGroup<Node>> Groups = new();

    public void Initialize(Graph<Node> graph) {
        this.Graph = graph;
        this.Groups = graph.GetNodes().Select(node => new NodeGroup<Node>(graph, node)).ToList();
    }

    public void PerformRound() {
        List<(int lhs, int rhs)> pairings = this.Groups.Select((group, index) => (index, group.FindPair(this.Groups)))
            .Where(node => node.Item2 >= 0)
            // Great language you got :)
            .ToList()!;

        // Deduplicate the pairings
        pairings = pairings
            .Where((pair, index) =>
                !pairings.Take(index).Contains((pair.rhs, pair.lhs))
            )
            .ToList();

        List<HashSet<int>> groupings = new();

        foreach (var (lhs, rhs) in pairings) {
            var existingGrouping = groupings.Find(grouping => grouping.Contains(lhs) || grouping.Contains(rhs));
            if (existingGrouping != null) {
                existingGrouping.Add(lhs);
                existingGrouping.Add(rhs);
            } else {
                groupings.Add(new HashSet<int> { lhs, rhs });
            }
        }

        HashSet<int> missingGroupings = new HashSet<int>(Enumerable.Range(0, this.Groups.Count));
        foreach (var grouping in groupings) {
            foreach (var id in grouping) {
                missingGroupings.Remove(id);
            }
        }

        this.Groups = Enumerable.Concat(
            missingGroupings.Select(x => this.Groups[x]),
            groupings.Select(grouping => new NodeGroup<Node>(
                grouping.Select(index => this.Groups[index]))
            )
        ).ToList();
    }

    public List<HashSet<Node>> NodeGroups() {
        return this.Groups.Select(group => group.Nodes).ToList();
    }

    public bool Finished() {
        return !this.Groups.Any(group => group.Edges.Count > 0);
    }
}
