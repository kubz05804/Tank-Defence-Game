using System;
using System.Collections.Generic;
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

        public Random random = new Random();

        public static int windowWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width; // Gets the width of the screen.
        public static int windowHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height; // Gets the height of the screen.
        public static float reloadingTime; // Attribute used to keep track of the reloading time after the gun is fired.
        public static float timer;

        private string zero; // Attribute used to display the remaining reloading time with a zero in front of the fullstop, as values under 1 are displayed without the zero by default.

        public SpriteFont gameFont; // The font used in the game.

        public static List<Missile> missiles; // A list used to store the missiles currently active.
        public static List<Enemy> enemies;

        public static Player player; // Declares an instance of the player class for the player tank.
        public static Missile missile; // Declares an instance of a missile.
        public static Enemy enemy;

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

            var chassisTexture = Content.Load<Texture2D>("Textures/chassis");
            var turretTexture = Content.Load<Texture2D>("Textures/turret");
            player = new Player(chassisTexture, turretTexture) // Creates a player tank and passes in the attributes in the square brackets.
            {
                Position = new Vector2(windowWidth / 2, windowHeight / 2),
                Origin = new Vector2(chassisTexture.Width / 2, chassisTexture.Height - 80),
                ReloadTime = 3f,
                Velocity = 3f
            };

            gameFont = Content.Load<SpriteFont>("Fonts/gameFont");

            missile = new Missile(Content.Load<Texture2D>("Textures/missile")); // Creates an instance of a missile so that it can be cloned into more missiles.
            missiles = new List<Missile>(); // Creates the list of missiles declared earlier.

            enemy = new Enemy(chassisTexture, turretTexture);
            enemies = new List<Enemy>();

            // Loads sounds
            player.ShotSound = Content.Load<SoundEffect>("Audio/shotSound");
            player.ClickSound = Content.Load<SoundEffect>("Audio/click");
            player.ReloadSound = Content.Load<SoundEffect>("Audio/reload");
            player.MotionSound = Content.Load<Song>("Audio/motion");
            MediaPlayer.IsRepeating = true;

            enemy.Spawn();
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            if (!player._reloaded) // Checks if player's gun is reloaded. If not, the reloading time will increase.
            {
                reloadingTime += gameTime.ElapsedGameTime.Milliseconds;
                if (player.ReloadTime * 1000 - reloadingTime < 1000) // Checks if the reloading time is less than 1 second, so that a zero can be placed in front of remaining reloading time if it is less than 1 second.
                    zero = "0";
                else
                    zero = "";
            }

            foreach (var sprite in missiles.ToArray()) // Calls the Update method in the Missiles class for each missile in the missile list to check if they have exceeded their lifespan.
                sprite.Update(gameTime, missiles);

            player.Update(gameTime, Content.Load<Texture2D>("Textures/missile")); // Calls the Update method in the Player class.
            foreach (var enemy in enemies)
            {
                enemy.Update(gameTime, Content.Load<Texture2D>("Textures/missile"));
            }
            PostUpdate(); // Calls the PostUpdate method which removes any expired missiles.

            timer += (float)gameTime.ElapsedGameTime.TotalSeconds;
            float spawnReload = 5f;

            if (timer > spawnReload)
            {
                //enemy.Spawn();
                timer = 0;
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            spriteBatch.Begin();

            player.Draw(spriteBatch); // Calls The Draw method in the Player class, which draws the player's tank.

            foreach (var sprite in missiles) // Draws each missile by calling the Draw method in the Missile class for each missile instance.
            {
                sprite.Draw(spriteBatch);
            }
            foreach (var sprite in enemies)
            {
                sprite.Draw(spriteBatch);
            }
            if (!player._reloaded) // Checks if the player's gun is being reloaded. If so, a reloading message is displayed.
            {
                spriteBatch.DrawString(gameFont, "Reloading!", new Vector2(player.Position.X + 100, player.Position.Y - 50), Color.Red);
                spriteBatch.DrawString(gameFont, zero + (((player.ReloadTime * 1000) - reloadingTime) / 1000).ToString("#.#") + "s left", new Vector2(player.Position.X + 100, player.Position.Y - 30), Color.Red);
            }

            spriteBatch.DrawString(gameFont, "Screen Width: " + windowWidth.ToString() + ", Screen Height: " + windowHeight.ToString(), new Vector2(10, 10), Color.White);
            spriteBatch.DrawString(gameFont, "Tank Position: " + player.Position.ToString(), new Vector2(10, 30), Color.White);
            spriteBatch.DrawString(gameFont, "Cursor Position: " + Mouse.GetState().Y + " " + Mouse.GetState().X, new Vector2(10, 50), Color.White);
            spriteBatch.DrawString(gameFont, "Turret Rotation: " + Math.Round(MathHelper.ToDegrees(player.CurrentTurretAngle),1) + " " + player.CurrentTurretAngle, new Vector2(10, 70), Color.White);
            spriteBatch.DrawString(gameFont, "Turret Target Angle: " + Math.Round(MathHelper.ToDegrees(player.TargetAngle),1) + " " + player.TargetAngle, new Vector2(10, 90), Color.White);
            spriteBatch.DrawString(gameFont, "Use WASD keys to move tank", new Vector2(10, 400), Color.White);
            spriteBatch.DrawString(gameFont, "Press Esc to exit  " + Math.Abs(-180 - 90), new Vector2(10, 420), Color.White);
            spriteBatch.DrawString(gameFont, "Reloaded: " + player._reloaded, new Vector2(10, 440), Color.White);
            spriteBatch.DrawString(gameFont, "Distance to player: " + enemy.distanceToPlayer, new Vector2(10, 460), Color.White);
            spriteBatch.DrawString(gameFont, "Enemy count: " + enemies.Count, new Vector2(10, 480), Color.White);

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
        }
    }
}
