using System;
using System.Collections.Generic;
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

        public Random random = new Random();

        public static int windowWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width; // Gets the width of the screen.
        public static int windowHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height; // Gets the height of the screen.
        public static float reloadingTime; // Attribute used to keep track of the reloading time after the gun is fired.
        public static float timer;

        private string zero; // Attribute used to display the remaining reloading time with a zero in front of the fullstop, as values under 1 are displayed without the zero by default.

        public static SpriteFont gameFont; // The font used in the game.

        public static List<Missile> missiles; // A list used to store the missiles currently active.
        public static List<Enemy> enemies;

        public static Player player; // Declares an instance of the player class for the player tank.
        public static Missile missile; // Declares an instance of a missile.
        public static Enemy enemy;

        public static bool collision;

        public static object[,] tanks = new object[,]
        {   // Model/Name    | Type   | Country | Year | HP | FP | Speed | Turret
            { "Cruiser IV"   ,"Light" ,"Britain","1941",400  ,40  ,4.5   ,44},
            { "M4A3E8 'Fury'","Medium","USA"    ,"1940",600  ,50  ,4     ,44},
            { "Churchill VII","Heavy" ,"Britain","1942",1000 ,60  ,2     ,44},
            { "Pz. IV H"     ,"Medium","Germany","1939",400  ,40  ,3.5   ,60}
        };

        public bool GameRunning;
        Btn playButton;

        public class Btn
        {
            private MouseState currentMouseState;
            private MouseState previousMouseState;
            private bool hovering;
            private Texture2D buttonTexture;
            private SpriteFont font;
            public string ButtonText;
            public Vector2 Position;
            public event EventHandler Click;
            public bool Clicked;

            public Rectangle ButtonBounds
            {
                get
                {
                    return new Rectangle((int)Position.X - buttonTexture.Width / 2, (int)Position.Y - buttonTexture.Height / 2, buttonTexture.Width, buttonTexture.Height);
                }
            }

            public Btn(Texture2D texture, SpriteFont Font)
            {
                buttonTexture = texture; font = Font;
            }

            public void Update(GameTime gameTime)
            {
                previousMouseState = currentMouseState;
                currentMouseState = Mouse.GetState();

                var mouseRectangle = new Rectangle(currentMouseState.X, currentMouseState.Y, 1, 1);

                hovering = false;

                if (mouseRectangle.Intersects(ButtonBounds))
                {
                    hovering = true;

                    if (currentMouseState.LeftButton == ButtonState.Released && previousMouseState.LeftButton == ButtonState.Pressed)
                    {
                        Click?.Invoke(this, new EventArgs());
                    }
                }
            }

            public void Draw(GameTime gametime, SpriteBatch spriteBatch)
            {
                var colour = Color.White;

                if (hovering)
                    colour = Color.Gray;

                spriteBatch.Draw(buttonTexture, ButtonBounds, colour);

                if (!string.IsNullOrEmpty(ButtonText))
                {
                    var x = (ButtonBounds.X + (ButtonBounds.Width / 2)) - (font.MeasureString(ButtonText).X / 2);
                    var y = (ButtonBounds.Y + (ButtonBounds.Height / 2)) - (font.MeasureString(ButtonText).Y / 2);

                    spriteBatch.DrawString(font, ButtonText, new Vector2(x, y), Color.White);
                }
            }
        }

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

            var chassisTexture = Content.Load<Texture2D>("Textures/m4a3e8 chassis");
            var turretTexture = Content.Load<Texture2D>("Textures/m4a3e8 turret");
            player = new Player(chassisTexture, turretTexture) // Creates a player tank and passes in the attributes in the square brackets.
            {
                Position = new Vector2(windowWidth / 2, windowHeight / 2),
                Origin = new Vector2(chassisTexture.Width / 2, chassisTexture.Height - 80),
                TurretOrigin = new Vector2(turretTexture.Width / 2, turretTexture.Height - 44), //44 - original; 44 - m4a3e8; 60 - pz iv h; 
                ReloadTime = 3f,
                Velocity = 3f
            };

            playButton = new Btn(Content.Load<Texture2D>("Textures/button"), Content.Load<SpriteFont>("Fonts/File"))
            {
                Position = new Vector2(windowWidth / 2, windowHeight / 2),
                ButtonText = "PLAY GAME",
            };

            playButton.Click += PlayButton_Click;

            gameFont = Content.Load<SpriteFont>("Fonts/gameFont");

            missile = new Missile(Content.Load<Texture2D>("Textures/missile")); // Creates an instance of a missile so that it can be cloned into more missiles.
            missiles = new List<Missile>(); // Creates the list of missiles declared earlier.

            var enemyChassis = Content.Load<Texture2D>("Textures/pz iv h chassis");
            var enemyTurret = Content.Load<Texture2D>("Textures/pz iv h turret");
            enemy = new Enemy(enemyChassis, enemyTurret, 4f, 50)
            {
                Origin = new Vector2(enemyChassis.Width / 2, enemyChassis.Height - 80),
                TurretOrigin = new Vector2(enemyTurret.Width / 2, enemyTurret.Height - 60)
            };
            enemies = new List<Enemy>();

            // Loads sounds
            player.ShotSound = Content.Load<SoundEffect>("Audio/shotSound");
            player.ClickSound = Content.Load<SoundEffect>("Audio/click");
            player.ReloadSound = Content.Load<SoundEffect>("Audio/reload");
            Missile.HitSound = Content.Load<SoundEffect>("Audio/hit");
            Tank.destroy = Content.Load<SoundEffect>("Audio/destroy");
            Tank.EnemyShotSound = Content.Load<SoundEffect>("Audio/shot2");
            player.MotionSound = Content.Load<Song>("Audio/motion");
            MediaPlayer.IsRepeating = true;
            //enemy.Spawn();

            //rectangle = new Texture2D(GraphicsDevice, 1, 1);
            //rectangle.SetData(new Color[] { Color.DarkSlateGray });
        }

        private void PlayButton_Click(object sender, EventArgs e)
        {
            GameRunning = true;
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            if (!GameRunning)
            {
                playButton.Update(gameTime);
            }
            else
            {
                if (!player._reloaded) // Checks if player's gun is reloaded. If not, the reloading time will increase.
                {
                    //reloadingTime += gameTime.ElapsedGameTime.Milliseconds;
                    if (player.ReloadTime * 1000 - player.Timer < 1000) // Checks if the reloading time is less than 1 second, so that a zero can be placed in front of remaining reloading time if it is less than 1 second.
                        zero = "0";
                    else
                        zero = "";
                }

                foreach (var sprite in missiles.ToArray()) // Calls the Update method in the Missiles class for each missile in the missile list to check if they have exceeded their lifespan.
                    sprite.Update(gameTime, missiles, player, enemies);
                foreach (var enemy in enemies)
                    enemy.Update(gameTime, Content.Load<Texture2D>("Textures/missile"));

                player.Update(gameTime, Content.Load<Texture2D>("Textures/missile")); // Calls the Update method in the Player class.

                PostUpdate(); // Calls the PostUpdate method which removes any expired missiles.

                timer += (float)gameTime.ElapsedGameTime.TotalSeconds;
                float spawnReload = 2f;

                if (timer > spawnReload && enemies.Count < 3)
                {
                    enemy.Spawn();
                    timer = 0;
                }
            }


            

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Gray);
            spriteBatch.Begin();

            if (!GameRunning)
            {
                playButton.Draw(gameTime, spriteBatch);
            }
            else
            {
                player.Draw(spriteBatch); // Calls The Draw method in the Player class, which draws the player's tank.

                foreach (var sprite in missiles) // Draws each missile by calling the Draw method in the Missile class for each missile instance.
                    sprite.Draw(spriteBatch);
                foreach (var sprite in enemies)
                    sprite.Draw(spriteBatch);


                if (!player._reloaded) // Checks if the player's gun is being reloaded. If so, a reloading message is displayed.
                {
                    spriteBatch.DrawString(gameFont, "Reloading!", new Vector2(player.Position.X + 100, player.Position.Y - 50), Color.Red);
                    spriteBatch.DrawString(gameFont, zero + (((player.ReloadTime * 1000) - player.Timer) / 1000).ToString("#.#") + "s left", new Vector2(player.Position.X + 100, player.Position.Y - 30), Color.Red);
                }

                spriteBatch.DrawString(gameFont, "Screen Width: " + windowWidth.ToString() + ", Screen Height: " + windowHeight.ToString(), new Vector2(10, 10), Color.White);
                spriteBatch.DrawString(gameFont, "Tank Position: " + player.Position.ToString(), new Vector2(10, 30), Color.White);
                spriteBatch.DrawString(gameFont, "Cursor Position: " + Mouse.GetState().X + " " + Mouse.GetState().Y, new Vector2(10, 50), Color.White);
                spriteBatch.DrawString(gameFont, "Turret Rotation: " + Math.Round(MathHelper.ToDegrees(player.CurrentTurretAngle), 1) + " " + player.CurrentTurretAngle, new Vector2(10, 70), Color.White);
                spriteBatch.DrawString(gameFont, "Turret Target Angle: " + Math.Round(MathHelper.ToDegrees(player.TargetAngle), 1) + " " + player.TargetAngle, new Vector2(10, 90), Color.White);
                spriteBatch.DrawString(gameFont, "Use WASD keys to move tank", new Vector2(10, 400), Color.White);
                spriteBatch.DrawString(gameFont, "Press Esc to exit  " + Math.Abs(-180 - 90), new Vector2(10, 420), Color.White);
                spriteBatch.DrawString(gameFont, "Reloaded: " + player._reloaded, new Vector2(10, 440), Color.White);
                spriteBatch.DrawString(gameFont, "Distance to player: " + enemy.distanceToPlayer, new Vector2(10, 460), Color.White);
                spriteBatch.DrawString(gameFont, "Enemy count: " + enemies.Count, new Vector2(10, 480), Color.White);
                //if (player.Bounds(player.Rotation, new Vector2(Mouse.GetState().X, Mouse.GetState().Y)))
                //{
                //    spriteBatch.DrawString(gameFont, "Collision!", new Vector2(10, 500), Color.Red);
                //}
                spriteBatch.DrawString(gameFont, "Point 1: " + player.pointTopLeft, new Vector2(10, 540), Color.White);
                spriteBatch.DrawString(gameFont, "Point 2: " + player.pointTopRight, new Vector2(10, 560), Color.White);
                spriteBatch.DrawString(gameFont, "Point 3: " + player.pointBottomRight, new Vector2(10, 580), Color.White);
                spriteBatch.DrawString(gameFont, "Point 4: " + player.pointBottomLeft, new Vector2(10, 600), Color.White);
            }

            

            spriteBatch.End();

            // TODO: Add your drawing code here

            base.Draw(gameTime);
        }

        private void PostUpdate() // Checks for expired missiles and removes them.
        {
            for (int i = 0; i < missiles.Count; i++)
            {
                if (missiles[i].IsRemoved)
                {
                    missiles.RemoveAt(i);
                    i++;
                }
            }

            for (int i = 0; i < enemies.Count; i++)
            {
                if (enemies[i].Health < 0)
                {
                    enemies.RemoveAt(i);
                    i++;
                    Tank.destroy.Play(volume: 0.4f, pitch: 0, pan: 0);
                }
            }
        }
    }
}
