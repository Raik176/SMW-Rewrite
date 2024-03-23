using Raylib_cs;
using SMW_Rewrite.Scripts.UI;

namespace SMW_Rewrite.Scripts.Scenes {
    internal class IngameScene : Scene {
        private Level.Level level;
        
        protected override void OnLoad() {
            level = Statics.deserializeLevel(File.ReadAllBytes(AppDomain.CurrentDomain.BaseDirectory + "Level\\lol.lvl"));
            
        }
        protected override UIElement[] OnUpdate() {
            Level.Area area = level.areas["main"];
            Raylib.ClearBackground(new Color(0,100,200,0)); //Hardcoding for now

            List<Level.LevelTile>[] tiles = [[],[],[]];
            foreach (var item in area.tiles) {
                tiles[item.layer].Add(item);
            }
            foreach (var tileList in tiles) {
                foreach (var tile in tileList) {
                    System.Drawing.Point p = Statics.GetTileById(tile.tileId).sprites[0];
                    Raylib.DrawTextureEx(Statics.mainTiles.GetTile(p.X, p.Y), new System.Numerics.Vector2(0 + (tile.x * 16 * 4), Raylib.GetScreenHeight() - (tile.y * 16 * 4)), 0, 4, Color.White);
                }
            }

            return base.OnUpdate();
        }
    }
}
