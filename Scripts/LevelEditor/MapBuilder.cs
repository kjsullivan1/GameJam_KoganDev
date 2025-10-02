using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GameJam_KoganDev.Scripts.LevelEditor
{
    internal class MapBuilder
    {
        private List<GroundTile> groundTiles = new List<GroundTile>();
        public List<GroundTile> GroundTiles { get { return groundTiles; } }
        private List<PlatformTile> platformTiles = new List<PlatformTile>();
        public List<PlatformTile> PlatformTiles { get { return platformTiles; } }

        private int width;
        private int height;

        public List<int[,]> yMapDims = new List<int[,]>();

        public List<int> rows = new List<int>();
        public List<int> cols = new List<int>();

        public int inLevel = 0;
        public Vector2 ScreenSize = Vector2.Zero;

        Rectangle currBounds = Rectangle.Empty;

        public int Width
        {
            get { return width; } 
        }
        public int Height
        {
            get { return height; }
        }

        public void Refresh(List<int[,]> maps, int size, int screenWidth, int screenHeight, List<Vector2> points)
        {
            groundTiles.Clear();
            platformTiles.Clear();

            ScreenSize = new Vector2(screenWidth, screenHeight);

            for (int i = 0; i < maps.Count; i++)
            {
                yMapDims.Add(maps[i]);
            }

            //Build Map
            BuildMap(maps, size, screenHeight, points);
        }

        private void BuildMap(List<int[,]> maps, int size, int screenHeight, List<Vector2> points)
        {
            for (int i = 0; i < maps.Count; i++)
            {
                rows.Add(maps[i].GetLength(0));
                cols.Add(maps[i].GetLength(1));

                int levelIn = (int)points[i].Y;

                for (int x = 0; x < cols[i]; x++)
                {
                    for (int y = 0; y < rows[i]; y++)
                    {
                        int num = maps[i][y, x];

                        if (num == 1) // Ground Tiles 
                        {
                            groundTiles.Add(new GroundTile(num, new Rectangle((x * size), (y * size) - (levelIn * screenHeight), size, size)));
                            groundTiles[groundTiles.Count - 1].mapPoint = new[] { y, x };
                        }

                        if (num == 2)
                        {
                            platformTiles.Add(new PlatformTile(num, new Rectangle((x * size), (y * size) - (levelIn * screenHeight), size, size)));
                            platformTiles[platformTiles.Count - 1].mapPoint = new[] { y, x };
                        }
                    }
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            foreach(GroundTile tile in groundTiles)
            {
                tile.Draw(spriteBatch);
            }
            foreach(PlatformTile tile in platformTiles)
            {
                tile.Draw(spriteBatch);
            }
        }
    }
}
