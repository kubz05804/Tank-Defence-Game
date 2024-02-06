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

        public static int windowWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width - 20; // Gets the width of the screen.
        public static int windowHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height - 100; // Gets the height of the screen.

        public static object[,] Tanks = new object[,] // 2D array containing the vehicles available in the game.
        {  
            // 0    | 1                 | 2        | 3              | 4      | 5    | 6   | 7     | 8   | 9   | 10  | 11   | 12   | 13   | 14    | 15
            { 1     , "Cruiser IV"      , "Light"  , "Britain"      , "1941" , 400  , 100 , 4.5f  , 90  , 105 , 2.5 , null , 8.5  , true , false , false },
            { 1     , "M4A3E8 'Fury'"   , "Medium" , "USA"          , "1940" , 600  , 150 , 4.0f  , 70  , 40  , 3.0 , null , 8.0  , true , false , false },
            { 1     , "T-34"            , "Medium" , "USSR"         , "1940" , 300  , 150 , 5.0f  , 100 , 100 , 3.0 , null , 8.0  , true , false , true  },
            { 1     , "Pz. IV H"        , "Medium" , "Germany"      , "1939" , 400  , 120 , 3.5f  , 80  , 60  , 3.5 , null , 6.0  , true , false , true  },
            { 1     , "T-34-85"         , "Medium" , "USSR"         , "1941" , 400  , 200 , 5.0f  , 90  , 100 , 3.0 , null , 8.0  , true , false , false },
            { 1     , "Churchill VII"   , "Heavy"  , "Britain"      , "1942" , 1000 , 200 , 2.4f  , 79  , 90  , 3.5 , null , 6.5  , true , false , false },
            { 1     , "KV-2"            , "Heavy"  , "USSR"         , "1940" , 900  , 500 , 2.0f  , 87  , 87  , 9.0 , null , 6.0  , true , true  , true  },
            { 1     , "Tiger 1"         , "Heavy"  , "Germany"      , "1942" , 1500 , 400 , 4.5f  , 90  , 90  , 6.0 , null , 9.0  , true , true  , true  },
            { 3     , "T-72"            , "Medium" , "USSR"         , "1969" , 2500 , 500 , 6.5f  , 90  , 90  , 3.5 , null , 8.0  , true , false , true  },
            { 3     , "T-84"            , "Medium" , "Ukraine"      , "1994" , 3600 , 1000, 6.0f  , 100 , 100 , 3.0 , null , 9.0  , true , false , false },
            { 3     , "M1A2 Abrams"     , "Heavy"  , "USA"          , "1992" , 6000 , 1500, 6.8f  , 90  , 90  , 4.0 , null , 10.0 , true , false , false },
            { 3     , "T-14 Armata"     , "Heavy"  , "Russia"       , "2014" , 6000 , 2000, 7.5f  , 80  , 80  , 2.0 , null , 11.0 , true , true  , true  },   

        };

        /* Comment
         * 0 = Tier
         * 1 = Model
         * 2 = Type
         * 3 = Nation of Origin
         * 4 = Year of Start of Production
         * 5 = Health Points
         * 6 = Firepower Points
         * 7 = Velocity (km/h / 10)
         * 8 = Chassis Origin Spacing
         * 9 = Turret Origin Spacing
         * 10 = Reload Time (seconds)
         * 11 = Profile Image
         * 12 = Muzzle Velocity
         * 13 = Available to Player
         * 14 = Boss
         * 15 = Available to Enemies
         */

        public static int NumOfTanks = Tanks.GetLength(0);
        public static List<int> PossibleEnemies = new List<int>();
        public static List<int> PossibleBosses = new List<int>();

        private bool gameIsRunning;

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

            gameIsRunning = false;

            for (int i = 0; i < NumOfTanks; i++)
            {
                Tanks[i, 11] = Content.Load<Texture2D>("Textures/" + Tanks[i, 1] + "/profile"); // Loads the profile image for each player vehicle and stores them in the Tanks array.
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
            gameIsRunning = true;

            // Creates a new instance of the game.
            mainGame = new MainGame(
                this,
                GraphicsDevice,
                Content,
                windowWidth,
                windowHeight,
                mainMenu.VehicleSelection,
                spriteBatch);

            // Loads the sounds.
            Sound.Click = Content.Load<SoundEffect>("Audio/click");
            Sound.Collision = Content.Load<SoundEffect>("Audio/hit");
            Sound.Destruction = Content.Load<SoundEffect>("Audio/destroy");
            Sound.EnemyShot = Content.Load<SoundEffect>("Audio/shot2");
            Sound.PlayerShot = Content.Load<SoundEffect>("Audio/shotSound");
            Sound.Reload = Content.Load<SoundEffect>("Audio/reload");
            Sound.Motion = Content.Load<Song>("Audio/motion");

            for (int i = 0; i < NumOfTanks; i++)
            {
                if ((bool)Tanks[i, 15] == true && (int)Tanks[i, 0] == (int)Tanks[mainMenu.VehicleSelection, 0] && (bool)Tanks[i, 14] == false)
                    PossibleEnemies.Add(i);
            }

            if (PossibleEnemies.Count == 0)
                PossibleEnemies.Add(NumOfTanks - 1);

            for (int i = 0; i < NumOfTanks; i++)
            {
                if ((bool)Tanks[i, 15] == true && (int)Tanks[i, 0] == (int)Tanks[mainMenu.VehicleSelection, 0] && (bool)Tanks[i, 14] == true)
                    PossibleBosses.Add(i);
            }

            if (PossibleBosses.Count == 0)
                PossibleBosses.Add(NumOfTanks - 1);

            mainMenu.Activated = false;
        }

        protected override void Update(GameTime gameTime)
        {
            //if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            //    Exit();

            if (gameIsRunning) // Checks if game is running
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

            if (gameIsRunning)
            {
                if (mainGame.PlayerDefeated)
                    backgroundColor = Color.Orange;

            }

            GraphicsDevice.Clear(backgroundColor);

            spriteBatch.Begin();

            if (!gameIsRunning)
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
