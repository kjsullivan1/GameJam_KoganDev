using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using GameJam_KoganDev.Scripts.LevelEditor;
using GameJam_KoganDev.Scripts;
using GameJam_KoganDev.Scripts.UI;
using Microsoft.Xna.Framework.Content;

namespace GameJam_KoganDev.Scripts
{
    internal class Cutscene
    {
        Rectangle bounds;
        int gameLevel;
        UIManager uI;
        Vector2 startPos;
        Rectangle playeRect;
        Rectangle responderRect;
        public Color playerColor = Color.White;

        Texture2D characterTexture;

        public int mcText = 0;
        public int rsText = 0;

        //Fire animation 
        public AnimationManager animManager;
        Texture2D fireTexture;
        Point FrameSize;//Size of frame
        Point CurrFrame;//Location of currFram on the sheet
        Point SheetSize;//num of frames.xy
        int fpms;
        public Cutscene(Rectangle bounds,ContentManager content ,int gameLevel, UIManager ui, Rectangle player, Rectangle enemyRect)
        {
            uI = ui;
            this.bounds = bounds;
            this.gameLevel = gameLevel;
            playeRect = player;
            responderRect = enemyRect;
            

            fireTexture = content.Load<Texture2D>("Player/AnimatedFire");
            FrameSize = new Point(640, 640);
            CurrFrame = new Point(0, 0);
            SheetSize = new Point(11, 1);
            fpms = 20;
            //animManager = new AnimationManager(fireTexture, FrameSize, CurrFrame, SheetSize, fpms, new Vector2(playeRect.X - 128, playeRect.Y));
            characterTexture = content.Load<Texture2D>("Player/PlayerTexture");

            CreateDialogue();
        }

        public void CreateDialogue()
        {
            
            switch (gameLevel) // substring 4 is game level, substring 3 is dialogue order // number C1D00 is character 1/dialogue 0/in game level 0
            {
                case 0: // Every other // Denial
                    #region Player Dialogue 
                    uI.AddTextBlock(UIHelper.CreateTextblock("C1D00", "This feels like death.", playeRect.X - 25, playeRect.Y - 175));
                    UITextBlock currItem = (UITextBlock)uI.uiElements["C1D00"];
                    UIHelper.SetElementBGRect(currItem, new Rectangle((int)currItem.Position.X, (int)currItem.Position.Y, 500, 150));

                    uI.AddTextBlock(UIHelper.CreateTextblock("C1D10", "I dont care, I dont want it to end...", playeRect.X - 25, playeRect.Y - 175));
                    currItem = (UITextBlock)uI.uiElements["C1D10"];
                    UIHelper.SetElementBGRect(currItem, new Rectangle((int)currItem.Position.X, (int)currItem.Position.Y, 500, 150));
                    #endregion

                    #region Responder Dialogue
                    uI.AddTextBlock(UIHelper.CreateTextblock("C2D00", "It is death, but you'll see, this will be \ngood for the both of us in the end.", responderRect.X - 400, responderRect.Y - 175));
                    currItem = (UITextBlock)uI.uiElements["C2D00"];
                    UIHelper.SetElementBGRect(currItem, new Rectangle((int)currItem.Position.X, (int)currItem.Position.Y, 500, 150));

                    uI.AddTextBlock(UIHelper.CreateTextblock("C2D10", "Also...I'm not even real, you just want \nto see me. You do know that right?", responderRect.X - 400, responderRect.Y - 175));
                    currItem = (UITextBlock)uI.uiElements["C2D10"];
                    UIHelper.SetElementBGRect(currItem, new Rectangle((int)currItem.Position.X, (int)currItem.Position.Y, 500, 150));

                    uI.AddTextBlock(UIHelper.CreateTextblock("C2D20", "That's not something that requires your \nchoice.", responderRect.X - 400, responderRect.Y - 175));
                    currItem = (UITextBlock)uI.uiElements["C2D20"];
                    UIHelper.SetElementBGRect(currItem, new Rectangle((int)currItem.Position.X, (int)currItem.Position.Y, 500, 150));
                    #endregion
                    break;
                case 1: // Bargaining
                    #region Player Dialogue
                    uI.AddTextBlock(UIHelper.CreateTextblock("C1D01", "You know... the changes we're looking for \nare just around the corner.", playeRect.X - 25, playeRect.Y - 175));
                    currItem = (UITextBlock)uI.uiElements["C1D01"];
                    UIHelper.SetElementBGRect(currItem, new Rectangle((int)currItem.Position.X, (int)currItem.Position.Y, 500, 150));

                    uI.AddTextBlock(UIHelper.CreateTextblock("C1D11", "I know I haven't been trying as hard to \nchange, but we're so close to making this \nthing be great.", playeRect.X - 25, playeRect.Y - 175));
                    currItem = (UITextBlock)uI.uiElements["C1D11"];
                    UIHelper.SetElementBGRect(currItem, new Rectangle((int)currItem.Position.X, (int)currItem.Position.Y, 500, 150));
                    #endregion

                    #region Responder Dialogue
                    uI.AddTextBlock(UIHelper.CreateTextblock("C2D01", "It's not JUST that dude. You weren't \nlistening were you?", responderRect.X - 400, responderRect.Y - 175));
                    currItem = (UITextBlock)uI.uiElements["C2D01"];
                    UIHelper.SetElementBGRect(currItem, new Rectangle((int)currItem.Position.X, (int)currItem.Position.Y, 500, 150));

                    uI.AddTextBlock(UIHelper.CreateTextblock("C2D11", "We've grown apart. I want different \nthings for myself.", responderRect.X - 400, responderRect.Y - 175));
                    currItem = (UITextBlock)uI.uiElements["C2D11"];
                    UIHelper.SetElementBGRect(currItem, new Rectangle((int)currItem.Position.X, (int)currItem.Position.Y, 500, 150));

                    uI.AddTextBlock(UIHelper.CreateTextblock("C2D21", "I'm not changing my mind this time.", responderRect.X - 400, responderRect.Y - 175));
                    currItem = (UITextBlock)uI.uiElements["C2D21"];
                    UIHelper.SetElementBGRect(currItem, new Rectangle((int)currItem.Position.X, (int)currItem.Position.Y, 500, 150));
                    #endregion
                    break;
                case 2: //Depression
                    #region Player Dialogue
                    uI.AddTextBlock(UIHelper.CreateTextblock("C1D02", "....", playeRect.X - 25, playeRect.Y - 175));
                    currItem = (UITextBlock)uI.uiElements["C1D02"];
                    UIHelper.SetElementBGRect(currItem, new Rectangle((int)currItem.Position.X, (int)currItem.Position.Y, 500, 150));

                    uI.AddTextBlock(UIHelper.CreateTextblock("C1D12", "I keep dreaming of you. Every night. \nI always end up waking in a panic.", playeRect.X - 25, playeRect.Y - 175));
                    currItem = (UITextBlock)uI.uiElements["C1D12"];
                    UIHelper.SetElementBGRect(currItem, new Rectangle((int)currItem.Position.X, (int)currItem.Position.Y, 500, 150));

                    uI.AddTextBlock(UIHelper.CreateTextblock("C1D22", "But what about all those memories? \nWhat am I supposed to do with them?", playeRect.X - 25, playeRect.Y - 175));
                    currItem = (UITextBlock)uI.uiElements["C1D22"];
                    UIHelper.SetElementBGRect(currItem, new Rectangle((int)currItem.Position.X, (int)currItem.Position.Y, 500, 150));
                    #endregion

                    #region Responder Dialogue
                    uI.AddTextBlock(UIHelper.CreateTextblock("C2D02", "....", responderRect.X - 400, responderRect.Y - 175));
                    currItem = (UITextBlock)uI.uiElements["C2D02"];
                    UIHelper.SetElementBGRect(currItem, new Rectangle((int)currItem.Position.X, (int)currItem.Position.Y, 500, 150));

                    uI.AddTextBlock(UIHelper.CreateTextblock("C2D12", "...", responderRect.X - 400, responderRect.Y - 175));
                    currItem = (UITextBlock)uI.uiElements["C2D12"];
                    UIHelper.SetElementBGRect(currItem, new Rectangle((int)currItem.Position.X, (int)currItem.Position.Y, 500, 150));
                    #endregion
                    break;
                case 3: // Anger
                    #region Player Dialogue
                    uI.AddTextBlock(UIHelper.CreateTextblock("C1D03", "You know...I started to hate the feeling \nI had around you.", playeRect.X - 25, playeRect.Y - 175));
                    currItem = (UITextBlock)uI.uiElements["C1D03"];
                    UIHelper.SetElementBGRect(currItem, new Rectangle((int)currItem.Position.X, (int)currItem.Position.Y, 500, 150));

                    uI.AddTextBlock(UIHelper.CreateTextblock("C1D13", "The way things were was wrong.", playeRect.X - 25, playeRect.Y - 175));
                    currItem = (UITextBlock)uI.uiElements["C1D13"];
                    UIHelper.SetElementBGRect(currItem, new Rectangle((int)currItem.Position.X, (int)currItem.Position.Y, 500, 150));

                    uI.AddTextBlock(UIHelper.CreateTextblock("C1D23", "....", playeRect.X - 25, playeRect.Y - 175));
                    currItem = (UITextBlock)uI.uiElements["C1D23"];
                    UIHelper.SetElementBGRect(currItem, new Rectangle((int)currItem.Position.X, (int)currItem.Position.Y, 500, 150));
                    #endregion

                    #region Responder Dialogue
                    uI.AddTextBlock(UIHelper.CreateTextblock("C2D03", "I didn't want things to end this way, but \nI'm so relieved I get to say this.", responderRect.X - 400, responderRect.Y - 175));
                    currItem = (UITextBlock)uI.uiElements["C2D03"];
                    UIHelper.SetElementBGRect(currItem, new Rectangle((int)currItem.Position.X, (int)currItem.Position.Y, 500, 150));

                    uI.AddTextBlock(UIHelper.CreateTextblock("C2D13", "Have you looked in the mirror lately? \nYou have let yourself go.", responderRect.X - 400, responderRect.Y - 175));
                    currItem = (UITextBlock)uI.uiElements["C2D13"];
                    UIHelper.SetElementBGRect(currItem, new Rectangle((int)currItem.Position.X, (int)currItem.Position.Y, 500, 150));

                    uI.AddTextBlock(UIHelper.CreateTextblock("C2D23", "The way you treat yourself is wrong. \nI can't believe I forced myself to \nfit your expectations", responderRect.X - 400, responderRect.Y - 175));
                    currItem = (UITextBlock)uI.uiElements["C2D23"];
                    UIHelper.SetElementBGRect(currItem, new Rectangle((int)currItem.Position.X, (int)currItem.Position.Y, 500, 150));

                    uI.AddTextBlock(UIHelper.CreateTextblock("C2D33", "For what it's worth, I'm sorry I wasted \nso much of your time.", responderRect.X - 400, responderRect.Y - 175));
                    currItem = (UITextBlock)uI.uiElements["C2D33"];
                    UIHelper.SetElementBGRect(currItem, new Rectangle((int)currItem.Position.X, (int)currItem.Position.Y, 500, 150));
                    #endregion
                    break;
                case 4: // Acceptance
                    #region Player Dialogue
                    uI.AddTextBlock(UIHelper.CreateTextblock("C1D04", "So...this is it huh?", playeRect.X - 25, playeRect.Y - 175));
                    currItem = (UITextBlock)uI.uiElements["C1D04"];
                    UIHelper.SetElementBGRect(currItem, new Rectangle((int)currItem.Position.X, (int)currItem.Position.Y, 500, 150));

                    uI.AddTextBlock(UIHelper.CreateTextblock("C1D14", "Since you're not real, I'll just ask...", playeRect.X - 25, playeRect.Y - 175));
                    currItem = (UITextBlock)uI.uiElements["C1D14"];
                    UIHelper.SetElementBGRect(currItem, new Rectangle((int)currItem.Position.X, (int)currItem.Position.Y, 500, 150));

                    uI.AddTextBlock(UIHelper.CreateTextblock("C1D24", "You think we would be able to try \nagain in the future?", playeRect.X - 25, playeRect.Y - 175));
                    currItem = (UITextBlock)uI.uiElements["C1D24"];
                    UIHelper.SetElementBGRect(currItem, new Rectangle((int)currItem.Position.X, (int)currItem.Position.Y, 500, 150));

                    uI.AddTextBlock(UIHelper.CreateTextblock("C1D34", "...", playeRect.X - 25, playeRect.Y - 175));
                    currItem = (UITextBlock)uI.uiElements["C1D34"];
                    UIHelper.SetElementBGRect(currItem, new Rectangle((int)currItem.Position.X, (int)currItem.Position.Y, 500, 150));

                    uI.AddTextBlock(UIHelper.CreateTextblock("C1D44", "You are still the coolest person \nI have ever met.", playeRect.X - 25, playeRect.Y - 175));
                    currItem = (UITextBlock)uI.uiElements["C1D44"];
                    UIHelper.SetElementBGRect(currItem, new Rectangle((int)currItem.Position.X, (int)currItem.Position.Y, 500, 150));
                    #endregion

                    #region Responder Dialogue
                    uI.AddTextBlock(UIHelper.CreateTextblock("C2D04", "Yup.", responderRect.X - 400, responderRect.Y - 175));
                    currItem = (UITextBlock)uI.uiElements["C2D04"];
                    UIHelper.SetElementBGRect(currItem, new Rectangle((int)currItem.Position.X, (int)currItem.Position.Y, 500, 150));

                    uI.AddTextBlock(UIHelper.CreateTextblock("C2D14", "I don't know man. Probably not, things \naren't what they were and they never \nwill be.", responderRect.X - 400, responderRect.Y - 175));
                    currItem = (UITextBlock)uI.uiElements["C2D14"];
                    UIHelper.SetElementBGRect(currItem, new Rectangle((int)currItem.Position.X, (int)currItem.Position.Y, 500, 150));

                    uI.AddTextBlock(UIHelper.CreateTextblock("C2D24", "...", responderRect.X - 400, responderRect.Y - 175));
                    currItem = (UITextBlock)uI.uiElements["C2D24"];
                    UIHelper.SetElementBGRect(currItem, new Rectangle((int)currItem.Position.X, (int)currItem.Position.Y, 500, 150));

                    //uI.AddTextBlock(UIHelper.CreateTextblock("C2D34", "And I know you're going to do great things.", responderRect.X - 400, responderRect.Y - 175));
                    //currItem = (UITextBlock)uI.uiElements["C2D34"];
                    //UIHelper.SetElementBGRect(currItem, new Rectangle((int)currItem.Position.X, (int)currItem.Position.Y, 500, 150));
                    #endregion
                    break;
            }
        }
        public void UpdateAnimation(GameTime gameTime)
        {
            animManager.Update(gameTime, new Vector2(playeRect.X - 128, playeRect.Y));
        }

        public void UpdateText(GameTime gameTime, bool isMC, bool isRS)
        {
            if (isMC)
            {
                UIHelper.SetElementVisibility("C1D", false, uI.uiElements); // Set all MC boxes to false
                UIHelper.SetElementVisibility("C2D", false, uI.uiElements);
                string currMCText = "C1D" + mcText + gameLevel;
                UIHelper.SetElementVisibility(currMCText, true, uI.uiElements); // Set desired text box to true
                mcText++;
            }
            else if (isRS)
            {
                UIHelper.SetElementVisibility("C2D", false, uI.uiElements);
                UIHelper.SetElementVisibility("C1D", false, uI.uiElements);
                string currRSText = "C2D" + rsText + gameLevel;
                UIHelper.SetElementVisibility(currRSText, true, uI.uiElements);
                rsText++;
            }
        }
        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(characterTexture, playeRect, playerColor);
            spriteBatch.Draw(characterTexture, responderRect, Color.Purple);
            spriteBatch.Draw(fireTexture, new Rectangle(playeRect.X - 128, playeRect.Y, 64, 64), Color.White);
            //animManager.Draw(spriteBatch, Color.White);
        }
    }
}
