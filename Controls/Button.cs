using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Text;

namespace Tank_Defence_Game.Controls
{
    public class Button : Component
    {
        private MouseState currentMouseState;
        private SpriteFont font;
        private bool hovering;
        private MouseState previousMouseState;
        private Texture2D buttonTexture;

        public event EventHandler Click;
        public bool Clicked;
        public Color PenColour;
        public Vector2 Position;
        public string ButtonText;
        public Rectangle ButtonBounds
        {
            get
            {
                return new Rectangle((int)Position.X - buttonTexture.Width / 2, (int)Position.Y - buttonTexture.Height / 2, buttonTexture.Width, buttonTexture.Height);
            }
        }
        public Button(Texture2D texture, SpriteFont spriteFont)
        {
            buttonTexture = texture;
            font = spriteFont;
        }

        public override void Update(GameTime gameTime)
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

        public override void Draw(GameTime gametime, SpriteBatch spriteBatch)
        {
            var colour = Color.White;

            if (hovering)
                colour = Color.Gray;

            spriteBatch.Draw(buttonTexture, ButtonBounds, colour);

            if (!string.IsNullOrEmpty(ButtonText))
            {
                var x = (ButtonBounds.X + (ButtonBounds.Width / 2)) - (font.MeasureString(ButtonText).X / 2);
                var y = (ButtonBounds.Y + (ButtonBounds.Height / 2)) - (font.MeasureString(ButtonText).Y / 2);

                spriteBatch.DrawString(font, ButtonText, new Vector2(x, y), Color.White);
            }
        }
    }
}
