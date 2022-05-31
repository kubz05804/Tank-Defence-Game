using System;
using System.Collections.Generic;
using System.Text;
using Tank_Defence_Game.Objects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;

namespace Tank_Defence_Game
{
    public class MainGame
    {
        public Random Random = new Random();

        private GraphicsDevice graphicsDevice;

        public Game1 Game;

        public static ContentManager Content;

        private Player player;
        private Enemy enemy;
        private Missile missile;
        private Enemy boss;

        private List<Missile> missiles;
        private List<Enemy> enemies;

        public bool Running;
        public bool Restart; // Indicates whether the player has chosen to restart the game.
        public bool PlayerDefeated;
        public bool Paused;

        private bool bossSpawned;
        private bool bossAlive;

        private char lastSpawnDirection;

        private PowerUpStack powerUpsInStore; // Creates a stack to store power ups available to the player (max 2).
        private PowerUpMessage powerUp; // Creates an instance of a power up message.

        private string powerUpCurrentlyInUse;

        private int playerTank;

        private SpriteFont font12;
        private SpriteFont font14;
        private SpriteFont font20;
        private SpriteFont font50;

        private SpriteBatch spriteBatch;
        
        private float timer;
        private float powerUpTimer;

        private const float initialSpawnRate = 12;
        private float currentSpawnRate;

        private Btn restartButton;
        private Btn exitButton;

        private Texture2D boxTexture;

        private int killCount;

        private KeyboardState previousKeyboardState;
        private KeyboardState currentKeyboardState;


        public MainGame(
            Game1 game,
            GraphicsDevice GraphicsDevice,
            ContentManager content,
            int windowWidth, int windowHeight, int playerTankSelection,
            SpriteBatch SpriteBatch)
        {
            Content = content;
            graphicsDevice = GraphicsDevice;
            spriteBatch = SpriteBatch;
            font12 = Content.Load<SpriteFont>("Fonts/font12");
            font14 = Content.Load<SpriteFont>("Fonts/font14");
            font20 = Content.Load<SpriteFont>("Fonts/font20");
            font50 = Content.Load<SpriteFont>("Fonts/font50");

            player = new Player(playerTankSelection);

            missile = new Missile(Content.Load<Texture2D>("Textures/missile"));
            missiles = new List<Missile>();

            enemies = new List<Enemy>();
            enemy = new Enemy(false);

            playerTank = playerTankSelection;

            restartButton = new Btn(Content.Load<Texture2D>("Textures/button"), font12, "RESTART", true, windowWidth / 2, windowHeight * 0.65f);
            exitButton = new Btn(Content.Load<Texture2D>("Textures/button"), font12, "EXIT", true, windowWidth / 2, windowHeight * 0.7f);

            restartButton.Click += RestartButton_Click;
            exitButton.Click += ExitButton_Click;

            Game = game;
            Restart = false;
            PlayerDefeated = false;
            Paused = false;

            bossSpawned = false;
            bossAlive = false;

            currentSpawnRate = initialSpawnRate;

            killCount = 0;

            powerUp = new PowerUpMessage();
            powerUpsInStore = new PowerUpStack();

            powerUpCurrentlyInUse = "None";

            boxTexture = new Texture2D(graphicsDevice, 1, 1);
            boxTexture.SetData(new Color[] { Color.White });
        }

        private void ExitButton_Click(object sender, EventArgs e)
        {
            Game.Exit();
        }

        private void RestartButton_Click(object sender, EventArgs e)
        {
            Restart = true;
        }

        public void Update(GameTime gameTime)
        {
            previousKeyboardState = currentKeyboardState;
            currentKeyboardState = Keyboard.GetState();

            if (currentKeyboardState.IsKeyDown(Keys.Space) && previousKeyboardState.IsKeyUp(Keys.Space) && powerUpCurrentlyInUse == "None")
                PowerUpEquip();

            if (currentKeyboardState.IsKeyDown(Keys.Escape) && previousKeyboardState.IsKeyUp(Keys.Escape) && !PlayerDefeated)
            {
                if (Paused)
                    Paused = false;
                else
                    Paused = true;
            }

            if (Paused)
            {
                restartButton.Update(gameTime);
                exitButton.Update(gameTime);
                return;
            }

            if (player.Health <= 0) // Checks if player is dead.
            {
                restartButton.Update(gameTime);
                exitButton.Update(gameTime);
                PlayerDefeated = true;
                Sound.MotionStop();
                return;
            }

            player.Update(gameTime, missile, missiles, player, enemies);

            foreach (var enemy in enemies)
                enemy.Update(gameTime, missile, missiles, player, enemies);
            foreach (var missile in missiles.ToArray())
                missile.Update(gameTime, missiles, player, enemies);

            SpriteExpirationCheck();

            timer += (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (killCount % 20 == 0 && killCount > 0 && !bossAlive)
            {
                enemy.Spawn(enemies, true, lastSpawnDirection);
                bossAlive = true;
            }

            if (timer > currentSpawnRate && !bossAlive || timer > 5 && enemies.Count == 0 || timer > 30 && bossAlive) 
            {
                currentSpawnRate = (float)Math.Pow(currentSpawnRate, 0.99);
                enemy.Spawn(enemies, false, lastSpawnDirection);
                timer = 0;
            }

            if (powerUp.IsAvailable)
            {
                if (powerUp.PickUp(player.Position))
                    PowerUpGeneration();
            }

            if (powerUpCurrentlyInUse != "None")
            {
                powerUpTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;

                if (powerUpTimer > 10 || powerUpTimer > 1 && powerUpCurrentlyInUse == "Health Boost") // Resets timer if it reaches 10 seconds or if the power up is a health boost.
                {
                    powerUpTimer = 0;
                    PowerUpClear();
                }
            }

        }

        public void SpriteExpirationCheck()
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
                if (enemies[i].Health <= 0)
                {
                    var position = enemies[i].Position;

                    if (enemies[i].Boss)
                    {
                        bossAlive = false;
                        player.Score += 1000;
                    }
                    else
                        player.Score += 200;

                    enemies.RemoveAt(i);
                    Sound.Destruction.Play(volume: 0.4f, pitch: 0, pan: 0);
                    killCount++;
                    i++;

                    if (killCount % 2 == 0)
                        powerUp.NewPowerUp(position);
                }
            }
        }

        public void PowerUpGeneration()
        {
            var r = Random.Next(6);
            var powerup = "";

            switch (r)
            {
                case 0: // Speed Boost
                    powerup = "Speed Boost";
                    break;
                case 1: // Firepower Boost
                    powerup = "Firepower Boost";
                    break;
                case 2: // Health Restore
                    powerup = "Health Boost";
                    break;
                case 3: // Armour Boost
                    powerup = "Armour Boost";
                    break;
                case 4: // Camouflage Net
                    powerup = "Camouflage Net";
                    break;
                case 5: // Reloading Boost
                    powerup = "Reloading Boost";
                    break;
            }

            powerUpsInStore.Push(powerup);
        }

        public void PowerUpEquip()
        {
            var nextPowerUp = powerUpsInStore.NextPowerUp();

            if (nextPowerUp == "Speed Boost")
            {
                player.Velocity *= 2;
            }
            else if (nextPowerUp == "Firepower Boost")
            {
                player.Firepower *= 4;
                if (playerTank == 4)
                    player.ReloadTime /= 4;
            }
            else if (nextPowerUp == "Health Boost")
            {
                player.Health = player.InitialHealth;
            }
            else if (nextPowerUp == "Armour Boost")
            {
                player.ArmourBoostEquipped = true;
            }
            else if (nextPowerUp == "Camouflage Net")
            {
                player.CamouflageNetEquipped = true;
            }
            else if (nextPowerUp == "Reloading Boost")
            {
                player.ReloadTime /= 2;
            }
            else
            {
                return;
            }

            powerUpCurrentlyInUse = powerUpsInStore.Pop();
        }

        public void PowerUpClear()
        {
            player.Velocity = (float)Game1.Tanks[playerTank, 6];
            player.Firepower = (int)Game1.Tanks[playerTank, 5];
            player.ArmourBoostEquipped = false;
            player.CamouflageNetEquipped = false;
            player.ReloadTime = (double)Game1.Tanks[playerTank, 9];

            powerUpCurrentlyInUse = "None";
        }

        public void Draw(GameTime gameTime)
        {
            if (powerUp.IsAvailable)
            {
                spriteBatch.Draw(boxTexture, new Rectangle((int)(powerUp.Position.X - (font12.MeasureString("POWER UP AVAILABLE").X / 2) - 20), (int)powerUp.Position.Y - 30, (int)font12.MeasureString("POWER UP AVAILABLE").X + 40, 40), Color.Orange);
                spriteBatch.DrawString(font12, "POWER UP AVAILABLE", powerUp.Position - new Vector2(font12.MeasureString("POWER UP AVAILABLE").X / 2, 20), Color.Black);
            }

            foreach (var missile in missiles)
                missile.Draw(spriteBatch);
            foreach (var enemy in enemies)
                enemy.Draw(spriteBatch, graphicsDevice);
            player.Draw(spriteBatch, graphicsDevice);

           
            var yourScore = "YOUR SCORE";

            spriteBatch.Draw(boxTexture, new Rectangle((int)(Game1.windowWidth / 2 - (font20.MeasureString(yourScore).X / 2) - 40), 0, (int)font20.MeasureString(yourScore).X + 80, 140), Color.Black);
            spriteBatch.DrawString(font20, yourScore, new Vector2(Game1.windowWidth / 2 - (font20.MeasureString(yourScore).X / 2), 20), Color.White);
            spriteBatch.DrawString(font50, Convert.ToString(player.Score), new Vector2(Game1.windowWidth / 2 - (font50.MeasureString(Convert.ToString(player.Score)).X / 2), 50), Color.White);

            if (powerUpsInStore.NextPowerUp() != "None")
            {
                spriteBatch.Draw(boxTexture, new Rectangle((int)(Game1.windowWidth / 4 - (font14.MeasureString("Power Up Available").X / 2) - 50), 0, (int)(font14.MeasureString("Power Up Available").X + 100), 120), Color.Black);
                spriteBatch.DrawString(font14, "Power Up Available", new Vector2(Game1.windowWidth / 4 - (font14.MeasureString("Power Up Available").X / 2), 20), Color.White);
                spriteBatch.DrawString(font20, powerUpsInStore.NextPowerUp(), new Vector2(Game1.windowWidth / 4 - (font20.MeasureString(powerUpsInStore.NextPowerUp()).X / 2), 50), Color.White);
                if (powerUpsInStore.Top == 1)
                    spriteBatch.DrawString(font14, powerUpsInStore.SecondaryPowerUp(), new Vector2(Game1.windowWidth / 4 - (font14.MeasureString(powerUpsInStore.SecondaryPowerUp()).X / 2), 80), Color.White);
            }
            

            if (powerUpCurrentlyInUse != "None")
            {
                spriteBatch.Draw(boxTexture, new Rectangle((int)(Game1.windowWidth / 2 - (font14.MeasureString("Power Up Currently in Use").X / 2) - 50 + Game1.windowWidth / 4), 0, (int)(font14.MeasureString("Power Up Currently in Use").X + 100), 160), Color.Black);
                spriteBatch.DrawString(font14, "Power Up Currently in Use", new Vector2(Game1.windowWidth / 2 - (font14.MeasureString("Power Up Currently in Use").X / 2) + Game1.windowWidth / 4, 20), Color.White);
                spriteBatch.DrawString(font20, powerUpCurrentlyInUse, new Vector2(Game1.windowWidth / 2 - (font20.MeasureString(powerUpCurrentlyInUse).X / 2) + Game1.windowWidth / 4, 50), Color.White);
            }

            if (powerUpCurrentlyInUse != "None" && powerUpCurrentlyInUse != "Health Boost")
            {
                var timeLeft = (10 - (powerUpTimer)).ToString("#.#") + "s";
                spriteBatch.DrawString(font50, timeLeft, new Vector2(Game1.windowWidth / 2 - (font50.MeasureString(timeLeft).X / 2) + Game1.windowWidth / 4, 70), Color.Red);
            }

            if (PlayerDefeated || Paused)
            {
                var menuMessage = "";

                spriteBatch.Draw(boxTexture, new Rectangle(Game1.windowWidth / 2 - 250, Game1.windowHeight / 2 - 20, 500, 300), Color.Black);

                if (PlayerDefeated)
                    menuMessage = "GAME OVER!";
                else
                {
                    menuMessage = "PAUSED";
                    spriteBatch.DrawString(font14, "Press ESC to unpause", new Vector2(Game1.windowWidth / 2 - (font14.MeasureString("Press ESC to unpause").X / 2), Game1.windowHeight / 2 + 90), Color.White);
                }


                spriteBatch.DrawString(font50, menuMessage, new Vector2(Game1.windowWidth / 2 - (font50.MeasureString(menuMessage).X / 2), Game1.windowHeight / 2), Color.White);

                var destroyedTally = "DESTROYED: " + killCount;
                spriteBatch.Draw(boxTexture, new Rectangle((int)(Game1.windowWidth / 2 - (font20.MeasureString(yourScore).X / 2) - 40), 140, (int)font20.MeasureString(yourScore).X + 80, 50), Color.Black);
                spriteBatch.DrawString(font14, destroyedTally, new Vector2(Game1.windowWidth / 2 - (font14.MeasureString(destroyedTally).X / 2), 140), Color.White);

                restartButton.Draw(gameTime, spriteBatch);
                exitButton.Draw(gameTime, spriteBatch);
            }
            else
            {
                spriteBatch.DrawString(font14, "KILL COUNT: " + killCount, new Vector2(10, 10), Color.Black);
            }

            spriteBatch.DrawString(font12, "v1.2 beta", new Vector2(Game1.windowWidth * 0.97f, Game1.windowHeight * 0.97f), Color.Black);
            spriteBatch.DrawString(font12, "Your tank: " + Game1.Tanks[playerTank, 0], new Vector2(10, Game1.windowHeight * 0.97f), Color.Black);
        }
    }
}
