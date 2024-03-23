using Raylib_cs;
using SMW_Rewrite.Scripts.Level;
using SMW_Rewrite.Scripts.UI;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace SMW_Rewrite.Scripts.Scenes {
    internal class IngameScene : Scene {
        private Level.Level level;
        private MarioActor mario;
        private string curArea;

        protected override void OnLoad() {
            curArea = "main";
            level = Statics.deserializeLevel(File.ReadAllBytes(AppDomain.CurrentDomain.BaseDirectory + "Level\\lol.lvl"));
            System.Drawing.Point start = level.areas[curArea].marioStart;
            mario = new MarioActor(new System.Numerics.Vector2(start.X, Raylib.GetScreenHeight() - (start.Y * Statics.tileScaleFactor) - 100));

        }
        protected override UIElement[] OnUpdate() {
            float delta = Raylib.GetFrameTime();
            Rectangle marioBounds = mario.GetBounds();
            Rectangle groundRect = new(marioBounds.X, marioBounds.Y + marioBounds.Height, marioBounds.Width, 1);
            Level.Area area = level.areas[curArea];
            Raylib.ClearBackground(new(0, 100, 200, 0)); //Hardcoding for now

            List<Level.LevelTile>[] tiles = [[], [], []];
            foreach (var item in area.tiles) {
                tiles[item.layer].Add(item);
            }
            Vector2 force = Vector2.Zero;
            force.Y += mario.gravity_nojump * delta;
            force.X += (Raylib.IsKeyDown(KeyboardKey.A) ? mario.walk_speed : (Raylib.IsKeyDown(KeyboardKey.D) ? -mario.walk_speed : 0)) * delta;

            int i = 0;
            foreach (var tileList in tiles) {
                foreach (var tile in tileList) {
                    GameTile gt = Statics.GetTileById(tile.tileId);
                    Vector2 pos = new(0 + (tile.x * Statics.tileScaleFactor), Raylib.GetScreenHeight() - (tile.y * Statics.tileScaleFactor));
                    System.Drawing.Point p = gt.sprites[0];
                    if (force.Y != 0 && Statics.CheckCollision(groundRect,gt.GetVertices(pos.X,pos.Y),gt.shape)) {
                        mario.isOnGround = true;
                        force.Y = 0;
                    }
                    Raylib.DrawTextureEx(Statics.mainTiles.GetTile(p.X, p.Y), pos, 0, Statics.generalScaleFactor, Color.White);
                }
                if (i == 1) { //TODO: refactor this
                    mario.Render();
                }
                i++;
            }
            marioBounds.Y += force.Y;
            marioBounds.X += force.X;
            mario.SetBounds(marioBounds);

            return base.OnUpdate();
        }
    }
}
