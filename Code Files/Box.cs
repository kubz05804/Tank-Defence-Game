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

        public Color Colour;

        public bool Active;

        public static Color ColourSelection = Color.DarkKhaki;
        public static Color ColourDefault = Color.Gray;
        public static Color ColourHovering = Color.DimGray;

        private MouseState currentMouseState;
        private MouseState previousMouseState;

        public Box(GraphicsDevice graphicsDevice, int width, int height, Rectangle container, int xPosition, int xSpacing, int ySpacing, int tankIndex)
        {
            Colour = ColourDefault;
            Texture = new Texture2D(graphicsDevice, 1, 1);
            Texture.SetData(new Color[] { Color.White });

            Rectangle = new Rectangle(xPosition, container.Y + ySpacing, width, height); // Establishes dimensions of the box.
            Image = new Rectangle(Rectangle.X + 10, Rectangle.Y + 10, Rectangle.Width - 20, (int)(Rectangle.Height * 0.3f)); // Establishes the dimensions of the profile image of the tank.

            TankIndex = tankIndex;

            Active = true;
        }

        public void Update(GameTime gameTime)
        {
            if (TankIndex == -1)
                Active = false;
            else
                Active = true;

            if (Active)
            {
                previousMouseState = currentMouseState;
                currentMouseState = Mouse.GetState();

                var mouseRectangle = new Rectangle(currentMouseState.X, currentMouseState.Y, 1, 1);

                if (mouseRectangle.Intersects(Rectangle)) // Checks if mouse cursor is hovering over the box.
                {
                    if (Colour != ColourSelection) // If a box is already selected, it will not change colour, even if the user hovers over it.
                        Colour = ColourHovering;

                    if (currentMouseState.LeftButton == ButtonState.Released && previousMouseState.LeftButton == ButtonState.Pressed) // Checks if the user clicks the box.
                        Click?.Invoke(this, new EventArgs());
                }
                else
                {
                    Colour = ColourDefault;
                }
            }

            
        }
    }
}
