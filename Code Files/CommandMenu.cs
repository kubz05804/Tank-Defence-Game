using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using System.Linq;

namespace Tank_Defence_Game.Code_Files
{
    public class CommandMenu
    {
        public Rectangle Box;

        private int width;
        private int height;

        private const int minHeight = 10;

        public List<CommandLine> CommandsEntered = new List<CommandLine>();
        public List<CommandLine> CommandsDisplayed = new List<CommandLine>();

        private KeyboardState currentKeyboardState;
        private KeyboardState previousKeyboardState;

        private string command;

        private bool open;
        private bool commandBeingDisplayed;
        private bool newCommandEntered;
        private bool triggerPause;

        private CommandLine commandLine;

        public CommandMenu()
        {
            command = "";
            open = false;
            commandBeingDisplayed = false;
            newCommandEntered = false;
            triggerPause = false;

            commandLine = new CommandLine();
        }

        public bool GetState()
        {
            return open;
        }

        public bool GetCommandDisplayState()
        { 
            return commandBeingDisplayed;
        }

        public void SetState(bool state)
        {
            open = state;
        }

        public bool NewCommandEntered()
        {
            return newCommandEntered;
        }

        public void SetNewCommandEntered(bool state)
        {
            newCommandEntered = state;
        }

        public bool GetPauseTriggerState()
        {
            return triggerPause;
        }

        public void SetPauseTriggerState(bool state)
        {
            triggerPause = state;
        }

        public void HandleInput()
        {
            Keys[] keys = currentKeyboardState.GetPressedKeys();

            if (keys.Count() > 0)
            {
                if (currentKeyboardState.IsKeyDown(keys[0]) && previousKeyboardState.IsKeyUp(keys[0]))
                {
                    if (keys[0].GetHashCode() >= 65 && keys[0].GetHashCode() < 90)
                    {
                        string character = keys[0].ToString().ToLower();
                        foreach (var key in keys)
                        {
                            if (key.GetHashCode() == 160 || key.GetHashCode() == 161)
                            {
                                character.ToUpper();
                                break;
                            }
                        }
                        command += character;
                    }

                    if (keys[0] == Keys.Back)
                        command = command.Remove(command.Length - 1);

                    if (keys[0] == Keys.Space)
                        command += " ";
                }
            }
            
            
        }

        public string GetCommand()
        {
            return command;
        }

        public void Update(GameTime gameTime)
        {
            previousKeyboardState = currentKeyboardState;
            currentKeyboardState = Keyboard.GetState();

            height = CommandsEntered.Count * 10 + minHeight;

            if (currentKeyboardState.IsKeyDown(Keys.Enter) && previousKeyboardState.IsKeyUp(Keys.Enter))
            {
                CommandsEntered.Add(commandLine.NewCommand(command));
                command = "";
                open = false;
                triggerPause = true;
            }

            for (int i = 0; i < CommandsDisplayed.Count; i++)
            {
                if (!CommandsDisplayed[i].GetState())
                    CommandsDisplayed.RemoveAt(i);
                else
                    CommandsDisplayed[i].Update(gameTime);
            }

            HandleInput();
        }

        public void Draw(SpriteBatch spriteBatch, SpriteFont font, int windowHeight, bool menuOpen)
        {
            if (menuOpen)
            {
                spriteBatch.DrawString(font, command + "_", new Vector2(10, windowHeight * 0.97f), Color.Black);

                for (int i = CommandsEntered.Count - 1; i >= 0; i--)
                {
                    spriteBatch.DrawString(font, CommandsEntered[i].Text, new Vector2(5, windowHeight - (CommandsEntered.Count - i) * 15 - 30), Color.Black);
                }
            }
            else
            {
                if (CommandsDisplayed.Count > 0)
                {
                    commandBeingDisplayed = true;

                    for (int i = CommandsDisplayed.Count - 1; i >= 0; i--)
                    {
                        spriteBatch.DrawString(font, CommandsDisplayed[i].Text, new Vector2(5, windowHeight - (CommandsDisplayed.Count - i) * 15 - 30), Color.Black);
                    }
                }
                else
                {
                    commandBeingDisplayed = false;
                }
            }
        }
    }
}
