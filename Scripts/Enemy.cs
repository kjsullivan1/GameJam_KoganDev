using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using GameJam_KoganDev.Scripts.LevelEditor;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace GameJam_KoganDev.Scripts
{
    internal class Enemy
    {
        #region Enemy Info
        public Rectangle enemyRect;
        public Vector2 position;
        public Player tPlayer;

        Texture2D texture;

        public Vector2 velocity;
        public Vector2 targetOffset = Vector2.Zero;
        public float moveSpeed = .75f;
        public float iMoveSpeed = .75f;
        public bool isColliding = false;
        int visionLength = 15;
        int pixelSize = 64;
        int tileSize = 58;
        public bool isDead = false;
        float angle = 0;
        #endregion
        #region Tracker vars
        public bool strafeUp;
        public bool strafeDown;
        public bool strafeLeft;
        public bool strafeRight;


        bool blockedRight;
        bool blockedLeft;
        bool blockedTop;
        bool blockedBottom;

        bool inSight = false;

        bool xDiag = false;
        bool prioXStrafe = false;

        public int points = 0;


        float pauseX = 0;
        float pauseY = 0;

        bool pause = false;

        Tile collidingTileX;
        Tile collidingTileY;

        public List<Vector2> targets = new List<Vector2>();
        public Vector2 target = new Vector2();

        public List<Rectangle> vision = new List<Rectangle>();

        public MapBuilder map;

        public Vector2 distToTravel = new Vector2();
        int[,] mapDims;

        bool fixer = true;
        bool stopper = false;
        ContentManager content;
        #endregion

        public Rectangle Rectangle
        {
            get { return enemyRect; }
        }

        int GetRow(int yMod)
        {
            return Math.Abs((int)((position.Y + yMod) / tileSize));
        }
        int GetCol(int xMod)
        {
            return Math.Abs((int)((position.X + xMod) /tileSize));
        }

        public Enemy(ContentManager Content, Vector2 spawnPos, MapBuilder map, int[,] mapDims, GraphicsDevice graphics, int levelNum)
        {
            position = spawnPos;
            enemyRect = new Rectangle(spawnPos.ToPoint(), new Point(tileSize, tileSize));
            this.map = map;
            content = Content;
            this.mapDims = mapDims;
            position = spawnPos;

            switch (levelNum)
            {
                case 0:
                    moveSpeed = 2.25f;
                    iMoveSpeed = moveSpeed;
                    break;
                case 1:
                    moveSpeed = 1.75f;
                    iMoveSpeed = moveSpeed;
                    break;
                case 2:
                    moveSpeed = 2f;
                    iMoveSpeed = moveSpeed;
                    break;
                case 3:
                    moveSpeed = 2.25f;
                    iMoveSpeed = moveSpeed;
                    break;
                case 4:
                    moveSpeed = 2.75f;
                    iMoveSpeed = moveSpeed;
                    break;
            }
        }

        public void SetTargetPersonality()
        {
            //Will determine a target personality of targeting left right up or down of the player
            Random rand = new Random();
            switch (rand.Next(4))
            {
                case 0: //Target right
                    targetOffset.X += 16;
                    break;
                case 1://Left
                    targetOffset.X -= 16;
                    break;
                case 2://Up
                    targetOffset.Y -= 16;
                    break;
                case 3://Down
                    targetOffset.Y += 16;
                    break;
            }
        }
        public void Upate(GameTime gameTime, Player player, MapBuilder map, Rectangle bounds)
        {
            this.map = map;
            this.tPlayer = player;
            enemyRect = new Rectangle((int)position.X, (int)position.Y, tileSize, tileSize);
            target = new Vector2(player.PlayerRect.Center.X + targetOffset.X, player.PlayerRect.Center.Y + targetOffset.Y);

            //Vector2 targetDir = new Vector2(player.PlayerRect.Center.X, player.PlayerRect.Center.Y) - new Vector2(enemyRect.Center.X, enemyRect.Center.Y);
            //angle = (float)Math.Atan2(targetDir.Y, targetDir.X);
            //destRe

            SetVision();
            blockedRight = false;
            blockedLeft = false;
            blockedTop = false;
            blockedBottom = false;

            //for(int i = vision.Count - 1; i >= 0; i--)
            //{
            //    foreach(PlatformTile tile in map.PlatformTiles)
            //    {
            //        if(vision[i].Contains(tile.Rectangle))
            //        {
            //            vision.Remove(vision[i]);
            //            break;
            //        }
            //    }
            //}
            #region Movement Logic
            inSight = false;
            foreach(Rectangle rect in vision)
            {
                if(rect.Intersects(player.PlayerRect))
                {
                    inSight = true;
                }
            }

            foreach (PlatformTile tile in map.PlatformTiles)
            {

                if (enemyRect.TouchLeftOf(tile.Rectangle)) //Right
                {
                    collidingTileX = tile;
                    blockedRight = true;
                    pauseY = 0;
                    if ((!strafeUp && !strafeDown))
                        strafeRight = false;

                    //position.X -= moveSpeed;
                }

                if (enemyRect.TouchRightOf(tile.Rectangle)) //Left
                {
                    collidingTileX = tile;
                    blockedLeft = true;
                    if ((!strafeUp && !strafeDown))
                        strafeLeft = false;
                    //position.X += moveSpeed;
                    pauseY = 0;
                }


                if (enemyRect.TouchBottomOf(tile.Rectangle)) //Bottom
                {
                    collidingTileY = tile;
                    blockedTop = true;
                    if (!strafeLeft && !strafeRight)
                        strafeUp = false;

                    //position.Y -= moveSpeed;
                }


                if (enemyRect.TouchTopOf(tile.Rectangle)) //Top
                {
                    collidingTileY = tile;
                    blockedBottom = true;
                    if (!strafeLeft && !strafeRight)
                        strafeDown = false;
                    //position.Y += moveSpeed;
                }
            }

            if (((distToTravel.X >= 0 && blockedRight) || (distToTravel.X <= 0 && blockedLeft)) || !prioXStrafe || strafeUp || strafeDown || pauseY <= 0 || pauseX <= 0)
            {
                if (strafeUp && !prioXStrafe)
                {
                    stopper = false;
                    if (distToTravel.Y < 0 && !blockedTop)
                    {
                        distToTravel.Y += moveSpeed;
                        position.Y -= moveSpeed;
                        blockedBottom = false;
                        stopper = true;
                        //animManager.isUp = true;
                        //animManager.isDown = false;
                    }
                    else if (distToTravel.Y >= 0)
                    {
                        strafeUp = false;
                        distToTravel.Y = 0;
                        stopper = false;
                        fixer = true;
                        //pauseY = 32;
                    }

                    if (blockedTop)
                    {
                        stopper = false;
                        if (distToTravel.X != 0)
                        {

                        }
                        else
                        {
                            strafeUp = false;

                        }
                    }

                }
                else if (strafeDown && !prioXStrafe)
                {
                    stopper = false;
                    if (distToTravel.Y > 0 && !blockedBottom)
                    {
                        distToTravel.Y -= moveSpeed;
                        position.Y += moveSpeed;
                        blockedTop = false;
                        stopper = true;
                       // animManager.isDown = true;
                        //animManager.isUp = false;
                    }
                    else if (distToTravel.Y <= 0)
                    {
                        strafeDown = false;
                        distToTravel.Y = 0;
                        stopper = false;
                        fixer = true;
                        //pauseY = 32;
                    }

                    if (blockedBottom)
                    {
                        stopper = false;
                        if (distToTravel.X != 0)
                        {

                        }
                        else
                        {
                            strafeDown = false;
                        }
                    }
                }
                else
                {
                    stopper = false;
                    if (!prioXStrafe)
                    {
                        stopper = false;
                        if (strafeUp)
                        {
                            strafeUp = false;
                        }
                        if (strafeDown)
                        {
                            strafeDown = false;
                        }
                    }
                    if (!strafeLeft && !strafeRight)
                    {
                        fixer = true;
                    }
                }
            }
            if (((distToTravel.Y >= 0 && blockedBottom) || (distToTravel.Y <= 0 && blockedTop)) || prioXStrafe || strafeLeft || strafeRight)
            {
                fixer = false;
                if (strafeLeft)
                {
                    if (distToTravel.X < 0 && !blockedLeft)
                    {
                        distToTravel.X += moveSpeed;
                        position.X -= moveSpeed;
                        blockedRight = false;
                       // animManager.isLeft = true;
                       // animManager.isRight = false;
                    }
                    else if (distToTravel.X >= 0)
                    {
                        strafeLeft = false;
                        if (!stopper)
                        {
                            fixer = true;
                        }
                        else if (!prioXStrafe)//Check the logic for this. Could be reason why pauses would happen with fixer = false
                        {
                            fixer = true;
                        }
                        prioXStrafe = false;
                        distToTravel.X = 0;
                        //fixer = true;
                        stopper = false;
                        //pauseX = 32;
                    }

                    if (blockedLeft)
                    {
                        prioXStrafe = false;
                        if (distToTravel.Y > 0 && blockedBottom)
                        {
                            fixer = true;

                        }
                        if (distToTravel.Y < 0 && blockedTop)
                        {
                            fixer = true;
                        }
                    }

                }
                else if (strafeRight)
                {
                    fixer = false;
                    if (distToTravel.X > 0 && !blockedRight)
                    {
                        distToTravel.X -= moveSpeed;
                        position.X += moveSpeed;
                        blockedLeft = false;
                        //animManager.isRight = true;
                        //animManager.isLeft = false;
                    }
                    else if (distToTravel.X <= 0)
                    {
                        strafeRight = false;
                        if (!stopper)
                        {
                            fixer = true;
                        }
                        else if (!prioXStrafe)
                        {
                            fixer = true;
                        }
                        prioXStrafe = false;
                        distToTravel.X = 0;

                    }

                    if (blockedRight)
                    {
                        prioXStrafe = false;

                        if (distToTravel.Y > 0 && blockedBottom)
                        {
                            fixer = true;
                        }
                        if (distToTravel.Y < 0 && blockedTop)
                        {
                            fixer = true;
                        }

                    }
                }
                else
                {
                    fixer = true;
                    if (strafeRight)
                    {
                        strafeRight = false;
                    }
                    if (strafeLeft)
                    {
                        strafeLeft = false;
                    }
                }
            }


            if (fixer && !stopper && !pause)
            {
                if (pauseX > 0)
                    pauseX -= moveSpeed;
                else if (pauseY > 0)
                    pauseY -= moveSpeed;

                //Handle Right Vision
                else if ((int)position.X < (int)target.X)
                {
                    if (blockedLeft && blockedBottom && blockedRight)
                    {
                        distToTravel.Y = -WallUntilOpening(GetRow(0), GetCol(0), "up", "right", GetRow(0), 0) * (tileSize * 2);
                        strafeUp = true;
                    }
                    else if (blockedLeft && blockedTop && blockedRight)
                    {
                        distToTravel.Y = WallUntilOpening(GetRow(32), GetCol(0), "down", "right", 15 - GetRow(0), 0) * (tileSize * 2);
                        strafeDown = true;
                    }
                    else if (blockedTop && blockedRight && blockedBottom)
                    {
                        distToTravel.X = -WallUntilOpening(GetRow(0), GetCol(0), "left", "up", GetCol(0), 0) * (tileSize);
                        strafeLeft = true;
                    }
                    else if (blockedRight && blockedTop && ((int)position.Y == (int)target.Y))
                    {

                        if (map.FloorIndexes.Contains(map.GetPoint(collidingTileY.mapPoint[0], collidingTileY.mapPoint[1] - 1, mapDims))
                             || map.FloorIndexes.Contains(map.GetPoint(collidingTileX.mapPoint[0] + 1, collidingTileX.mapPoint[1], mapDims)))
                        {
                            ChooseDiagStrafe(collidingTileY.mapPoint[0], collidingTileY.mapPoint[1], "up", "right");
                        }
                        else
                        {
                            distToTravel.X = -WallUntilOpening(collidingTileY.mapPoint[0], collidingTileY.mapPoint[1], "left", "up", collidingTileY.mapPoint[0] - 1, 0) * (tileSize * 2);
                            strafeLeft = true;

                            ChooseYStrafe(collidingTileX.mapPoint[0], collidingTileX.mapPoint[1] + (int)(distToTravel.X / 32), "", "right");

                        }
                    }
                    else if (blockedRight && blockedTop)
                    {
                        if ((int)position.Y > (int)target.Y && map.FloorIndexes.Contains(map.GetPoint(collidingTileY.mapPoint[0], collidingTileY.mapPoint[1] - 1, mapDims))
                            || (int)position.Y > (int)target.Y && map.FloorIndexes.Contains(map.GetPoint(collidingTileX.mapPoint[0] + 1, collidingTileX.mapPoint[1], mapDims)))
                        {
                            xDiag = true;
                        }
                        else if ((int)position.Y > (int)target.Y)
                        {
                            distToTravel.Y = -WallUntilOpening(collidingTileY.mapPoint[0], collidingTileY.mapPoint[1], "up", "right", (collidingTileY.mapPoint[0]), 0) * tileSize;
                            strafeUp = true;
                            prioXStrafe = true;
                        }
                        if ((int)position.Y < (int)target.Y && map.FloorIndexes.Contains(map.GetPoint(collidingTileY.mapPoint[0], collidingTileY.mapPoint[1] - 1, mapDims))
                             || (int)position.Y < (int)target.Y && map.FloorIndexes.Contains(map.GetPoint(collidingTileY.mapPoint[0] + 1, collidingTileY.mapPoint[1], mapDims)))
                        {
                            xDiag = true;
                        }
                        else if ((int)position.Y < (int)target.Y)
                        {
                            distToTravel.Y = WallUntilOpening(collidingTileX.mapPoint[0], collidingTileX.mapPoint[1], "down", "right", 15 - (collidingTileX.mapPoint[0]), 0) * (tileSize * 2);
                            strafeDown = true;
                        }
                    }
                    else if (blockedRight && blockedBottom && ((int)position.Y == (int)target.Y))
                    {

                        if (map.FloorIndexes.Contains(map.GetPoint(collidingTileY.mapPoint[0], collidingTileY.mapPoint[1] - 1, mapDims))
                             || map.FloorIndexes.Contains(map.GetPoint(collidingTileX.mapPoint[0] - 1, collidingTileX.mapPoint[1], mapDims)))
                        {
                            ChooseDiagStrafe(collidingTileY.mapPoint[0], collidingTileY.mapPoint[1], "down", "right");
                        }
                        else
                        {
                            distToTravel.X = -WallUntilOpening(collidingTileY.mapPoint[0], collidingTileY.mapPoint[1], "left", "down", collidingTileY.mapPoint[1] + 1, 0) * (tileSize * 2);
                            strafeLeft = true;

                            ChooseYStrafe(collidingTileX.mapPoint[0], collidingTileX.mapPoint[1] + (int)(distToTravel.X / 32), "", "right");
                            prioXStrafe = true;
                        }


                    }
                    else if (blockedRight && blockedBottom)
                    {
                        if ((int)position.Y > (int)target.Y && map.FloorIndexes.Contains(map.GetPoint(collidingTileY.mapPoint[0], collidingTileY.mapPoint[1], mapDims) - 1)
                             || (int)position.Y > (int)target.Y && map.FloorIndexes.Contains(map.GetPoint(collidingTileX.mapPoint[0] - 1, collidingTileX.mapPoint[1], mapDims))) // 
                        {
                            xDiag = true;
                        }
                        else if ((int)position.Y > (int)target.Y)
                        {

                        }
                        if ((int)position.Y < (int)target.Y && map.FloorIndexes.Contains(map.GetPoint(collidingTileY.mapPoint[0], collidingTileY.mapPoint[1], mapDims) - 1)
                             || (int)position.Y < (int)target.Y && map.FloorIndexes.Contains(map.GetPoint(collidingTileX.mapPoint[0] - 1, collidingTileX.mapPoint[1], mapDims)))// 
                        {
                            xDiag = true;
                        }
                        else if ((int)position.Y < (int)target.Y)
                        {
                            distToTravel.Y = -WallUntilOpening(collidingTileX.mapPoint[0], collidingTileX.mapPoint[1], "up", "right", collidingTileX.mapPoint[0], 0) * (tileSize * 2);
                            strafeUp = true;
                        }

                    }
                    else if (blockedRight && ((int)position.Y == (int)target.Y))
                    {
                        ChooseYStrafe(collidingTileX.mapPoint[0], collidingTileX.mapPoint[1], "", "left");
                    }
                    else if (blockedRight && (!blockedTop && !blockedBottom))
                    {

                    }
                    else if (!blockedRight)
                    {
                        position.X += moveSpeed;
                        //animManager.isRight = true;
                        //animManager.isLeft = false;
                        if (distToTravel.X < 0)
                        {
                            distToTravel.X = 0;
                        }
                    }


                }

                //Handle Left Vision
                else if ((int)position.X > (int)target.X)
                {

                    if (blockedLeft && blockedBottom && blockedRight)
                    {
                        distToTravel.Y = -WallUntilOpening(GetRow(0), GetCol(0), "up", "left", GetRow(0), 0) * (tileSize * 2);
                        strafeUp = true;
                    }
                    else if (blockedLeft && blockedTop && blockedRight)
                    {
                        distToTravel.Y = WallUntilOpening(GetRow(32), GetCol(0), "down", "right", 15 - GetRow(0), 0) * (tileSize * 2);
                        strafeDown = true;
                    }
                    else if (blockedTop && blockedLeft && blockedBottom)
                    {
                        distToTravel.X = WallUntilOpening(GetRow(0), GetCol(32), "right", "up", 25 - GetCol(0), 0) * (tileSize * 2);
                        strafeRight = true;
                    }
                    else if (blockedLeft && blockedTop && ((int)position.Y == (int)target.Y))
                    {
                        if (map.FloorIndexes.Contains(map.GetPoint(collidingTileY.mapPoint[0], collidingTileY.mapPoint[1], mapDims) + 1)
                             || map.FloorIndexes.Contains(map.GetPoint(collidingTileX.mapPoint[0] + 1, collidingTileX.mapPoint[1], mapDims)))
                        {
                            ChooseDiagStrafe(collidingTileY.mapPoint[0], collidingTileY.mapPoint[1], "up", "left");
                        }
                        else
                        {
                            distToTravel.X = WallUntilOpening(collidingTileY.mapPoint[0], collidingTileY.mapPoint[1], "right", "up", 25 - collidingTileY.mapPoint[1], 0) * (tileSize * 2);
                            strafeRight = true;

                            ChooseYStrafe(collidingTileX.mapPoint[0], collidingTileX.mapPoint[1] + (int)(distToTravel.X / 32), "", "left");
                            prioXStrafe = true;
                        }


                    }
                    else if (blockedLeft && blockedTop)
                    {
                        if ((int)position.Y > (int)target.Y && map.FloorIndexes.Contains(map.GetPoint(collidingTileY.mapPoint[0], collidingTileY.mapPoint[1] + 1, mapDims))
                             || (int)position.Y > (int)target.Y && map.FloorIndexes.Contains(map.GetPoint(collidingTileX.mapPoint[0] + 1, collidingTileX.mapPoint[1], mapDims)))
                        {
                            xDiag = true;
                        }
                        else if ((int)position.Y > (int)target.Y)
                        {
                            distToTravel.Y = -WallUntilOpening(collidingTileX.mapPoint[0], collidingTileX.mapPoint[1], "up", "left", collidingTileX.mapPoint[0], 0) * (tileSize * 2);
                            strafeUp = true;
                            prioXStrafe = true;
                        }
                        if ((int)position.Y < (int)target.Y && map.FloorIndexes.Contains(map.GetPoint(collidingTileY.mapPoint[0], collidingTileY.mapPoint[1] + 1, mapDims))
                             || (int)position.Y < (int)target.Y && map.FloorIndexes.Contains(map.GetPoint(collidingTileX.mapPoint[0] + 1, collidingTileX.mapPoint[1], mapDims)))
                        {
                            xDiag = true;
                        }
                        else if ((int)position.Y < (int)target.Y)
                        {
                            distToTravel.Y = WallUntilOpening(collidingTileX.mapPoint[0], collidingTileX.mapPoint[1], "down", "left", 15 - collidingTileX.mapPoint[0], 0) * (tileSize * 2);
                            strafeDown = true;
                        }
                    }
                    else if (blockedLeft && blockedBottom && ((int)position.Y == (int)target.Y))
                    {
                        if (map.FloorIndexes.Contains(map.GetPoint(collidingTileY.mapPoint[0], collidingTileY.mapPoint[1], mapDims) + 1)
                             || map.FloorIndexes.Contains(map.GetPoint(collidingTileX.mapPoint[0] - 1, collidingTileX.mapPoint[1], mapDims)))
                        {
                            ChooseDiagStrafe(collidingTileY.mapPoint[0], collidingTileY.mapPoint[1], "up", "left");
                        }
                        else
                        {
                            distToTravel.X = WallUntilOpening(collidingTileY.mapPoint[0], collidingTileY.mapPoint[1], "right", "down", 25 - collidingTileY.mapPoint[1], 0) * (tileSize * 2);
                            strafeRight = true;

                            ChooseYStrafe(collidingTileX.mapPoint[0], collidingTileX.mapPoint[1] + (int)(distToTravel.X / 32), "", "left");
                            prioXStrafe = true;
                        }
                        //distToTravel.Y = (tileSize * 2) + 1;
                        //strafeDown = true;


                    }
                    else if (blockedLeft && blockedBottom)
                    {
                        if ((int)position.Y > (int)target.Y && map.FloorIndexes.Contains(map.GetPoint(collidingTileY.mapPoint[0], collidingTileY.mapPoint[1] + 1, mapDims))
                             && (int)position.Y > (int)target.Y && map.FloorIndexes.Contains(map.GetPoint(collidingTileX.mapPoint[0] - 1, collidingTileX.mapPoint[1], mapDims)))
                        {
                            xDiag = true;
                        }
                        else if ((int)position.Y > (int)target.Y)
                        {
                            distToTravel.Y = -WallUntilOpening(collidingTileX.mapPoint[0], collidingTileX.mapPoint[1], "up", "left", collidingTileX.mapPoint[0], 0) * (tileSize);
                            strafeUp = true;
                        }
                        if ((int)position.Y < (int)target.Y && map.FloorIndexes.Contains(map.GetPoint(collidingTileY.mapPoint[0], collidingTileY.mapPoint[1] + 1, mapDims))
                             && (int)position.Y < (int)target.Y && map.FloorIndexes.Contains(map.GetPoint(collidingTileX.mapPoint[0] - 1, collidingTileX.mapPoint[1], mapDims)))
                        {
                            xDiag = true;
                        }
                        else if ((int)position.Y < (int)target.Y)
                        {

                            distToTravel.Y = WallUntilOpening(collidingTileY.mapPoint[0], collidingTileY.mapPoint[1], "down", "left", 15 - collidingTileY.mapPoint[0], 0) * (tileSize * 2);
                            strafeDown = true;
                            prioXStrafe = true;
                        }


                    }
                    else if (blockedLeft && ((int)position.Y == (int)target.Y))
                    {
                        ChooseYStrafe(collidingTileX.mapPoint[0], collidingTileX.mapPoint[1], "", "right");
                    }
                    else if (blockedLeft && (!blockedTop && !blockedBottom))
                    {
                        //ChooseYStrafe(collidingTileX.mapPoint[0], collidingTileX.mapPoint[1], "", "right");
                    }
                    else if (!blockedLeft)
                    {
                        position.X -= moveSpeed;
                        //animManager.isLeft = true;
                        //animManager.isRight = false;
                        if (distToTravel.X > 0)
                        {
                            distToTravel.X = 0;
                        }
                        //blockedRight = false;
                    }


                }

                //Handle Up vision
                if ((int)position.Y > (int)target.Y)
                {
                    if (blockedTop && blockedRight && blockedLeft)
                    {
                        distToTravel.Y = WallUntilOpening(GetRow(32), GetCol(0), "down", "left", 15 - GetRow(0), 0) * (tileSize * 2);
                        strafeDown = true;
                    }
                    else if (blockedTop && blockedRight && blockedBottom)
                    {
                        distToTravel.X = -WallUntilOpening(GetRow(0), GetCol(0), "left", "down", GetCol(0), 0) * (tileSize * 2);
                        strafeLeft = true;
                    }
                    else if (blockedTop && blockedLeft && blockedBottom)
                    {
                        distToTravel.X = WallUntilOpening(GetRow(0), GetCol(32), "right", "down", 25 - GetCol(0), 0) * (tileSize * 2);
                        strafeRight = true;
                    }
                    else if (blockedTop && blockedRight && ((int)position.X == (int)target.X))
                    {
                        if (map.FloorIndexes.Contains(map.GetPoint(collidingTileX.mapPoint[0] + 1, collidingTileX.mapPoint[1], mapDims))
                             || map.FloorIndexes.Contains(map.GetPoint(collidingTileY.mapPoint[0], collidingTileY.mapPoint[1] - 1, mapDims)))
                        {
                            ChooseDiagStrafe(collidingTileX.mapPoint[0], collidingTileX.mapPoint[1], "up", "right");
                        }
                        else
                        {
                            distToTravel.Y = WallUntilOpening(collidingTileX.mapPoint[0], collidingTileX.mapPoint[1], "down", "right", 15 - collidingTileX.mapPoint[0], 0) * (tileSize * 2);
                            strafeDown = true;
                            ChooseXStrafe(collidingTileY.mapPoint[0] + (int)(distToTravel.Y / 32), collidingTileY.mapPoint[1], "");
                        }
                    }
                    else if (blockedTop && blockedRight)
                    {
                        if ((int)position.X < (int)target.X && xDiag)
                        {
                            ChooseDiagStrafe(collidingTileX.mapPoint[0], collidingTileX.mapPoint[1], "up", "right");
                            xDiag = false;
                        }
                        else if ((int)position.X < (int)target.X)
                        {
                            distToTravel.X = -WallUntilOpening(collidingTileY.mapPoint[0], collidingTileY.mapPoint[1], "left", "up", collidingTileY.mapPoint[1], 0) * (tileSize * 2);
                            strafeLeft = true;
                        }
                        if ((int)position.X > (int)target.X && xDiag)
                        {
                            ChooseDiagStrafe(collidingTileX.mapPoint[0], collidingTileX.mapPoint[1], "up", "left");
                            xDiag = false;
                        }
                        else if ((int)position.X > (int)target.X)
                        {
                            //distToTravel.X = WallUntilOpening(collidingTileY.mapPoint[0], collidingTileY.mapPoint[1], "right", "up", 25 - collidingTileY.mapPoint[1], 0) * (tileSize);
                            //strafeRight = true;
                        }
                    }
                    else if (blockedTop && blockedLeft && ((int)position.X == (int)target.X))
                    {
                        if (map.FloorIndexes.Contains(map.GetPoint(collidingTileX.mapPoint[0] + 1, collidingTileX.mapPoint[1], mapDims))
                             || map.FloorIndexes.Contains(map.GetPoint(collidingTileY.mapPoint[0], collidingTileY.mapPoint[1] + 1, mapDims)))
                        {
                            ChooseDiagStrafe(collidingTileX.mapPoint[0], collidingTileX.mapPoint[1], "up", "left");
                        }
                        else
                        {
                            distToTravel.Y = WallUntilOpening(collidingTileX.mapPoint[0], collidingTileX.mapPoint[1], "down", "right", 15 - collidingTileX.mapPoint[0], 0) * (tileSize * 2);
                            strafeDown = true;
                            ChooseXStrafe(collidingTileY.mapPoint[0] + (int)(distToTravel.Y / 32), collidingTileY.mapPoint[1], "");
                        }

                    }
                    else if (blockedTop && blockedLeft)
                    {
                        if ((int)position.X > (int)target.X && xDiag)
                        {
                            ChooseDiagStrafe(collidingTileX.mapPoint[0], collidingTileX.mapPoint[1], "up", "left");
                            xDiag = false;
                        }
                        else if ((int)position.X > (int)target.X)
                        {
                            distToTravel.X = WallUntilOpening(collidingTileY.mapPoint[0], collidingTileY.mapPoint[1], "right", "up", 25 - collidingTileY.mapPoint[1], 0) * (tileSize * 2);
                            strafeRight = true;
                        }
                        if ((int)position.X < (int)target.X && xDiag)
                        {
                            ChooseDiagStrafe(collidingTileX.mapPoint[0], collidingTileX.mapPoint[1], "up", "right");
                            xDiag = false;
                        }
                        else if ((int)position.X < (int)target.X)
                        {

                            //distToTravel.X = -WallUntilOpening(collidingTileY.mapPoint[0], collidingTileY.mapPoint[1], "left", "up", collidingTileY.mapPoint[1], 0) * tileSize;
                            //strafeLeft = true;
                        }
                        xDiag = false;

                    }
                    else if (blockedTop && ((int)position.X == (int)target.X))
                    {
                        ChooseXStrafe(collidingTileY.mapPoint[0], collidingTileY.mapPoint[1], "up");
                    }
                    else if (blockedTop && (!blockedLeft && !blockedRight))
                    {
                        //ChooseXStrafe(collidingTileY.mapPoint[0], collidingTileY.mapPoint[1], "up");
                    }
                    else if (!blockedTop)
                    {
                        position.Y -= moveSpeed;
                        //animManager.isUp = true;
                        //animManager.isDown = false;
                        if (distToTravel.Y > 0)
                        {
                            distToTravel.Y = 0;
                        }

                    }
                }

                //Handle down vision
                else if ((int)position.Y < (int)target.Y)
                {
                    if (blockedBottom && blockedLeft && blockedRight)
                    {
                        distToTravel.Y = -WallUntilOpening(GetRow(0), GetCol(0), "up", "left", GetRow(0), 0) * tileSize;
                        strafeUp = true;
                    }
                    else if (blockedBottom && blockedLeft && blockedTop)
                    {
                        distToTravel.X = WallUntilOpening(GetRow(0), GetCol(32), "right", "down", 25 - GetCol(0), 0) * tileSize;
                        strafeRight = true;
                    }
                    else if (blockedBottom && blockedRight && blockedTop)
                    {
                        distToTravel.X = -WallUntilOpening(GetRow(0), GetCol(0), "left", "down", GetCol(0), 0) * tileSize;
                        strafeLeft = true;
                    }
                    else if (blockedBottom && blockedRight && ((int)position.X == (int)target.X))
                    {
                        if (map.FloorIndexes.Contains(map.GetPoint(collidingTileX.mapPoint[0] - 1, collidingTileX.mapPoint[1], mapDims))
                             || map.FloorIndexes.Contains(map.GetPoint(collidingTileY.mapPoint[0], collidingTileY.mapPoint[1] - 1, mapDims)))
                        {
                            ChooseDiagStrafe(collidingTileX.mapPoint[0], collidingTileX.mapPoint[1], "down", "right");
                        }
                        else
                        {
                            distToTravel.Y = -WallUntilOpening(collidingTileX.mapPoint[1], collidingTileX.mapPoint[1], "up", "right", collidingTileX.mapPoint[0], 0) * (tileSize * 2);
                            strafeUp = true;
                            ChooseXStrafe(collidingTileY.mapPoint[0] + (int)(distToTravel.Y / 32), collidingTileY.mapPoint[1], "");
                        }

                    }
                    else if (blockedBottom && blockedRight)
                    {
                        if ((int)position.X < (int)target.X && xDiag)
                        {
                            ChooseDiagStrafe(collidingTileX.mapPoint[0], collidingTileX.mapPoint[1], "down", "right");
                            xDiag = false;
                        }
                        else if ((int)position.X < (int)target.X)
                        {
                            distToTravel.X = -WallUntilOpening(collidingTileY.mapPoint[0], collidingTileY.mapPoint[1], "left", "down", collidingTileY.mapPoint[1], 0) * (tileSize * 2);
                            strafeLeft = true;
                        }
                        if ((int)position.X > (int)target.X && xDiag)
                        {
                            ChooseDiagStrafe(collidingTileX.mapPoint[0], collidingTileX.mapPoint[1], "down", "left");
                            xDiag = false;
                        }
                        else if ((int)position.X > (int)target.X)
                        {
                            //distToTravel.X = WallUntilOpening(collidingTileY.mapPoint[0], collidingTileY.mapPoint[1], "right","down", 25 - collidingTileY.mapPoint[1], 0) * tileSize;
                            //strafeRight = true;
                        }


                    }
                    else if (blockedBottom && blockedLeft && ((int)position.X == (int)target.X))
                    {

                        if (map.FloorIndexes.Contains(map.GetPoint(collidingTileX.mapPoint[0] - 1, collidingTileX.mapPoint[1], mapDims))
                             || map.FloorIndexes.Contains(map.GetPoint(collidingTileY.mapPoint[0], collidingTileY.mapPoint[1] + 1, mapDims)))
                        {
                            ChooseDiagStrafe(collidingTileX.mapPoint[0], collidingTileX.mapPoint[1], "down", "left");
                            //if it is one or the other need to set a condition to check if the enemy needs to strafe X if still blocked bottom to the left or the right 
                            if (map.FloorIndexes.Contains(map.GetPoint(collidingTileX.mapPoint[0] - 1, collidingTileX.mapPoint[1], mapDims))
                                && map.FloorIndexes.Contains(map.GetPoint(collidingTileY.mapPoint[0], collidingTileY.mapPoint[1] + 1, mapDims)) == false)
                            {
                                //strafe left
                                if (map.PlatformIndexes.Contains(map.GetPoint(collidingTileX.mapPoint[0] + (int)(distToTravel.Y / 32), collidingTileX.mapPoint[1] + (int)(distToTravel.X / 32), mapDims)))
                                {

                                }
                            }
                            if (map.FloorIndexes.Contains(map.GetPoint(collidingTileX.mapPoint[0] - 1, collidingTileX.mapPoint[1], mapDims)) == false
                                && map.FloorIndexes.Contains(map.GetPoint(collidingTileY.mapPoint[0], collidingTileY.mapPoint[1] + 1, mapDims)))
                            {
                                //strafe right 
                            }

                        }
                        else
                        {
                            distToTravel.Y = -WallUntilOpening(collidingTileX.mapPoint[0], collidingTileX.mapPoint[1], "up", "left", collidingTileX.mapPoint[0], 0) * (tileSize * 2);
                            strafeUp = true;
                            ChooseXStrafe(collidingTileY.mapPoint[0] + (int)(distToTravel.Y / 32), collidingTileY.mapPoint[1], "");
                        }
                        //distToTravel.X = -tileSize;
                        //strafeLeft = true;

                    }
                    else if (blockedBottom && blockedLeft)
                    {
                        if ((int)position.X < (int)target.X && xDiag)
                        {
                            ChooseDiagStrafe(collidingTileX.mapPoint[0], collidingTileX.mapPoint[1], "down", "right"); //Needs to check Down + Right and Up + Left ONLY
                            xDiag = false;
                        }
                        else if ((int)position.X < (int)target.X)
                        {
                            //distToTravel.X = -WallUntilOpening(collidingTileY.mapPoint[0], collidingTileY.mapPoint[1], "left", "down", collidingTileY.mapPoint[1], 0) * tileSize;
                            //strafeLeft = true;
                        }
                        if ((int)position.X > (int)target.X && xDiag)
                        {
                            ChooseDiagStrafe(collidingTileX.mapPoint[0], collidingTileX.mapPoint[1], "down", "left");
                            xDiag = false;
                        }
                        else if ((int)position.X > (int)target.X)
                        {
                            distToTravel.X = WallUntilOpening(collidingTileY.mapPoint[0], collidingTileY.mapPoint[1], "right", "down", 25 - collidingTileY.mapPoint[1], 0) * (tileSize * 2);
                            strafeRight = true;
                        }

                    }
                    else if (blockedBottom && ((int)position.X == (int)target.X))
                    {
                        ChooseXStrafe(collidingTileY.mapPoint[0], collidingTileY.mapPoint[1], "down");
                    }
                    else if (blockedBottom && (!blockedLeft && !blockedRight))
                    {

                        //ChooseXStrafe(collidingTileY.mapPoint[0], collidingTileY.mapPoint[1], "down");
                        //check if going to the right or left. if ChooseXStrafe didnt result in desired outcome. Restart
                    }
                    else if (!blockedBottom)
                    {
                        position.Y += moveSpeed;
                        //animManager.isDown = true;
                        //animManager.isUp = false;
                        if (distToTravel.Y < 0)
                        {
                            distToTravel.Y = 0;
                        }
                        //blockedBottom = false;
                    }

                }
            }
            #endregion
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(content.Load<Texture2D>("MapTiles/Tile1"), enemyRect, Color.Blue);
        }
        #region Methods
        private void SetVision()
        {
       
            pixelSize = 64;
            vision.Clear();
            Vector2 pos = new Vector2((int)enemyRect.X, (int)(enemyRect.Y / pixelSize) * 64);

            for (int i = 1; i < visionLength + 1; i++)
            {
                vision.Add(new Rectangle((int)pos.X, (int)pos.Y - (pixelSize * i), pixelSize, pixelSize));//Up
                vision.Add(new Rectangle((int)pos.X, (int)pos.Y + (pixelSize * i), pixelSize, pixelSize));//Down
            }

            //vision.Add(new Rectangle((int)pos.X, (int)pos.Y - (pixelSize * 1), pixelSize, pixelSize));//Up
            for (int i = 1; i < visionLength + 1; i++)//Right
            {
                vision.Add(new Rectangle((int)pos.X + (pixelSize * i), (int)pos.Y, pixelSize, pixelSize));
                for (int j = i + 1; j < visionLength + 1; j++) //Right and Up
                {
                    vision.Add(new Rectangle((int)pos.X + (pixelSize * i), (int)pos.Y - (pixelSize * j), pixelSize, pixelSize));
                }
                for (int k = i + 1; k < visionLength + 1; k++)//Right and down
                {
                    vision.Add(new Rectangle((int)pos.X + (pixelSize * i), (int)pos.Y + (pixelSize * k), pixelSize, pixelSize));
                }
            }

            for (int i = 1; i < visionLength + 1; i++)//Left
            {
                vision.Add(new Rectangle((int)pos.X - (pixelSize * i), (int)pos.Y, pixelSize, pixelSize));
                for (int j = i + 1; j < visionLength + 1; j++) //Left and Up
                {
                    vision.Add(new Rectangle((int)pos.X - (pixelSize * i), (int)pos.Y - (pixelSize * j), pixelSize, pixelSize));
                }
                for (int k = i + 1; k < visionLength + 1; k++)//Left and down
                {
                    vision.Add(new Rectangle((int)pos.X - (pixelSize * i), (int)pos.Y + (pixelSize * k), pixelSize, pixelSize));
                }
            }

            for (int i = 1; i < visionLength + 1; i++) //Up and down
            {
                //vision.Add(new Rectangle(pos.X + (int)(rectangle.Width / 3.5f), enemyRect.Top + (enemyRect.Height * 1), enemyRect.Width / 2, pixelSize / 4));
                for (int j = i; j < visionLength + 1; j++) //Left and up 
                {
                    vision.Add(new Rectangle((int)pos.X - (pixelSize * j), (int)pos.Y - (pixelSize * i), pixelSize, pixelSize));
                }
                for (int j = i; j < visionLength + 1; j++) //Right and Up
                {
                    vision.Add(new Rectangle((int)pos.X + (pixelSize * j), (int)pos.Y - (pixelSize * i), pixelSize, pixelSize));
                }
                //vision.Add(new Rectangle((int)pos.X, (int)pos.Y + (pixelSize * i), pixelSize, pixelSize));//Down
                for (int k = i; k < visionLength + 1; k++)//Left and down
                {
                    vision.Add(new Rectangle((int)pos.X - (pixelSize * k), (int)pos.Y + (pixelSize * i), pixelSize, pixelSize));
                }
                for (int k = i; k < visionLength + 1; k++)//Right and down
                {
                    vision.Add(new Rectangle((int)pos.X + (pixelSize * k), (int)pos.Y + (pixelSize * i), pixelSize, pixelSize));
                }
            }

            pixelSize = 62;

        }

        int DistForm(Vector2 pos1, Vector2 pos2)
        {
            return (int)Math.Sqrt(Math.Pow(pos1.X - pos2.X, 2) + Math.Pow(pos1.Y - pos2.Y, 2));

        }

        void ChooseDiagStrafe(int row, int col, string dirY, string dirX) //Need to send row and col of the tile to right or left or up and down depending on direction going and block direction
        {
            int numUp = 1;
            int numDown = 1;
            int numLeft = 1;
            int numRight = 1;

            Vector2 UpRight = new Vector2();
            Vector2 UpLeft = new Vector2();
            Vector2 DownRight = new Vector2();
            Vector2 DownLeft = new Vector2();
            UpRight = Vector2.One;
            UpLeft = Vector2.One;
            DownRight = Vector2.One;
            DownLeft = Vector2.One;

            bool endUp = false;
            bool endDown = false;
            bool endLeft = false;
            bool endRight = false;
            int tempCol = 0;
            int tempRow = 0;
            switch (dirX)
            {

                case "right":
                    numRight = 1;
                    switch (dirY)
                    {

                        case "up":
                            {
                                numUp = 1;
                                tempCol = col;
                                col--;
                                for (int i = row + 1; i < 15 && col >= 0; i++) //Down and Left
                                {
                                    //col--;

                                    if (map.PlatformIndexes.Contains(map.GetPoint(i, col, mapDims)))
                                    {

                                        numDown++;
                                        numLeft++;
                                        DownLeft = new Vector2(numLeft, numDown);
                                    }
                                    else if (map.FloorIndexes.Contains(map.GetPoint(i, col, mapDims)))
                                    {
                                        break;
                                    }


                                    if (i == 14 || col == 0 && map.PlatformIndexes.Contains(map.GetPoint(i, col, mapDims)))
                                    {
                                        endDown = true;
                                        endLeft = true;
                                        break;
                                    }
                                    else if (map.PlatformIndexes.Contains(map.GetPoint(i, col + 1, mapDims)))
                                    {
                                        endDown = true;
                                        endLeft = true;
                                        break;
                                    }
                                    col--;
                                }
                                numDown = 1;
                                numLeft = 1;
                                col = tempCol;
                                col++;
                                for (int i = row - 1; i >= 0 && col < 25; i--)//Up and Right
                                {
                                    //col++;
                                    if (map.PlatformIndexes.Contains(map.GetPoint(i, col, mapDims)))
                                    {

                                        numUp++;
                                        numRight++;
                                        UpRight = new Vector2(numRight, numUp);
                                    }
                                    else if (map.FloorIndexes.Contains(map.GetPoint(i, col, mapDims)))
                                    {
                                        break;
                                    }


                                    if (i == 0 || col == 24 && map.PlatformIndexes.Contains(map.GetPoint(i, col, mapDims)))
                                    {
                                        endUp = true;
                                        endRight = true;
                                        break;
                                    }
                                    else if (map.PlatformIndexes.Contains(map.GetPoint(i, col - 1, mapDims)))
                                    {
                                        endUp = true;
                                        endRight = true;
                                        break;
                                    }
                                    col++;
                                }
                                numRight = 1;
                                numUp = 1;
                                tempRow = row;
                                row--;
                                col = tempCol;
                                for (int i = col - 1; i >= 0 && row >= 0; i--)//Up and left
                                {
                                    //row--;
                                    if (map.PlatformIndexes.Contains(map.GetPoint(row, i, mapDims)))
                                    {

                                        numUp++;
                                        numLeft++;
                                        UpLeft = new Vector2(numLeft, numUp);
                                    }
                                    else if (map.FloorIndexes.Contains(map.GetPoint(row, i, mapDims)))
                                    {

                                        break;
                                    }

                                    if (i == 0 || row == 0 && map.PlatformIndexes.Contains(map.GetPoint(row, i, mapDims))) //Logic can be changed to move over obstacles and walls
                                    {

                                        endUp = true;
                                        endLeft = true;
                                        break;
                                    }
                                    else if (map.PlatformIndexes.Contains(map.GetPoint(row, i + 1, mapDims)))
                                    {

                                        endUp = true;
                                        endLeft = true;
                                        break;
                                    }
                                    row--;
                                }
                                numUp = 1;
                                numLeft = 1;
                                row = tempRow;
                                row++;
                                for (int i = col + 1; i < 25 && row < 15; i++) //Down and Right
                                {
                                    //row++;
                                    if (map.PlatformIndexes.Contains(map.GetPoint(row, i, mapDims)))
                                    {
                                        //row++;
                                        numDown++;
                                        numRight++;
                                        DownRight = new Vector2(numRight, numDown);
                                    }
                                    else if (map.FloorIndexes.Contains(map.GetPoint(row, i, mapDims)))
                                    {

                                        break;
                                    }

                                    if (i == 24 || row == 14 && map.PlatformIndexes.Contains(map.GetPoint(row, i, mapDims)))
                                    {

                                        endDown = true;
                                        endRight = true;
                                        break;
                                    }
                                    else if (map.PlatformIndexes.Contains(map.GetPoint(row + 1, i, mapDims)))
                                    {

                                        endDown = true;
                                        endRight = true;
                                        break;
                                    }
                                    row++;
                                }

                            }

                            if (UpLeft.Length() > DownRight.Length()) // if left and up is more than right and down
                            {
                                if (endDown && endRight)
                                {
                                    distToTravel.Y = -UpLeft.Y * (tileSize * 3);
                                    distToTravel.X = -UpLeft.X * (tileSize);
                                    strafeUp = true;
                                    strafeLeft = true;
                                    prioXStrafe = true;
                                }
                                else
                                {
                                    distToTravel.Y = DownRight.Y * (tileSize * 1.5f);
                                    distToTravel.X = DownRight.X * (tileSize * 2);
                                    strafeDown = true;
                                    strafeRight = true;
                                }


                            }
                            else if (DownRight.Length() > UpLeft.Length()) //if right and down is more than left and up
                            {
                                if (endUp && endLeft)
                                {
                                    distToTravel.Y = DownRight.Y * (tileSize);
                                    distToTravel.X = DownRight.X * (tileSize * 2);
                                    strafeDown = true;
                                    strafeRight = true;
                                }
                                else
                                {
                                    distToTravel.Y = -UpLeft.Y * (tileSize * 3);
                                    distToTravel.X = -UpLeft.X * (tileSize);
                                    strafeUp = true;
                                    strafeLeft = true;
                                    prioXStrafe = true;
                                }

                            }
                            else if (UpLeft.Length() == DownRight.Length())
                            {
                                if (endDown && endRight)
                                {
                                    distToTravel.Y = -UpLeft.Y * (tileSize * 3);
                                    distToTravel.X = -UpLeft.X * (tileSize);
                                    strafeUp = true;
                                    strafeLeft = true;
                                    prioXStrafe = true;
                                }
                                else if (endUp && endLeft)
                                {
                                    distToTravel.Y = DownRight.Y * (tileSize);
                                    distToTravel.X = DownRight.X * (tileSize * 2);
                                    strafeDown = true;
                                    strafeRight = true;
                                }
                                else
                                {
                                    Random rand = new Random();

                                    if (rand.Next(11) > 5)
                                    {
                                        distToTravel.Y = DownRight.Y * (tileSize);
                                        distToTravel.X = DownRight.X * (tileSize * 2);
                                        strafeDown = true;
                                        strafeRight = true;
                                    }
                                    else
                                    {
                                        distToTravel.Y = -UpLeft.Y * (tileSize * 3);
                                        distToTravel.X = -UpLeft.X * (tileSize);
                                        strafeUp = true;
                                        strafeLeft = true;
                                        prioXStrafe = true;
                                    }
                                }

                            }
                            else if (DownLeft.Length() > UpRight.Length())//Left and Down is more than Right and Up
                            {
                                if (endUp && endRight)
                                {
                                    distToTravel.X = -DownLeft.X * (tileSize * 2);
                                    distToTravel.Y = DownLeft.Y * (tileSize * 2);
                                    strafeLeft = true;
                                    strafeDown = true;
                                }
                                else
                                {
                                    distToTravel.Y = -UpRight.Y * (tileSize);
                                    distToTravel.X = UpRight.X * (tileSize * 2);
                                    strafeUp = true;
                                    strafeRight = true;
                                    prioXStrafe = true;
                                }
                            }
                            else if (UpRight.Length() > DownLeft.Length())//Right and Up is more than Left and Down
                            {
                                if (endDown && endLeft)
                                {
                                    distToTravel.Y = -UpRight.Y * (tileSize);
                                    distToTravel.X = UpRight.X * (tileSize * 2);
                                    strafeUp = true;
                                    strafeRight = true;
                                    prioXStrafe = true;
                                }
                                else
                                {
                                    distToTravel.X = -DownLeft.X * (tileSize * 2);
                                    distToTravel.Y = DownLeft.Y * (tileSize * 2);
                                    strafeLeft = true;
                                    strafeDown = true;
                                }

                            }
                            else if (DownLeft.Length() == UpRight.Length())
                            {
                                if (endUp && endRight)
                                {
                                    distToTravel.X = -DownLeft.X * (tileSize * 2);
                                    distToTravel.Y = DownLeft.Y * (tileSize * 2);
                                    strafeLeft = true;
                                    strafeDown = true;
                                }
                                else if (endDown && endLeft)
                                {
                                    distToTravel.Y = -UpRight.Y * (tileSize);
                                    distToTravel.X = UpRight.X * (tileSize * 2);
                                    strafeUp = true;
                                    strafeRight = true;
                                    prioXStrafe = true;
                                }
                                else
                                {

                                    Random rand = new Random();

                                    if (rand.Next(11) > 5)
                                    {
                                        distToTravel.Y = -UpRight.Y * (tileSize);
                                        distToTravel.X = UpRight.X * (tileSize * 2);
                                        strafeUp = true;
                                        strafeRight = true;
                                        prioXStrafe = true;
                                    }
                                    else
                                    {
                                        distToTravel.X = -DownLeft.X * (tileSize * 2);
                                        distToTravel.Y = DownLeft.Y * (tileSize * 2);
                                        strafeLeft = true;
                                        strafeDown = true;
                                    }
                                }

                            }

                            //SetStrafe(numUp, numDown, numLeft, numRight, dirX, dirY);
                            break;
                        case "down":

                            numDown = 1;
                            tempCol = col;
                            col--;
                            for (int i = row + 1; i < 15 && col >= 0; i++) //Down and Left
                            {
                                //col--;

                                if (map.PlatformIndexes.Contains(map.GetPoint(i, col, mapDims)))
                                {

                                    numDown++;
                                    numLeft++;
                                    DownLeft = new Vector2(numLeft, numDown);
                                }
                                else if (map.FloorIndexes.Contains(map.GetPoint(i, col, mapDims)))
                                {

                                    break;
                                }


                                if (i == 14 || col == 0 && map.PlatformIndexes.Contains(map.GetPoint(i, col, mapDims)))
                                {

                                    endDown = true;
                                    endLeft = true;
                                    break;
                                }
                                else if (map.PlatformIndexes.Contains(map.GetPoint(i, col - 1, mapDims)))
                                {

                                    endDown = true;
                                    endLeft = true;
                                    break;
                                }
                                col--;
                            }
                            numDown = 1;
                            numLeft = 1;
                            col = tempCol;
                            col++;
                            for (int i = row - 1; i >= 0 && col < 25; i--)//Up and Right
                            {
                                //col++;
                                if (map.PlatformIndexes.Contains(map.GetPoint(i, col, mapDims)))
                                {

                                    numUp++;
                                    numRight++;
                                    UpRight = new Vector2(numRight, numUp);
                                }
                                else if (map.FloorIndexes.Contains(map.GetPoint(i, col, mapDims)))
                                {

                                    break;
                                }


                                if (i == 0 || col == 24 && map.PlatformIndexes.Contains(map.GetPoint(i, col, mapDims)))
                                {

                                    endUp = true;
                                    endRight = true;
                                    break;
                                }
                                else if (map.PlatformIndexes.Contains(map.GetPoint(i, col - 1, mapDims)))
                                {
                                    endUp = true;
                                    endRight = true;
                                    break;
                                }
                                col++;
                            }
                            numUp = 1;
                            numRight = 1;
                            tempRow = row;
                            row--;
                            col = tempCol;
                            for (int i = col - 1; i >= 0 && row >= 0; i--)//Up and left
                            {
                                //row--;
                                if (map.PlatformIndexes.Contains(map.GetPoint(row, i, mapDims)))
                                {

                                    numUp++;
                                    numLeft++;
                                    UpLeft = new Vector2(numLeft, numUp);
                                }
                                else if (map.FloorIndexes.Contains(map.GetPoint(row, i, mapDims)))
                                {

                                    break;
                                }

                                if (i == 0 || row == 0 && map.PlatformIndexes.Contains(map.GetPoint(row, i, mapDims))) //Logic can be changed to move over obstacles and walls
                                {

                                    endUp = true;
                                    endLeft = true;
                                    break;
                                }
                                else if (map.PlatformIndexes.Contains(map.GetPoint(row, i + 1, mapDims)))
                                {

                                    endUp = true;
                                    endLeft = true;
                                    break;
                                }
                                row--;
                            }
                            numUp = 1;
                            numLeft = 1;
                            row = tempRow;
                            row++;
                            for (int i = col + 1; i < 25 && row < 15; i++) //Down and Right
                            {
                                //row++;
                                if (map.PlatformIndexes.Contains(map.GetPoint(row, i, mapDims)))
                                {

                                    numDown++;
                                    numRight++;
                                    DownRight = new Vector2(numRight, numDown);
                                }
                                else if (map.FloorIndexes.Contains(map.GetPoint(row, i, mapDims)))
                                {

                                    break;
                                }

                                if (i == 24 || row == 14 && map.PlatformIndexes.Contains(map.GetPoint(row, i, mapDims)))
                                {


                                    endDown = true;
                                    endRight = true;
                                    break;
                                }
                                else if (map.PlatformIndexes.Contains(map.GetPoint(row, i - 1, mapDims)))
                                {

                                    endDown = true;
                                    endRight = true;
                                    break;
                                }
                                row++;
                            }


                            if (DownLeft.Length() > UpRight.Length())//Left and Down is more than Right and Up
                            {
                                if (endRight && endUp)
                                {
                                    distToTravel.X = -DownLeft.X * (tileSize);
                                    distToTravel.Y = DownLeft.Y * (tileSize * 3);
                                    strafeLeft = true;
                                    strafeDown = true;
                                }
                                else
                                {
                                    distToTravel.Y = -UpRight.Y * (tileSize);
                                    distToTravel.X = UpRight.X * (tileSize * 2);
                                    strafeUp = true;
                                    strafeRight = true;
                                    prioXStrafe = true;
                                }

                            }
                            else if (UpRight.Length() > DownLeft.Length())//Right and Up is more than Left and Down
                            {
                                if (endDown && endLeft)
                                {
                                    distToTravel.Y = -UpRight.Y * (tileSize * 2);
                                    distToTravel.X = UpRight.X * (tileSize * 2);
                                    strafeUp = true;
                                    strafeRight = true;
                                    prioXStrafe = true;
                                }
                                else
                                {
                                    distToTravel.X = -DownLeft.X * (tileSize * 1.5f);
                                    distToTravel.Y = DownLeft.Y * (tileSize * 2);
                                    strafeLeft = true;
                                    strafeDown = true;
                                }

                            }
                            else if (UpRight.Length() == DownLeft.Length())
                            {
                                if (endDown && endLeft)
                                {
                                    distToTravel.Y = -UpRight.Y * (tileSize);
                                    distToTravel.X = UpRight.X * (tileSize * 2);
                                    strafeUp = true;
                                    strafeRight = true;
                                    prioXStrafe = true;
                                }
                                else if (endUp && endRight)
                                {
                                    distToTravel.X = -DownLeft.X * (tileSize);
                                    distToTravel.Y = DownLeft.Y * (tileSize * 2);
                                    strafeLeft = true;
                                    strafeDown = true;
                                }
                                else
                                {
                                    Random rand = new Random();

                                    if (rand.Next(11) > 5)
                                    {
                                        distToTravel.Y = -UpRight.Y * (tileSize);
                                        distToTravel.X = UpRight.X * (tileSize * 2);
                                        strafeUp = true;
                                        strafeRight = true;
                                        prioXStrafe = true;
                                    }
                                    else
                                    {
                                        distToTravel.X = -DownLeft.X * (tileSize * 2);
                                        distToTravel.Y = DownLeft.Y * (tileSize * 2);
                                        strafeLeft = true;
                                        strafeDown = true;
                                    }
                                }

                            }
                            else if (UpLeft.Length() > DownRight.Length()) // if left and up is more than right and down
                            {
                                if (endDown && endRight)
                                {
                                    distToTravel.Y = -UpLeft.Y * (tileSize * 3);
                                    distToTravel.X = -UpLeft.X * (tileSize);
                                    strafeUp = true;
                                    strafeLeft = true;
                                    prioXStrafe = true;
                                }
                                else
                                {
                                    distToTravel.Y = DownRight.Y * (tileSize);
                                    distToTravel.X = DownRight.X * (tileSize * 2);
                                    strafeDown = true;
                                    strafeRight = true;
                                }


                            }
                            else if (DownRight.Length() > UpLeft.Length()) //if right and down is more than left and up
                            {
                                if (endUp && endLeft)
                                {
                                    distToTravel.Y = DownRight.Y * (tileSize);
                                    distToTravel.X = DownRight.X * (tileSize * 2);
                                    strafeDown = true;
                                    strafeRight = true;
                                }
                                else
                                {
                                    distToTravel.Y = -UpLeft.Y * (tileSize * 3);
                                    distToTravel.X = -UpLeft.X * (tileSize);
                                    strafeUp = true;
                                    strafeLeft = true;
                                    prioXStrafe = true;
                                }

                            }
                            else if (DownRight.Length() == UpLeft.Length())
                            {

                                if (endLeft && endUp)
                                {
                                    distToTravel.Y = DownRight.Y * (tileSize);
                                    distToTravel.X = DownRight.X * (tileSize * 2);
                                    strafeDown = true;
                                    strafeRight = true;
                                }
                                else if (endRight && endDown)
                                {
                                    distToTravel.Y = -UpLeft.Y * (tileSize * 3);
                                    distToTravel.X = -UpLeft.X * (tileSize);
                                    strafeUp = true;
                                    strafeLeft = true;
                                    prioXStrafe = true;
                                }
                                else
                                {
                                    Random rand = new Random();
                                    if (rand.Next(11) > 5)
                                    {
                                        distToTravel.Y = DownRight.Y * (tileSize);
                                        distToTravel.X = DownRight.X * (tileSize * 2);
                                        strafeDown = true;
                                        strafeRight = true;
                                    }
                                    else
                                    {
                                        distToTravel.Y = -UpLeft.Y * (tileSize * 3);
                                        distToTravel.X = -UpLeft.X * (tileSize);
                                        strafeUp = true;
                                        strafeLeft = true;
                                        prioXStrafe = true;
                                    }
                                }

                            }

                            //SetStrafe(numUp, numDown, numLeft, numRight, dirX, dirY);
                            break;
                    }
                    break;
                case "left":
                    numLeft = 1;
                    switch (dirY)
                    {
                        case "up":
                            {
                                numUp = 1;
                                tempCol = col;
                                col--;
                                for (int i = row + 1; i < 15 && col >= 0; i++) //Down and Left
                                {
                                    //col--;

                                    if (map.PlatformIndexes.Contains(map.GetPoint(i, col, mapDims)))
                                    {

                                        numDown++;
                                        numLeft++;
                                        DownLeft = new Vector2(numLeft, numDown);
                                    }
                                    else if (map.FloorIndexes.Contains(map.GetPoint(i, col, mapDims)))
                                    {

                                        break;
                                    }


                                    if (i == 14 || col == 0 && map.PlatformIndexes.Contains(map.GetPoint(i, col, mapDims)))
                                    {

                                        endDown = true;
                                        endLeft = true;
                                        break;
                                    }
                                    else if (map.PlatformIndexes.Contains(map.GetPoint(i + 1, col, mapDims)))
                                    {

                                        endDown = true;
                                        endLeft = true;
                                        break;
                                    }
                                    col--;

                                }
                                numLeft = 1;
                                numDown = 1;
                                col = tempCol;
                                col++;
                                for (int i = row - 1; i >= 0 && col < 25; i--)//Up and Right
                                {
                                    //col++;
                                    if (map.PlatformIndexes.Contains(map.GetPoint(i, col, mapDims)))
                                    {

                                        numUp++;
                                        numRight++;
                                        UpRight = new Vector2(numRight, numUp);
                                    }
                                    else if (map.FloorIndexes.Contains(map.GetPoint(i, col, mapDims)))
                                    {

                                        break;
                                    }

                                    if (i == 0 || col == 24 && map.PlatformIndexes.Contains(map.GetPoint(i, col, mapDims)))
                                    {

                                        endUp = true;
                                        endRight = true;
                                        break;
                                    }
                                    else if (map.PlatformIndexes.Contains(map.GetPoint(i, col + 1, mapDims)))
                                    {
                                        endUp = true;
                                        endRight = true;
                                        break;
                                    }

                                    col++;
                                }
                                numUp = 1;
                                numRight = 1;
                                tempRow = row;
                                row--;
                                col = tempCol;
                                for (int i = col - 1; i >= 0 && row >= 0; i--)//Up and left
                                {
                                    //row--;
                                    if (map.PlatformIndexes.Contains(map.GetPoint(row, i, mapDims)))
                                    {

                                        numUp++;
                                        numLeft++;
                                        UpLeft = new Vector2(numLeft, numUp);
                                    }
                                    else if (map.FloorIndexes.Contains(map.GetPoint(row, i, mapDims)))
                                    {

                                        break;
                                    }

                                    if (i == 0 || row == 0 && map.PlatformIndexes.Contains(map.GetPoint(row, i, mapDims))) //Logic can be changed to move over obstacles and walls
                                    {

                                        endUp = true;
                                        endLeft = true;
                                        break;
                                    }
                                    else if (map.PlatformIndexes.Contains(map.GetPoint(row, i + 1, mapDims)))
                                    {

                                        endUp = true;
                                        endLeft = true;
                                        break;
                                    }
                                    row--;
                                }
                                numUp = 1;
                                numLeft = 1;
                                row = tempRow;
                                row++;
                                for (int i = col + 1; i < 25 && row < 15; i++) //Down and Right
                                {
                                    //row++;
                                    if (map.PlatformIndexes.Contains(map.GetPoint(row, i, mapDims)))
                                    {

                                        numDown++;
                                        numRight++;
                                        DownRight = new Vector2(numRight, numDown);
                                    }
                                    else if (map.FloorIndexes.Contains(map.GetPoint(row, i, mapDims)))
                                    {

                                        break;
                                    }

                                    if (i == 24 || row == 14 && map.PlatformIndexes.Contains(map.GetPoint(row, i, mapDims)))
                                    {

                                        endDown = true;
                                        endRight = true;
                                        break;
                                    }
                                    else if (map.PlatformIndexes.Contains(map.GetPoint(row, i + 1, mapDims)))
                                    {

                                        endDown = true;
                                        endRight = true;
                                        break;
                                    }
                                    row++;
                                }
                            }
                            numDown = 1;
                            if (DownLeft.Length() > UpRight.Length())//Left and Down is more than Right and Up
                            {
                                if (endUp && endRight)
                                {
                                    distToTravel.X = -DownLeft.X * (tileSize * 2);
                                    distToTravel.Y = DownLeft.Y * (tileSize);
                                    strafeLeft = true;
                                    strafeDown = true;
                                }
                                else
                                {
                                    distToTravel.Y = -UpRight.Y * (tileSize * 2);
                                    distToTravel.X = UpRight.X * (tileSize);
                                    strafeUp = true;
                                    strafeRight = true;
                                    prioXStrafe = true;
                                }

                            }
                            else if (UpRight.Length() > DownLeft.Length())//Right and Up is more than Left and Down
                            {
                                if (endDown && endLeft)
                                {
                                    distToTravel.Y = -UpRight.Y * (tileSize * 2);
                                    distToTravel.X = UpRight.X * (tileSize);
                                    strafeUp = true;
                                    strafeRight = true;
                                    prioXStrafe = true;
                                }
                                else
                                {
                                    distToTravel.X = -DownLeft.X * (tileSize * 2.5f);
                                    distToTravel.Y = DownLeft.Y * (tileSize);
                                    strafeLeft = true;
                                    strafeDown = true;
                                }

                            }
                            else if (DownLeft.Length() == UpRight.Length())
                            {
                                if (endUp && endRight)
                                {
                                    distToTravel.X = -DownLeft.X * (tileSize * 2);
                                    distToTravel.Y = DownLeft.Y * (tileSize);
                                    strafeLeft = true;
                                    strafeDown = true;
                                }
                                else if (endDown && endLeft)
                                {
                                    distToTravel.Y = -UpRight.Y * (tileSize);
                                    distToTravel.X = UpRight.X * (tileSize * 2);
                                    strafeUp = true;
                                    strafeRight = true;
                                    prioXStrafe = true;
                                }
                                else
                                {
                                    Random rand = new Random();

                                    if (rand.Next(11) > 5)
                                    {
                                        distToTravel.Y = -UpRight.Y * (tileSize);
                                        distToTravel.X = UpRight.X * (tileSize * 2);
                                        strafeUp = true;
                                        strafeRight = true;
                                        prioXStrafe = true;
                                    }
                                    else
                                    {
                                        distToTravel.X = -DownLeft.X * (tileSize * 2);
                                        distToTravel.Y = DownLeft.Y * (tileSize);
                                        strafeLeft = true;
                                        strafeDown = true;
                                    }
                                }

                            }
                            else if (UpLeft.Length() > DownRight.Length()) // if left and up is more than right and down
                            {
                                if (endDown && endRight)
                                {
                                    distToTravel.Y = -UpLeft.Y * (tileSize * 3);
                                    distToTravel.X = -UpLeft.X * (tileSize);
                                    strafeUp = true;
                                    strafeLeft = true;
                                    prioXStrafe = true;
                                }
                                else
                                {
                                    distToTravel.Y = DownRight.Y * (tileSize);
                                    distToTravel.X = DownRight.X * (tileSize * 2);
                                    strafeDown = true;
                                    strafeRight = true;
                                }


                            }
                            else if (DownRight.Length() > UpLeft.Length()) //if right and down is more than left and up
                            {
                                if (endUp && endLeft)
                                {
                                    distToTravel.Y = DownRight.Y * (tileSize);
                                    distToTravel.X = DownRight.X * (tileSize * 2);
                                    strafeDown = true;
                                    strafeRight = true;
                                }
                                else
                                {
                                    distToTravel.Y = -UpLeft.Y * (tileSize * 3);
                                    distToTravel.X = -UpLeft.X * (tileSize);
                                    strafeUp = true;
                                    strafeLeft = true;
                                    prioXStrafe = true;
                                }

                            }
                            else if (UpLeft.Length() == DownRight.Length())
                            {
                                if (endDown && endRight)
                                {
                                    distToTravel.Y = -UpLeft.Y * (tileSize * 3);
                                    distToTravel.X = -UpLeft.X * (tileSize);
                                    strafeUp = true;
                                    strafeLeft = true;
                                    prioXStrafe = true;
                                }
                                else if (endUp && endLeft)
                                {
                                    distToTravel.Y = DownRight.Y * (tileSize);
                                    distToTravel.X = DownRight.X * (tileSize * 2);
                                    strafeDown = true;
                                    strafeRight = true;
                                }
                                else
                                {
                                    Random rand = new Random();

                                    if (rand.Next(11) > 5)
                                    {
                                        distToTravel.Y = DownRight.Y * (tileSize);
                                        distToTravel.X = DownRight.X * (tileSize * 2);
                                        strafeDown = true;
                                        strafeRight = true;
                                    }
                                    else
                                    {
                                        distToTravel.Y = -UpLeft.Y * (tileSize * 3);
                                        distToTravel.X = -UpLeft.X * (tileSize);
                                        strafeUp = true;
                                        strafeLeft = true;
                                        prioXStrafe = true;
                                    }
                                }


                            }
                            break;
                        case "down":
                            numDown = 1;
                            tempCol = col;
                            col--;
                            for (int i = row + 1; i < 15 && col >= 0; i++) //Down and Left
                            {
                                //col--;

                                if (map.PlatformIndexes.Contains(map.GetPoint(i, col, mapDims)))
                                {

                                    numDown++;
                                    numLeft++;
                                    DownLeft = new Vector2(numLeft, numDown);
                                }
                                else if (map.FloorIndexes.Contains(map.GetPoint(i, col, mapDims)))
                                {
                                    break;
                                }


                                if (i == 14 || col == 0 && map.PlatformIndexes.Contains(map.GetPoint(i, col, mapDims)))
                                {
                                    endDown = true;
                                    endLeft = true;
                                    break;
                                }
                                else if (map.PlatformIndexes.Contains(map.GetPoint(i, col + 1, mapDims)))
                                {

                                    endDown = true;
                                    endLeft = true;
                                    break;
                                }
                                col--;
                            }
                            numLeft = 1;
                            numDown = 1;
                            col = tempCol;
                            col++;
                            for (int i = row - 1; i >= 0 && col < 25; i--)//Up and Right
                            {
                                //col++;
                                if (map.PlatformIndexes.Contains(map.GetPoint(i, col, mapDims)))
                                {

                                    numUp++;
                                    numRight++;
                                    UpRight = new Vector2(numUp, numRight);
                                }
                                else if (map.FloorIndexes.Contains(map.GetPoint(i, col, mapDims)))
                                {
                                    break;
                                }


                                if (i == 0 || col == 24 && map.PlatformIndexes.Contains(map.GetPoint(i, col, mapDims)))
                                {
                                    endUp = true;
                                    endRight = true;
                                    break;
                                }
                                else if (map.PlatformIndexes.Contains(map.GetPoint(i, col - 1, mapDims)))
                                {
                                    endUp = true;
                                    endRight = true;
                                    break;
                                }
                                col++;
                            }
                            numUp = 1;
                            numRight = 1;
                            tempRow = row;
                            row--;
                            col = tempCol;
                            for (int i = col - 1; i >= 0 && row >= 0; i--)//Up and left
                            {
                                //row--;
                                if (map.PlatformIndexes.Contains(map.GetPoint(row, i, mapDims)))
                                {

                                    numUp++;
                                    numLeft++;
                                    UpLeft = new Vector2(numLeft, numUp);
                                }
                                else if (map.FloorIndexes.Contains(map.GetPoint(row, i, mapDims)))
                                {
                                    break;
                                }


                                if (i == 0 || row == 0 && map.PlatformIndexes.Contains(map.GetPoint(row, i, mapDims))) //Logic can be changed to move over obstacles and walls
                                {
                                    endUp = true;
                                    endLeft = true;
                                    break;
                                }
                                else if (map.PlatformIndexes.Contains(map.GetPoint(row, i + 1, mapDims)))
                                {
                                    endUp = true;
                                    endLeft = true;
                                    break;
                                }
                                row--;
                            }
                            numUp = 1;
                            numLeft = 1;
                            row = tempRow;
                            row++;
                            for (int i = col + 1; i < 25 && row < 15; i++) //Down and Right
                            {
                                //row++;
                                if (map.PlatformIndexes.Contains(map.GetPoint(row, i, mapDims)))
                                {

                                    numDown++;
                                    numRight++;
                                    DownRight = new Vector2(numRight, numDown);
                                }
                                else if (map.FloorIndexes.Contains(map.GetPoint(row, i, mapDims)))
                                {
                                    break;
                                }

                                if (i == 24 || row == 14 && map.PlatformIndexes.Contains(map.GetPoint(row, i, mapDims)))
                                {
                                    endDown = true;
                                    endRight = true;
                                    break;
                                }
                                else if (map.PlatformIndexes.Contains(map.GetPoint(row, i + 1, mapDims)))
                                {
                                    endDown = true;
                                    endRight = true;
                                    break;
                                }
                                row++;
                            }
                            //Check if numX == numX      and      if numY==numY
                            if (UpLeft.Length() > DownRight.Length()) // if left and up is more than right and down
                            {
                                if (endDown && endRight)
                                {
                                    distToTravel.Y = -UpLeft.Y * (tileSize);
                                    distToTravel.X = -UpLeft.X * (tileSize);
                                    strafeUp = true;
                                    strafeLeft = true;
                                    prioXStrafe = true;
                                }
                                else
                                {
                                    distToTravel.Y = DownRight.Y * (tileSize * 2);
                                    distToTravel.X = DownRight.X * (tileSize * 2);
                                    strafeDown = true;
                                    strafeRight = true;
                                }


                            }
                            else if (DownRight.Length() > UpLeft.Length()) //if right and down is more than left and up
                            {
                                if (endUp && endLeft)
                                {
                                    distToTravel.Y = DownRight.Y * (tileSize * 2);
                                    distToTravel.X = DownRight.X * (tileSize * 2);
                                    strafeDown = true;
                                    strafeRight = true;
                                }
                                else
                                {
                                    distToTravel.Y = -UpLeft.Y * (tileSize);
                                    distToTravel.X = -UpLeft.X * (tileSize * 3);
                                    strafeUp = true;
                                    strafeLeft = true;
                                    prioXStrafe = true;
                                }

                            }
                            else if (UpLeft.Length() == DownRight.Length())
                            {
                                if (endDown && endRight)
                                {
                                    distToTravel.Y = -UpLeft.Y * (tileSize * 3);
                                    distToTravel.X = -UpLeft.X * (tileSize);
                                    strafeUp = true;
                                    strafeLeft = true;
                                    prioXStrafe = true;
                                }
                                else if (endUp && endLeft)
                                {
                                    distToTravel.Y = DownRight.Y * (tileSize);
                                    distToTravel.X = DownRight.X * (tileSize * 2);
                                    strafeDown = true;
                                    strafeRight = true;
                                }
                                else
                                {
                                    Random rand = new Random();

                                    if (rand.Next(11) > 5)
                                    {
                                        distToTravel.Y = DownRight.Y * (tileSize);
                                        distToTravel.X = DownRight.X * (tileSize * 2);
                                        strafeDown = true;
                                        strafeRight = true;
                                    }
                                    else
                                    {
                                        distToTravel.Y = -UpLeft.Y * (tileSize * 3);
                                        distToTravel.X = -UpLeft.X * (tileSize);
                                        strafeUp = true;
                                        strafeLeft = true;
                                        prioXStrafe = true;
                                    }
                                }


                            }
                            else if (DownLeft.Length() > UpRight.Length())//Left and Down is more than Right and Up
                            {
                                if (endUp && endRight)
                                {
                                    distToTravel.X = -DownLeft.X * (tileSize * 2);
                                    distToTravel.Y = DownLeft.Y * (tileSize * 2);
                                    strafeLeft = true;
                                    strafeDown = true;
                                }
                                else
                                {
                                    distToTravel.Y = -UpRight.Y * (tileSize);
                                    distToTravel.X = UpRight.X * (tileSize * 2);
                                    strafeUp = true;
                                    strafeRight = true;
                                    prioXStrafe = true;
                                }

                            }
                            else if (UpRight.Length() > DownLeft.Length())//Right and Up is more than Left and Down
                            {
                                if (endDown && endLeft)
                                {
                                    distToTravel.Y = -UpRight.Y * (tileSize);
                                    distToTravel.X = UpRight.X * (tileSize * 2);
                                    strafeUp = true;
                                    strafeRight = true;
                                    prioXStrafe = true;
                                }
                                else
                                {
                                    distToTravel.X = -DownLeft.X * (tileSize * 2);
                                    distToTravel.Y = DownLeft.Y * (tileSize * 2);
                                    strafeLeft = true;
                                    strafeDown = true;
                                }

                            }
                            else if (DownLeft.Length() == UpRight.Length())
                            {
                                if (endUp && endRight)
                                {
                                    distToTravel.X = -DownLeft.X * (tileSize * 2);
                                    distToTravel.Y = DownLeft.Y * (tileSize * 2);
                                    strafeLeft = true;
                                    strafeDown = true;
                                }
                                else if (endDown && endLeft)
                                {
                                    distToTravel.Y = -UpRight.Y * (tileSize);
                                    distToTravel.X = UpRight.X * (tileSize * 2);
                                    strafeUp = true;
                                    strafeRight = true;
                                    prioXStrafe = true;
                                }
                                else
                                {
                                    Random rand = new Random();

                                    if (rand.Next(11) > 5)
                                    {
                                        distToTravel.Y = -UpRight.Y * (tileSize);
                                        distToTravel.X = UpRight.X * (tileSize * 2);
                                        strafeUp = true;
                                        strafeRight = true;
                                        prioXStrafe = true;
                                    }
                                    else
                                    {
                                        distToTravel.X = -DownLeft.X * (tileSize * 2);
                                        distToTravel.Y = DownLeft.Y * (tileSize * 2);
                                        strafeLeft = true;
                                        strafeDown = true;
                                    }
                                }

                            }

                            //SetStrafe(numUp, numDown, numLeft, numRight, dirX, dirY);
                            break;
                    }
                    break;
            }

            //SetStrafe(numUp, numDown, numLeft, numRight);
        }

        void ChooseYStrafe(int row, int col, string dir, string blockDir)
        {
            int numDown = 1; //Number of walls going down
            int numUp = 1; //Number of walls going up
            bool endDown = false;
            bool endUp = false;
            for (int i = row + 1; i < 15; i++)
            {
                if (blockDir == "left" && map.PlatformIndexes.Contains(map.GetPoint(i, col - 1, mapDims)))
                {
                    endDown = true;
                    break;

                }
                if (blockDir == "right" && map.PlatformIndexes.Contains(map.GetPoint(i, col + 1, mapDims)))
                {
                    endDown = true; ;
                    break;

                }

                if (i < 15 && map.PlatformIndexes.Contains(map.GetPoint(i, col, mapDims)))
                {
                    numDown++;
                }
                else
                {
                    break;
                }
            }
            for (int i = row - 1; i >= 0; i--)
            {
                if (blockDir == "left" && map.PlatformIndexes.Contains(map.GetPoint(i, col - 1, mapDims)))
                {
                    endUp = true;
                    break;
                }
                if (blockDir == "right" && map.PlatformIndexes.Contains(map.GetPoint(i, col + 1, mapDims)))
                {
                    endUp = true;
                    break;
                }
                if (i > 0 && map.PlatformIndexes.Contains(map.GetPoint(i, col, mapDims)))
                {
                    numUp++;
                }
                else
                {
                    break;
                }
            }



            if (numDown < numUp)
            {
                if (endDown)
                {
                    strafeUp = true;
                    strafeDown = false;

                    if (dir == "up")
                    {
                        distToTravel.Y = -numUp * (tileSize * 2);
                    }
                    if (dir == "down")
                    {
                        distToTravel.Y = -numUp * (tileSize);
                    }
                    if (dir == "")
                    {
                        distToTravel.Y = -numUp * (tileSize * 2);
                    }
                }
                else
                {
                    strafeDown = true;
                    strafeUp = false;

                    if (dir == "up")
                    {
                        distToTravel.Y = numDown * (tileSize * 2);
                    }
                    if (dir == "down")
                    {
                        distToTravel.Y = numDown * (tileSize * 2);
                    }
                    if (dir == "")
                    {
                        distToTravel.Y = numDown * (tileSize * 2);
                    }
                }


            }
            if (numUp < numDown)
            {
                if (endUp)
                {
                    strafeDown = true;
                    strafeUp = false;

                    if (dir == "up")
                    {
                        distToTravel.Y = numDown * (tileSize * 2);
                    }
                    if (dir == "down")
                    {
                        distToTravel.Y = numDown * (tileSize * 2);
                    }
                    if (dir == "")
                    {
                        distToTravel.Y = numDown * (tileSize * 2);
                    }
                }
                else
                {
                    strafeUp = true;
                    strafeDown = false;

                    if (dir == "up")
                    {
                        distToTravel.Y = -numUp * (tileSize * 2);
                    }
                    if (dir == "down")
                    {
                        distToTravel.Y = -numUp * (tileSize);
                    }
                    if (dir == "")
                    {
                        distToTravel.Y = -numUp * (tileSize * 2);
                    }
                }

            }

            if (numUp == numDown)
            {
                if (endUp)
                {
                    strafeDown = true;
                    strafeUp = false;

                    if (dir == "up")
                    {
                        distToTravel.Y = numDown * (tileSize * 2);
                    }
                    if (dir == "down")
                    {
                        distToTravel.Y = numDown * (tileSize * 2);
                    }
                    if (dir == "")
                    {
                        distToTravel.Y = numDown * (tileSize * 2);
                    }
                }
                else if (endDown)
                {
                    strafeUp = true;
                    strafeDown = false;

                    if (dir == "up")
                    {
                        distToTravel.Y = -numUp * (tileSize * 2);
                    }
                    if (dir == "down")
                    {
                        distToTravel.Y = -numUp * (tileSize);
                    }
                    if (dir == "")
                    {
                        distToTravel.Y = -numUp * (tileSize * 2);
                    }
                }
                else
                {
                    Random rand = new Random();
                    if (rand.Next(0, 2) > 0)
                    {
                        strafeUp = true;
                        strafeDown = false;
                        if (dir == "up")
                        {
                            distToTravel.Y = -numUp * (tileSize * 2);
                        }
                        if (dir == "down")
                        {
                            distToTravel.Y = -numUp * (tileSize);
                        }
                        if (dir == "")
                        {
                            distToTravel.Y = -numUp * (tileSize * 2);
                        }

                    }
                    else
                    {
                        strafeDown = true;
                        strafeUp = false;
                        if (dir == "up")
                        {
                            distToTravel.Y = numDown * (tileSize * 2);
                        }
                        if (dir == "down")
                        {
                            distToTravel.Y = numDown * (tileSize);
                        }
                        if (dir == "")
                        {
                            distToTravel.Y = numDown * (tileSize * 2);
                        }
                    }
                }

            }
        }

        int WallUntilOpening(int row, int col, string checkDirConst, string checkDir, int blocksout, int tilesFromCenter)
        {
            int walls = 1;

            switch (checkDirConst)
            {
                case "right":
                    switch (checkDir)
                    {
                        case "up":
                            for (int i = 1; i <= blocksout; i++)//Checking to the right up tilesFrom
                            {
                                if (map.PlatformIndexes.Contains(map.GetPoint(row - tilesFromCenter, col + i, mapDims)))
                                {
                                    walls++;
                                }
                                if (map.FloorIndexes.Contains(map.GetPoint(row - tilesFromCenter, col + i, mapDims)))
                                {
                                    break;
                                }
                            }
                            break;
                        case "down":
                            for (int i = 1; i <= blocksout; i++)//Checking to right down tilesFrom
                            {
                                if (map.PlatformIndexes.Contains(map.GetPoint(row + tilesFromCenter, col + i, mapDims)))
                                {
                                    walls++;
                                }
                                if (map.FloorIndexes.Contains(map.GetPoint(row + tilesFromCenter, col + i, mapDims)))
                                {
                                    break;
                                }
                            }
                            break;

                    }

                    break;
                case "left":

                    switch (checkDir)
                    {
                        case "up":
                            for (int i = 1; i <= blocksout; i++)//Checking to the left up one
                            {
                                if (map.PlatformIndexes.Contains(map.GetPoint(row - tilesFromCenter, col - i, mapDims)))
                                {
                                    walls++;
                                }
                                if (map.FloorIndexes.Contains(map.GetPoint(row - tilesFromCenter, col - i, mapDims)))
                                {
                                    break;
                                }
                            }
                            break;
                        case "down":
                            for (int i = 1; i <= blocksout; i++)//Checking to the left down one
                            {
                                if (map.PlatformIndexes.Contains(map.GetPoint(row + tilesFromCenter, col - i, mapDims)))
                                {
                                    walls++;
                                }
                                if (map.FloorIndexes.Contains(map.GetPoint(row + tilesFromCenter, col - i, mapDims)))
                                {
                                    break;
                                }
                            }
                            break;

                    }
                    break;
                case "up":
                    switch (checkDir)
                    {
                        case "right":
                            for (int i = 1; i <= blocksout; i++)//Checking upwards to the right one
                            {
                                if (map.PlatformIndexes.Contains(map.GetPoint(row - i, col + tilesFromCenter, mapDims)))
                                {
                                    walls++;
                                }
                                if (map.FloorIndexes.Contains(map.GetPoint(row - i, col + tilesFromCenter, mapDims)))
                                {
                                    break;
                                }
                            }
                            break;
                        case "left":
                            for (int i = 1; i <= blocksout; i++)//Checking upwards to the left once
                            {
                                if (map.PlatformIndexes.Contains(map.GetPoint(row - i, col - tilesFromCenter, mapDims)))
                                {
                                    walls++;
                                }
                                if (map.FloorIndexes.Contains(map.GetPoint(row - i, col - tilesFromCenter, mapDims)))
                                {
                                    break;
                                }
                            }
                            break;
                    }
                    break;
                case "down":
                    switch (checkDir)
                    {
                        case "right":
                            for (int i = 1; i <= blocksout; i++) //Checking downwards to the right once
                            {
                                if (map.PlatformIndexes.Contains(map.GetPoint(row + i, col + tilesFromCenter, mapDims)))
                                {
                                    walls++;
                                }
                                if (map.FloorIndexes.Contains(map.GetPoint(row + i, col + tilesFromCenter, mapDims)))
                                {
                                    break;
                                }
                            }
                            break;
                        case "left":
                            for (int i = 1; i <= blocksout; i++) //Checking downwards to the left once
                            {
                                if (map.PlatformIndexes.Contains(map.GetPoint(row + i, col - tilesFromCenter, mapDims)))
                                {
                                    walls++;
                                }
                                if (map.FloorIndexes.Contains(map.GetPoint(row + i, col - tilesFromCenter, mapDims)))
                                {
                                    break;
                                }
                            }
                            break;

                    }
                    break;
            }
            return walls;
        }

        void ChooseXStrafe(int row, int col, string blocDir)
        {
            int numRight = 1;
            int numLeft = 1;

            bool endRight = false;
            bool endLeft = false;

            for (int i = col + 1; i < 25; i++)
            {
                if (blocDir == "up" && map.PlatformIndexes.Contains(map.GetPoint(row + 1, i, mapDims)))
                {
                    endRight = true;
                    break;

                }
                if (blocDir == "down" && map.PlatformIndexes.Contains(map.GetPoint(row - 1, i, mapDims)))
                {
                    endRight = true;
                    break;

                }
                if (map.PlatformIndexes.Contains(map.GetPoint(row, i, mapDims)))
                {
                    numRight++;
                }
                else
                {
                    break;
                }
            }
            for (int i = col - 1; i > 0; i--)
            {
                if (blocDir == "up" && map.PlatformIndexes.Contains(map.GetPoint(row + 1, i, mapDims)))
                {
                    endLeft = true;
                    break;

                }
                if (blocDir == "down" && map.PlatformIndexes.Contains(map.GetPoint(row - 1, i, mapDims)))
                {
                    endLeft = true;
                    break;
                }
                if (map.PlatformIndexes.Contains(map.GetPoint(row, i, mapDims)))
                {

                    numLeft++;
                }
                else
                {
                    break;
                }
            }

            if (numRight < numLeft)
            {
                if (endRight)
                {
                    strafeLeft = true;
                    strafeRight = false;

                    distToTravel.X = -numLeft * (tileSize);
                }
                else
                {
                    strafeRight = true;
                    strafeLeft = false;
                    distToTravel.X = numRight * (tileSize * 2);
                }


            }
            if (numLeft < numRight)
            {

                if (endLeft)
                {
                    strafeRight = true;
                    strafeLeft = false;
                    distToTravel.X = numRight * (tileSize * 2);
                }
                else
                {
                    strafeLeft = true;
                    strafeRight = false;

                    distToTravel.X = -numLeft * (tileSize * 2);
                }


            }

            if (numLeft == numRight)
            {

                if (endRight)
                {
                    strafeLeft = true;
                    strafeRight = false;

                    distToTravel.X = -numLeft * (tileSize * 2);
                }
                else if (endLeft)
                {
                    strafeRight = true;
                    strafeLeft = false;
                    distToTravel.X = numRight * (tileSize * 2);
                }
                else
                {
                    Random rand = new Random();
                    if (rand.Next(0, 2) > 0)
                    {

                        strafeLeft = true;
                        strafeRight = false;

                        distToTravel.X = -numLeft * (tileSize * 2);
                    }
                    else
                    {
                        strafeRight = true;
                        strafeLeft = false;
                        distToTravel.X = numRight * (tileSize * 2);
                    }
                }
            }
        }

        public float RandFloat(int min, int max)
        {
            Random r = new Random();
            float decimalNumber;
            string beforePoint = r.Next(min, max).ToString();//number before decimal point
            string afterPoint = r.Next(5, 10).ToString();
            string afterPoint2 = r.Next(0, 10).ToString();
            string afterPoint3 = r.Next(0, 10).ToString();//1st decimal point
                                                          //string secondDP = r.Next(0, 9).ToString();//2nd decimal point
            string combined = beforePoint + "." + afterPoint + afterPoint2 + afterPoint3;
            return decimalNumber = float.Parse(combined);
        }

        #endregion
    }
}
