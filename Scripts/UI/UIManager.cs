using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Microsoft.Xna.Framework.Input;
using System.Collections;
using System.ComponentModel.DataAnnotations.Schema;
using System.Windows.Forms;


namespace GameJam_KoganDev.Scripts.UI
{
    internal class UIManager
    {
        public Dictionary<string, UIWidget> uiElements = new Dictionary<string, UIWidget>();
        Game1 game;

        Vector2 dims;
        bool transition = false; // transition menu scene

        public bool willDelete = false; //delete save

        public float MasterVolume = 1;
        public float MusicVolume = 1;
        public float EffectVolume = 1;

        bool isVolume = false;
        public Rectangle SelectedRect = Rectangle.Empty;

        public UIManager()
        {

        }

        public void CreateUIElements(Vector2 dims, Game1 game)
        {
            uiElements.Clear();
            this.dims = dims;
            this.game = game;

            //Text blocks get a BGrect AND Rect call
            //Button just gets BGrect
            uiElements.Add("SkillSelection", UIHelper.CreateTextblock("SkillSelection", "Skill:\nBreak: infinite use", (int)(1000), (int)(50)));
            UITextBlock currItem = (UITextBlock)uiElements["SkillSelection"];
            UIHelper.SetElementRect(currItem, new Rectangle(currItem.Position.ToPoint(), new Point(200, 50)));
            //UIHelper.SetElementBGRect(currItem, new Rectangle(currItem.Position.ToPoint(), new Point(200, 50)));

            #region Title Screen
            uiElements.Add("MainMenuTitle", UIHelper.CreateTextblock("MainMenuTitle", "Death of a Thing", ((int)dims.X / 2) - (550 / 2), (int)(dims.Y / 4)));
            currItem = (UITextBlock)uiElements["MainMenuTitle"];
            UIHelper.SetElementBGRect(currItem, new Rectangle((int)currItem.Position.X, (int)currItem.Position.Y, 600, 150));

            uiElements.Add("MainMenuPlay", UIHelper.CreateButton("MainMenuPlay", "Play", ((int)dims.X / 2) - (int)(250/2.5f), (int)(dims.Y / 2) - (64 * 1)));
            UIHelper.SetRectangle(uiElements["MainMenuPlay"], 250, 100);

            uiElements.Add("MainCreditsBtn", UIHelper.CreateButton("MainCreditsBtn", "Credits", ((int)dims.X / 2) - (int)(250 / 2.5f), 
                UIHelper.GetRectangle(uiElements["MainMenuPlay"]).Bottom + 20));
            UIHelper.SetRectangle(uiElements["MainCreditsBtn"], 250, 100);

            uiElements.Add("MainMenuHowTo", UIHelper.CreateButton("MainMenuHowTo", "How to Play", ((int)dims.X / 2) - (int)(250 / 2.5f),
                UIHelper.GetRectangle(uiElements["MainCreditsBtn"]).Bottom + 20));
            UIHelper.SetRectangle(uiElements["MainMenuHowTo"], 250, 100);

            uiElements.Add("MainMenuQuit", UIHelper.CreateButton("MainMenuQuit", "Quit", ((int)dims.X / 2) - (int)(250 / 2.5f),
                UIHelper.GetRectangle(uiElements["MainMenuHowTo"]).Bottom + 20));
            UIHelper.SetRectangle(uiElements["MainMenuQuit"], 250, 100);
            #endregion

            #region How to Play
            //uiElements.Add
            #endregion

            #region Menu Credits
            uiElements.Add("MenuCredits", UIHelper.CreateTextblock("MenuCredits", "Music: Logan (Heat)\n\nBackground Tiles: Logan (Heat)\n\nCampfire & Platform tiles: Free Online\n\nDevelopment: Keegan Sullivan (KoganDev)",
                ((int)(dims.X/2) - 500), (int)(0 + (dims.Y / 4))));
            currItem = (UITextBlock)uiElements["MenuCredits"];
            UIHelper.SetElementBGRect(currItem, new Rectangle((int)currItem.Position.X, (int)currItem.Position.Y, 1000, 600));

            uiElements.Add("MenuCreditsBtn", UIHelper.CreateButton("MenuCreditsBtn", "Back",
                ((int)(dims.X / 2) - 175), UIHelper.GetElementBGRect(uiElements["MenuCredits"]).Bottom + 15));
            UIHelper.SetRectangle(uiElements["MenuCreditsBtn"], 250, 100);
            #endregion

            foreach (UIWidget widget in uiElements.Values)
            {
                if (widget is UIButton)
                {
                    ((UIButton)widget).Clicked += new
                    UIButton.ClickHandler(UIButton_Clicked);
                }
            }
        }

        #region Helper Methods
        public void AddTextBlock(UITextBlock textBlock)
        {
            uiElements.Add(textBlock.ID, textBlock);
        }

        public void CreateEndLevel(int gameLevel, Rectangle currBounds)
        {
            switch(gameLevel)
            {
                case 0:
                    uiElements.Add("BeatLevel" + gameLevel, UIHelper.CreateTextblock("BeatLevel" + gameLevel,
                        "You earned the CREATE SKILL\n\nThis will allow you to create blocks below you.\n\n Use when CREATE is the assigned skill [^] (UP ARROW)\n and you are pressing the [LShift] SKILL key\n\nPress [Space] to Continue",
                        currBounds.X - 225, currBounds.Center.Y - 250));
                    UITextBlock currItem = (UITextBlock)uiElements["BeatLevel" + gameLevel];
                    UIHelper.SetElementBGRect(currItem, new Rectangle((int)currItem.Position.X, (int)currItem.Position.Y, 1500, 600));
                    UIHelper.SetElementVisibility("BeatLevel" + gameLevel, true, uiElements);
                    break;
                case 1:
                    uiElements.Add("BeatLevel" + gameLevel, UIHelper.CreateTextblock("BeatLevel" + gameLevel,
                       "You earned the DASH SKILL\n\nThis will allow you to dash a short distance in the direction \nyou're moving in. It will also destroy the dark creatures\n\n Use when DASH is the assigned skill [v] (DOWN ARROW)\n and you are pressing the [LShift] SKILL key\n\nPress [Space] to Continue",
                       currBounds.X - 225, currBounds.Center.Y - 250));
                    currItem = (UITextBlock)uiElements["BeatLevel" + gameLevel];
                    UIHelper.SetElementBGRect(currItem, new Rectangle((int)currItem.Position.X, (int)currItem.Position.Y, 1500, 600));
                    UIHelper.SetElementVisibility("BeatLevel" + gameLevel, true, uiElements);
                    break;
                case 2:
                    uiElements.Add("BeatLevel" + gameLevel, UIHelper.CreateTextblock("BeatLevel" + gameLevel,
                     "You earned the POWER-JUMP SKILL\n\nThis will allow you to jump an additional time \n\n Use when POWER-JUMP is the assigned skill \n[>] (RIGHT ARROW) and you are pressing the [LShift] SKILL key\n\nPress [Space] to Continue",
                     currBounds.X - 225, currBounds.Center.Y - 250));
                    currItem = (UITextBlock)uiElements["BeatLevel" + gameLevel];
                    UIHelper.SetElementBGRect(currItem, new Rectangle((int)currItem.Position.X, (int)currItem.Position.Y, 1500, 600));
                    UIHelper.SetElementVisibility("BeatLevel" + gameLevel, true, uiElements);
                    break;
                case 3:
                    uiElements.Add("BeatLevel" + gameLevel, UIHelper.CreateTextblock("BeatLevel" + gameLevel,
                     "You earned an additional power-up spawn\n\nThis adds an additional power-up spawn to each level \n\n\n\nPress [Space] to Continue",
                     currBounds.X - 225, currBounds.Center.Y - 250));
                    currItem = (UITextBlock)uiElements["BeatLevel" + gameLevel];
                    UIHelper.SetElementBGRect(currItem, new Rectangle((int)currItem.Position.X, (int)currItem.Position.Y, 1500, 600));
                    UIHelper.SetElementVisibility("BeatLevel" + gameLevel, true, uiElements);
                    break;
                case 4:
                    uiElements.Add("BeatLevel" + gameLevel, UIHelper.CreateTextblock("BeatLevel" + gameLevel,
                    "\n\n\n\n\n\n\nPress [Space] to Continue",
                    currBounds.X - 225, currBounds.Center.Y - 250));
                    currItem = (UITextBlock)uiElements["BeatLevel" + gameLevel];
                    UIHelper.SetElementBGRect(currItem, new Rectangle((int)currItem.Position.X, (int)currItem.Position.Y, 1500, 600));
                    UIHelper.SetElementVisibility("BeatLevel" + gameLevel, true, uiElements);
                    break;
            }
        }

        public void CreatePreLevel(int gameLevel, string text, Rectangle currBounds)
        {
            switch(text)
            {
                case "Stage 1: Denial":
                    if (uiElements.ContainsKey("PreLevel") == false)
                    {
                        uiElements.Add("PreLevel", UIHelper.CreateTextblock("PreLevel", text, ((int)currBounds.Width / 2) - (275), (int)(currBounds.Height / 4)));
                        UITextBlock currItem = (UITextBlock)uiElements["PreLevel"];
                        UIHelper.SetElementBGRect(currItem, new Rectangle((int)currItem.Position.X, (int)currItem.Position.Y, 600, 150));
                    }
                    else
                    {
                        UIHelper.SetElementText(uiElements["PreLevel"], text);
                    }
                    break;
                case "Stage 2: Bargaining":
                    if (uiElements.ContainsKey("PreLevel") == false)
                    {
                        uiElements.Add("PreLevel", UIHelper.CreateTextblock("PreLevel", text, ((int)currBounds.Width / 2) - (825), (int)(currBounds.Height / 4)));
                        UITextBlock currItem = (UITextBlock)uiElements["PreLevel"];
                        UIHelper.SetElementBGRect(currItem, new Rectangle((int)currItem.Position.X, (int)currItem.Position.Y, 600, 150));
                    }
                    else
                    {
                        UIHelper.SetElementRect(uiElements["PreLevel"], new Rectangle(((int)currBounds.Width / 2) - (825), (int)(currBounds.Height / 4), 600, 150));
                        //UIHelper.SetElementBGRect(uiElements["PreLevel"], new Rectangle(((int)currBounds.Width / 2) - (825), (int)(currBounds.Height / 4), 600, 150));
                        UIHelper.SetElementText(uiElements["PreLevel"], text);
                    }
                    break;
                case "Stage 3: Depression":
                    if (uiElements.ContainsKey("PreLevel") == false)
                    {
                        uiElements.Add("PreLevel", UIHelper.CreateTextblock("PreLevel", text, ((int)currBounds.Width / 2) - (825), (int)(currBounds.Height / 4)));
                        UITextBlock currItem = (UITextBlock)uiElements["PreLevel"];
                        UIHelper.SetElementBGRect(currItem, new Rectangle((int)currItem.Position.X, (int)currItem.Position.Y, 600, 150));
                    }
                    else
                    {
                        UIHelper.SetElementRect(uiElements["PreLevel"], new Rectangle(((int)currBounds.Width / 2) - (825), (int)(currBounds.Height / 4), 600, 150));
                        //UIHelper.SetElementBGRect(uiElements["PreLevel"], new Rectangle(((int)currBounds.Width / 2) - (825), (int)(currBounds.Height / 4), 600, 150));
                        UIHelper.SetElementText(uiElements["PreLevel"], text);
                    }
                    break;
                case "Stage 4: Anger":
                    if (uiElements.ContainsKey("PreLevel") == false)
                    {
                        uiElements.Add("PreLevel", UIHelper.CreateTextblock("PreLevel", text, ((int)currBounds.Width / 2) - (745), (int)(currBounds.Height / 4)));
                        UITextBlock currItem = (UITextBlock)uiElements["PreLevel"];
                        UIHelper.SetElementBGRect(currItem, new Rectangle((int)currItem.Position.X, (int)currItem.Position.Y, 600, 150));
                    }
                    else
                    {
                        UIHelper.SetElementRect(uiElements["PreLevel"], new Rectangle(((int)currBounds.Width / 2) - (745), (int)(currBounds.Height / 4), 600, 150));
                        //UIHelper.SetElementBGRect(uiElements["PreLevel"], new Rectangle(((int)currBounds.Width / 2) - (825), (int)(currBounds.Height / 4), 600, 150));
                        UIHelper.SetElementText(uiElements["PreLevel"], text);
                    }
                    break;
                case "Stage 5: Acceptance":
                    if (uiElements.ContainsKey("PreLevel") == false)
                    {
                        uiElements.Add("PreLevel", UIHelper.CreateTextblock("PreLevel", text, ((int)currBounds.Width / 2) - (825), (int)(currBounds.Height / 4)));
                        UITextBlock currItem = (UITextBlock)uiElements["PreLevel"];
                        UIHelper.SetElementBGRect(currItem, new Rectangle((int)currItem.Position.X, (int)currItem.Position.Y, 600, 150));
                    }
                    else
                    {
                        UIHelper.SetElementRect(uiElements["PreLevel"], new Rectangle(((int)currBounds.Width / 2) - (825), (int)(currBounds.Height / 4), 600, 150));
                        //UIHelper.SetElementBGRect(uiElements["PreLevel"], new Rectangle(((int)currBounds.Width / 2) - (825), (int)(currBounds.Height / 4), 600, 150));
                        UIHelper.SetElementText(uiElements["PreLevel"], text);
                    }
                    break;
            }
           
            UIHelper.SetElementVisibility("PreLevel", true, uiElements);
        }

        public void CreateEndGameCredits(Rectangle bounds)
        {
            uiElements.Add("EndGameCredits", UIHelper.CreateTextblock("EndGameCredits", "Music: Logan (Heat)\n\nBackground Tiles: Logan (Heat)\n\nCampfire & Platform tiles: Free Online\n\nDevelopment: Keegan Sullivan (KoganDev)", 
                ((int)bounds.X), (int)(bounds.Y + (bounds.Height/2) - (bounds.Height + 384))));
            UITextBlock currItem = (UITextBlock)uiElements["EndGameCredits"];
            UIHelper.SetElementBGRect(currItem, new Rectangle((int)currItem.Position.X, (int)currItem.Position.Y, 1000, 600));
            UIHelper.SetElementVisibility("EndGameCredits", true, uiElements);
        }

        public void UIButton_Clicked(object sender, UIButtonArgs e)
        {
            string buttonName = e.ID;

            if(buttonName == "MainMenuPlay")
            {
                game.startGame = true;
                game.gameState = Game1.GameStates.PreLevel;
                
                UIHelper.SetElementVisibility("MainMenu", false, uiElements);
                UIHelper.SetElementVisibility("MainCreditsBtn", false, uiElements);
            }
            else if(buttonName == "MainCreditsBtn")
            {
                UIHelper.SetElementVisibility("MainMenu", false, uiElements);
                UIHelper.SetElementVisibility("MainCreditsBtn", false, uiElements);
                UIHelper.SetElementVisibility("MenuCredits", true, uiElements);
            }
            else if(buttonName == "MainMenuHowTo")
            {
                UIHelper.SetElementVisibility("MainMenu", false, uiElements);
                UIHelper.SetElementVisibility("MainCreditsBtn", false, uiElements);
            }
            else if(buttonName == "MenuCreditsBtn")
            {
                UIHelper.SetElementVisibility("MainMenu", true, uiElements);
                UIHelper.SetElementVisibility("MainCreditsBtn", true, uiElements);
                UIHelper.SetElementVisibility("MenuCredits", false, uiElements);
            }
            else if(buttonName == "MainMenuQuit")
            {
                Application.Exit();
            }
        }

        public void UpdateTextBlock(string keyWord, Rectangle currBounds)
        {
            switch (keyWord)
            {

            }

        }

        public void UpdateButton(string keyWord, float moveSpeed, Game1 game)
        {
            UIButton button = (UIButton)uiElements[keyWord];
            switch (keyWord)
            {
                case "MainMenuPlay":
                    
                    break;
            }
        }
        #endregion

        public void Draw(SpriteBatch spriteBatch)
        {
            foreach(UIWidget widget in uiElements.Values)
            {
                widget.Draw(spriteBatch);
            }
        }
    }
}
