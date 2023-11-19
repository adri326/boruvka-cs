using Raylib_cs;

namespace Boruvka;

class Program {
    public static void Main() {
        Raylib.InitWindow(800, 480, "Boruvska visualization");

        Graph<int> g = new();
        Random rng = new();
        for (int i = 0; i < 25; i++) {
            g.AddEdge(rng.Next(50), rng.Next(50));
        }

        GraphPosition<int> gp = new(g);

        for (int i = 0; i < 10; i++) {
            gp.Step(0.2F);
        }

        int iteration = 0;
        while (!Raylib.WindowShouldClose()) {
            iteration += 1;
            Raylib.BeginDrawing();
            Raylib.ClearBackground(Color.WHITE);

            gp.Step(0.5F / (float)Math.Sqrt(iteration + 10));
            gp.Draw(800, 480);

            Raylib.EndDrawing();
        }

        Raylib.CloseWindow();
    }
}
