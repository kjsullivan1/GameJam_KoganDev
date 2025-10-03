using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using GameJam_KoganDev.Scripts.LevelEditor;
using System.Collections.Generic;
using GameJam_KoganDev.Scripts;

namespace GameJam_KoganDev
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;
        public LevelBuilder levelBuilder = new LevelBuilder();
        List<int[,]> gameLevels = new List<int[,]>();
        MapBuilder mapBuilder = new MapBuilder();

        Player player;

        Camera camera;

        List<Vector2> levelIndexes = new List<Vector2>();

        public int pixelSize = 64;

        public Dictionary<string, Keys> Keybinds = new Dictionary<string, Keys>();

        Enemy enemy;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            graphics.PreferredBackBufferWidth = 1920;
            graphics.PreferredBackBufferHeight = 1080;

            graphics.ApplyChanges();

            Tile.Content = Content;


            camera = new Camera(GraphicsDevice.Viewport, new Vector2(graphics.PreferredBackBufferWidth / 2, graphics.PreferredBackBufferHeight / 2));
            camera.Zoom = 1f;
            levelBuilder.Initialize();

            Keybinds.Add("Jump", Keys.Space);
            Keybinds.Add("MoveLeft", Keys.A);
            Keybinds.Add("MoveRight", Keys.D);
            Keybinds.Add("Skill", Keys.LeftShift);
            Keybinds.Add("BreakSkill", Keys.Left);
            Keybinds.Add("CreateSkill", Keys.Up);
            Keybinds.Add("DashSkill", Keys.Down);
            Keybinds.Add("PowerJumpSkill", Keys.Right);

            Rectangle currBounds = new Rectangle(0,0 , graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight);
            player = new Player(new Rectangle(0, 768, 50, 50), Content, Keybinds, currBounds);
            player.mapBuilder = mapBuilder;

            

            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            levelBuilder.StartLevel(0, 0);
            gameLevels.Add(levelBuilder.gameMap);
            levelIndexes.Add(new Vector2(0, gameLevels.Count - 1));
            mapBuilder.Refresh(gameLevels, pixelSize, graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight);

            camera.Position = new Vector2(camera.Position.X - (mapBuilder.yMapDims[0].GetLength(1) * 64)/2, camera.Position.Y);
            enemy = new Enemy(Content, player.Position, mapBuilder, mapBuilder.yMapDims[0], GraphicsDevice, 0);
            // TODO: use this.Content to load your game content here
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            Rectangle currBounds = new Rectangle(0, 0 - (graphics.PreferredBackBufferHeight * player.levelIn), graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight);

            player.Update(gameTime);

            for(int i = levelBuilder.createItems.Count - 1; i >= 0; i--)
            {
                if (levelBuilder.createItems[i].Intersects(player.PlayerRect))
                {
                    player.numCreate++;
                    levelBuilder.createItems.RemoveAt(i);
                }
            }
            for(int i = levelBuilder.dashItems.Count - 1; i >= 0; i --)
            {
                if (levelBuilder.dashItems[i].Intersects(player.PlayerRect))
                {
                    player.numDashes++;
                    levelBuilder.dashItems.RemoveAt(i);
                }
            }
            for(int i = levelBuilder.powerJumpItems.Count - 1; i >= 0;i --)
            {
                if (levelBuilder.powerJumpItems[i].Intersects(player.PlayerRect))
                {
                    player.numPowerJump++;
                    levelBuilder.powerJumpItems.RemoveAt(i);
                }
            }

            // TODO: Add your update logic here
            enemy.Upate(gameTime, player, mapBuilder, currBounds);
            camera.Update(new Vector2(camera.Position.X, ((camera.viewport.Y + camera.viewport.Height) / 2) - (camera.viewport.Height * player.levelIn)));

            if(player.addLevel)
            {
                if(player.levelIn >= mapBuilder.yMapDims.Count)
                {
                    levelBuilder.CreateNewSection((camera.viewport.Height * player.levelIn));
                    mapBuilder.yMapDims.Add(levelBuilder.gameMap);

                    mapBuilder.Refresh(new List<int[,]>(mapBuilder.yMapDims), pixelSize, camera.viewport.Width, camera.viewport.Height);
                    //camera.Position = new Vector2(camera.Position.X, 0 - (camera.viewport.Height/2 * player.levelIn));
                }
                player.addLevel = false;
            }
            else if(player.changeLevel)
            {
                //camera.Position = new Vector2(camera.Position.X, 0 + (camera.viewport.Height / 2 * player.levelIn));
                player.changeLevel = false;
            }


                base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here

            Window.Title = "Position: " + player.Position + "    Velocity: " + player.Velocity + "   DoubleJ: " + player.hasDoubleJumped;

            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, null, null, null, null, camera.transform);

            mapBuilder.Draw(spriteBatch);
            player.Draw(spriteBatch);
            enemy.Draw(spriteBatch);
            levelBuilder.DrawItems(spriteBatch, Content);
            //foreach(Vector2 itemSpawn in levelBuilder.itemSpawns)
            //{
            //    spriteBatch.Draw(Content.Load<Texture2D>("MapTiles/Tile1"), new Rectangle(new Point((int)itemSpawn.X * 64, ((int)itemSpawn.Y * 64) -64), new Point(64, 64)), Color.Blue);
            //}

            spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}
