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

        public MainGame mainGame;
        public MainMenu mainMenu;

        public static int windowWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width; // Gets the width of the screen.
        public static int windowHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height; // Gets the height of the screen.

        public static object[,] Tanks = new object[,]
        {   // Model/Name    | Type   | Country | Year | HP | FP | Speed | Origin | Turret Origin | Reload Time |
            { "Cruiser IV"   ,"Light" ,"Britain","1941",400  ,100 ,4.5f  ,90      ,105            ,2.5          , null , "Light"},
            { "M4A3E8 'Fury'","Medium","USA"    ,"1940",600  ,150 ,4.0f  ,70      ,40             ,3.0          , null , "Medium"},
            { "Churchill VII","Heavy" ,"Britain","1942",1000 ,200 ,2.0f  ,79      ,90             ,3.5          , null , "Heavy"},
            { "Pz. IV H"     ,"Medium","Germany","1939",400  ,120 ,3.5f  ,60      ,90             ,3.5          , null , "Medium"}
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
            graphics.IsFullScreen = false; // Sets the game to full screen mode.
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
                Tanks[i, 10] = Content.Load<Texture2D>("Textures/" + Tanks[i,0] + "/profile");
            }

            LoadMenu();
        }

        private void LoadMenu()
        {
            mainMenu = new MainMenu(GraphicsDevice, windowWidth, windowHeight, Content.Load<Texture2D>("Textures/button"), Content.Load<SpriteFont>("Fonts/default"), Content.Load<SpriteFont>("Fonts/subtitle"), Content.Load<SpriteFont>("Fonts/title"));
        }

        private bool GameActivated()
        {
            if (mainMenu.Activated)
                return true;
            else
                return false;
        }

        private void LaunchGame()
        {
            GameRunning = true;
            PlayerDefeated = false;

            mainGame = new MainGame(
                this, GraphicsDevice,
                windowWidth,
                windowHeight,
                (int)Tanks[mainMenu.VehicleSelection, 7],
                (int)Tanks[mainMenu.VehicleSelection, 8],
                Content.Load<SpriteFont>("Fonts/File"),
                Content.Load<SpriteFont>("Fonts/File"),
                Content.Load<SpriteFont>("Fonts/GameOver"),
                Content.Load<Texture2D>("Textures/" + Tanks[mainMenu.VehicleSelection,0] + "/chassis"),
                Content.Load<Texture2D>("Textures/" + Tanks[mainMenu.VehicleSelection,0] + "/turret"),
                Content.Load<Texture2D>("Textures/" + Tanks[3,0] + "/chassis"),
                Content.Load<Texture2D>("Textures/" + Tanks[3,0] + "/turret"),
                Content.Load<Texture2D>("Textures/missile"),
                Content.Load<Texture2D>("Textures/button"),
                Tanks, spriteBatch, mainMenu.VehicleSelection);


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

            if (GameRunning)
            {
                if (!mainGame.PlayerDefeated)
                {
                    mainGame.Update(gameTime);
                    if (mainGame.Restart)
                    {
                        mainGame.Restart = false;
                        LoadContent();
                    }
                }
            }
            else
            {
                mainMenu.Update(gameTime);
                if (GameActivated())
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
                    backgroundColor = Color.OrangeRed;

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
