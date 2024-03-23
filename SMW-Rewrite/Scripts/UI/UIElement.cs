using Raylib_cs;
using System.Numerics;

namespace SMW_Rewrite.Scripts.UI {
    internal enum TextAlignment {
        Left,
        Center,
        Right
    }
    /// <summary>
    /// Represents a base class for UI elements.
    /// </summary>
    abstract internal class UIElement {
        protected Rectangle bounds;
        public bool active;

        public UIElement(Rectangle bounds) {
            this.bounds = bounds;
            this.active = true;
        }

        /// <summary>
        /// Sets the bounds of the UI element.
        /// </summary>
        /// <param name="bounds">The bounds to set.</param>
        /// <exception cref="NullReferenceException">Thrown when the bounds are null.</exception>
        public void SetBounds(Rectangle bounds) {
            if (bounds.Size == Vector2.Zero && bounds.Position == Vector2.Zero) throw new NullReferenceException("Bounds cannot be null");
            this.bounds = bounds;
        }

        /// <summary>
        /// Updates & renders the UI element.
        /// </summary>
        public void Update() {
            if (IsInElement(Raylib.GetMousePosition()) && Raylib.IsMouseButtonPressed(MouseButton.Left)) OnClick();

            Render();
        }

        /// <summary>
        /// Renders the UI element.
        /// </summary>
        public abstract void Render();

        /// <summary>
        /// Destroys the UI element.
        /// </summary>
        public abstract void Destroy();

        /// <summary>
        /// Checks if a point is within the bounds of the UI element.
        /// </summary>
        /// <param name="point">The point to check.</param>
        /// <returns>True if the point is within the bounds; otherwise, false.</returns>
        protected bool IsInElement(Vector2 point) {
            return (point.X >= bounds.X && point.X <= (bounds.X + bounds.Width) &&
                    point.Y >= bounds.Y && point.Y <= (bounds.Y + bounds.Height));
        }

        protected Rectangle ConvertRectangle(System.Drawing.Rectangle rectangle) {
            Rectangle rect = new();
            rect.X = rectangle.X;
            rect.Y = rectangle.Y;
            rect.Width = rectangle.Width;
            rect.Height = rectangle.Height;
            return rect;
        }

        /// <summary>
        /// Called when the UI element is clicked.
        /// </summary>
        protected virtual void OnClick() {

        }
    }
}