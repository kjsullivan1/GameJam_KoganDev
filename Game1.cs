using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using GameJam_KoganDev.Scripts.LevelEditor;
using System.Collections.Generic;
using GameJam_KoganDev.Scripts;
using GameJam_KoganDev.Scripts.UI;
using Accessibility;

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
        Vector2 initalCamPos;

       

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

        public enum GameStates { TitleScreen, inGame, EndLevel, CutScene, Credits, PreLevel}
        public GameStates gameState = GameStates.TitleScreen;

        public MouseState ms;
        public MouseState prevMs;
        Vector2 mousePos = Vector2.Zero;

        KeyboardState prevKB;

        List<int> sectionsPerLevel = new List<int>();

        Cutscene cutSceneManager;
        //public bool keyBindActive = false;

        bool fade = false;
        float fadeRate = 1;

        int cutSceneDialogue = 1;
        Color playerColor = Color.White;

        float preLevelTime = 3600f;
        float tPreLevelTime = 3600f;

        public bool startGame = false;

        SoundManager soundManager;
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
            sectionsPerLevel.Add(0);//Level 2
            sectionsPerLevel.Add(0); //Level 3
            sectionsPerLevel.Add(0);//Level 4
            sectionsPerLevel.Add(0);//Level 5
           
            UIManager = new UIManager();

            

            base.Initialize();
        }

        public void SetSkill(string text)
        {
            UIHelper.SetElementText(UIManager.uiElements["SkillSelection"], text);
        }

        public void CreateEnemies()
        {
            if(createdEnemies == 0)
            {
                switch(gameLevel)
                {
                    case 0:
                        AddAndPlaySound("denial1Theme", true);
                        break;
                    case 1:
                        AddAndPlaySound("bargaining1Theme", true);
                        break;
                    case 2:
                        AddAndPlaySound("depression1Theme", true);
                        break;
                    case 3:
                        AddAndPlaySound("anger1Theme", true);
                        break;
                    case 4:
                        AddAndPlaySound("acceptance1Theme", true);
                        break;
                }
            }
            createdEnemies++;

            int numEnemies = 1;

            switch(gameLevel)
            {
                case 0:
                    numEnemies = 1;
                    break;
                case 1:
                    numEnemies = 1;
                    break;
                case 2:
                    numEnemies = 2;
                    break;
                case 3:
                    numEnemies = 3;
                    break;
                case 4:
                    numEnemies = 4;
                    break;
            }

            for (int i = 0; i < numEnemies; i++)
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
            UIHelper.cutSceneFont = Content.Load<SpriteFont>("Fonts/Text");
            UIManager.CreateUIElements(new Vector2(graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight), this);

            UIHelper.SetElementVisibility("MainMenu", true, UIManager.uiElements);
            UIHelper.SetElementVisibility("MainCreditsBtn", true, UIManager.uiElements);

            soundManager = new SoundManager(Content, 1, 1, 1);
            soundManager.AddSound("menuThemeGamejam1", true);
            soundManager.PlaySound();
            //enemy = new Enemy(Content, player.Position, mapBuilder, mapBuilder.yMapDims[0], GraphicsDevice, 0);
            // TODO: use this.Content to load your game content here
        }

        public void StartGame()
        {
            gameState = GameStates.inGame;
            UIHelper.SetElementVisibility("SkillSelection", true, UIManager.uiElements);

            currBounds = new Rectangle(0, 0, graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight);
            player = new Player(new Rectangle(0, graphics.PreferredBackBufferHeight - 120, 50, 50), Content, Keybinds, currBounds, this);
            player.mapBuilder = mapBuilder;

            createdEnemies = 0;
            enemyStartPos = player.Position;
            levelBuilder.createItems.Clear();
            levelBuilder.dashItems.Clear();
            levelBuilder.powerJumpItems.Clear();
            gameLevels.Clear();
            levelBuilder.StartLevel(player.levelIn, graphics.PreferredBackBufferHeight * player.levelIn, gameLevel);
            gameLevels.Add(levelBuilder.gameMap);
            levelIndexes.Add(new Vector2(0, gameLevels.Count - 1));
            mapBuilder.Refresh(gameLevels, pixelSize, graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight);
           

            camera.Position = new Vector2(camera.Position.X - (mapBuilder.yMapDims[0].GetLength(1) * 64) / 2, camera.Position.Y);

            switch (gameLevel)
            {
                case 0:
                    AddAndPlaySound("denial0Theme", true);
                    break;
                case 1:
                    AddAndPlaySound("bargaining0Theme", true);
                    break;
                case 2:
                    AddAndPlaySound("depression0Theme", true);
                    break;
                case 3:
                    AddAndPlaySound("anger0Theme", true);
                    break;
                case 4:
                    AddAndPlaySound("acceptance0Theme", true);
                    break;
            }
        }

        public void StartNewLevel()
        {
            gameLevel++;
            gameState = GameStates.inGame;
            UIHelper.SetElementVisibility("SkillSelection", true, UIManager.uiElements);
            UIHelper.SetElementVisibility("BeatLevel", false, UIManager.uiElements);

            enemies.Clear();
            createdEnemies = 0;

            currBounds = new Rectangle(0, 0, graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight);
            
            mapBuilder.yMapDims.Clear();
            int tNumCreate = player.numCreate;
            int tNumDash = player.numDashes;
            int tNumJump = player.numPowerJump;
            player = new Player(new Rectangle(0, graphics.PreferredBackBufferHeight - 120, 50, 50), Content, Keybinds, currBounds, this);
            player.mapBuilder = mapBuilder;
            player.playerColor = playerColor;
            enemyStartPos = player.Position;
            player.numCreate = tNumCreate;
            player.numDashes = tNumDash;
            player.numPowerJump = tNumJump;
           
            levelBuilder.StartLevel(player.levelIn, graphics.PreferredBackBufferHeight * player.levelIn, gameLevel);
            gameLevels.Clear();
            gameLevels.Add(levelBuilder.gameMap);
            levelIndexes.Add(new Vector2(0, gameLevels.Count - 1));
            //mapBuilder = new MapBuilder();
            mapBuilder.PlatformTiles.Clear();
            mapBuilder.Refresh(gameLevels, pixelSize, graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight);

            camera.Position = new Vector2(camera.Position.X, ((camera.viewport.Y + camera.viewport.Height) / 2) - (camera.viewport.Height * player.levelIn));
            switch (gameLevel)
            {
                case 0:
                    AddAndPlaySound("denial0Theme", true);
                    break;
                case 1:
                    AddAndPlaySound("bargaining0Theme", true);
                    break;
                case 2:
                    AddAndPlaySound("depression0Theme", true);
                    break;
                case 3:
                    AddAndPlaySound("anger0Theme", true);
                    break;
                case 4:
                    AddAndPlaySound("acceptance0Theme", true);
                    break;
            }

            UpdateSkillPos(new Rectangle(currBounds.X, currBounds.Y, currBounds.Width, currBounds.Height));
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();
            KeyboardState kb = Keyboard.GetState();
            switch(gameState)
            {
                case GameStates.TitleScreen:
                    soundManager.Update(gameTime);
                    camera.Update(new Vector2(graphics.PreferredBackBufferWidth / 2, graphics.PreferredBackBufferHeight / 2));
                    UseMouse();
                    if(startGame)
                    {
                        currBounds = new Rectangle(0, 0, graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight);
                        UIManager.CreatePreLevel(gameLevel, "Stage 1: Denial", currBounds);
                    }
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
                            player.UpdateSkillText();
                        }
                    }
                    for (int i = levelBuilder.dashItems.Count - 1; i >= 0; i--)
                    {
                        if (levelBuilder.dashItems[i].Intersects(player.PlayerRect))
                        {
                            player.numDashes++;
                            levelBuilder.dashItems.RemoveAt(i);
                            player.UpdateSkillText();
                        }
                    }
                    for (int i = levelBuilder.powerJumpItems.Count - 1; i >= 0; i--)
                    {
                        if (levelBuilder.powerJumpItems[i].Intersects(player.PlayerRect))
                        {
                            player.numPowerJump++;
                            levelBuilder.powerJumpItems.RemoveAt(i);
                            player.UpdateSkillText();
                        }
                    }

                    // TODO: Add your update logic here
                    //enemy.Upate(gameTime, player, mapBuilder, currBounds);
                    int num = 0;
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
                      
                        if (enemies[i].enemyRect.Contains(player.PlayerRect.Center))
                        {
                            if(player.playerState == Player.PlayerStates.Dashing)
                            {
                                enemies.RemoveAt(i);
                            }
                            else
                            {
                                num++;
                                player.jumpForce = player.iJumpForce / 2;
                                player.maxMoveSpeed = player.iMaxMS / 2;
                            }
                        }
                    }
                    if(num == 0 && player.jumpForce != player.iJumpForce)
                    {
                        player.jumpForce = player.iJumpForce;
                        player.maxMoveSpeed = player.iMaxMS;
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
                            UIHelper.SetElementVisibility("SkillSelection", false, UIManager.uiElements);
                        }
                        else if (player.levelIn >= mapBuilder.yMapDims.Count)
                        {
                            levelBuilder.CreateNewSection((camera.viewport.Height * player.levelIn), gameLevel);
                            mapBuilder.yMapDims.Add(levelBuilder.gameMap);
                            UpdateSkillPos(new Rectangle(currBounds.X, currBounds.Y - currBounds.Height, currBounds.Width, currBounds.Height));

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
                        gameState = GameStates.CutScene;
                        CreateCutsceneMap();
                        player.playerColor = playerColor;
                        cutSceneManager.playerColor = playerColor;
                        UIHelper.SetElementVisibility("EndLevel", false, UIManager.uiElements);
                        AddAndPlaySound("dialogueTheme", true);
                        //cutSceneManager = new Cutscene(currBounds, gameLevel, UIManager, new Rectangle())
                        //gameState = GameStates.inGame;
                        //StartNewLevel();
                    }
                    break;
                case GameStates.CutScene:
                    camera.Update(new Vector2(camera.Position.X, graphics.PreferredBackBufferHeight / 2));
                    if(fade)
                    {

                    }
                    else
                    {
                        if(kb.IsKeyDown(Keys.Space) && prevKB.IsKeyUp(Keys.Space))
                        {
                            switch(gameLevel)
                            {
                                case 0:
                                    switch (cutSceneDialogue)
                                    {
                                        case 1:
                                            cutSceneManager.UpdateText(gameTime, false, true);
                                            break;
                                        case 2:
                                            cutSceneManager.UpdateText(gameTime, false, true);
                                            break;
                                        case 3:
                                            cutSceneManager.UpdateText(gameTime, true, false);
                                            break;
                                        case 4:
                                            cutSceneManager.UpdateText(gameTime, false, true);
                                            break;
                                        case 5:
                                            UIHelper.SetElementVisibility("C1D", false, UIManager.uiElements);
                                            UIHelper.SetElementVisibility("C2D", false, UIManager.uiElements);
                                            break;
                                        case 6:
                                            gameState = GameStates.PreLevel;
                                            UIManager.CreatePreLevel(gameLevel, "Stage 2: Bargaining", currBounds);
                                            cutSceneDialogue = 0;
                                            AddAndPlaySound("levelStartTheme", false);
                                            break;
                                        default:
                                            
                                            break;
                                    }
                                    break;

                                case 1:
                                    switch (cutSceneDialogue)
                                    {
                                        case 1:
                                            cutSceneManager.UpdateText(gameTime, true, false);
                                            break;
                                        case 2:
                                            cutSceneManager.UpdateText(gameTime, false, true);
                                            break;
                                        case 3:
                                            cutSceneManager.UpdateText(gameTime, false, true);
                                            break;
                                        case 4:
                                            cutSceneManager.UpdateText(gameTime, false, true);
                                            break;
                                        case 5:
                                            UIHelper.SetElementVisibility("C1D", false, UIManager.uiElements);
                                            UIHelper.SetElementVisibility("C2D", false, UIManager.uiElements);
                                            break;
                                        case 6:
                                            gameState = GameStates.PreLevel;
                                            UIManager.CreatePreLevel(gameLevel, "Stage 3: Depression", currBounds);
                                            cutSceneDialogue = 0;
                                            AddAndPlaySound("levelStartTheme", false);
                                            break;
                                    }

                                    break;
                                case 2:
                                    switch(cutSceneDialogue)
                                    {
                                        case 1:
                                            cutSceneManager.UpdateText(gameTime, false, true);
                                            break;
                                        case 2:
                                            cutSceneManager.UpdateText(gameTime, true, false);
                                            break;
                                        case 3:
                                            cutSceneManager.UpdateText(gameTime, true, false);
                                            break;
                                        case 4:
                                            cutSceneManager.UpdateText(gameTime, false, true);
                                            break;
                                        case 5:
                                            UIHelper.SetElementVisibility("C1D", false, UIManager.uiElements);
                                            UIHelper.SetElementVisibility("C2D", false, UIManager.uiElements);
                                            break;
                                        case 6:
                                            gameState = GameStates.PreLevel;
                                            UIManager.CreatePreLevel(gameLevel, "Stage 4: Anger", currBounds);
                                            cutSceneDialogue = 0;
                                            AddAndPlaySound("levelStartTheme", false);
                                            break;
                                    }
                                    break;
                                case 3:
                                    switch(cutSceneDialogue)
                                    {
                                        case 1:
                                            cutSceneManager.UpdateText(gameTime, false, true);
                                            break;
                                        case 2:
                                            cutSceneManager.UpdateText(gameTime, true, false);
                                            break;
                                        case 3:
                                            cutSceneManager.UpdateText(gameTime, true, false);
                                            break;
                                        case 4:
                                            cutSceneManager.UpdateText(gameTime, false, true);
                                            break;
                                        case 5:
                                            cutSceneManager.UpdateText(gameTime, true, false);
                                            break;
                                        case 6:
                                            cutSceneManager.UpdateText(gameTime, false, true);
                                            break;
                                        case 7:
                                            UIHelper.SetElementVisibility("C1D", false, UIManager.uiElements);
                                            UIHelper.SetElementVisibility("C2D", false, UIManager.uiElements);
                                            break;
                                        case 8:
                                            AddAndPlaySound("levelStartTheme", false);
                                            gameState = GameStates.PreLevel;
                                            UIManager.CreatePreLevel(gameLevel, "Stage 5: Acceptance", currBounds);
                                            cutSceneDialogue = 0;
                                            break;

                                    }
                                    break;
                                case 4:
                                    switch(cutSceneDialogue)
                                    {
                                        case 1:
                                            cutSceneManager.UpdateText(gameTime, false, true);
                                            break;
                                        case 2:
                                            cutSceneManager.UpdateText(gameTime, true, false);
                                            break;
                                        case 3:
                                            cutSceneManager.UpdateText(gameTime, true, false);
                                            break;
                                        case 4:
                                            cutSceneManager.UpdateText(gameTime, false, true);
                                            break;
                                        case 5:
                                            cutSceneManager.UpdateText(gameTime, true, false);
                                            break;
                                        case 6:
                                            cutSceneManager.UpdateText(gameTime, false, true);
                                            break;
                                        case 7:
                                            cutSceneManager.UpdateText(gameTime, true, false);
                                            break;
                                        case 8:
                                            //cutSceneManager.UpdateText(gameTime, false, true);
                                            UIHelper.SetElementVisibility("C1D", false, UIManager.uiElements);
                                            UIHelper.SetElementVisibility("C2D", false, UIManager.uiElements);
                                            break;
                                        case 9:
                                            // End game with Pan up into title
                                            UIHelper.SetElementVisibility("BeatLevel", false, UIManager.uiElements);
                                            gameState = GameStates.Credits;
                                            initalCamPos = camera.Position;
                                            UIManager.CreateEndGameCredits(currBounds);
                                            break;
                                    }
                                    break;
                            }
                            cutSceneDialogue++;
                        }
                    }
                    break;
                case GameStates.PreLevel:
                    soundManager.Update(gameTime);
                    preLevelTime -= (float)gameTime.ElapsedGameTime.TotalMilliseconds;
                    camera.Update(new Vector2(camera.Position.X, graphics.PreferredBackBufferHeight / 2));
                    if (preLevelTime < 0)
                    { 
                        preLevelTime = tPreLevelTime;
                        if(startGame)
                        {
                            StartGame();
                            startGame = false;
                        }
                        else
                        {
                            StartNewLevel();
                        }
                        UIHelper.SetElementVisibility("PreLevel", false, UIManager.uiElements);
                        
                    }
                    break;
                case GameStates.Credits:
                    float moveSpeed = 1.5f;
                    UseMouse();
                    if(camera.Position.Y > initalCamPos.Y - graphics.PreferredBackBufferHeight)
                            camera.Position = new Vector2(camera.Position.X, camera.Position.Y - moveSpeed);
                    camera.Update(camera.Position);
                    break;
            }


            prevKB = kb;

                base.Update(gameTime);
        }

        public void UpdateSkillPos(Rectangle bounds)
        {
            UIManager.UpdateTextBlock("SkillSelection", bounds);
        }

        public void AddAndPlaySound(string fileName, bool isLoop)
        {
            soundManager.StopCurrSounds();
            soundManager.ClearSounds();
            soundManager.AddSound(fileName, isLoop);
            soundManager.PlaySound();
        }

        public void AddAndReplaceSound(string fileName)
        {
            soundManager.StopCurrSounds();
            //soundManager.AddSoundAtTime(fileName);
            soundManager.PlaySound();
        }

        private void CreateCutsceneMap()
        {
            currBounds = new Rectangle(0, 0, graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight);
            int[,] cutSceneMap = new int[mapBuilder.yMapDims[0].GetLength(0), mapBuilder.yMapDims[0].GetLength(1)];
            mapBuilder.yMapDims.Clear();
            gameLevels.Clear();

           for(int y = cutSceneMap.GetLength(0) - 7; y < cutSceneMap.GetLength(0);  y++)
            {
                for(int x = 0; x < cutSceneMap.GetLength(1); x++)
                {
                    cutSceneMap[y, x] = 1;
                }
            }
            gameLevels.Add(cutSceneMap);
            levelIndexes.Add(new Vector2(0, gameLevels.Count - 1));
            //mapBuilder = new MapBuilder();
            mapBuilder.PlatformTiles.Clear();
            mapBuilder.Refresh(gameLevels, pixelSize, graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight);
            fade = true;
            fadeRate = 1f;

            int playerX = ((cutSceneMap.GetLength(1) - (cutSceneMap.GetLength(1) / 3) - 1) * 64);
            int playerY = ((cutSceneMap.GetLength(0) - 8) * 64) + 20;

            int responderX = 0 + (((cutSceneMap.GetLength(1) / 3)) * 64);
            
            cutSceneManager = new Cutscene(currBounds,Content ,gameLevel, UIManager, new Rectangle(playerX, playerY, player.PlayerRect.Width, player.PlayerRect.Height)
                , new Rectangle(responderX, playerY, player.PlayerRect.Width, player.PlayerRect.Height));
        }

        private void UseMouse()
        {
            this.IsMouseVisible = true;
            ms = Mouse.GetState();

            //UIManager.UpdateTextBlock("MainMenuTitle", Rectangle.Empty);

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
                    GraphicsDevice.Clear(Color.Black);
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
                            GraphicsDevice.Clear(Color.DarkRed);
                            break;
                        case 4:
                            GraphicsDevice.Clear(Color.ForestGreen);
                            break;
                    }
                    //GraphicsDevice.Clear(Color.SlateGray);

                    // TODO: Add your drawing code here

                   // Window.Title = "Position: " + player.Position + "    Velocity: " + player.Velocity + "   DoubleJ: " + player.hasDoubleJumped;

                    spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, null, null, null, null, camera.transform);

                    mapBuilder.Draw(spriteBatch);
                    player.Draw(spriteBatch);
                    //enemy.Draw(spriteBatch);
                    foreach (Enemy enemy in enemies)
                    {
                        enemy.Draw(spriteBatch);
                    }
                    levelBuilder.DrawItems(spriteBatch, Content);
                    UIManager.Draw(spriteBatch);
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
                            playerColor = Color.SlateGray;
                            break;
                        case 1:
                            GraphicsDevice.Clear(Color.DarkGoldenrod);
                            playerColor = Color.DarkGoldenrod;
                            break;
                        case 2:
                            GraphicsDevice.Clear(Color.RoyalBlue);
                            playerColor = Color.RoyalBlue;
                            break;
                        case 3:
                            GraphicsDevice.Clear(Color.DarkRed);
                            playerColor = Color.DarkRed;
                            break;
                        case 4:
                            GraphicsDevice.Clear(Color.ForestGreen);
                            playerColor = Color.ForestGreen;
                            break;
                    }
               
                    spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, null, null, null, null, camera.transform);
                    mapBuilder.Draw(spriteBatch);
                    player.Draw(spriteBatch);
                    UIManager.Draw(spriteBatch);
                    spriteBatch.End();

                    break;
                case GameStates.CutScene:
                    Color shadeColor = Color.Black * .45f;
                    float minRate = .45f;
                    switch (gameLevel + 1)
                    {
                        case 0:
                            GraphicsDevice.Clear(Color.SlateGray);
                            break;
                        case 1:
                            GraphicsDevice.Clear(Color.DarkGoldenrod);
                            minRate = .60f;
                            break;
                        case 2:
                            GraphicsDevice.Clear(Color.RoyalBlue);
                            minRate = .60f;
                            break;
                        case 3:
                            GraphicsDevice.Clear(Color.DarkRed);
                            minRate = .60f;
                            break;
                        case 4:
                            GraphicsDevice.Clear(Color.ForestGreen);
                            minRate = .60f;
                            break;
                    }
                   
                    spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, null, null, null, null, camera.transform);
                    
                    mapBuilder.Draw(spriteBatch);

                    cutSceneManager.Draw(spriteBatch);
                    

                    spriteBatch.Draw(Content.Load<Texture2D>("Player/PlayerTexture"), new Rectangle(currBounds.X - (64 * 8), currBounds.Y, currBounds.Width + 64, currBounds.Height),
                         Color.Black * minRate);
                    UIManager.Draw(spriteBatch);
                    if (fadeRate > minRate)
                    {
                        fadeRate -= .0025f;
                        spriteBatch.Draw(Content.Load<Texture2D>("Player/PlayerTexture"), new Rectangle(currBounds.X - (64 * 8), currBounds.Y, currBounds.Width + 64, currBounds.Height),
                        Color.Black * fadeRate);
                    }
                    else
                    {
                        fade = false;
                        if(cutSceneManager.mcText == 0 && cutSceneManager.rsText == 0)
                        {
                            switch (gameLevel)
                            {
                                case 0:
                                    cutSceneManager.UpdateText(gameTime, true, false);
                                    break;
                                case 1:
                                    cutSceneManager.UpdateText(gameTime, true, false);
                                    break;
                                case 2:
                                    cutSceneManager.UpdateText(gameTime, true, false);
                                    break;
                                case 3:
                                    cutSceneManager.UpdateText(gameTime, false, true);
                                    break;
                                case 4:
                                    cutSceneManager.UpdateText(gameTime, true, false);
                                    break;
                            }
                        }
                       
                       
                    }
                        
                        spriteBatch.End();
                    break;
                case GameStates.PreLevel:
                    GraphicsDevice.Clear(Color.Black);
                    spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, null, null, null, null, camera.transform);
                    UIManager.Draw(spriteBatch);
                    spriteBatch.End();
                    break;
                case GameStates.Credits:
                    GraphicsDevice.Clear(Color.Black);
                    spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, null, null, null, null, camera.transform);

                    mapBuilder.Draw(spriteBatch);
                    cutSceneManager.Draw(spriteBatch);
                    
                    spriteBatch.Draw(Content.Load<Texture2D>("Player/PlayerTexture"), new Rectangle(currBounds.X - (64 * 8), currBounds.Y, currBounds.Width + 64, currBounds.Height * 2),
                        Color.Black * .45f);
                    UIManager.Draw(spriteBatch);
                    spriteBatch.End();

                    break;


            }
           
            base.Draw(gameTime);
        }
    }
}
