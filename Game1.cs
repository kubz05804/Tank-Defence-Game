using System;
using System.Collections.Generic;
using System.Threading;
using Tank_Defence_Game.Objects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Audio;

namespace Tank_Defence_Game
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;

        private MainGame mainGame;
        private MainMenu mainMenu;

        public static int windowWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width; // Gets the width of the screen.
        public static int windowHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height; // Gets the height of the screen.

        public static object[,] Tanks = new object[,] // 2D array containing the vehicles available in the game.
        {   // Model/Name | Type | Country | Year | HP | FP | Speed | Origin Spacing | Turret Origin Spacing | Reload Time | Profile Image | Type
            { "Cruiser IV"   ,"Light" ,"Britain","1941",400  ,100 ,4.5f  ,90      ,105            ,2.5          , null , "Light"},
            { "M4A3E8 'Fury'","Medium","USA"    ,"1940",600  ,150 ,4.0f  ,70      ,40             ,3.0          , null , "Medium"},
            { "Churchill VII","Heavy" ,"Britain","1942",1000 ,200 ,2.0f  ,79      ,90             ,3.5          , null , "Heavy"},
            { "Pz. IV H"     ,"Medium","Germany","1939",400  ,120 ,3.5f  ,80      ,60             ,3.5          , null , "Medium"} // 60 90
        };

        public bool GameRunning;
        public bool PlayerDefeated;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }
        
        protected override void Initialize()
        {
            MediaPlayer.Volume = 0.07f; // Sets the engine sound volume (MotionSound).
            graphics.PreferredBackBufferWidth = windowWidth;
            graphics.PreferredBackBufferHeight = windowHeight;
            graphics.IsFullScreen = false;
            graphics.ApplyChanges();

            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            GameRunning = false;
            PlayerDefeated = false;

            for (int i = 0; i <= 2; i++)
            {
                Tanks[i, 10] = Content.Load<Texture2D>("Textures/" + Tanks[i,0] + "/profile"); // Loads the profile image for each player vehicle and stores them in the Tanks array.
            }

            LoadMenu(); // Creates an instance of the main menu.
        }

        private void LoadMenu()
        {
            mainMenu = new MainMenu(
                Content,
                GraphicsDevice,
                windowWidth, windowHeight,
                Content.Load<Texture2D>("Textures/button"),
                Content.Load<SpriteFont>("Fonts/font10"),
                Content.Load<SpriteFont>("Fonts/font12"),
                Content.Load<SpriteFont>("Fonts/font14"),
                Content.Load<SpriteFont>("Fonts/font20"));
        }

        private bool GameActivated()
        {
            if (mainMenu.Activated) // Checks if the game has been activated from the main menu.
                return true;
            else
                return false;
        }

        private void LaunchGame()
        {
            GameRunning = true;
            PlayerDefeated = false;

            // Creates a new instance of the game.
            mainGame = new MainGame(
                this,
                Content,
                windowWidth,
                windowHeight,
                mainMenu.VehicleSelection,
                spriteBatch);

            // Loads the sounds.
            Sound.Click = Content.Load<SoundEffect>("Audio/Click");
            Sound.Collision = Content.Load<SoundEffect>("Audio/hit");
            Sound.Destruction = Content.Load<SoundEffect>("Audio/destroy");
            Sound.EnemyShot = Content.Load<SoundEffect>("Audio/shot2");
            Sound.PlayerShot = Content.Load<SoundEffect>("Audio/shotSound");
            Sound.Reload = Content.Load<SoundEffect>("Audio/reload");
            Sound.Motion = Content.Load<Song>("Audio/motion");

            mainMenu.Activated = false;
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            if (GameRunning) // Checks if game is running
            {
                mainGame.Update(gameTime);
                if (mainGame.Restart) // Checks if the Restart option has been chosen.
                {
                    mainGame.Restart = false;
                    LoadContent(); // If the player chose to restart the game, LoadContent() will be called, resetting already existent attributes. 
                }
            }
            else // If game is not running, the Update() method will be called in the mainMenu.
            {
                mainMenu.Update(gameTime); 
                if (GameActivated()) // Checks if the game has been activated from the main menu.
                    LaunchGame();
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            var backgroundColor = Color.LightGray;

            if (mainGame != null && GameRunning)
            {
                if (mainGame.PlayerDefeated)
                    backgroundColor = Color.Orange;

            }

            GraphicsDevice.Clear(backgroundColor);

            spriteBatch.Begin();

            if (!GameRunning)
                mainMenu.Draw(gameTime, spriteBatch);
            else
                mainGame.Draw(gameTime);

            spriteBatch.End();

            base.Draw(gameTime);
        }

        public void Quit()
        {
            this.Exit();
        }
    }
}
