using Raylib_cs;

namespace Boruvka;

class Program {
    public static void Main() {
        Raylib.InitWindow(800, 480, "Boruvska visualization");

        while (!Raylib.WindowShouldClose()) {
            Raylib.BeginDrawing();
            Raylib.ClearBackground(Color.WHITE);


            Raylib.DrawCircle(100, 100, 50, Color.BEIGE);

            Raylib.DrawText("Hello, world!", 12, 12, 20, Color.BLACK);

            Raylib.EndDrawing();
        }

        Raylib.CloseWindow();
    }
}
