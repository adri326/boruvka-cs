using NUnit.Framework;
using Boruvka;
using NUnit.Framework.Internal;

namespace Boruvka.Test;

public class Tests {
    Randomizer rnd = new();

    [SetUp]
    public void Setup() {
        rnd = new();
    }

    // Checks that AddEdge respects commutativity
    [Test]
    public void AddEdgeComm() {
        for (var iteration = 0; iteration < 100; iteration++) {
            UInt32 length = this.rnd.NextUInt(10);
            Graph<UInt32> g = new();

            for (var n = 0; n < length; n++) {
                var fst = this.rnd.NextUInt(length);
                var snd = this.rnd.NextUInt(length);
                g.AddEdge(fst, snd);

                Assert.IsTrue(g.HasEdge(fst, snd));
                Assert.IsTrue(g.HasEdge(snd, fst));
            }

            for (UInt32 fst = 0; fst < length; fst++) {
                for (UInt32 snd = 0; snd < length; snd++) {
                    Assert.AreEqual(g.HasEdge(fst, snd), g.HasEdge(snd, fst));
                }
            }
        }
    }

    [Test]
    public void AddEdgeAddsNodes() {
        Graph<int> g1 = new();
        g1.AddEdge(1, 2);
        Assert.IsTrue(g1.HasNode(1));
        Assert.IsTrue(g1.HasNode(2));
        Assert.IsTrue(g1.HasEdge(1, 2));

        Graph<String> g2 = new();
        g2.AddEdge("Hello", "World");
        Assert.IsTrue(g2.HasNode("Hello"));
        Assert.IsTrue(g2.HasNode("World"));
        Assert.IsFalse(g2.HasNode("Star"));

        g2.AddEdge("World", "Star");
        Assert.IsTrue(g2.HasNode("Hello"));
        Assert.IsTrue(g2.HasNode("World"));
        Assert.IsTrue(g2.HasNode("Star"));
        Assert.IsTrue(g2.HasEdge("Hello", "World"));
        Assert.IsTrue(g2.HasEdge("World", "Star"));
    }

    [Test]
    public void RemoveNode() {
        Graph<int> g = new();

        g.AddEdge(1, 2);
        g.AddEdge(3, 4);
        g.AddEdge(2, 3);

        g.RemoveNode(3);

        Assert.IsFalse(g.HasNode(3));
        Assert.IsFalse(g.HasEdge(3, 4));
        Assert.IsFalse(g.HasEdge(4, 3));
        Assert.IsFalse(g.HasEdge(3, 2));
        Assert.IsFalse(g.HasEdge(2, 3));
        Assert.IsTrue(g.HasEdge(1, 2));
        Assert.IsTrue(g.HasEdge(2, 1));
    }

    [Test]
    public void RemoveEdge() {
        Graph<int> g = new();

        g.AddEdge(1, 2);
        g.RemoveEdge(1, 2);

        Assert.IsFalse(g.HasEdge(1, 2));
        Assert.IsFalse(g.HasEdge(2, 1));
        Assert.IsTrue(g.HasNode(1));
        Assert.IsTrue(g.HasNode(2));
    }
}
