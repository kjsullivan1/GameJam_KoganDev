using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Input;

namespace GameJam_KoganDev.Scripts.UI
{
    static class UIHelper
    {
        public static SpriteFont textFont;
        public static SpriteFont buttonFont;
        public static SpriteFont endLevelFont;
        public static SpriteFont cutSceneFont;
        public static Texture2D textBackground;
        public static Texture2D playBtnBG;

        public static UIButton CreateButton(string id, string text, int x, int y)//ButtonTexture Width and Height need to change
        {
            UIButton button = null;

            //switch (id)
            //{
            //    case "MainMenuPlay":
            //        button = new UIButton(id, new Vector2(x, y), new Vector2(400, 125), buttonFont, text, Color.White, playBtnBG);
            //        button.Disabled = false;
            //        button.TextOffset = new Vector2(100, 30);
            //        break;
            //    case "MainMenuLoad":
            //        button = new UIButton(id, new Vector2(x, y), new Vector2(400, 125), buttonFont, text, Color.White, playBtnBG);
            //        button.Disabled = false;
            //        button.TextOffset = new Vector2(100, 30);
            //        break;
            //    case "MainMenuHowTo":
            //        button = new UIButton(id, new Vector2(x,y), new Vector2(400, 125), buttonFont, text, Color.White, playBtnBG);
            //        button.Disabled = false;
            //        button.TextOffset = new Vector2(50, 30);
            //        break;

            //}
            if (id.Contains("MainMenu"))
            {
                if (id.Contains("How"))
                {
                    button = new UIButton(id, new Vector2(x, y), new Vector2(400, 125), buttonFont, text, Color.White, playBtnBG);
                    button.Disabled = false;
                    button.TextOffset = new Vector2(50, 30);
                }
                else if(id.Contains("Credits"))
                {
                    button = new UIButton(id, new Vector2(x, y), new Vector2(400, 125), buttonFont, text, Color.White, playBtnBG);
                    button.Disabled = false;
                    button.TextOffset = new Vector2(85, 30);
                }
                else
                {
                    button = new UIButton(id, new Vector2(x, y), new Vector2(400, 125), buttonFont, text, Color.White, playBtnBG);
                    button.Disabled = false;
                    button.TextOffset = new Vector2(100, 30);
                }

            }
            else if(id.Contains("MainCreditsBtn"))
            {
                button = new UIButton(id, new Vector2(x, y), new Vector2(400, 125), buttonFont, text, Color.White, playBtnBG);
                button.Disabled = false;
                button.TextOffset = new Vector2(85, 30);
            }
            else if(id.Contains("MenuCreditsBtn"))
            {
                button = new UIButton(id, new Vector2(x, y), new Vector2(400, 125), buttonFont, text, Color.White, playBtnBG);
                button.Disabled = false;
                button.TextOffset = new Vector2(100, 30);
            }
            else if(id.Contains("HowTo"))
            {
                button = new UIButton(id, new Vector2(x, y), new Vector2(400, 125), buttonFont, text, Color.White, playBtnBG);
                button.Disabled = false;
                button.TextOffset = new Vector2(100, 30);
            }
            else if(id.Contains("EndGameBtn"))
            {
                button = new UIButton(id, new Vector2(x, y), new Vector2(400, 125), buttonFont, text, Color.White, playBtnBG);
                button.Disabled = false;
                button.TextOffset = new Vector2(60, 30);
            }

                return button;
        }

        public static UITextBlock CreateTextblock(string id, string text, int x, int y)
        {
            UITextBlock textBlock = null;

            if (id.Contains("MainMenu") || id.Contains("PreLevel"))
            {
                
                textBlock = new UITextBlock(id, new Vector2(x, y), new Vector2(65, 30), textFont, text, Color.White, textBackground);
            }
            if(id.Contains("BeatLevel"))
            {
                textBlock = new UITextBlock(id, new Vector2(x, y), new Vector2(65, 30), endLevelFont, text, Color.White, textBackground);
            }

            if(id.Contains("MenuCredits"))
            {
                textBlock = new UITextBlock(id, new Vector2(x, y), new Vector2(65, 30), endLevelFont, text, Color.White, textBackground);
            }

            if(id.Contains("EndGameCredits"))
            {
                textBlock = new UITextBlock(id, new Vector2(x, y), new Vector2(65, 30), endLevelFont, text, Color.White, textBackground);
            }

            if(id.Contains("SkillSelection"))
            {
                textBlock = new UITextBlock(id, new Vector2(x, y), new Vector2(5, 5), cutSceneFont, text, Color.Black, textBackground);
            }

            if(id.Contains("HowTo"))
            {
                textBlock = new UITextBlock(id, new Vector2(x, y), new Vector2(65, -175), endLevelFont, text, Color.White, textBackground);
            }

            #region Character1 Dialogue
            if (id.Contains("C1D"))
            {
                textBlock = new UITextBlock(id, new Vector2(x, y), new Vector2(30, 18), cutSceneFont, text, Color.White, textBackground);

            }
            #endregion
            #region Character2 Dialogue
            if (id.Contains("C2D"))
            {
                textBlock = new UITextBlock(id, new Vector2(x, y), new Vector2(30, 18), cutSceneFont, text, Color.Crimson, textBackground);
            }
            #endregion
            return textBlock;
        }

        #region Helper Methods
        public static void SetButtonState(string keyWord, Boolean disabled, Dictionary<string, UIWidget> uiElements)
        {
            foreach (string widget in uiElements.Keys)
            {
                if (uiElements[widget].ID.Contains(keyWord))
                    if (uiElements[widget] is UIButton)
                        ((UIButton)uiElements[widget]).Disabled =
                       disabled;
            }
        }

        public static void SetElementVisibility(string keyWord, Boolean visible, Dictionary<string, UIWidget> uiElements)
        {
            foreach (string widget in uiElements.Keys)
            {

                if (uiElements[widget].ID.Contains(keyWord) && uiElements[widget].ID != "SettingsMenuTitle")
                    ((UIWidget)uiElements[widget]).Visible = visible;
            }


        }

        public static bool IsTextBlock(UIWidget uiElement)
        {
            if (uiElement is UITextBlock)
                return true;
            else
                return false;
        }

        public static bool IsButton(UIWidget uiElement)
        {
            if (uiElement is UIButton)
                return true;
            else
                return false;
        }

        public static void SetElementText(UIWidget uiElement, string text)
        {
            if (uiElement is UITextBlock)
                ((UITextBlock)uiElement).Text = text;
        }

        public static void SetButtonText(UIWidget uiElement, string text)
        {
            if (uiElement is UIButton)
                ((UIButton)uiElement).Text = text;
        }

        public static void SetElementRect(UIWidget uiElement, Rectangle rect)
        {
            if (uiElement is UITextBlock)
                ((UITextBlock)uiElement).Rect = rect;
        }

        public static void SetElementBGRect(UIWidget uiElement, Rectangle rect)
        {
            if (uiElement is UITextBlock)
                ((UITextBlock)uiElement).TextureRect = rect;
        }
        public static void UpdatePlayerUI(UIWidget uiElement, Rectangle bounds)
        {
            if (uiElement is UITextBlock)
            {

                ((UITextBlock)uiElement).TextureRect = new Rectangle(bounds.X, bounds.Y, ((UITextBlock)uiElement).TextureRect.Width, ((UITextBlock)uiElement).TextureRect.Height);


            }

        }

        public static void UpdateHealthBarX(UIWidget uiElement, int xPos)
        {
            if (uiElement is UITextBlock)
                ((UITextBlock)uiElement).TextureRect = new Rectangle(xPos, ((UITextBlock)uiElement).TextureRect.Y, ((UITextBlock)uiElement).TextureRect.Width, ((UITextBlock)uiElement).TextureRect.Height);
        }

        public static void UpdateHealthBarY(UIWidget uiElement, int yPos)
        {
            if (uiElement is UITextBlock)
                ((UITextBlock)uiElement).TextureRect = new Rectangle(((UITextBlock)uiElement).TextureRect.X, yPos, ((UITextBlock)uiElement).TextureRect.Width, ((UITextBlock)uiElement).TextureRect.Height);
        }

        public static Rectangle GetElementBGRect(UIWidget uiElement)
        {
            if (uiElement is UITextBlock)
                return ((UITextBlock)uiElement).TextureRect;
            else
                return Rectangle.Empty;
        }

        public static Rectangle GetElementRect(UIWidget uiElement)
        {
            if (uiElement is UITextBlock)
                return ((UITextBlock)uiElement).Rect;
            else
                return Rectangle.Empty;
        }

        public static Rectangle GetRectangle(UIWidget uiElement)
        {
            if (uiElement is UIButton)
                return ((UIButton)uiElement).Bounds;
            else
                return Rectangle.Empty;
        }

        public static void SetRectangle(UIWidget uiElement, int width, int height)
        {
            if (uiElement is UIButton)
                ((UIButton)uiElement).SetBounds(width, height);
        }

        public static void SetRectangle(UIWidget uiElement, Rectangle rectangle)
        {
            if (uiElement is UIButton)
                ((UIButton)uiElement).Bounds = rectangle;
        }
        #endregion
    }
}
