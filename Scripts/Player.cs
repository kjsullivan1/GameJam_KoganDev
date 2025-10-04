using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameJam_KoganDev.Scripts.LevelEditor;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace GameJam_KoganDev.Scripts
{
    internal class Player
    {
        Rectangle playerRect = new Rectangle();
        Vector2 position = new Vector2();
        float moveSpeedX = .75f;
        float jumpForce = -18f;
        float gravity = 1.15f;
        Vector2 velocity = new Vector2();
        float friction = .35f;
        int terminalVel = 20;
        float maxMoveSpeed = 3.5f;
        float iMaxMS = 3.5f;

        Texture2D playerTexture;

        public Dictionary<string, Keys> Keybinds = new Dictionary<string, Keys>();

        public int levelIn = 0;
        public MapBuilder mapBuilder;

        enum PlayerStates { Movement, Jumping, Dashing }
        PlayerStates playerState = PlayerStates.Movement;
        bool isFalling = true;

        enum PlayerSkills { Break, Dash, Create, PowerJump }
        PlayerSkills playerSkill = PlayerSkills.Break;
        public int numDashes = 0;
        public int numCreate = 0;
        public int numPowerJump = 0;

        KeyboardState prevKB;

        float frameRate = 0;
        int shortJumpDelay = 10;
        int doubleJumpWindow = 9; // The closer the 0, the tighter the window to double jump 
        public bool hasDoubleJumped = false;

        bool blockLeft = false;

        float distance = 64 * 2.75f;
        public float dashDistance;

        public int goalPosY;
        public int minPosY;
        public int heightBounds;
        public bool addLevel = false;
        public bool changeLevel = false;

        Game1 game;
        int jumpCount = 0;
       // int enemyCreated = 1;

        public Rectangle PlayerRect
        {
            get { return playerRect; }
            set { playerRect = value; position = new Vector2(value.X, value.Y); }
        }
        public Vector2 Position
        {
            get { return position; }
            set
            {
                playerRect = new Rectangle((int)value.X, (int)value.Y, playerRect.Width, playerRect.Height);
                position = value;
            }
        }
        public Vector2 Velocity
        {
            get { return velocity; }
            set { velocity = value; }
        }

        public Player(Rectangle playerRect, ContentManager content, Dictionary<string, Keys> keys, Rectangle currBounds, Game1 game)
        {
            this.playerRect = playerRect;
            Position = new Vector2(playerRect.X, playerRect.Y);
            playerTexture = content.Load<Texture2D>("Player/PlayerTexture");

            Keybinds = keys;

            dashDistance = distance;
            goalPosY = 0;
            minPosY = currBounds.Height;
            heightBounds = currBounds.Height;
            this.game = game;
            //maxShortJumpDelay = shortJumpDelay;
        }

        public void Update(GameTime gameTime)
        {
            isFalling = true;
            frameRate = 1 / (float)gameTime.ElapsedGameTime.TotalSeconds;

            KeyboardState currKB = Keyboard.GetState();

            if (currKB.IsKeyDown(Keybinds["MoveRight"]))
            {
                velocity.X += moveSpeedX;

            }

            if (currKB.IsKeyDown(Keybinds["MoveLeft"]))
            {
                velocity.X -= moveSpeedX;
            }

            if (currKB.IsKeyDown(Keybinds["Jump"]) && prevKB.IsKeyUp(Keybinds["Jump"]) && playerState != PlayerStates.Jumping && velocity.Y < 8)
            {
                playerState = PlayerStates.Jumping;
                if(jumpCount == 0)
                {
                    jumpCount++;
                    game.CreateEnemies();
                }
               
                if (isFalling)
                {
                    //hasDoubleJumped = f;
                    velocity.Y = 0;
                }
                velocity.Y += jumpForce;
                isFalling = true;

            }
            else if (currKB.IsKeyDown(Keybinds["Jump"]) && prevKB.IsKeyUp(Keybinds["Jump"]) && playerState == PlayerStates.Jumping && !hasDoubleJumped)
            {
                if (velocity.Y < doubleJumpWindow && velocity.Y > -doubleJumpWindow)
                {
                    velocity.Y = 0;
                    velocity.Y += jumpForce / 1.5f;
                    hasDoubleJumped = true;
                }
            }

            if (currKB.IsKeyDown(Keybinds["BreakSkill"]) && prevKB.IsKeyUp(Keybinds["BreakSkill"]))
            {
                playerSkill = PlayerSkills.Break;
            }
            else if (currKB.IsKeyDown(Keybinds["CreateSkill"]) && prevKB.IsKeyUp(Keybinds["CreateSkill"]))
            {
                playerSkill = PlayerSkills.Create;
            }
            else if (currKB.IsKeyDown(Keybinds["DashSkill"]) && prevKB.IsKeyUp(Keybinds["DashSkill"]))
            {
                playerSkill = PlayerSkills.Dash;
            }
            else if (currKB.IsKeyDown(Keybinds["PowerJumpSkill"]) && prevKB.IsKeyUp(Keybinds["PowerJumpSkill"]))
            {
                playerSkill = PlayerSkills.PowerJump;
            }

            Position += Velocity;
            if (isFalling && playerState != PlayerStates.Dashing )
                velocity.Y += gravity;

            if(playerState == PlayerStates.Dashing )
            {
                if(velocity.X > 0)
                {
                    dashDistance -= velocity.X;
                }
                else
                {
                    dashDistance += velocity.X;
                }

                if(dashDistance <= 0)
                {
                    playerState = PlayerStates.Movement;
                    dashDistance = distance;
                    maxMoveSpeed = iMaxMS;
                }
            }

            if(velocity.X > 0)
            {
                velocity.X -= friction;
                if(velocity.X<0)
                     velocity.X = 0;

                if(velocity.X > maxMoveSpeed)
                    velocity.X = maxMoveSpeed;
            }
            else if(velocity.X < 0)
            {
                velocity.X += friction;
                if (velocity.X > 0)
                    velocity.X = 0;

                if(velocity.X < -maxMoveSpeed)
                    velocity.X = -maxMoveSpeed;
            }

            if(isFalling && velocity.Y > terminalVel)
            {
                velocity.Y = terminalVel;
            }

            Collisions(currKB);

            if (currKB.IsKeyDown(Keybinds["Skill"]))
            {
                switch(playerSkill)
                {
                    case PlayerSkills.Create:
                        if(isFalling && prevKB.IsKeyUp(Keybinds["Skill"]) && numCreate > 0)
                        {
                            Point playerMP = new Point(playerRect.Center.X / 64, (playerRect.Center.Y + (heightBounds * levelIn)) / 64 );

                            if (playerMP.Y + 1 < mapBuilder.currMap.GetLength(0) && mapBuilder.currMap[playerMP.Y + 1, playerMP.X] != 2)
                            {
                                mapBuilder.yMapDims[levelIn][playerMP.Y + 1, playerMP.X] = 2;
                                mapBuilder.RefreshPlatforms(levelIn, new Point(playerMP.X, playerMP.Y + 1), true);
                                numCreate--;
                            }
                        }
                        break;
                    case PlayerSkills.PowerJump:
                        if (prevKB.IsKeyUp(Keybinds["Jump"]) && currKB.IsKeyDown(Keybinds["Jump"]) && numPowerJump > 0)
                        {
                            velocity.Y = 0;
                            velocity.Y += jumpForce;
                            numPowerJump--;
                        }
                        break;
                    case PlayerSkills.Dash:
                        if (prevKB.IsKeyUp(Keybinds["Skill"]) && numDashes > 0 && currKB.IsKeyDown(Keybinds["MoveRight"]) ||
                            prevKB.IsKeyUp(Keybinds["Skill"]) && numDashes > 0 && currKB.IsKeyDown(Keybinds["MoveLeft"]))
                        {
                            float dashForce = 8f;
                            maxMoveSpeed += dashForce;
                            velocity.Y = 0;
                            playerState = PlayerStates.Dashing;
                            if(velocity.X > 0)
                            {
                                velocity.X += dashForce;
                            }
                            else
                            {
                                velocity.X -= dashForce;
                            }
                            numDashes--;

                            if(isFalling == false)
                            {
                                Position = new Vector2(position.X, position.Y - 2);
                            }
                        }
                        break;
                }
            }

            prevKB = currKB;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(playerTexture, PlayerRect, Color.White);
        }

        private void Collisions(KeyboardState kb)
        {
            foreach(GroundTile groundTile in mapBuilder.GroundTiles)
            {
                if(playerRect.TouchTopOf(groundTile.Rectangle))
                {
                    PlayerRect = new Rectangle((int)position.X, groundTile.Rectangle.Y - (playerRect.Height + 1), playerRect.Width, playerRect.Height);
                    isFalling = false;
                    velocity.Y = 0;
                    if (playerState != PlayerStates.Dashing)
                        playerState = PlayerStates.Movement;
                    hasDoubleJumped = false;
                    maxMoveSpeed = iMaxMS;
                }
            }
            bool refresh = false;
            PlatformTile brokenTile;
            foreach(PlatformTile platformTile in mapBuilder.PlatformTiles)
            {
                if(playerRect.TouchTopOf(platformTile.Rectangle) && isFalling)
                {
                    PlayerRect = new Rectangle((int)position.X, platformTile.Rectangle.Y - (playerRect.Height + 1), playerRect.Width, playerRect.Height);
                    isFalling = false;
                    velocity.Y = 0;
                    if (playerState != PlayerStates.Dashing)
                        playerState = PlayerStates.Movement;
                    hasDoubleJumped = false;
                    maxMoveSpeed = iMaxMS;
                }

                if(playerRect.TouchBottomOf(platformTile.Rectangle))
                {
                    if (kb.IsKeyDown(Keybinds["Skill"]))
                    {
                        switch (playerSkill)
                        {
                            case PlayerSkills.Break:
                                if (velocity.Y < 0)
                                {
                                    platformTile.TakeDamage();
                                    if(platformTile.health <= 0)
                                    {
                                        mapBuilder.yMapDims[levelIn][platformTile.mapPoint[0], platformTile.mapPoint[1]] = 0;
                                        refresh = true;
                                    }
                                }
                                break;
                        }
                    }

                    PlayerRect = new Rectangle((int)position.X, platformTile.Rectangle.Bottom + 1, playerRect.Width, playerRect.Height);
                    playerState = PlayerStates.Movement;
                    //isFalling = false;
                    velocity.Y = 1f;
                    Position += velocity;

                   
                }

                if(playerRect.TouchLeftOf(platformTile.Rectangle))
                {
                    if (playerState == PlayerStates.Dashing)
                    {
                        playerState = PlayerStates.Movement;
                        dashDistance = distance;
                        maxMoveSpeed = iMaxMS;
                    }

                    PlayerRect = new Rectangle(platformTile.Rectangle.Left - (playerRect.Width + 1), (int)position.Y, playerRect.Width, playerRect.Height);
                    if (velocity.X > 0)
                        velocity.X = 0;
                }

                if(playerRect.TouchRightOf(platformTile.Rectangle))
                {
                    if (playerState == PlayerStates.Dashing)
                    {
                        playerState = PlayerStates.Movement;
                        dashDistance = distance;
                        maxMoveSpeed = iMaxMS;
                    }

                    PlayerRect = new Rectangle(platformTile.Rectangle.Right + 1, (int)position.Y, playerRect.Width, playerRect.Height);
                    if (velocity.X < 0)
                        velocity.X = 0;
                }
            }
            if(refresh)
            {
                mapBuilder.RefreshPlatforms(levelIn, Point.Zero);
            }


            if (playerRect.Y + playerRect.Height/4 < goalPosY)
            {
                levelIn++;

                if(levelIn > mapBuilder.yMapDims.Count - 1 && game.createdEnemies != mapBuilder.yMapDims.Count - 1)
                {
                    jumpCount = 0;

                }
                else if(levelIn == mapBuilder.yMapDims.Count - 1 && game.createdEnemies == mapBuilder.yMapDims.Count - 1)
                {
                    jumpCount = 0;
                }


                goalPosY = -(heightBounds * (levelIn));
                addLevel = true;
                minPosY -= heightBounds;
                //velocity.Y += jumpForce;
                PlayerRect = new Rectangle(playerRect.X, playerRect.Y - (playerRect.Height + 2), playerRect.Width, playerRect.Height);
                game.enemyStartPos = position;
            }
            if(playerRect.Y > minPosY)
            {
                levelIn--;

                if(levelIn < mapBuilder.yMapDims.Count)
                {
                    jumpCount = 1;
                }

                changeLevel = true;
                minPosY += heightBounds;
                goalPosY = -(heightBounds * levelIn);
                PlayerRect = new Rectangle(playerRect.X, playerRect.Y, playerRect.Width, playerRect.Height);
                if (levelIn < 0)
                {
                    //Die
                }
            }
        }
    }
}
