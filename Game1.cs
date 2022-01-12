using System;
using System.Collections.Generic;
using System.Threading;
using Tank_Defence_Game.Objects;
using Tank_Defence_Game.Controls;
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

        public Enemy EnemyTank;

        public static int windowWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width; // Gets the width of the screen.
        public static int windowHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height; // Gets the height of the screen.

        public static object[,] Tanks = new object[,]
        {   // Model/Name    | Type   | Country | Year | HP | FP | Speed | Turret Spacing | Reload Time |
            { "Cruiser IV"   ,"Light" ,"Britain","1941",400  ,100 ,4.5f  ,110              ,2.5          , null , "Light"},
            { "M4A3E8 'Fury'","Medium","USA"    ,"1940",600  ,150 ,4.0f  ,44              ,3.0          , null , "Medium"},
            { "Churchill VII","Heavy" ,"Britain","1942",1000 ,200 ,2.0f  ,150              ,3.5          , null , "Heavy"},
            { "Pz. IV H"     ,"Medium","Germany","1939",400  ,120 ,3.5f  ,60              ,3.5          , null , "Medium"}
        };

        public bool GameRunning;
        public bool PlayerDefeated;
        Btn playButton;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }
        
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

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
                Tanks[i, 9] = Content.Load<Texture2D>("Textures/" + Tanks[i,0] + "/profile");
            }

            //playButton = new Btn(Content.Load<Texture2D>("Textures/button"), Content.Load<SpriteFont>("Fonts/File"))
            //{
            //    Position = new Vector2(windowWidth / 2, windowHeight * 0.85f),
            //    ButtonText = "PLAY GAME",
            //};

            //playButton.Click += PlayButton_Click;

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

            //EnemyTank = new Enemy(Content.Load<Texture2D>("Textures/" + Tanks[3,0] + " chassis"), Content.Load<Texture2D>("Textures/" + Tanks[3,0] + " turret"), (float)Tanks[3,6], (int)Tanks[3,4]);
            //EnemyTank = new Enemy(Content.Load<Texture2D>("Textures/Pz. IV H chassis"), Content.Load<Texture2D>("Textures/Pz. IV H turret"), Content.Load<SpriteFont>("Fonts/File"), 3.5f, 400);

            mainGame = new MainGame(windowWidth, windowHeight,
                Content.Load<SpriteFont>("Fonts/File"),
                Content.Load<SpriteFont>("Fonts/File"),
                Content.Load<SpriteFont>("Fonts/GameOver"),
                Content.Load<Texture2D>("Textures/" + Tanks[mainMenu.VehicleSelection,0] + "/chassis"),
                Content.Load<Texture2D>("Textures/" + Tanks[mainMenu.VehicleSelection,0] + "/turret"),
                Content.Load<Texture2D>("Textures/" + Tanks[3,0] + "/chassis"),
                Content.Load<Texture2D>("Textures/" + Tanks[3,0] + "/turret"),
                (int)Tanks[mainMenu.VehicleSelection,7], Content.Load<Texture2D>("Textures/missile"), Tanks, spriteBatch, mainMenu.VehicleSelection);


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
                    mainGame.Update(gameTime);
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


                //spriteBatch.DrawString(gameFont, "Screen Width: " + windowWidth.ToString() + ", Screen Height: " + windowHeight.ToString(), new Vector2(10, 10), Color.White);
                //spriteBatch.DrawString(gameFont, "Tank Position: " + player.Position.ToString(), new Vector2(10, 30), Color.White);
                //spriteBatch.DrawString(gameFont, "Cursor Position: " + Mouse.GetState().X + " " + Mouse.GetState().Y, new Vector2(10, 50), Color.White);
                //spriteBatch.DrawString(gameFont, "Turret Rotation: " + Math.Round(MathHelper.ToDegrees(player.CurrentTurretAngle), 1) + " " + player.CurrentTurretAngle, new Vector2(10, 70), Color.White);
                //spriteBatch.DrawString(gameFont, "Turret Target Angle: " + Math.Round(MathHelper.ToDegrees(player.TargetAngle), 1) + " " + player.TargetAngle, new Vector2(10, 90), Color.White);
                //spriteBatch.DrawString(gameFont, "Use WASD keys to move tank", new Vector2(10, 400), Color.White);
                //spriteBatch.DrawString(gameFont, "Press Esc to exit  " + Math.Abs(-180 - 90), new Vector2(10, 420), Color.White);
                //spriteBatch.DrawString(gameFont, "Reloaded: " + player._reloaded, new Vector2(10, 440), Color.White);
                //spriteBatch.DrawString(gameFont, "Distance to player: " + enemy.distanceToPlayer, new Vector2(10, 460), Color.White);
                //spriteBatch.DrawString(gameFont, "Enemy count: " + enemies.Count, new Vector2(10, 480), Color.White);
                ////if (player.Bounds(player.Rotation, new Vector2(Mouse.GetState().X, Mouse.GetState().Y)))
                ////{
                ////    spriteBatch.DrawString(gameFont, "Collision!", new Vector2(10, 500), Color.Red);
                ////}
                //spriteBatch.DrawString(gameFont, "Point 1: " + player.pointTopLeft, new Vector2(10, 540), Color.White);
                //spriteBatch.DrawString(gameFont, "Point 2: " + player.pointTopRight, new Vector2(10, 560), Color.White);
                //spriteBatch.DrawString(gameFont, "Point 3: " + player.pointBottomRight, new Vector2(10, 580), Color.White);
                //spriteBatch.DrawString(gameFont, "Point 4: " + player.pointBottomLeft, new Vector2(10, 600), Color.White);

            spriteBatch.End();

            // TODO: Add your drawing code here

            base.Draw(gameTime);
        }
    }
}
