using System;
using System.Collections.Generic;
using System.IO.MemoryMappedFiles;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SharpDX.Direct3D11;

namespace GameJam_KoganDev.Scripts.LevelEditor
{
    internal class MapBuilder
    {
        private List<GroundTile> groundTiles = new List<GroundTile>();
        public List<GroundTile> GroundTiles { get { return groundTiles; } }
        private List<PlatformTile> platformTiles = new List<PlatformTile>();
        public List<PlatformTile> PlatformTiles { get { return platformTiles; } }
        private List <BackgroundTile> backgroundTiles = new List<BackgroundTile>();
        public List<BackgroundTile> BackgroundTiles { get { return backgroundTiles; } }

        private int width;
        private int height;

        public List<int[,]> yMapDims = new List<int[,]>();
        public int[,] currMap;

        public List<int> rows = new List<int>();
        public List<int> cols = new List<int>();

        public int inLevel = 0;
        public Vector2 ScreenSize = Vector2.Zero;

        Rectangle currBounds = Rectangle.Empty;

        public List<Vector2> points = new List<Vector2>();

        public List<int> FloorIndexes = new List<int>();
        public List<int> PlatformIndexes = new List<int>();


        public int Width
        {
            get { return width; } 
        }
        public int Height
        {
            get { return height; }
        }

        public int GetPoint(int row, int col, int[,] mapDims)
        {
            if (row >= 15)
                row = 14;
            else if (row <= 0)
                row = 0;
            if (col >= 15)
                col = 14;
            else if(col <= 0) col = 0;
                return mapDims[row, col];
        }

        public void RefreshPlatforms(int levelIn, Point addedPoint, bool adding = false)
        {
            if(adding)
            {

                platformTiles.Add(new PlatformTile(2, new Rectangle(addedPoint.X * 64, ((addedPoint.Y * 64)) - (levelIn * (int)ScreenSize.Y), 64, 64)));
                platformTiles[platformTiles.Count - 1].mapPoint = new[] { addedPoint.Y, addedPoint.X };

                //for (int y = 0; y < currMap.GetLength(0); y++)
                //{
                //    for (int x = 0; x < currMap.GetLength(1); x++)
                //    {
                //        int num = yMapDims[yMapDims.Count - 1][y, x];

                //        if (num == 2 && platformTiles.Contains(new PlatformTile(num, new Rectangle(x * 64, (y * 64) - (levelIn * (int)ScreenSize.Y), 64, 64))) == false)
                //        {
                          
                //        }
                //    }
                //}
            }
           

            for(int i = platformTiles.Count - 1;  i >= 0; i--)
            {
                if (platformTiles[i].isBroken)
                {
                    platformTiles.RemoveAt(i);
                }
            }
        }

        public void AddSection(int[,] map)
        {
            yMapDims.Add(map);
        }

        public void Refresh(List<int[,]> maps, int size, int screenWidth, int screenHeight)
        {
            groundTiles.Clear();
            platformTiles.Clear();
           
            //this.points = points;

            ScreenSize = new Vector2(screenWidth, screenHeight);

            for (int i = 0; i < maps.Count; i++)
            {
                if (yMapDims.Contains(maps[i]) == false)
                    yMapDims.Add(maps[i]);
            }

            //Build Map
            currMap = maps[maps.Count - 1];
            BuildMap(maps, size, screenHeight);
        }

        public void ChangeMap(int index)
        {
            currMap = yMapDims[index];
        }

        private void BuildMap(List<int[,]> maps, int size, int screenHeight)
        {
            for (int i = 0; i < maps.Count; i++)
            {
                rows.Add(maps[i].GetLength(0));
                cols.Add(maps[i].GetLength(1));

                

                for (int x = 0; x < cols[i]; x++)
                {
                    for (int y = 0; y < rows[i]; y++)
                    {
                        int num = maps[i][y, x];

                        if (num == 1) // Ground Tiles 
                        {
                            groundTiles.Add(new GroundTile(num, new Rectangle((x * size), (y * size) - (i * screenHeight), size, size)));
                            groundTiles[groundTiles.Count - 1].mapPoint = new[] { y, x };
                        }

                        if (num == 2)
                        {
                            backgroundTiles.Add(new BackgroundTile(num, new Rectangle((x * size), (y * size) - (i * screenHeight), size, size)));
                            backgroundTiles[backgroundTiles.Count - 1].mapPoint = new[] { y, x };
                            platformTiles.Add(new PlatformTile(num, new Rectangle((x * size), (y * size) - (i * screenHeight), size, size)));
                            platformTiles[platformTiles.Count - 1].mapPoint = new[] { y, x };
                        }

                        if(num == 0)
                        {
                            backgroundTiles.Add(new BackgroundTile(num, new Rectangle((x * size), (y * size) - (i * screenHeight), size, size)));
                            backgroundTiles[backgroundTiles.Count - 1].mapPoint = new[] { y, x };
                        }
                    }
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (BackgroundTile tile in backgroundTiles)
            {
                tile.Draw(spriteBatch);
            }
            foreach (GroundTile tile in groundTiles)
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
