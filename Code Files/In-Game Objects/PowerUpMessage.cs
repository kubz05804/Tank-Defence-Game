using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Tank_Defence_Game
{
    public class PowerUpMessage
    {
        private Vector2 position; public Vector2 Position { get { return position; } }
        private string message = "POWER UP"; public string Message { get { return message; } }
        private bool isAvailable = false; public bool IsAvailable { get { return isAvailable; } set { isAvailable = value; } }
        

        public PowerUpMessage()
        {
            
        }

        public bool PickUp(Vector2 PlayerPosition)
        {
            if (Vector2.Distance(position, PlayerPosition) <= 150)
            {
                IsAvailable = false;
                return true;
            }

            return false;
        }

        public void NewPowerUp(Vector2 Position)
        {
            isAvailable = true;
            position = Position;
        }
    }
}
