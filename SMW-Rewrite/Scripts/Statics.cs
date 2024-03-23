using Raylib_cs;
using SMW_Rewrite.Scripts.Level;
using System.Collections.ObjectModel;
using System.Drawing;
using System.IO.Compression;
using System.Numerics;

namespace SMW_Rewrite.Scripts {
    internal static class Statics {
        public static readonly Font normal;
        public static readonly byte[] fileSignature = GenerateFileSignature();
        public static readonly ReadOnlyCollection<Level.GameTile> tiles;
        public static readonly Tilesheet mainTiles;
        public static readonly int generalScaleFactor = 4;
        public static readonly int tileScaleFactor = 16 * generalScaleFactor;
        static Statics() {
            normal = LoadEmbeddedFont("SMW_Rewrite.Assets.Fonts.Normal.ttf");
            List<Level.GameTile> tiles = [
                new GameTile("leftCornerGrass",[new Point(0,0)],"plains",0),
                new GameTile("upGrassStraight",[new Point(1,0)],"plains",0),
                new GameTile("rightCornerGrass",[new Point(2,0)],"plains",0),
                new GameTile("upToRight",[new Point(3,1)],"plains",0),
                new GameTile("leftGrassDown",[new Point(0,1)],"plains",0),
                new GameTile("plainDirt",[new Point(1,1)],"plains",0),
                new GameTile("rightGrassDown",[new Point(2,1)],"plains",0),
                new GameTile("upToLeft",[new Point(3,0)],"plains",0)
            ];

            Statics.tiles = new ReadOnlyCollection<Level.GameTile>(tiles);
            mainTiles = LoadEmbeddedTilesheet("SMW_Rewrite.Assets.plains.png", 16);
        }

        public static Level.GameTile GetTileById(string id) {
            foreach (var tile in tiles) {
                if (tile.tileId == id) return tile;
            }
            return null;
        }

        private static Font LoadEmbeddedFont(string resourceName) {
            var assembly = System.Reflection.Assembly.GetExecutingAssembly();

            using Stream stream = assembly.GetManifestResourceStream(resourceName);
            byte[] bytes = new byte[stream.Length];
            stream.Read(bytes, 0, bytes.Length);

            Font font = Raylib.LoadFontFromMemory(Path.GetExtension(resourceName), bytes, 32, [], 0);

            return font;
        }
        public static Texture2D LoadEmbeddedImage(string resourceName) {
            var assembly = System.Reflection.Assembly.GetExecutingAssembly();

            using Stream stream = assembly.GetManifestResourceStream(resourceName);
            byte[] bytes = new byte[stream.Length];
            stream.Read(bytes, 0, bytes.Length);

            Image image = Raylib.LoadImageFromMemory(Path.GetExtension(resourceName), bytes);
            Texture2D texture = Raylib.LoadTextureFromImage(image);

            Raylib.UnloadImage(image);

            return texture;
        }
        public static Tilesheet LoadEmbeddedTilesheet(string resourceName, int tileSize) {
            var assembly = System.Reflection.Assembly.GetExecutingAssembly();

            using Stream stream = assembly.GetManifestResourceStream(resourceName);
            byte[] bytes = new byte[stream.Length];
            stream.Read(bytes, 0, bytes.Length);

            Tilesheet tilesheet = new Tilesheet(bytes, tileSize);

            return tilesheet;
        }

        private static byte[] GenerateFileSignature() { return [0xa1, 0xb2, 0xc3, 0xd4]; }

        public static byte[] CompressWithZlib(byte[] data) {
            using MemoryStream output = new();
            using (DeflateStream compressor = new(output, CompressionMode.Compress)) {
                compressor.Write(data, 0, data.Length);
            }
            return output.ToArray();
        }
        public static byte[] DecompressWithZlib(byte[] compressedData) {
            using MemoryStream input = new(compressedData);
            using MemoryStream output = new();
            using (DeflateStream decompressor = new(input, CompressionMode.Decompress)) {
                decompressor.CopyTo(output);
            }
            return output.ToArray();
        }

        public static Level.Level deserializeLevel(byte[] dat) {
            for (int i = 0; i < fileSignature.Length; i++) {
                if (fileSignature[i] != dat[i]) { //Uh, this aint a level idiot!!

                }
            }
            int sizeComp = dat.Length;

            bool isCompressed = dat[fileSignature.Length] == 1;

            byte[] data = new byte[dat.Length - fileSignature.Length - 1];
            Array.Copy(dat, fileSignature.Length + 1, data, 0, data.Length);

            if (isCompressed) data = DecompressWithZlib(data);
            int sizeUncomp = data.Length;
            using (MemoryStream memoryStream = new MemoryStream(data)) {
                using (BinaryReader reader = new BinaryReader(memoryStream)) {
                    Level.Level level = new Level.Level();
                    level.version = reader.ReadByte();
                    level.id = reader.ReadString();
                    level.name = reader.ReadString();
                    level.areas = new Dictionary<string, Area>();

                    level.compressed = isCompressed;
                    level.fileSizeCompressed = sizeComp;
                    level.fileSizeUncompressed = sizeUncomp;
                    level.tilemapCount = reader.ReadInt32();
                    Dictionary<int, string> tileMap = new Dictionary<int, string>();
                    for (int i = 0; i < level.tilemapCount; i++) {
                        tileMap.Add(i, reader.ReadString());
                    }

                    int areaCount = reader.ReadSByte();
                    for (int i = 0; i < areaCount; i++) {
                        Level.Area area = new Level.Area();

                        area.version = reader.ReadByte();
                        string areaId = reader.ReadString();
                        area.theme = reader.ReadString();
                        area.sizeX = reader.ReadInt32();
                        area.sizeY = reader.ReadInt32();
                        area.tiles = new List<LevelTile>();

                        System.Drawing.Point msp = new();
                        msp.X = reader.ReadInt32();
                        msp.Y = reader.ReadInt32();
                        area.marioStart = msp;

                        int tileCount = reader.ReadInt32();
                        for (int j = 0; j < tileCount; j++) {
                            Level.LevelTile tile = new Level.LevelTile();
                            tile.x = reader.ReadInt32();
                            tile.y = reader.ReadInt32();
                            tile.layer = reader.ReadSByte();
                            tile.tileId = tileMap[reader.ReadInt32()];

                            sbyte tileDataCount = reader.ReadSByte();
                            Dictionary<string, string> tileData = new Dictionary<string, string>();
                            for (int k = 0; k < tileDataCount; k++) {
                                reader.ReadString(); reader.ReadString();
                            }

                            tile.tileData = tileData;

                            area.tiles.Add(tile);
                        }

                        level.areas.Add(areaId, area);
                    }

                    return level;
                }
            }
        }

        public static bool CheckCollision(Raylib_cs.Rectangle collider1, Vector2[] shapeVertices, RenderShape shapeType) {
            List<Vector2> axes = [];

            axes.Add(new Vector2(1, 0));
            axes.Add(new Vector2(0, 1));

            if (shapeType == RenderShape.Triangle) {
                for (int i = 0; i < 3; i++) {
                    Vector2 edge = shapeVertices[(i + 1) % 3] - shapeVertices[i];
                    axes.Add(new Vector2(-edge.Y, edge.X));
                }
            }

            foreach (Vector2 axis in axes) {
                float minPlayer = float.PositiveInfinity;
                float maxPlayer = float.NegativeInfinity;
                foreach (Vector2 vertex in new Vector2[] {
                    new(collider1.X, collider1.Y),
                    new(collider1.X + collider1.Width, collider1.Y),
                    new(collider1.X, collider1.Y + collider1.Height),
                    new(collider1.X + collider1.Width, collider1.Y + collider1.Height)
                }) {
                    float projection = Vector2.Dot(vertex, axis);
                    minPlayer = Math.Min(minPlayer, projection);
                    maxPlayer = Math.Max(maxPlayer, projection);
                }

                float minShape = float.PositiveInfinity;
                float maxShape = float.NegativeInfinity;
                foreach (Vector2 vertex in shapeVertices) {
                    float projection = Vector2.Dot(vertex, axis);
                    minShape = Math.Min(minShape, projection);
                    maxShape = Math.Max(maxShape, projection);
                }

                if (!(maxPlayer >= minShape && maxShape >= minPlayer)) return false;
            }

            return true;
        }
    }

    namespace Level {
        public class Level {
            public Level() {
                areas = new Dictionary<string, Area>();
                id = "N/A";
                name = "N/A (Could not load)";
                fileSizeCompressed = -1;
                compressed = false;
                fileSizeUncompressed = -1;
                version = -1;
                tilemapCount = -1;
            }

            public Dictionary<string, Area> areas;
            public string id;
            public string name;

            //optional (technically, as its only used in the debug money)
            public int fileSizeCompressed;
            public bool compressed;
            public int fileSizeUncompressed;
            public short version;
            public int tilemapCount;
        }
        public class Area {
            public string theme = "";
            public int sizeX;
            public int sizeY;
            public Point marioStart;
            public List<LevelTile> tiles = [];

            //optional too
            public short version;
        }
        public class LevelTile {
            public int x;
            public int y;
            public int layer;
            public string tileId = "";
            public Dictionary<string, string>? tileData;
        }
        public class GameTile {
            public readonly string tileId;
            public readonly string tilesheet;
            public readonly Point[] sprites;
            public readonly int fps;
            public readonly RenderShape shape;

            public GameTile(string id, Point[] sprites, string tilesheet, int fps) : this(id, sprites, tilesheet, fps, RenderShape.Rectangle) {

            }
            public GameTile(string id, Point[] sprites, string tilesheet, int fps, RenderShape shape) {
                this.tileId = id;
                this.tilesheet = tilesheet;
                this.sprites = sprites;
                this.fps = fps;
                this.shape = shape;
            }

            public Vector2[] GetVertices(float x, float y) {
                switch (shape) {
                    case RenderShape.Rectangle:
                        return [
                            new(x,y),
                            new(x + 16,y),
                            new(x + 16, y + 16),
                            new(x, y + 16)
                            ];
                    case RenderShape.Triangle:
                        return [
                            new (x,y),
                            new(x + 16,y),
                            new(x + 16 / 2, y + 16)
                            ];
                }
                return [];
            }
        }
        public enum RenderShape {
            Rectangle,
            Triangle
        }
    }
}
