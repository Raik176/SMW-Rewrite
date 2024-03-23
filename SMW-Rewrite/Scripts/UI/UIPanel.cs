using Raylib_cs;
using System.Numerics;

namespace SMW_Rewrite.Scripts.UI {
    internal class UIPanel : UIElement {
        private Color color;
        private Texture2D texture;

        public UIPanel(Rectangle bounds, Color color) : base(bounds) {
            this.color = color;
        }
        public UIPanel(Rectangle bounds, Color color, Texture2D texture) : base(bounds) {
            this.color = color;
            this.texture = texture;
        }

        public override void Destroy() {
            Raylib.UnloadTexture(texture);
        }

        public override void Render() {
            if (texture.Id != 0) {
                Raylib.DrawTexturePro(texture, new Rectangle(0, 0, texture.Width, texture.Height),
                                      new Rectangle(bounds.X, bounds.Y, bounds.Width, bounds.Height),
                                      new Vector2(0, 0), 0f, color);
            } else {
                Raylib.DrawRectangleRec(bounds, color);
            }
        }
    }
}
