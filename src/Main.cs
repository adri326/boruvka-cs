using Raylib_cs;

namespace Boruvka;

class Program {
    private int Iteration = 0;
    private int width = 800;
    private int height = 480;
    private int connections = 50;
    private int nodes = 75;
    private int AutomatonIteration = 0;

    DrawGraph<int> Drawer;
    GraphAutomaton<int> Automaton;

    public Program() {
        Graph<int> g = new();
        Random rng = new();
        for (int i = 0; i < connections; i++) {
            g.AddEdge(rng.Next(nodes), rng.Next(nodes));
        }

        this.Drawer = new(g);
        this.Automaton = new Boruvka<int>();
    }

    public void Run() {
        Raylib.InitWindow(this.width, this.height, "Boruvska visualization");

        bool prevLMB = false;
        bool prevRMB = false;

        while (!Raylib.WindowShouldClose()) {
            this.Iteration += 1;

            bool LMB = Raylib.IsMouseButtonDown(MouseButton.MOUSE_BUTTON_LEFT);
            if (!LMB && prevLMB) {
                if (AutomatonIteration == 0) {
                    this.Automaton.Initialize(this.Drawer.GraphPosition.Graph);
                    this.Drawer.SetGroupings(this.Automaton.NodeGroups());
                    this.AutomatonIteration = 1;
                } else if (!this.Automaton.Finished()) {
                    this.Automaton.PerformRound();
                    this.Drawer.SetGroupings(this.Automaton.NodeGroups());
                    this.AutomatonIteration += 1;
                } else {
                    // Reset the automaton
                    this.Automaton.Initialize(this.Drawer.GraphPosition.Graph);
                    this.Drawer.SetGroupings(this.Automaton.NodeGroups());
                    this.AutomatonIteration = 1;
                }

            }
            prevLMB = LMB;

            bool RMB = Raylib.IsMouseButtonDown(MouseButton.MOUSE_BUTTON_RIGHT);
            if (!RMB && prevRMB) {
                Graph<int> g = new();
                Random rng = new();
                for (int i = 0; i < connections; i++) {
                    g.AddEdge(rng.Next(nodes), rng.Next(nodes));
                }

                this.Drawer = new(g);
                this.Automaton = new Boruvka<int>();
                this.Iteration = 0;
                this.AutomatonIteration = 0;
            }
            prevRMB = RMB;

            this.Draw();
        }

        Raylib.CloseWindow();
    }

    private void Draw() {
        Raylib.BeginDrawing();
        Raylib.ClearBackground(Color.WHITE);

        this.Drawer.Step(this.Iteration);
        this.Drawer.Draw(this.width, this.height);

        Raylib.DrawText("Iteration: " + this.AutomatonIteration, 4, 4, 20, Color.BLACK);
        Raylib.DrawText("Groups: " + this.Automaton.NodeGroups().Count, 4, 28, 20, Color.BLACK);
        Color color = this.Automaton.Finished() ? Color.GREEN : Color.DARKBROWN;
        Raylib.DrawText("Finished: " + (this.Automaton.Finished() ? "True" : "False"), 4, 52, 20, color);

        Raylib.EndDrawing();
    }

    public static void Main() {
        new Program().Run();
    }
}
