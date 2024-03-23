using Raylib_cs;
using System.Numerics;

namespace SMW_Rewrite.Scripts.UI {
    //TODO: Fix Button collision and make it look prettier
    internal class UIButton : UIElement {
        private Color normalColor;
        private Color hoverColor;
        private Color currentColor;
        private Color borderColor;
        private Color textColor;
        private int borderWidth;
        private string text;
        private int fontSize;
        private Font font;
        private TextAlignment textAlignment;
        private bool isMouseHovering;
        private bool centerX;

        public event EventHandler Clicked;
        public event EventHandler MouseEntered;
        public event EventHandler MouseLeft;

        public UIButton(Rectangle bounds, string text, int fontSize, Color normalColor, Color hoverColor, Color borderColor, int borderWidth, TextAlignment textAlignment, Font font = default)
            : base(bounds) {
            this.text = text;
            this.fontSize = fontSize;
            this.font = font;
            this.normalColor = normalColor;
            this.hoverColor = hoverColor;
            this.borderColor = borderColor;
            this.borderWidth = borderWidth;
            this.textAlignment = textAlignment;
            this.currentColor = normalColor;
        }
        public UIButton(Rectangle bounds, string text, int fontSize, Color normalColor, Color hoverColor, Color borderColor, int borderWidth, Font font = default)
            : this(bounds, text, fontSize, normalColor, hoverColor, borderColor, borderWidth, TextAlignment.Left, font) { }
        public UIButton(Rectangle bounds, string text, int fontSize, Color normalColor, Color hoverColor, Font font = default)
            : this(bounds, text, fontSize, normalColor, hoverColor, Color.White, 0, font) { }
        public UIButton(Rectangle bounds, string text, int fontSize, Color normalColor, Font font = default)
            : this(bounds, text, fontSize, normalColor, normalColor, font) { }
        public UIButton(Rectangle bounds, string text, int fontSize, Font font = default)
            : this(bounds, text, fontSize, Color.White, font) { }
        public UIButton(Rectangle bounds, string text, Font font = default)
            : this(bounds, text, 12, Color.White, font) { }

        public UIButton TextColor(Color c) {
            textColor = c;
            return this;
        }
        public UIButton CenterHorizontally() {
            centerX = true;
            return this;
        }

        public override void Render() {
            UpdateButtonState();
            bounds.X = (Raylib.GetScreenWidth() - bounds.Width) / 2;
            Raylib.DrawRectangleLinesEx(bounds, borderWidth, borderColor);
            Raylib.DrawRectangleRec(bounds, currentColor);

            Vector2 position = Vector2.Zero;

            switch (textAlignment) {
                case TextAlignment.Left:
                    position.X = bounds.X + borderWidth + 10;
                    break;
                case TextAlignment.Center:
                    position.X = bounds.X + (bounds.Width / 2) - (Raylib.MeasureTextEx(font, text, fontSize, 0).X / 2);
                    break;
                case TextAlignment.Right:
                    position.X = bounds.X + bounds.Width - Raylib.MeasureTextEx(font, text, fontSize, 0).X - borderWidth - 10;
                    break;
            }

            position.Y = bounds.Y + (bounds.Height / 2) - (fontSize / 2);

            Raylib.DrawTextEx(font, text, position, fontSize, 0, textColor);
        }

        private void UpdateButtonState() {
            isMouseHovering = IsInElement(Raylib.GetMousePosition());

            if (isMouseHovering) {
                currentColor = hoverColor;
                MouseEntered?.Invoke(this, EventArgs.Empty);
            } else {
                currentColor = normalColor;
                MouseLeft?.Invoke(this, EventArgs.Empty);
            }
        }

        protected override void OnClick() {
            Clicked?.Invoke(this, EventArgs.Empty);
        }

        public override void Destroy() {
            
        }
    }
}