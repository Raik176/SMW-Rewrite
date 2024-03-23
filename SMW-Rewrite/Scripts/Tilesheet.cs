using Raylib_cs;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using SMW_Rewrite.Scripts.UI;

namespace SMW_Rewrite.Scripts {
    internal class Tilesheet {
        private Texture2D[,] tiles;
        private int imgWidth;
        public Tilesheet(byte[] image, int tileSize) {
            using MemoryStream ms = new(image);
            using Image<Rgba32> img = SixLabors.ImageSharp.Image.Load<Rgba32>(ms);

            imgWidth = img.Width;
            int tilesWide = imgWidth / tileSize;
            int tilesHigh = img.Height / tileSize;


            tiles = new Texture2D[tilesWide, tilesHigh];
            for (int y = 0; y < tilesHigh; y++) {
                for (int x = 0; x < tilesWide; x++) {
                    Image<Rgba32> tile = img.Clone();
                    tile.Mutate(ctx => ctx.Crop(new SixLabors.ImageSharp.Rectangle(x * tileSize,y * tileSize,tileSize,tileSize)));

                    using MemoryStream stream = new();
                    tile.SaveAsPng(stream);

                    Raylib_cs.Image raylibImg = Raylib.LoadImageFromMemory(".png",stream.ToArray());
                    tiles[x, y] = Raylib.LoadTextureFromImage(raylibImg);
                    Raylib.UnloadImage(raylibImg);
                }
            }
        }

        public Texture2D GetTile(int x, int y) {
            return tiles[x, y];
        }
    }
}
