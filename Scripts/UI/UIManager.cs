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

        public void CreateUIElements(Vector2 dims, Game1 game)
        {
            uiElements.Clear();
            this.dims = dims;
            this.game = game;

            //Text blocks get a BGrect AND Rect call
            //Button just gets BGrect

            #region Title Screen
            uiElements.Add("MainMenuTitle", UIHelper.CreateTextblock("MainMenuTitle", "Death of a Thing", ((int)dims.X / 2) - (550 / 2), (int)(dims.Y / 4)));
            UITextBlock currItem = (UITextBlock)uiElements["MainMenuTitle"];
            UIHelper.SetElementBGRect(currItem, new Rectangle((int)currItem.Position.X, (int)currItem.Position.Y, 600, 150));

            uiElements.Add("MainMenuPlay", UIHelper.CreateButton("MainMenuPlay", "Play", ((int)dims.X / 2) - (int)(250/2.5f), (int)(dims.Y / 2) - (64 * 1)));
            UIHelper.SetRectangle(uiElements["MainMenuPlay"], 250, 100);

            uiElements.Add("MainMenuLoad", UIHelper.CreateButton("MainMenuLoad", "Load", ((int)dims.X / 2) - (int)(250 / 2.5f), 
                UIHelper.GetRectangle(uiElements["MainMenuPlay"]).Bottom + 20));
            UIHelper.SetRectangle(uiElements["MainMenuLoad"], 250, 100);

            uiElements.Add("MainMenuHowTo", UIHelper.CreateButton("MainMenuHowTo", "How to Play", ((int)dims.X / 2) - (int)(250 / 2.5f),
                UIHelper.GetRectangle(uiElements["MainMenuLoad"]).Bottom + 20));
            UIHelper.SetRectangle(uiElements["MainMenuHowTo"], 250, 100);

            uiElements.Add("MainMenuQuit", UIHelper.CreateButton("MainMenuQuit", "Quit", ((int)dims.X / 2) - (int)(250 / 2.5f),
                UIHelper.GetRectangle(uiElements["MainMenuHowTo"]).Bottom + 20));
            UIHelper.SetRectangle(uiElements["MainMenuQuit"], 250, 100);
            #endregion

            #region How to Play
            //uiElements.Add
            #endregion

            #region Settings
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
            }
        }


        public void UIButton_Clicked(object sender, UIButtonArgs e)
        {
            string buttonName = e.ID;

            if(buttonName == "MainMenuPlay")
            {
                game.StartGame();
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
