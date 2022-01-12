using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Tank_Defence_Game
{
    public class Btn
    {
        private MouseState currentMouseState;
        private MouseState previousMouseState;
        private bool hovering;
        private Texture2D buttonTexture;
        private SpriteFont font;
        public string ButtonText;
        public Vector2 Position;
        public event EventHandler Click;
        public bool Clicked;
        public bool Available;

        public Btn(Texture2D texture, SpriteFont Font)
        {
            buttonTexture = texture; font = Font;
            Available = false;
        }

        public Rectangle ButtonBounds
        {
            get
            {
                return new Rectangle((int)Position.X - buttonTexture.Width / 2, (int)Position.Y - buttonTexture.Height / 2, buttonTexture.Width, buttonTexture.Height);
            }
        }

        public void Update(GameTime gameTime)
        {
            previousMouseState = currentMouseState;
            currentMouseState = Mouse.GetState();

            var mouseRectangle = new Rectangle(currentMouseState.X, currentMouseState.Y, 1, 1);

            hovering = false;

            if (mouseRectangle.Intersects(ButtonBounds))
            {
                hovering = true;

                if (currentMouseState.LeftButton == ButtonState.Released && previousMouseState.LeftButton == ButtonState.Pressed)
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

                if (!string.IsNullOrEmpty(ButtonText))
                {
                    spriteBatch.DrawString(font, ButtonText, new Vector2(
                        (ButtonBounds.X + (ButtonBounds.Width / 2)) - (font.MeasureString(ButtonText).X / 2),
                        (ButtonBounds.Y + (ButtonBounds.Height / 2)) - (font.MeasureString(ButtonText).Y / 2)),
                        Color.White);
                }
            }
        }
    }
}