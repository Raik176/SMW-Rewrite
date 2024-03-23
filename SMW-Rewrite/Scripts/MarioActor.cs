using Raylib_cs;
using SMW_Rewrite.Scripts.UI;
using System.Numerics;

namespace SMW_Rewrite.Scripts {
    internal class MarioActor : UIElement {
        public int pMeter;
        public int moveState; // 0 = walking, 1 = running, 2 = sprinting (prun)
        public int decelFrames;
        public int jumpFrames;
        public float speed;
        public bool isOnGround;
        public bool walkingRight;

        public readonly int gravity_jumping = 675;
        public readonly int gravity_nojump = 1350;

        public readonly int jump_speed = 300;
        public readonly float jump_speed_incr = 9.375f;

        public readonly int walk_speed = 75;
        public readonly float walk_accel = 337.5f;
        public readonly float walk_decel = 562.5f;

        public readonly int run_speed = 135;
        public readonly float run_accel = 337.5f;
        public readonly float run_decel = 1125f;

        public readonly int prun_speed = 180;
        public MarioActor(Vector2 position) : base(new Rectangle(position, new Vector2(Statics.tileScaleFactor, Statics.tileScaleFactor))) {
            pMeter = 0;
            decelFrames = 0;
            jumpFrames = 0;
        }

        public override void Destroy() {

        }

        public override void Render() {
            Raylib.DrawRectangle((int)bounds.X, (int)bounds.Y, (int)bounds.Width, (int)bounds.Height, Color.Lime);
        }
    }
}
