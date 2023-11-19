using System.Numerics;
using Raylib_cs;

namespace Boruvka;

public class GraphPosition<Node> where Node: IEquatable<Node>, IComparable {
    Graph<Node> Graph;
    private readonly Dictionary<Node, Vector2> Positions;
    Random rng = new();

    public GraphPosition(Graph<Node> Graph) {
        this.Graph = Graph;
        this.Positions = new();
        foreach (var node in Graph.GetNodes()) {
            this.Positions.Add(node, new((float)this.rng.NextDouble(), (float)this.rng.NextDouble()));
        }
    }

    static private float AttractiveForce(float distance) {
        const float A = -2.0F;
        const float B = 12.0F;

        const float M = 2*A/B;

        return
            (float)Math.Exp(-distance * distance / B) * distance * M
            + (float)Math.Exp(-distance / B) * (A / B);
    }

    static private float RepulsiveForce(float distance) {
        const float C = 1.6F;

        if (distance <= 0.5F) distance = 0.5F;
        if (distance >= 10.0F) return 0.0F;

        return C * (float)Math.Exp(-distance) * (distance + 2.0F) / (distance * distance * distance);
    }

    static private Vector2 Gravity(Vector2 position) {
        var distance = Distance(position, Vector2.Zero);
        var normalized = position / distance;
        return -normalized * (float)Math.Sqrt(distance) * 0.001F;
    }

    static private float Distance(Vector2 thisNode, Vector2 otherNode) {
        return (float)Math.Sqrt(
            Math.Pow(thisNode.X - otherNode.X, 2)
            + Math.Pow(thisNode.Y - otherNode.Y, 2)
        );
    }

    // Returns ∇_{x,y} distance(thisNode, otherNode)
    static private Vector2 DistanceNabla(Vector2 thisNode, Vector2 otherNode) {
        var dist = Distance(thisNode, otherNode);
        return new(
            (thisNode.X - otherNode.X) / dist,
            (thisNode.Y - otherNode.Y) / dist
        );
    }

    private Vector2 Force(Node thisNode, Node otherNode) {
        var thisPos = this.Positions[thisNode];
        var otherPos = this.Positions[otherNode];

        var distNabla = DistanceNabla(thisPos, otherPos);
        var dist = Distance(thisPos, otherPos);

        var repulsion = RepulsiveForce(dist);

        if (this.Graph.HasEdge(thisNode, otherNode)) {
            var attraction = AttractiveForce(dist);
            return distNabla * (repulsion + attraction) + Gravity(thisPos);
        } else {
            return distNabla * repulsion + Gravity(thisPos);
        }
    }

    private Vector2 Force(Node thisNode) {
        Vector2 sum = new(0.0F, 0.0F);

        foreach (var otherNode in this.Graph.GetNodes()) {
            if (otherNode.Equals(thisNode)) continue;
            sum += this.Force(thisNode, otherNode);
        }

        return sum;
    }

    public void Step(float delta) {
        // We define V(node) = Σ_{i ∈ edges(nodes)} f(dist(node, i)) + Σ_{i} g(dist(node, i))
        // With f(x) = d[(A*(e^(-x²/B)+e^(-x/B))]/dx
        // And g(x) = d[Ce^(-x)/x²]/dx

        foreach (var node in this.Graph.GetNodes()) {
            var acceleration = this.Force(node);

            Vector2 position = this.Positions.GetValueOrDefault(node, new(0.0F, 0.0F));
            position.X += acceleration.X * delta;
            position.Y += acceleration.Y * delta;
            this.Positions[node] = position;
        }

        // Re-center coordinates
        float maxX = float.NegativeInfinity;
        float minX = float.PositiveInfinity;
        float maxY = float.NegativeInfinity;
        float minY = float.PositiveInfinity;

        foreach (Vector2 position in this.Positions.Values) {
            maxX = Math.Max(position.X, maxX);
            maxY = Math.Max(position.Y, maxY);
            minX = Math.Min(position.X, minX);
            minY = Math.Min(position.Y, minY);
        }

        Vector2 center = new(
            (maxX + minX) / 2.0F,
            (maxY + minY) / 2.0F
        );
        foreach (var node in this.Graph.GetNodes()) {
            this.Positions[node] -= center;
        }
    }

    // TODO: move this to another class
    public void Draw(int width, int height) {
        // Normalize coordinates
        float maxX = float.NegativeInfinity;
        float minX = float.PositiveInfinity;
        float maxY = float.NegativeInfinity;
        float minY = float.PositiveInfinity;

        foreach (Vector2 position in this.Positions.Values) {
            maxX = Math.Max(position.X, maxX);
            maxY = Math.Max(position.Y, maxY);
            minX = Math.Min(position.X, minX);
            minY = Math.Min(position.Y, minY);
        }

        float normalizeFactor = 0.9F * Math.Min(width / (maxX - minX), height / (maxY - minY));
        Dictionary<Node, (Node node, Vector2)> normalizedPos = this.Positions.Select((node, index) => (
            node.Key,
            new Vector2(
                node.Value.X * normalizeFactor + width / 2.0F,
                node.Value.Y * normalizeFactor + height / 2.0F
            )
        )).ToDictionary(tuple => tuple.Item1);

        foreach (var (fst, snd) in this.Graph.GetEdges()) {
            var (_, fstPos) = normalizedPos[fst];
            var (_, sndPos) = normalizedPos[snd];
            Raylib.DrawLineV(fstPos, sndPos, Color.BEIGE);
        }

        foreach (var (node, (_, pos)) in normalizedPos) {
            Raylib.DrawCircleV(pos, 5.0F, Color.GOLD);
        }
    }
}
