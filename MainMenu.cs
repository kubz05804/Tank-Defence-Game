using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;

namespace Tank_Defence_Game
{
    public class MainMenu
    {
        private Texture2D backgroundTexture;
        private Rectangle backgroundRectangle;

        private List<Box> boxes;
        private Box box1;
        private Box box2;
        private Box box3;

        public int VehicleSelection;
        private bool selected;
        public bool Activated;

        private SpriteFont subtitleFont;
        private string subtitle = "Choose your vehicle from the selection available below.";

        private Btn playButton;

        public MainMenu(GraphicsDevice graphicsDevice, int windowWidth, int windowHeight, Texture2D button, SpriteFont buttonFont)
        {
            Activated = false;
            selected = false;

            backgroundTexture = new Texture2D(graphicsDevice, 1, 1); backgroundTexture.SetData(new Color[] { Color.Gray });

            var backgroundWidth = (int)(windowWidth * 0.8);
            var backgroundHeight = (int)(windowHeight * 0.8);

            backgroundRectangle = new Rectangle((windowWidth - backgroundWidth) / 2, (windowHeight - backgroundHeight) / 2, backgroundWidth, backgroundHeight);

            int boxWidth = (int)(backgroundRectangle.Width * 0.2);
            int boxHeight = (int)(backgroundRectangle.Height * 0.6);
            int xSpacing = (int)(backgroundRectangle.Width * 0.1);
            int ySpacing = (int)(backgroundRectangle.Height * 0.25);

            boxes = new List<Box>();
            box1 = new Box(graphicsDevice, boxWidth, boxHeight, backgroundRectangle, backgroundRectangle.X + xSpacing, xSpacing, ySpacing, 0);
            box2 = new Box(graphicsDevice, boxWidth, boxHeight, backgroundRectangle, box1.Rectangle.X + boxWidth + xSpacing, xSpacing, ySpacing, 1);
            box3 = new Box(graphicsDevice, boxWidth, boxHeight, backgroundRectangle, box2.Rectangle.X + boxWidth + xSpacing, xSpacing, ySpacing, 2);

            boxes.Add(box1);
            boxes.Add(box2);
            boxes.Add(box3);

            box1.Click += Box1_Click;
            box2.Click += Box2_Click;
            box3.Click += Box3_Click;

            playButton = new Btn(button, buttonFont)
            {
                Position = new Vector2(windowWidth / 2, windowHeight * 0.85f),
                ButtonText = "PLAY GAME",
            };

            playButton.Click += PlayButton_Click;

            subtitleFont = buttonFont;
        }

        private void PlayButton_Click(object sender, EventArgs e)
        {
            if (playButton.Available)
                Activated = true;
        }

        private void Box3_Click(object sender, EventArgs e)
        {
            Selection(box3);
        }

        private void Box2_Click(object sender, EventArgs e)
        {
            Selection(box2);
        }

        private void Box1_Click(object sender, EventArgs e)
        {
            Selection(box1);
        }

        public void Update(GameTime gameTime)
        {
            foreach (var box in boxes)
                box.Update(gameTime);
            playButton.Update(gameTime);
        }

        public void Selection(Box selectedBox)
        {
            if (selectedBox.Colour == Box.ColourSelection)
            {
                selectedBox.Colour = Box.ColourDefault;

                playButton.Available = false;
            }
            else
            {
                selected = true;
                selectedBox.Colour = Box.ColourSelection;
                VehicleSelection = selectedBox.TankIndex;

                foreach (var box in boxes)
                {
                    if (box != selectedBox)
                        box.Colour = Box.ColourDefault;
                }

                playButton.Available = true;
            }

        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(backgroundTexture, backgroundRectangle, Color.White);
            spriteBatch.DrawString(subtitleFont, subtitle, new Vector2(
                backgroundRectangle.X + (backgroundRectangle.Width / 2) - (subtitleFont.MeasureString(subtitle).X / 2),
                backgroundRectangle.Y + (backgroundRectangle.Height * 0.2f)), Color.White);

            foreach (var box in boxes)
            {
                if (box.hovering)
                {
                    spriteBatch.Draw(box.Texture, box.Rectangle, box.Colour);
                }
                else
                {
                    spriteBatch.Draw(box.Texture, box.Rectangle, box.Colour);
                }
                spriteBatch.Draw((Texture2D)Game1.Tanks[box.TankIndex,8], box.Image, Color.White);
            }

            playButton.Draw(gameTime, spriteBatch);
        }
    }
}
