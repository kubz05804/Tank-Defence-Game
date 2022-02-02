﻿using System;
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

        public Game1 Game;

        public static SpriteFont HealthFont;
        public SpriteFont ReloadingFont;
        public SpriteFont GameOverFont;
        public ContentManager Content;

        public Player Player;
        public Enemy EnemyTank;
        public Missile Missile;

        public List<Missile> Missiles;
        public List<Enemy> Enemies;

        public Texture2D enemyChassis;
        public Texture2D enemyTurret;

        public bool Restart;

        public PowerUpStack PowerUpsInStore;

        private bool firstEnemy;

        private SpriteBatch spriteBatch;
        
        private float timer;
        private const float enemySpawnRate = 12;
        private float currentSpawnRate;

        private Btn restartButton;
        private Btn exitButton;

        private int killCount;

        private bool playerDefeated; public bool PlayerDefeated { get; set; }

        public MainGame(
            Game1 game, GraphicsDevice graphicsDevice,
            int windowWidth, int windowHeight, int origin, int turretOrigin,
            SpriteFont healthFont, SpriteFont reloadingFont, SpriteFont gameOverFont,
            Texture2D playerChassis, Texture2D playerTurret, Texture2D enemyChassis, Texture2D enemyTurret, Texture2D missileTexture, Texture2D buttonTexture,
            object[,] Tanks, SpriteBatch SpriteBatch, int playerTank)
        {
            spriteBatch = SpriteBatch;
            HealthFont = healthFont; ReloadingFont = reloadingFont; GameOverFont = gameOverFont;

            Player = new Player(playerChassis, playerTurret, HealthFont, playerTank, ReloadingFont, spriteBatch)
            {
                Position = new Vector2(windowWidth / 2, windowHeight / 2),
                Origin = new Vector2(playerChassis.Width / 2, playerChassis.Height - origin),
                TurretOrigin = new Vector2(playerTurret.Width / 2, playerTurret.Height - turretOrigin),
            };

            Missile = new Missile(missileTexture);
            Missiles = new List<Missile>();

            Enemies = new List<Enemy>();
            EnemyTank = new Enemy(enemyChassis, enemyTurret, HealthFont, 3);

            playerDefeated = false;

            restartButton = new Btn(buttonTexture, healthFont)
            {
                Position = new Vector2(windowWidth / 2, windowHeight * 0.65f),
                ButtonText = "RESTART",
                Available = true,
            };

            exitButton = new Btn(buttonTexture, healthFont)
            {
                Position = new Vector2(windowWidth / 2, windowHeight * 0.70f),
                ButtonText = "EXIT",
                Available = true,
            };

            restartButton.Click += RestartButton_Click;
            exitButton.Click += ExitButton_Click;

            Game = game;
            Restart = false;
            firstEnemy = true;

            currentSpawnRate = enemySpawnRate;

            killCount = 0;
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
            if (Player.Health <= 0)
            {
                restartButton.Update(gameTime);
                exitButton.Update(gameTime);
                playerDefeated = true;
                Sound.MotionStop();
                return;
            }

            Player.Update(gameTime, Missile, Missiles, Player, Enemies);

            foreach (var enemy in Enemies)
                enemy.Update(gameTime, Missile, Missiles, Player, Enemies);
            foreach (var missile in Missiles.ToArray())
                missile.Update(gameTime, Missiles, Player, Enemies);

            SpriteExpirationCheck();

            timer += (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (timer > currentSpawnRate || timer > 5 && firstEnemy)
            {
                currentSpawnRate = (float)Math.Pow(currentSpawnRate, 0.99);
                EnemyTank.Spawn(Enemies);
                timer = 0;
                if (firstEnemy)
                    firstEnemy = false;
            }

        }

        public void SpriteExpirationCheck()
        {
            for (int i = 0; i < Missiles.Count; i++)
            {
                if (Missiles[i].IsRemoved)
                {
                    Missiles.RemoveAt(i);
                    i++;
                }
            }

            for (int i = 0; i < Enemies.Count; i++)
            {
                if (Enemies[i].Health <= 0)
                {
                    Enemies.RemoveAt(i);
                    Sound.Destruction.Play(volume: 0.4f, pitch: 0, pan: 0);
                    Player.Score += 200;
                    killCount++;
                    i++;
                }
            }
        }

        public void PowerUpGeneration()
        {
            var r = Random.Next(1, 5);

            if (PowerUpsInStore.Full())
            {
                return;
            }

            switch (r)
            {
                case 1: // Speed Boost
                    PowerUpsInStore.Push("Speed Boost");
                    break;
                case 2: // Firepower Boost
                    PowerUpsInStore.Push("Firepower Boost");
                    break;
                case 3: // Health Restore
                    PowerUpsInStore.Push("Health Boost");
                    break;
                case 4: // Armour Boost
                    PowerUpsInStore.Push("Armour Boost");
                    break;
                case 5: // Camouflage Net
                    PowerUpsInStore.Push("Camouflage Net");
                    break;
            }
        }

        public void Draw(GameTime gameTime)
        {
            foreach (var missile in Missiles)
                missile.Draw(spriteBatch);
            foreach (var enemy in Enemies)
                enemy.Draw(spriteBatch);
            Player.Draw(spriteBatch);
            Player.DrawHealth(spriteBatch);
            foreach (var enemy in Enemies)
                enemy.DrawHealth(spriteBatch);

            if (playerDefeated)
            {
                var gameOverMessage = "GAME OVER";
                spriteBatch.DrawString(GameOverFont, gameOverMessage, new Vector2(Game1.windowWidth / 2 - (GameOverFont.MeasureString(gameOverMessage).X / 2), Game1.windowHeight / 2 - 100), Color.Red);

                restartButton.Draw(gameTime, spriteBatch);
                exitButton.Draw(gameTime, spriteBatch);
            }

            var yourScore = "Your score";
            spriteBatch.DrawString(GameOverFont, yourScore, new Vector2(Game1.windowWidth / 2 - (GameOverFont.MeasureString(yourScore).X / 2), 20), Color.White);
            spriteBatch.DrawString(GameOverFont, Convert.ToString(Player.Score), new Vector2(Game1.windowWidth / 2 - (GameOverFont.MeasureString(Convert.ToString(Player.Score)).X / 2), 100), Color.White);
            spriteBatch.DrawString(HealthFont, "Power Up Available", new Vector2(Game1.windowWidth / 4 - (HealthFont.MeasureString("Power Up Available").X / 2), 20), Color.White);
            spriteBatch.DrawString(HealthFont, "None", new Vector2(Game1.windowWidth / 4 - (HealthFont.MeasureString("None").X / 2), 50), Color.White);
            spriteBatch.DrawString(HealthFont, "Power Up Currently in Use", new Vector2(Game1.windowWidth / 2 - (HealthFont.MeasureString("Power Up Currently in Use").X / 2) + Game1.windowWidth / 4, 20), Color.White);
            spriteBatch.DrawString(HealthFont, "None", new Vector2(Game1.windowWidth / 2 - (HealthFont.MeasureString("None").X / 2) + Game1.windowWidth / 4, 50), Color.White);
        }
    }
}
