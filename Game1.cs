using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using GameJam_KoganDev.Scripts.LevelEditor;
using System.Collections.Generic;
using GameJam_KoganDev.Scripts;
using GameJam_KoganDev.Scripts.UI;

namespace GameJam_KoganDev
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;
        public LevelBuilder levelBuilder = new LevelBuilder();
        List<int[,]> gameLevels = new List<int[,]>();
        MapBuilder mapBuilder = new MapBuilder();
        UIManager UIManager;

        Player player;

        Camera camera;

        List<Vector2> levelIndexes = new List<Vector2>();

        public Rectangle currBounds;
        public int pixelSize = 64;

        public Dictionary<string, Keys> Keybinds = new Dictionary<string, Keys>();

        Enemy enemy;
        List<Enemy> enemies = new List<Enemy>();
        public int createdEnemies = 0;
        public int gameLevel = 0;
        public Vector2 enemyStartPos;

        public enum GameStates { TitleScreen, inGame, EndLevel, CutScene, Credits}
        public GameStates gameState = GameStates.TitleScreen;

        public MouseState ms;
        public MouseState prevMs;
        Vector2 mousePos = Vector2.Zero;

        KeyboardState prevKB;

        List<int> sectionsPerLevel = new List<int>();
        

        //public bool keyBindActive = false;

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

            graphics.IsFullScreen = false;
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

            sectionsPerLevel.Add(0);//Level 1
            sectionsPerLevel.Add(1);//Level 2
            sectionsPerLevel.Add(2); //Level 3
            sectionsPerLevel.Add(3);//Level 4
            sectionsPerLevel.Add(4);//Level 5
           
            UIManager = new UIManager();

            

            base.Initialize();
        }

        public void CreateEnemies()
        {
            
                createdEnemies++;
                for (int i = 0; i < gameLevel + 1; i++)
                {
                    enemies.Add(new Enemy(Content, enemyStartPos, mapBuilder, mapBuilder.currMap, GraphicsDevice, gameLevel));
                }
            
           
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            UIHelper.textFont = Content.Load<SpriteFont>("Fonts/TitleFont");
            UIHelper.buttonFont = Content.Load<SpriteFont>("Fonts/ButtonFont");
            UIHelper.endLevelFont = Content.LoadLocalized<SpriteFont>("Fonts/EndLevel");
            UIHelper.textBackground = Content.Load<Texture2D>("MapTiles/Black");
            UIHelper.playBtnBG = Content.Load<Texture2D>("Player/PlayerTexture");
            UIManager.CreateUIElements(new Vector2(graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight), this);
            //enemy = new Enemy(Content, player.Position, mapBuilder, mapBuilder.yMapDims[0], GraphicsDevice, 0);
            // TODO: use this.Content to load your game content here
        }

        public void StartGame()
        {
            gameState = GameStates.inGame;

            currBounds = new Rectangle(0, 0, graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight);
            player = new Player(new Rectangle(0, 768, 50, 50), Content, Keybinds, currBounds, this);
            player.mapBuilder = mapBuilder;
            enemyStartPos = player.Position;

            levelBuilder.StartLevel(player.levelIn, graphics.PreferredBackBufferHeight * player.levelIn, gameLevel);
            gameLevels.Add(levelBuilder.gameMap);
            levelIndexes.Add(new Vector2(0, gameLevels.Count - 1));
            mapBuilder.Refresh(gameLevels, pixelSize, graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight);

            camera.Position = new Vector2(camera.Position.X - (mapBuilder.yMapDims[0].GetLength(1) * 64) / 2, camera.Position.Y);
        }

        public void StartNewLevel()
        {
            gameState = GameStates.inGame;

            enemies.Clear();
            createdEnemies = 0;

            currBounds = new Rectangle(0, 0, graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight);
            
            mapBuilder.yMapDims.Clear();
            player = new Player(new Rectangle(0, 768, 50, 50), Content, Keybinds, currBounds, this);
            player.mapBuilder = mapBuilder;
            enemyStartPos = player.Position;
           
            levelBuilder.StartLevel(player.levelIn, graphics.PreferredBackBufferHeight * player.levelIn, gameLevel);
            gameLevels.Clear();
            gameLevels.Add(levelBuilder.gameMap);
            levelIndexes.Add(new Vector2(0, gameLevels.Count - 1));
            //mapBuilder = new MapBuilder();
            mapBuilder.PlatformTiles.Clear();
            mapBuilder.Refresh(gameLevels, pixelSize, graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight);

            camera.Position = new Vector2(camera.Position.X, ((camera.viewport.Y + camera.viewport.Height) / 2) - (camera.viewport.Height * player.levelIn));
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();
            KeyboardState kb = Keyboard.GetState();
            switch(gameState)
            {
                case GameStates.TitleScreen:
                    UIHelper.SetElementVisibility("MainMenu", true, UIManager.uiElements);
                    camera.Update(new Vector2(graphics.PreferredBackBufferWidth / 2, graphics.PreferredBackBufferHeight / 2));
                    UseMouse();
                    break;
                case GameStates.inGame:
                    currBounds = new Rectangle(0, 0 - (graphics.PreferredBackBufferHeight * player.levelIn), graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight);

                    player.Update(gameTime);

                    for (int i = levelBuilder.createItems.Count - 1; i >= 0; i--)
                    {
                        if (levelBuilder.createItems[i].Intersects(player.PlayerRect))
                        {
                            player.numCreate++;
                            levelBuilder.createItems.RemoveAt(i);
                        }
                    }
                    for (int i = levelBuilder.dashItems.Count - 1; i >= 0; i--)
                    {
                        if (levelBuilder.dashItems[i].Intersects(player.PlayerRect))
                        {
                            player.numDashes++;
                            levelBuilder.dashItems.RemoveAt(i);
                        }
                    }
                    for (int i = levelBuilder.powerJumpItems.Count - 1; i >= 0; i--)
                    {
                        if (levelBuilder.powerJumpItems[i].Intersects(player.PlayerRect))
                        {
                            player.numPowerJump++;
                            levelBuilder.powerJumpItems.RemoveAt(i);
                        }
                    }

                    // TODO: Add your update logic here
                    //enemy.Upate(gameTime, player, mapBuilder, currBounds);
                    for (int i = enemies.Count - 1; i >= 0; i--)
                    {
                        enemies[i].Upate(gameTime, player, mapBuilder, currBounds);

                        for (int j = i - 1; j >= 0; j--)
                        {
                            if (enemies[j].enemyRect.Intersects(enemies[i].enemyRect))
                            {
                                enemies[j].moveSpeed /= 2;
                            }
                            else if (enemies[j].moveSpeed != enemies[j].iMoveSpeed)
                            {
                                enemies[j].moveSpeed = enemies[j].iMoveSpeed;
                            }
                        }
                    }
                    camera.Update(new Vector2(camera.Position.X, ((camera.viewport.Y + camera.viewport.Height) / 2) - (camera.viewport.Height * player.levelIn)));

                    if (player.addLevel)
                    {
                        if(player.levelIn > sectionsPerLevel[gameLevel]) // Create end section of Level
                        {
                            int[,] section = new int[mapBuilder.currMap.GetLength(0), mapBuilder.currMap.GetLength(1)];

                            for(int x = 0; x < section.GetLength(1); x++)
                            {
                                section[section.GetLength(0) - 1, x] = 1;
                            }

                            mapBuilder.yMapDims.Add(section);
                            enemies.Clear();

                            mapBuilder.Refresh(new List<int[,]>(mapBuilder.yMapDims), pixelSize, camera.viewport.Width, camera.viewport.Height);

                            gameState = GameStates.EndLevel;
                            currBounds = new Rectangle(0, 0 - (graphics.PreferredBackBufferHeight * player.levelIn), graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight);
                            UIManager.CreateEndLevel(gameLevel, currBounds);
                        }
                        else if (player.levelIn >= mapBuilder.yMapDims.Count)
                        {
                            levelBuilder.CreateNewSection((camera.viewport.Height * player.levelIn), gameLevel);
                            mapBuilder.yMapDims.Add(levelBuilder.gameMap);

                            mapBuilder.Refresh(new List<int[,]>(mapBuilder.yMapDims), pixelSize, camera.viewport.Width, camera.viewport.Height);
                            //camera.Position = new Vector2(camera.Position.X, 0 - (camera.viewport.Height/2 * player.levelIn));
                        }
                        player.addLevel = false;
                    }
                    else if (player.changeLevel)
                    {
                        //camera.Position = new Vector2(camera.Position.X, 0 + (camera.viewport.Height / 2 * player.levelIn));
                        player.changeLevel = false;
                    }
                    break;
                case GameStates.EndLevel:
                    camera.Update(new Vector2(camera.Position.X, ((camera.viewport.Y + camera.viewport.Height) / 2) - (camera.viewport.Height * player.levelIn)));
                    currBounds = new Rectangle(0, 0 - (graphics.PreferredBackBufferHeight * player.levelIn), graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight);

                    if(kb.IsKeyDown(Keys.Space) && prevKB.IsKeyUp(Keys.Space))
                    {
                        gameLevel++;
                        //gameState = GameStates.inGame;
                        StartNewLevel();
                    }
                    break;
                case GameStates.Credits:
                    break;
            }


            prevKB = kb;

                base.Update(gameTime);
        }

        private void UseMouse()
        {
            this.IsMouseVisible = true;
            ms = Mouse.GetState();

            UIManager.UpdateTextBlock("MainMenuTitle", Rectangle.Empty);

            //bool hitButton = false;

            if ((ms.X > 0) && (ms.Y > 0) &&
               (ms.X < camera.viewport.Width) &&
               (ms.Y < camera.viewport.Height))
            {
                mousePos = new Vector2((int)(ms.Position.X + (camera.Position.X - (camera.viewport.Width / 2))),
                    (int)(ms.Position.Y + (camera.Position.Y - (camera.viewport.Height / 2)))); //Can be Moved to UIManager. Sending the mspos
                if (ms.RightButton == ButtonState.Released)
                {
                    if (ms.LeftButton == ButtonState.Released && prevMs.LeftButton == ButtonState.Pressed)
                    {
                        //if (keyBindActive)
                        //    keyBindActive = false;
                        foreach (UIWidget widget in UIManager.uiElements.Values)
                        {
                            if (widget is UIButton)
                            {
                                if (graphics.IsFullScreen)
                                {
                                    ((UIButton)widget).HitTest(
                                new Point((int)mousePos.X, (int)mousePos.Y));
                                }
                                else
                                {
                                    ((UIButton)widget).HitTest(
                                new Point((int)mousePos.X, (int)mousePos.Y));
                                }

                            }
                        }
                    }
                    else
                    {
                        foreach (UIWidget widget in UIManager.uiElements.Values)
                        {
                            if (widget is UIButton)
                            {
                                ((UIButton)widget).Pressed = false;
                            }
                        }
                    }
                }
            }
            else
                mousePos = Vector2.Zero;


            //float reticleSpeed = 3f;

           // UseController();
            prevMs = ms;
        }

        protected override void Draw(GameTime gameTime)
        {
            switch(gameState)
            {
                case GameStates.TitleScreen:
                    GraphicsDevice.Clear(Color.CornflowerBlue);
                    spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, null, null, null, null, camera.transform);
                    UIManager.Draw(spriteBatch);
                    spriteBatch.End();
                    break;
                case GameStates.inGame:
                    switch (gameLevel)
                    {
                        case 0:
                            GraphicsDevice.Clear(Color.SlateGray);
                            break;
                        case 1:
                            GraphicsDevice.Clear(Color.DarkGoldenrod);
                            break;
                        case 2:
                            GraphicsDevice.Clear(Color.RoyalBlue);
                            break;
                        case 3:
                            GraphicsDevice.Clear(Color.Crimson);
                            break;
                        case 4:
                            GraphicsDevice.Clear(Color.ForestGreen);
                            break;
                    }
                    //GraphicsDevice.Clear(Color.SlateGray);

                    // TODO: Add your drawing code here

                    Window.Title = "Position: " + player.Position + "    Velocity: " + player.Velocity + "   DoubleJ: " + player.hasDoubleJumped;

                    spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, null, null, null, null, camera.transform);

                    mapBuilder.Draw(spriteBatch);
                    player.Draw(spriteBatch);
                    //enemy.Draw(spriteBatch);
                    foreach (Enemy enemy in enemies)
                    {
                        enemy.Draw(spriteBatch);
                    }
                    levelBuilder.DrawItems(spriteBatch, Content);
                    //foreach(Vector2 itemSpawn in levelBuilder.itemSpawns)
                    //{
                    //    spriteBatch.Draw(Content.Load<Texture2D>("MapTiles/Tile1"), new Rectangle(new Point((int)itemSpawn.X * 64, ((int)itemSpawn.Y * 64) -64), new Point(64, 64)), Color.Blue);
                    //}

                    spriteBatch.End();
                    break;
                case GameStates.EndLevel:
                    switch (gameLevel)
                    {
                        case 0:
                            GraphicsDevice.Clear(Color.SlateGray);
                            break;
                        case 1:
                            GraphicsDevice.Clear(Color.DarkGoldenrod);
                            break;
                        case 2:
                            GraphicsDevice.Clear(Color.RoyalBlue);
                            break;
                        case 3:
                            GraphicsDevice.Clear(Color.Crimson);
                            break;
                        case 4:
                            GraphicsDevice.Clear(Color.ForestGreen);
                            break;
                    }
                    spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, null, null, null, null, camera.transform);
                    mapBuilder.Draw(spriteBatch);
                    player.Draw(spriteBatch);
                    UIManager.Draw(spriteBatch);
                    spriteBatch.End();

                    break;
            }
           
            base.Draw(gameTime);
        }
    }
}
