using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;

namespace Tank_Defence_Game.Objects
{
    public class Enemy : Tank, ICloneable
    { 
        private float spawnRate;
        public float distanceToPlayer;
        public float followDistance = 300f;

        private static Vector2 playerPosition;

        private bool withinRange;
        private bool currentlyFacingPlayer = true;
        private bool previouslyFacingPlayer;

        Random random = new Random();

        public Enemy(Texture2D chassis, Texture2D turret)
            : base(chassis, turret)
        {
            Origin = new Vector2(chassis.Width / 2, chassis.Height - 80);
            TurretOrigin = new Vector2(turret.Width / 2, turret.Height - 44);
            _reloadTime = 4f;
            InitialHealth = 50; Health = InitialHealth;
        }

        public override void Update(GameTime gameTime, Texture2D missileTexture)
        {
            playerPosition = Game1.player.Position;
            previouslyFacingPlayer = currentlyFacingPlayer;

            CurrentTurretAngle = (float)(Math.Atan2(Game1.player.Position.Y - _currentPosition.Y, Game1.player.Position.X - _currentPosition.X) + MathHelper.ToRadians(90)); // Turret rotation angle
            _turretDirection = new Vector2((float)Math.Cos(MathHelper.ToRadians(90) - CurrentTurretAngle), -(float)Math.Sin(MathHelper.ToRadians(90) - CurrentTurretAngle));
            Gunpoint = _currentPosition + _turretDirection * 100;

            var distanceToPlayer = Vector2.Distance(Position, playerPosition);
            if (distanceToPlayer > followDistance)
            {
                Rotate(playerPosition);
                if (currentlyFacingPlayer)
                    Motion(playerPosition);
            }

            if (distanceToPlayer <= Game1.player.Chassis.Height - 30)
                Game1.collision = true;
            else
                Game1.collision = false;
        
            if (_reloaded)
            {
                Game1.missile.AddBullet(Game1.missiles, _turretDirection, Gunpoint, velocity * 2, CurrentTurretAngle, true);
                _reloaded = false;
            }
            else
            {
                Timer += (float)gameTime.ElapsedGameTime.TotalMilliseconds;

                if (Timer > _reloadTime * 1000)
                {
                    _reloaded = true;
                    Timer = 0;
                }
            }
        }

        public void Spawn()
        {
            var enemy = Clone() as Enemy;

            int x = random.Next(-100, Game1.windowWidth + 100);
            int y = random.Next(-100, Game1.windowHeight + 100);
            if (x < 0 | x > Game1.windowWidth)
            {
                if (x < 0)
                    x = -100;
                else
                    x = Game1.windowWidth + 100;
            }
            else
            {
                if (y < 0)
                    y = -100;
                else
                    y = Game1.windowHeight + 100;
            }

            enemy._currentPosition = new Vector2(x, y);
            enemy.velocity = 3f;
            enemy._enemy = true;

            Game1.enemies.Add(enemy);
        }

        public void Rotate(Vector2 playerPosition)
        {
            var targetChassisAngle = (float)Math.Atan2(playerPosition.Y - _currentPosition.Y, playerPosition.X - _currentPosition.X) + MathHelper.ToRadians(90);

            if ((targetChassisAngle > _chassisRotation - 0.055f && targetChassisAngle < _chassisRotation + 0.055f))
            {
                _chassisRotation = targetChassisAngle;
            }

            if (targetChassisAngle == _chassisRotation)
            {
                currentlyFacingPlayer = true;
                return;
            }
            else
                currentlyFacingPlayer = false;

            if (_chassisRotation < targetChassisAngle)
            {
                if (Math.Abs(targetChassisAngle - _chassisRotation) < MathF.PI)
                    moveDirection = 1;
                else
                    moveDirection = -1;
            }
            else
            {
                if (Math.Abs(targetChassisAngle - _chassisRotation) < MathF.PI)
                    moveDirection = -1;
                else
                    moveDirection = 1;
            }

            _chassisRotation += 0.03f * moveDirection;

            if (_chassisRotation >= 1.5 * MathF.PI)
                _chassisRotation = MathHelper.ToRadians(-90);
            if (_chassisRotation < -MathF.PI / 2)
                _chassisRotation = MathHelper.ToRadians(270);
        }

        public void Motion(Vector2 playerPosition)
        {
            var distance = playerPosition - Position;
            _chassisRotation = (float)Math.Atan2(distance.Y, distance.X) + MathHelper.ToRadians(90);
            _currentChassisDirection = new Vector2((float)Math.Cos(MathHelper.ToRadians(90) - _chassisRotation), -(float)Math.Sin(MathHelper.ToRadians(90) - _chassisRotation));
            var currentDistance = Vector2.Distance(Position, playerPosition);
            Position += _currentChassisDirection * MathHelper.Min((float)Math.Abs(currentDistance - followDistance), velocity);
        }

        //public override void Draw(SpriteBatch spriteBatch)
        //{
        //    Origin = new Vector2(Chassis.Width / 2, Chassis.Height - 80);
        //    //spriteBatch.Draw(Game1.rectangle, new Rectangle((int)(_currentPosition.X - Origin.X), (int)(_currentPosition.Y - Origin.Y), Chassis.Width, Chassis.Height), Color.Red);
        //    //spriteBatch.Draw(Game1.rectangle, _currentPosition, new Rectangle((int)_currentPosition.X, (int)_currentPosition.Y, Chassis.Width, Chassis.Height), Color.Black, 0, Origin, SpriteEffects.None, 0f) ;
        //    spriteBatch.Draw(Chassis, _currentPosition, null, Color.White, _chassisRotation, Origin, 1, SpriteEffects.None, 0f);
        //    spriteBatch.Draw(Turret, _currentPosition, null, Color.White, CurrentTurretAngle, Origin, 1, SpriteEffects.None, 0f);
        //}

        public object Clone()
        {
            return this.MemberwiseClone();
        }
    }
}
