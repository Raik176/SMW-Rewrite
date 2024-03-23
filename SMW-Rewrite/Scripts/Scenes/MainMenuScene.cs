using Raylib_cs;
using SMW_Rewrite.Scripts.UI;
using System.Numerics;

namespace SMW_Rewrite.Scripts.Scenes {
    internal class MainMenuScene : Scene {
        Texture2D icon;
        protected override void OnLoad() {
            icon = Statics.LoadEmbeddedImage("SMW_Rewrite.Assets.title.png");
        }

        protected override UIElement[] OnUpdate() {
            int centerX = (Raylib.GetScreenWidth() - icon.Width) / 2;
            Raylib.DrawTextureEx(icon, new Vector2(centerX, Raylib.GetScreenHeight() * 0.1f), 0, 0.75f, Color.White);

            UIButton levelEditor = new(new Rectangle(centerX, Raylib.GetScreenHeight() * 0.45f, 100, 30), I18n.GetValue("ui.title.editor"), 18, Color.Red, default, Statics.normal);
            levelEditor.TextColor(Color.Black).CenterHorizontally().Clicked += delegate {
                Program.LoadScene(new IngameScene());
            };
            UIButton levels = new(new Rectangle(centerX, Raylib.GetScreenHeight() * 0.5f, 100, 30), I18n.GetValue("ui.title.levels"), 18, Color.Red, default, Statics.normal);
            levels.TextColor(Color.Black).CenterHorizontally().Clicked += delegate {
                Program.LoadScene(new IngameScene());
            };

            return [levelEditor, levels];
        }

        protected override void OnUnload() {
            Raylib.UnloadTexture(icon);
        }
    }
}
