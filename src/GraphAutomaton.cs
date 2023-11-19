namespace Boruvka;

public interface GraphAutomaton<Node> where Node: IEquatable<Node>, IComparable {
    /// <summary>
    /// Called once before the automaton is stepped through
    /// </summary>
    public void Initialize(Graph<Node> graph);

    /// <summary>
    /// Called for each "round" of the automaton
    /// </summary>
    public void PerformRound();

    /// <summary>
    /// Optionally let the renderer know which node groups have already been constituted.
    /// </summary>
    public List<HashSet<Node>> NodeGroups() {
        return new();
    }

    /// <summary>
    /// Should return `true` if it cannot step any further
    /// </summary>
    public bool Finished();
}
