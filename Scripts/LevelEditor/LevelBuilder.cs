using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Button;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;

namespace GameJam_KoganDev.Scripts.LevelEditor
{
    public class LevelBuilder
    {
        // 9x15 game size // will need to place either the map or the camera to position the game in the middle of view 
        int gameCols = 15; // size of the game view 
        int gameRows = 15; // size of the game view 

        public int[,] gameMap;

        int groundTileIndex = 1;
        int platformTileIndex = 2;

        int maxPlatforms = 5;
        int minPlatforms = 4;
        
        Random random = new Random();
        bool isJumpable = false;

        bool checkRow = false;
   
        int minOpenings = 5;
        int numOpenings = 0;


        public LevelBuilder()
        {

        }

        public void Initialize()
        {
            gameMap = new int[gameRows, gameCols];
        }

        public void StartLevel(int level)
        {
            switch(level)
            {
                default:
                    CreateStart(level);
                    break;
            }
        }

        void CreateStart(int level)
        {
            List<Rectangle> currRow = new List<Rectangle>(); //represents the platform tiles on the row that is "currently" being made
            int platformChance = 80;
            int numPlatforms = 0;
            //Add Ground tiles to the bottom row 
            for (int i = 0; i < gameCols; i++)
            {
                gameMap[gameRows - 1, i] = groundTileIndex;
            }

           
            bool doesRemake = true;
            int rowsMade = 0;
            for (int y = gameRows - 3; y >= 0; y--) //Start from the 3 row from the bottom going completely right, then up one row 
            {
                doesRemake = true;
                numOpenings = 0;
                numPlatforms = 0;
                while (doesRemake && rowsMade >= 1)
                {

                    List<Rectangle> prevRow = new List<Rectangle>(currRow);
                    numPlatforms = 0;
                    CreateRow(currRow, ref platformChance, ref numPlatforms, y);
                    rowsMade++;
                    checkRow = true;
                    if (checkRow && rowsMade >= 2)
                    {
                       
                        foreach (Rectangle prevPlatform in prevRow)
                        {
                            bool openUp = true;
                            bool openUpLeft = true;
                            bool openUpRight = true;
                            bool openRight = true;
                            bool openLeft = true;

                            Dictionary<string, Rectangle> checkRects = new Dictionary<string, Rectangle>();
                            checkRects.Add("Up", new Rectangle(prevPlatform.X, prevPlatform.Y - prevPlatform.Height, prevPlatform.Width, prevPlatform.Height));
                            checkRects.Add("UpRight", new Rectangle(prevPlatform.Right, prevPlatform.Y - prevPlatform.Height, prevPlatform.Width, prevPlatform.Height));
                            checkRects.Add("UpLeft", new Rectangle(prevPlatform.X - prevPlatform.Width, prevPlatform.Y - prevPlatform.Height, prevPlatform.Width, prevPlatform.Height));
                            checkRects.Add("Left", new Rectangle(prevPlatform.X - prevPlatform.Width, prevPlatform.Y, prevPlatform.Width, prevPlatform.Height));
                            checkRects.Add("Right", new Rectangle(prevPlatform.Right, prevPlatform.Y, prevPlatform.Width, prevPlatform.Height));

                            
                            for (int i = 0; i < currRow.Count; i++)
                            {
                                int z = 0;
                                foreach (Rectangle rect in checkRects.Values)
                                {
                                    if(rect == currRow[i])
                                    {
                                        switch (z)
                                        {
                                            case 0: // if Up
                                                openUp = false;
                                                break;
                                            case 1:
                                                openUpRight = false;
                                                break;
                                            case 2:
                                                openUpLeft = false;
                                                break;
                                            case 3:
                                                openLeft = false;
                                                break;
                                            case 4:
                                                openRight = false;
                                                break;
                                        }
                                    }
                                    z++;
                                }
                                //if (checkRects.Values.Contains(currRow[i]))
                                //{
                                    
                                //}
                                //else
                                //{
                                //    z++;
                                //}
                            }


                            doesRemake = DetermineRemake(openUp, openUpRight, openUpLeft, openLeft, openRight);

                            if(!doesRemake && numOpenings < minOpenings)
                            {
                                numOpenings++;
                                doesRemake = true;
                            }
                            else if(numOpenings >= minOpenings)
                            {
                                doesRemake = false;
                                for (int i = 0; i < currRow.Count; i++)
                                {
                                    gameMap[y, (currRow[i].X / 64)] = platformTileIndex;
                                }
                            }

                            
                        }

                    }
                }
                if(rowsMade == 0)
                {
                    CreateRow(currRow, ref platformChance, ref numPlatforms, y);
                    rowsMade++;
                    for(int i = 0; i < currRow.Count; i++)
                    {
                        gameMap[y, (currRow[i].X / 64)] = platformTileIndex;
                    }
                }

              
            }
        }

        bool DetermineRemake(bool openUp, bool openUpRight, bool openUpLeft, bool openLeft, bool openRight)
        {
            bool remake = true;
            int numOpen = 10; // point system to determine optimal openings
            int points = 0;

            if(!openUp) // The logic is 
            {
                if(openUpRight)
                {
                    points += 3; // if open up and to the sides while the top is blocked, not bad but not good
                }
                if(openUpLeft)
                {
                    points += 3;
                }
                if(openLeft && openUpLeft) // open on the sides AND above (on the sides) is optimal, guranteeing success
                {
                    points += 6 ;
                }
                else if(openLeft) // just being open on the side isn't the best, but can still create opportunities given the chance
                {
                    points += 2;
                }
                if (openRight && openUpRight)// open on the sides AND above (on the sides) is optimal, guranteeing success
                {
                    points += 6;
                }
                else if(openRight)// just being open on the side isn't the best, but can still create opportunities given the chance
                {
                    points += 2;
                }

            }
            else // Open Up == true
            {
                if(!openRight && !openLeft) // Least optimal, but can sometimes create opportunities
                {
                    points += 2;
                }
                if(openRight)
                {
                    if(openUpRight)
                    {
                        points += 6; // Most optimal being open on all sides on the right 
                    }
                    else
                    {
                        points += 3;
                    }
                }
                else if(openUpRight)
                {
                    points += 3;
                }
                if (openLeft)
                {
                    if(openUpLeft)
                    {
                        points += 6;
                    }
                    else
                    {
                        points += 3;
                    }
                }
                else if(openUpLeft)
                {
                    points += 3;
                }
            }

            if(points >= numOpen)
            {
                remake = false;
            }

                return remake;
            
        }

        private void CreateRow(List<Rectangle> currRow, ref int platformChance, ref int numPlatforms, int y)
        {
            int i = 0;
            currRow.Clear();
            if(random.Next(101) > 50)
            {
                while (numPlatforms < minPlatforms || i < gameCols - 1)
                {
                    for (i = 1; i < gameCols - 1; i++)
                    {
                        if (random.Next(101) <= platformChance && numPlatforms < maxPlatforms && currRow.Contains(new Rectangle(i * 64, (y * 64), 64, 64)) == false)
                        {
                            //gameMap[y, i] = platformTileIndex;
                            currRow.Add(new Rectangle(i * 64, (y * 64), 64, 64));
                            numPlatforms++;
                            platformChance = 20;

                        }
                    }
                    if (platformChance >= 20)
                    {
                        platformChance -= 45;
                    }
                    else
                    {
                        platformChance = 85;
                    }
                }
            }
            else
            {
                while (numPlatforms < minPlatforms || i > 1)
                {
                    for (i = gameCols - 2; i > 0; i--)
                    {
                        if (random.Next(101) <= platformChance && numPlatforms < maxPlatforms && currRow.Contains(new Rectangle(i * 64, (y * 64), 64, 64)) == false)
                        {
                            //gameMap[y, i] = platformTileIndex;
                            currRow.Add(new Rectangle(i * 64, (y * 64), 64, 64));
                            numPlatforms++;
                            platformChance = 20;

                        }
                    }
                    if (platformChance >= 20)
                    {
                        platformChance -= 45;
                    }
                    else
                    {
                        platformChance = 85;
                    }
                }
            }
            

            
        }
    }
}
