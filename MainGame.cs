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
        public static SpriteFont HealthFont;
        public SpriteFont ReloadingFont;
        public ContentManager Content;

        public Player Player;
        public Enemy EnemyTank;
        public Missile Missile;

        public List<Missile> Missiles;
        public List<Enemy> Enemies;

        public Texture2D enemyChassis;
        public Texture2D enemyTurret;

        private SpriteBatch spriteBatchMainGame;
        
        private float timer;

        public MainGame(int windowWidth, int windowHeight, SpriteFont healthFont, SpriteFont reloadingFont, Texture2D playerChassis, Texture2D playerTurret, int turretSpacing, Texture2D missileTexture, object[,] Tanks, Enemy enemyTank, SpriteBatch SpriteBatch)
        {
            spriteBatchMainGame = SpriteBatch;
            HealthFont = healthFont; ReloadingFont = reloadingFont;

            Player = new Player(playerChassis, playerTurret, HealthFont, ReloadingFont, spriteBatchMainGame)
            {
                Position = new Vector2(windowWidth / 2, windowHeight / 2),
                Origin = new Vector2(playerChassis.Width / 2, playerChassis.Height - 80),
                TurretOrigin = new Vector2(playerTurret.Width / 2, playerTurret.Height - turretSpacing),
                ReloadTime = 3f,
                Velocity = 3f,
            };


            Missile = new Missile(missileTexture);
            Missiles = new List<Missile>();

            Enemies = new List<Enemy>();
            EnemyTank = enemyTank;

        }

        public void Update(GameTime gameTime)
        {
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
                if (Enemies[i].Health < 0)
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
                missile.Draw(spriteBatchMainGame);
            foreach (var enemy in Enemies)
                enemy.Draw(spriteBatchMainGame);
            Player.Draw(spriteBatchMainGame);
            Player.DrawHealth(spriteBatchMainGame);
            foreach (var enemy in Enemies)
                enemy.DrawHealth(spriteBatchMainGame);
        }
    }
}
