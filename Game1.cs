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


            player = new Player(new Rectangle(0, 768, 50, 50), Content, Keybinds);
            player.mapBuilder = mapBuilder;

            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            levelBuilder.StartLevel(0);
            gameLevels.Add(levelBuilder.gameMap);
            levelIndexes.Add(new Vector2(0, gameLevels.Count - 1));
            mapBuilder.Refresh(gameLevels, pixelSize, graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight, levelIndexes);
            // TODO: use this.Content to load your game content here
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();
            player.Update(gameTime);
            // TODO: Add your update logic here
            camera.Update(camera.Position);
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here

            Window.Title = "Position: " + player.Position + "    Velocity: " + player.Velocity;

            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, null, null, null, null, camera.transform);

            mapBuilder.Draw(spriteBatch);
            player.Draw(spriteBatch);

            spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}
