using System;
using System.Collections.Generic;
using System.Text;

namespace Tank_Defence_Game
{
    public class PowerUpStack
    {
        public string[] Content = new string[2];
        public int Top = -1;

        public bool Empty()
        {
            if (Top == -1)
                return true;
            return false;
        }

        public bool Full()
        {
            if (Content.Length == 2)
                return true;
            return false;
        }

        public void Push(string powerup)
        {
            if (!Full())
            {
                Top++;
                Content[Top] = powerup;
            }
            else
                return;
        }

        public string Pop()
        {
            string PowerUpUsed;
            if (!Empty())
            {
                PowerUpUsed = Content[Top];
                Content[Top] = null;
                Top--;
                return PowerUpUsed;
            }

            return null;
        }

        public string NextPowerUp()
        {
            return Content[Top];
        }
    }
}
