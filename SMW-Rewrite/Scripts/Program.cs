using Raylib_cs;
using SMW_Rewrite.Scripts.Scenes;

namespace SMW_Rewrite.Scripts;

class Program {
    private static Scene scene { get; set; }
    public static void Main() {
        Raylib.SetConfigFlags(ConfigFlags.BorderlessWindowMode);
        Raylib.InitWindow(Raylib.GetScreenWidth(), Raylib.GetScreenHeight(), "Super Mario World - PC Edition"); //800,480
        Raylib.SetTargetFPS(60);

        LoadScene(new MainMenuScene());

        while (!Raylib.WindowShouldClose()) {
            Raylib.BeginDrawing();
            Raylib.ClearBackground(Color.White);

            scene?.Update();
            Raylib.DrawText($"FPS: {Raylib.GetFPS()}", 1, 0, 18, Color.Black);

            Raylib.EndDrawing();
        }

        Raylib.CloseWindow();
    }

    public static void LoadScene(Scene sc) {
        scene?.Unload();
        scene = sc;
        scene.Load();
    }
}