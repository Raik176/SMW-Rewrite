using Raylib_cs;
using SMW_Rewrite.Scripts.UI;
using System.Numerics;

namespace SMW_Rewrite.Scripts {
    internal class MarioActor : UIElement {
        public int pMeter;
        public bool isOnGround;
        public readonly int gravity_jumping = 675;
        public readonly int gravity_nojump = 1350;
        public readonly int walk_speed = 75;
        public readonly int run_speed = 0;
        public readonly int prun_speed = 0;
        public MarioActor(Vector2 position) : base(new Rectangle(position, new Vector2(Statics.tileScaleFactor, Statics.tileScaleFactor))) {
            pMeter = 0;
        }

        public override void Destroy() {

        }

        public override void Render() {
            Raylib.DrawRectangle((int)bounds.X, (int)bounds.Y, (int)bounds.Width, (int)bounds.Height, Color.Lime);
        }
    }
}
