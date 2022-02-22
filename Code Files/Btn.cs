using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;

namespace Tank_Defence_Game
{
    public class Btn
    {
        public event EventHandler Click;

        public bool Clicked;
        public bool Available;

        private string buttonText;
        private Vector2 position;

        private MouseState currentMouseState;
        private MouseState previousMouseState;

        private Texture2D buttonTexture;
        private SpriteFont font;

        private bool hovering;
        private Rectangle mouseRectangle;

        public Btn(Texture2D texture, SpriteFont buttonFont, string text, bool availability, float x, float y)
        {
            buttonTexture = texture;
            font = buttonFont;
            position = new Vector2(x, y);
            buttonText = text;
            Available = availability;
        }

        public Rectangle ButtonBounds
        {
            get
            {
                return new Rectangle((int)position.X - buttonTexture.Width / 2, (int)position.Y - buttonTexture.Height / 2, buttonTexture.Width, buttonTexture.Height); // Establishes dimensions of the button.
            }
        }

        public void Update(GameTime gameTime)
        {
            previousMouseState = currentMouseState;
            currentMouseState = Mouse.GetState();

            mouseRectangle = new Rectangle(currentMouseState.X, currentMouseState.Y, 1, 1);

            hovering = false;

            if (mouseRectangle.Intersects(ButtonBounds)) // Checks if the mouse cursor is hovering over the button.
            {
                hovering = true;

                if (currentMouseState.LeftButton == ButtonState.Released && previousMouseState.LeftButton == ButtonState.Pressed) // Checks if the user has clicked the button.
                {
                    Click?.Invoke(this, new EventArgs());
                }
            }
        }

        public void Draw(GameTime gametime, SpriteBatch spriteBatch)
        {
            if (Available)
            {
                var colour = Color.White;

                if (hovering)
                    colour = Color.Gray;

                spriteBatch.Draw(buttonTexture, ButtonBounds, colour);

                if (!string.IsNullOrEmpty(buttonText)) // Checks if button text is empty.
                {
                    spriteBatch.DrawString(font, buttonText, new Vector2(
                        (ButtonBounds.X + (ButtonBounds.Width / 2)) - (font.MeasureString(buttonText).X / 2),
                        (ButtonBounds.Y + (ButtonBounds.Height / 2)) - (font.MeasureString(buttonText).Y / 2)),
                        Color.White); // Aligns button text in the middle of the button.
                }
            }
        }
    }
}