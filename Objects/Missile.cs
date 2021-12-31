using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;


namespace Tank_Defence_Game.Objects
{
    public class Missile : ICloneable
    {
        private Texture2D texture; // Texture of the missile.
        private Vector2 direction; // The direction the missile should be flying in.
        private Vector2 position; // The position of the missile.
        private Vector2 origin; // Center point of the missile sprite.
        private float missileVelocity; // Linear velocity of the missile.
        private float lifespan; // Dictates how long a missile should be active for before being removed.
        private float rotation; // The angle of rotation of the missile sprite - or in other words, in which direction it should be facing.
        private float timer; // Creates a timer to keep track of missile lifespans.
        public static SoundEffect HitSound;
        private bool parentIsEnemy;
        private bool isRemoved; public bool IsRemoved { get; set; } // Creates a boolean value that states whether the missile should be removed.

        public Missile(Texture2D missileTexture)
        {
            texture = missileTexture; // Assigns the texture to the missile.
            origin = new Vector2(texture.Width / 2, texture.Height / 2); // Sets the origin of the missile sprite.
        }

        public void Update(GameTime gameTime, List<Missile> missiles, Player player, List<Enemy> enemies)
        {
            timer += (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (timer > lifespan)
                isRemoved = true;

            position += direction * missileVelocity;

            MissileCollision(enemies, player);
        }

        public void AddBullet(List<Missile> missiles, Vector2 direction, Vector2 origin, float velocity, float rotation, bool enemy)
        {
            var missile = Clone() as Missile;
            missile.direction = direction;
            missile.position = origin;
            missile.missileVelocity = velocity;
            missile.lifespan = 6;
            missile.rotation = rotation;
            missile.parentIsEnemy = enemy;

            missiles.Add(missile);
        }

        public void MissileCollision(List<Enemy> enemies, Player player)
        {
            if (parentIsEnemy)
            {
                if (Vector2.Distance(position, player.Position) <= player.Chassis.Height - 100)
                {
                    IsRemoved = true;
                    Sound.Collision.Play(volume: 0.3f, pitch: 0, pan: 0);
                    player.Health -= 20;
                }
            }
            else
            {
                foreach (var enemyTank in enemies)
                {
                    if (Vector2.Distance(position, enemyTank.Position) <= enemyTank.Chassis.Height - 75) //110 - original | 75 - pz iv h
                    {
                        IsRemoved = true;
                        Sound.Collision.Play(volume: 0.3f, pitch: 0, pan: 0);
                        enemyTank.Health -= 20;
                    }
                }
            }
        }

        public virtual void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, position, null, Color.White, rotation, origin, 1, SpriteEffects.None, 0);
        }

        public object Clone()
        {
            return this.MemberwiseClone();
        }
    }
}
