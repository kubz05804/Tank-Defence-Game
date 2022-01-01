using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Tank_Defence_Game
{
    public class Box
    {
        public Texture2D Texture;
        public Rectangle Rectangle;
        public Rectangle Image;
        public int TankIndex;
        public event EventHandler Click;

        private MouseState currentMouseState;
        private MouseState previousMouseState;
        public bool hovering;

        public Color Colour;

        public static Color ColourSelection = Color.DarkKhaki;
        public static Color ColourDefault = Color.Gray;
        public static Color ColourHovering = Color.DimGray;

        public Box(GraphicsDevice graphicsDevice, int width, int height, Rectangle container, int xPosition, int xSpacing, int ySpacing, int tankIndex)
        {
            Colour = ColourDefault;
            Texture = new Texture2D(graphicsDevice, 1, 1);
            Texture.SetData(new Color[] { Color.White });

            Rectangle = new Rectangle(xPosition, container.Y + ySpacing, width, height);
            Image = new Rectangle(Rectangle.X + 10, Rectangle.Y + 10, Rectangle.Width - 20, (int)(Rectangle.Height * 0.3f));

            TankIndex = tankIndex;
        }

        public void Update(GameTime gameTime)
        {
            previousMouseState = currentMouseState;
            currentMouseState = Mouse.GetState();

            var mouseRectangle = new Rectangle(currentMouseState.X, currentMouseState.Y, 1, 1);

            if (Colour != ColourSelection)
                Colour = ColourDefault;
            
            if (mouseRectangle.Intersects(Rectangle))
            {
                if (Colour == ColourSelection)
                    Colour = ColourSelection;
                else
                    Colour = ColourHovering;

                if (currentMouseState.LeftButton == ButtonState.Released && previousMouseState.LeftButton == ButtonState.Pressed)
                {
                    Click?.Invoke(this, new EventArgs());
                }
            }
        }
    }
}
