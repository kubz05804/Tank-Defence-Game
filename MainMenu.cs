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
        public int TabIndex;
        private bool selected;
        public bool Activated;

        public SpriteFont SubtitleFont;
        public SpriteFont TitleFont;
        public SpriteFont DefaultFont;
        private string subtitle = "Choose your vehicle from the selection available below.";

        private Btn playButton;

        private KeyboardState currentKeyboardState;
        private KeyboardState previousKeyboardState;

        public MainMenu(GraphicsDevice graphicsDevice, int windowWidth, int windowHeight, Texture2D button, SpriteFont defaultFont, SpriteFont subtitleFont, SpriteFont titleFont)
        {
            Activated = false;
            selected = false;

            backgroundTexture = new Texture2D(graphicsDevice, 1, 1); backgroundTexture.SetData(new Color[] { Color.Gray });

            var backgroundWidth = (int)(windowWidth * 0.95);
            var backgroundHeight = (int)(windowHeight * 0.9);

            backgroundRectangle = new Rectangle((windowWidth - backgroundWidth) / 2, (windowHeight - backgroundHeight) / 2, backgroundWidth, backgroundHeight);

            int boxWidth = (int)(backgroundRectangle.Width * 0.2);
            int boxHeight = (int)(backgroundRectangle.Height * 0.7);
            int xSpacing = (int)(backgroundRectangle.Width * 0.1);
            int ySpacing = (int)(backgroundRectangle.Height * 0.15);

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

            playButton = new Btn(button, subtitleFont)
            {
                Position = new Vector2(windowWidth / 2, windowHeight * 0.85f),
                ButtonText = "PLAY GAME",
                Available = false,
            };

            playButton.Click += PlayButton_Click;

            SubtitleFont = subtitleFont;
            TitleFont = titleFont;
            DefaultFont = defaultFont;

            TabIndex = -1;
        }

        private void PlayButton_Click(object sender, EventArgs e)
        {
            Activate();
        }

        private void Box3_Click(object sender, EventArgs e)
        {
            Selection(box3);
            TabIndex = 2;
        }

        private void Box2_Click(object sender, EventArgs e)
        {
            Selection(box2);
            TabIndex = 1;
        }

        private void Box1_Click(object sender, EventArgs e)
        {
            Selection(box1);
            TabIndex = 0;
        }

        public void Update(GameTime gameTime)
        {
            previousKeyboardState = currentKeyboardState;
            currentKeyboardState = Keyboard.GetState();

            if (currentKeyboardState.IsKeyDown(Keys.Tab) && previousKeyboardState.IsKeyUp(Keys.Tab))
                TabIndex++;

            if (TabIndex > 2)
                TabIndex = 0;

            foreach (var box in boxes)
            {
                box.Update(gameTime);
                if (TabIndex != -1)
                {
                    var selectedBox = boxes[TabIndex];
                    selectedBox.Colour = Box.ColourSelection;
                    selected = true;
                    VehicleSelection = selectedBox.TankIndex;

                    var prevIndex = TabIndex - 1;
                    if (prevIndex < 0)
                        prevIndex = 2;

                    boxes[prevIndex].Colour = Box.ColourDefault;
                    playButton.Available = true;
                }
            }

            foreach (var box in boxes)
            {
                box.Update(gameTime);
                if (TabIndex != -1)
                {
                    var selectedBox = boxes[TabIndex];
                    selectedBox.Colour = Box.ColourSelection;
                    selected = true;
                    VehicleSelection = selectedBox.TankIndex;

                    var prevIndex = TabIndex - 1;
                    if (prevIndex < 0)
                        prevIndex = 2;

                    boxes[prevIndex].Colour = Box.ColourDefault;
                    playButton.Available = true;
                }
            }

            playButton.Update(gameTime);

            if (Keyboard.GetState().IsKeyDown(Keys.Enter))
                Activate();
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

        public void Activate()
        {
            if (playButton.Available)
                Activated = true;
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(backgroundTexture, backgroundRectangle, Color.White);
            //spriteBatch.DrawString(TitleFont, subtitle, new Vector2(
            //    backgroundRectangle.X + (backgroundRectangle.Width / 2) - (SubtitleFont.MeasureString(subtitle).X / 2),
            //    backgroundRectangle.Y + (backgroundRectangle.Height * 0.2f)), Color.White);

            foreach (var box in boxes)
            {
                spriteBatch.Draw(box.Texture, box.Rectangle, box.Colour);

                spriteBatch.Draw((Texture2D)Game1.Tanks[box.TankIndex, 10], box.Image, Color.White);

                var labelCountry = "Origin Country";
                var labelSpeed = "Speed";
                var labelFirepower = "Firepower/Damage";
                var labelHealth = "Health";
                var labelType = "Type";

                var tank = Convert.ToString(Game1.Tanks[box.TankIndex, 0]);
                var country = Convert.ToString(Game1.Tanks[box.TankIndex, 2]);
                var speed = Convert.ToString((int)((float)Game1.Tanks[box.TankIndex, 6] * 10)) + " km/h";
                var firepower = Convert.ToString(Game1.Tanks[box.TankIndex, 5]);
                var health = Convert.ToString(Game1.Tanks[box.TankIndex, 4]);
                var type = Convert.ToString(Game1.Tanks[box.TankIndex, 11]);

                var textStart = (float)(box.Rectangle.Y + (box.Rectangle.Height * 0.02) + box.Image.Height);

                spriteBatch.DrawString(TitleFont, tank, new Vector2(box.Rectangle.X + (box.Rectangle.Width / 2) - (TitleFont.MeasureString(tank).X / 2), textStart), Color.White);

                spriteBatch.DrawString(SubtitleFont, labelCountry, new Vector2(box.Rectangle.X + (box.Rectangle.Width / 2) - (SubtitleFont.MeasureString(labelCountry).X / 2), textStart + 60), Color.White);
                spriteBatch.DrawString(DefaultFont, country, new Vector2(box.Rectangle.X + (box.Rectangle.Width / 2) - (DefaultFont.MeasureString(country).X / 2), textStart + 80), Color.White);
                spriteBatch.DrawString(SubtitleFont, labelSpeed, new Vector2(box.Rectangle.X + (box.Rectangle.Width / 2) - (SubtitleFont.MeasureString(labelSpeed).X / 2), textStart + 120), Color.White);
                spriteBatch.DrawString(DefaultFont, speed, new Vector2(box.Rectangle.X + (box.Rectangle.Width / 2) - (DefaultFont.MeasureString(speed).X / 2), textStart + 140), Color.White);
                spriteBatch.DrawString(SubtitleFont, labelFirepower, new Vector2(box.Rectangle.X + (box.Rectangle.Width / 2) - (SubtitleFont.MeasureString(labelFirepower).X / 2), textStart + 180), Color.White);
                spriteBatch.DrawString(DefaultFont, firepower, new Vector2(box.Rectangle.X + (box.Rectangle.Width / 2) - (DefaultFont.MeasureString(firepower).X / 2), textStart + 200), Color.White);
                spriteBatch.DrawString(SubtitleFont, labelHealth, new Vector2(box.Rectangle.X + (box.Rectangle.Width / 2) - (SubtitleFont.MeasureString(labelHealth).X / 2), textStart + 240), Color.White);
                spriteBatch.DrawString(DefaultFont, health, new Vector2(box.Rectangle.X + (box.Rectangle.Width / 2) - (DefaultFont.MeasureString(health).X / 2), textStart + 260), Color.White);
                spriteBatch.DrawString(SubtitleFont, labelType, new Vector2(box.Rectangle.X + (box.Rectangle.Width / 2) - (SubtitleFont.MeasureString(labelType).X / 2), textStart + 300), Color.White);
                spriteBatch.DrawString(DefaultFont, type, new Vector2(box.Rectangle.X + (box.Rectangle.Width / 2) - (DefaultFont.MeasureString(type).X / 2), textStart + 320), Color.White);
            }

            playButton.Draw(gameTime, spriteBatch);
        }
    }
}