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
        float moveSpeedX = 1.15f;
        float jumpForce = -22f;
        float gravity = 1.75f;
        Vector2 velocity = new Vector2();
        float friction = .35f;
        int terminalVel = 20;
        float maxMoveSpeed = 4f;

        Texture2D playerTexture;

        public Dictionary<string, Keys> Keybinds = new Dictionary<string, Keys>();

        int levelIn = 0;
        public MapBuilder mapBuilder;

        enum PlayerStates { Movement, Jumping }
        PlayerStates playerState = PlayerStates.Movement;
        bool isFalling = true;

        KeyboardState prevKB;

        float frameRate = 0;
        int shortJumpDelay = 10;
        int doubleJumpWindow = 9; // The closer the 0, the tighter the window to double jump 
        bool hasDoubleJumped = false;

        public Rectangle PlayerRect
        {
            get { return playerRect; }
            set {  playerRect = value; position = new Vector2(value.X, value.Y); }
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

        public Player(Rectangle playerRect, ContentManager content, Dictionary<string, Keys> keys)
        {
            this.playerRect = playerRect;
            Position = new Vector2(playerRect.X, playerRect.Y);
            playerTexture = content.Load<Texture2D>("Player/PlayerTexture");

            Keybinds = keys;

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

            if (currKB.IsKeyDown(Keybinds["Jump"]) && prevKB.IsKeyUp(Keybinds["Jump"]) && playerState != PlayerStates.Jumping)
            {
                playerState = PlayerStates.Jumping;

                if(isFalling)
                {
                    hasDoubleJumped = true;
                    velocity.Y = 0;
                }
                velocity.Y += jumpForce;
                isFalling = true;
               
            }

            if (currKB.IsKeyDown(Keybinds["Jump"]) && prevKB.IsKeyUp(Keybinds["Jump"]) && playerState == PlayerStates.Jumping && !hasDoubleJumped)
            {
                if(velocity.Y < doubleJumpWindow && velocity.Y > -doubleJumpWindow)
                {
                    velocity.Y = 0;
                    velocity.Y += jumpForce / 1.5f;
                    hasDoubleJumped = true;
                }
            }

            Position += Velocity;
            if(isFalling)
                velocity.Y += gravity;

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

            Collisions();
            prevKB = currKB;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(playerTexture, PlayerRect, Color.White);
        }

        private void Collisions()
        {
            foreach(GroundTile groundTile in mapBuilder.GroundTiles)
            {
                if(playerRect.TouchTopOf(groundTile.Rectangle))
                {
                    PlayerRect = new Rectangle((int)position.X, groundTile.Rectangle.Y - (playerRect.Height + 1), playerRect.Width, playerRect.Height);
                    isFalling = false;
                    velocity.Y = 0;
                    playerState = PlayerStates.Movement;
                    hasDoubleJumped = false;
                }
            }

            foreach(PlatformTile platformTile in mapBuilder.PlatformTiles)
            {
                if(playerRect.TouchTopOf(platformTile.Rectangle) && isFalling)
                {
                    PlayerRect = new Rectangle((int)position.X, platformTile.Rectangle.Y - (playerRect.Height + 1), playerRect.Width, playerRect.Height);
                    isFalling = false;
                    velocity.Y = 0;
                    playerState = PlayerStates.Movement;
                    hasDoubleJumped = false;
                }

                if(playerRect.TouchBottomOf(platformTile.Rectangle))
                {
                    PlayerRect = new Rectangle((int)position.X, platformTile.Rectangle.Bottom + 1, playerRect.Width, playerRect.Height);
                    playerState = PlayerStates.Movement;
                    //isFalling = false;
                    velocity.Y = 1f;
                    Position += velocity;
                }

                if(playerRect.TouchLeftOf(platformTile.Rectangle))
                {
                    PlayerRect = new Rectangle(platformTile.Rectangle.Left - (playerRect.Width + 1), (int)position.Y, playerRect.Width, playerRect.Height);
                    if (velocity.X > 0)
                        velocity.X = 0;
                }

                if(playerRect.TouchRightOf(platformTile.Rectangle))
                {
                    PlayerRect = new Rectangle(platformTile.Rectangle.Right + 1, (int)position.Y, playerRect.Width, playerRect.Height);
                    if (velocity.X < 0)
                        velocity.X = 0;
                }
            }
        }
    }
}
