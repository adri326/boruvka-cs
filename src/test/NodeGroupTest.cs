using NUnit.Framework;
using Boruvka;
using NUnit.Framework.Internal;


namespace Boruvka.Test;

public class NodeGroupTest {
    Randomizer rnd = new();

    [SetUp]
    public void Setup() {
        rnd = new(100);
    }

    [Test]
    public void NodeGroupUnion() {
        for (int i = 0; i < 100; i++) {
            Graph<int> graph = new();
            int count = this.rnd.Next(30) + 10;
            for (int n = 0; n < count; n++) {
                graph.AddEdge(this.rnd.Next(count), this.rnd.Next(count));
            }

            List<NodeGroup<int>> allGroups = graph.GetNodes().Select(node => new NodeGroup<int>(graph, node)).ToList();

            NodeGroup<int> sum = allGroups[0];

            // Imagine having an Enumerate() method
            foreach (var (group, index) in allGroups.Select((v, i) => (v, i)).Skip(1)) {
                // Imagine having array literals
                sum = new NodeGroup<int>(new List<NodeGroup<int>> { group, sum });

                var expected = new HashSet<int>(allGroups.Take(index + 1).SelectMany(group => group.Nodes));

                // NOTE: We can't use AreEqual, because of [https://github.com/nunit/nunit/issues/3441],
                // and because `HashSet.Equals` only looks for reference equality (even though `==` does that already)
                Assert.That(
                    sum.Nodes,
                    Is.EquivalentTo(expected)
                );

                foreach (var node in sum.Nodes) {
                    foreach (var edge in graph.GetEdgesOf(node)) {
                        if (sum.Nodes.Contains(edge)) continue;

                        Assert.IsTrue(sum.Edges.Contains(edge));
                    }
                }
            }
        }
    }
}
