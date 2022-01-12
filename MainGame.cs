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

        private SpriteBatch spriteBatch;
        
        private float timer;

        private Btn restartButton;
        private Btn exitButton;

        private bool playerDefeated; public bool PlayerDefeated { get; set; }


        public MainGame(int windowWidth, int windowHeight, SpriteFont healthFont, SpriteFont reloadingFont, SpriteFont gameOverFont, Texture2D playerChassis, Texture2D playerTurret, Texture2D enemyChassis, Texture2D enemyTurret, int turretSpacing, Texture2D missileTexture, object[,] Tanks, SpriteBatch SpriteBatch, int playerTank)
        {
            spriteBatch = SpriteBatch;
            HealthFont = healthFont; ReloadingFont = reloadingFont; GameOverFont = gameOverFont;

            Player = new Player(playerChassis, playerTurret, HealthFont, playerTank, ReloadingFont, spriteBatch)
            {
                Position = new Vector2(windowWidth / 2, windowHeight / 2),
                Origin = new Vector2(playerChassis.Width / 2, playerChassis.Height - 100),
                TurretOrigin = new Vector2(playerTurret.Width / 2, playerTurret.Height - turretSpacing),
            };


            Missile = new Missile(missileTexture);
            Missiles = new List<Missile>();

            Enemies = new List<Enemy>();
            EnemyTank = new Enemy(enemyChassis, enemyTurret, HealthFont, 3);

            playerDefeated = false;
        }

        public void Update(GameTime gameTime)
        {
            if (Player.Health <= 0)
            {
                playerDefeated = true;
                return;
            }

            Player.Update(gameTime, Missile, Missiles, Player, Enemies);

            foreach (var missile in Missiles.ToArray())
                missile.Update(gameTime, Missiles, Player, Enemies);
            foreach (var enemy in Enemies)
                enemy.Update(gameTime, Missile, Missiles, Player, Enemies);

            SpriteExpirationCheck();

            timer += (float)gameTime.ElapsedGameTime.TotalSeconds;
            float spawnReload = 2f;

            if (timer > spawnReload && Enemies.Count < 3)
            {
                EnemyTank.Spawn(Enemies);
                timer = 0;
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
                    i++;
                }
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
                spriteBatch.DrawString(GameOverFont, gameOverMessage, new Vector2(Game1.windowWidth / 2 - (GameOverFont.MeasureString(gameOverMessage).X / 2), Game1.windowHeight / 2), Color.Red);
            }
        }
    }
}
