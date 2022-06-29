using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;


namespace Tank_Defence_Game.Code_Files
{
    public class CommandLine : ICloneable
    {
        private int timer;

        public string Text;

        private bool active;

        public CommandLine()
        {
            active = true;
            timer = 0;
        }

        public void SetState(bool state)
        {
            active = state;
        }

        public bool GetState()
        {
            return active;
        }

        public int GetTimer()
        {
            return timer;
        }

        public void Update(GameTime gameTime)
        {
            timer += (int)gameTime.ElapsedGameTime.TotalMilliseconds;

            if (timer > 5000)
            {
                timer = 0;
                active = false;
            }
        }

        public CommandLine NewCommand(string text)
        {
            var command = Clone() as CommandLine;
            command.Text = text;
            return command;
        }


        public object Clone()
        {
            return this.MemberwiseClone();
        }
    }
}
