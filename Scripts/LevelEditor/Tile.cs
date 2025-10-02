using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace GameJam_KoganDev.Scripts.LevelEditor
{
    internal class Tile
    {
        public Texture2D texture;

        private Rectangle rect;
        public int[] mapPoint;
        public Color tileColor = Color.White;
        private static ContentManager content;

        public Rectangle Rectangle
        {
            get { return rect; }
            set { rect = value; }
        }

        public void SetX(int x)
        {
            rect.X = x;
        }

        public void SetY(int y)
        {
            rect.Y = y;
        }

        public static ContentManager Content
        {
            protected get { return content; }
            set { content = value; }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, Rectangle, tileColor);
        }

        public void DrawInDirection(SpriteBatch spriteBatch, string dir)
        {
            switch(dir)
            {
                case "right":
                    spriteBatch.Draw(texture, Rectangle, Rectangle, color: Color.White);
                    break;
                case "left":
                    spriteBatch.Draw(texture, Rectangle,new Rectangle(0,0,Rectangle.Width,Rectangle.Height), tileColor, 0, new Vector2(Rectangle.Center.X, Rectangle.Center.Y), SpriteEffects.FlipHorizontally, 0);
                    break;
                case "up":
                    spriteBatch.Draw(texture, Rectangle, tileColor);
                        break;
                case "down":
                    spriteBatch.Draw(texture, Rectangle, new Rectangle(0, 0, Rectangle.Width, Rectangle.Height), tileColor, 0, new Vector2(Rectangle.Center.X, Rectangle.Center.Y), SpriteEffects.FlipVertically, 0);
                    break;
            }
        }

    }

    class PlatformTile : Tile
    {
        Random rand = new Random();
        public int health;
        List<Texture2D> textures = new List<Texture2D>();
        public PlatformTile(int i, Rectangle rect)
        {
            texture = Content.Load<Texture2D>("MapTiles/Tile" + i);
            this.Rectangle = rect;
            health = rand.Next(2, 4);
            switch(health) // based on health, load in broken tiles
            {
                case 2:
                    break;
                case 3:
                    break;
            }
        }
    }

    class GroundTile : Tile
    {
        public GroundTile(int i, Rectangle rect)
        {
            texture = Content.Load<Texture2D>("MapTiles/Tile" + i);
            this.Rectangle = rect;
        }
    }
}
