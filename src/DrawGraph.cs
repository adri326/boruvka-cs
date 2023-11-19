using System.Numerics;
using Raylib_cs;

namespace Boruvka;

public class DrawGraph<Node> where Node: IEquatable<Node>, IComparable {
    public readonly GraphPosition<Node> GraphPosition;
    readonly float Speed = 1.0F;
    private Dictionary<Node, (Node node, Vector2 pos)> NormalizedPos = new();
    private List<HashSet<Node>> Groupings = new();
    private List<Color> Colors = new();

    public DrawGraph(Graph<Node> graph) {
        this.GraphPosition = new(graph);

        this.StepFast();
    }

    public void SetGroupings(List<HashSet<Node>> groupings) {
        this.Groupings = groupings;
        Random rng = new();
        while (this.Colors.Count < this.Groupings.Count) {
            byte[] rgb = {0, 0, 0};
            rng.NextBytes(rgb);
            this.Colors.Add(new Color(rgb[0], rgb[1], rgb[2], (byte)255));
        }
    }

    public void Step(int iteration) {
        this.GraphPosition.Step(this.Speed / (float)Math.Sqrt(iteration + 10));
    }

    private void StepFast() {
        int count = 200;
        for (int i = 1; i < count; i++) {
            this.GraphPosition.Step((float)i / count * 0.5F);
        }
    }

    private void UpdateNormalizedPos(int width, int height) {
        // Normalize coordinates
        float maxX = float.NegativeInfinity;
        float minX = float.PositiveInfinity;
        float maxY = float.NegativeInfinity;
        float minY = float.PositiveInfinity;

        foreach (Vector2 position in this.GraphPosition.GetPositions().Select(pair => pair.pos)) {
            maxX = Math.Max(position.X, maxX);
            maxY = Math.Max(position.Y, maxY);
            minX = Math.Min(position.X, minX);
            minY = Math.Min(position.Y, minY);
        }

        float normalizeFactor = 0.9F * Math.Min(width / (maxX - minX), height / (maxY - minY));
        this.NormalizedPos = this.GraphPosition.GetPositions().Select((pair, index) => (
            pair.node,
            new Vector2(
                pair.pos.X * normalizeFactor + width / 2.0F,
                pair.pos.Y * normalizeFactor + height / 2.0F
            )
        )).ToDictionary(tuple => tuple.node);
    }

    private int GroupOf(Node node) {
        return this.Groupings.FindIndex(group => group.Contains(node));
    }

    private void DrawEdge(Node fst, Node snd) {
        if (!this.NormalizedPos.ContainsKey(fst) || !this.NormalizedPos.ContainsKey(snd)) return;

        var (_, fstPos) = this.NormalizedPos[fst];
        var (_, sndPos) = this.NormalizedPos[snd];

        var fstGrouping = this.GroupOf(fst);
        var sndGrouping = this.GroupOf(snd);

        if (fstGrouping != -1 && fstGrouping == sndGrouping) {
            Raylib.DrawLineEx(fstPos, sndPos, 2, this.Colors[fstGrouping]);
        } else {
            Raylib.DrawLineV(fstPos, sndPos, Color.BEIGE);
        }
    }

    private void DrawNode(Node node) {
        var grouping = this.GroupOf(node);
        if (grouping != -1) {
            Raylib.DrawCircleV(this.NormalizedPos[node].pos, 5.0F, this.Colors[grouping]);
        } else {
            Raylib.DrawCircleV(this.NormalizedPos[node].pos, 5.0F, Color.GOLD);
        }
    }

    public void Draw(int width, int height) {
        this.UpdateNormalizedPos(width, height);

        foreach (var (fst, snd) in this.GraphPosition.Graph.GetEdges()) {
            this.DrawEdge(fst, snd);
        }

        foreach (var (node, (_, pos)) in this.NormalizedPos) {
            this.DrawNode(node);
        }
    }
}
