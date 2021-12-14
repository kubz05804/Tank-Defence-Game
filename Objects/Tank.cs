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
    public class Tank
    {
        public Texture2D Chassis;
        public Texture2D Turret;

        protected Vector2 _currentPosition; public Vector2 Position { get { return _currentPosition; } set { _currentPosition = value; } }
        protected Vector2 _previousPosition;
        protected Vector2 _currentChassisDirection;
        protected Vector2 _previousChassisDirection;
        protected Vector2 _turretDirection;
        public Vector2 Origin;
        public Vector2 TurretOrigin;
        public Vector2 Gunpoint;

        protected float _chassisRotation;
        public float CurrentTurretAngle = 0;
        protected float _rotationVelocity = 0.03f;
        protected float _reloadTimeLeft;
        protected float _timer;

        public bool _reloaded;
        protected bool _isMoving;
        protected bool _wasMoving;
        protected bool _enemy;

        protected int moveDirection;
        protected float velocity = 3f; public float Velocity { get; set; }

        public Tank(Texture2D chassis, Texture2D turret)
        {
            Chassis = chassis;
            Turret = turret;
        }

        public virtual void Update(GameTime gameTime, Texture2D missile)
        {
            

        }

        protected bool Collision(int direction)
        {
            var position = _currentPosition;
            var chassisDirection = _currentChassisDirection;

            position += (_currentChassisDirection * direction) * velocity;

            if (_enemy)
            {
                if (Vector2.Distance(position, Game1.player.Position) <= Game1.player.Chassis.Height - 30)
                {
                    return true;
                }
            }

            foreach (var enemy in Game1.enemies)
            {
                if (Vector2.Distance(position, enemy.Position) <= enemy.Chassis.Height - 30)
                {
                    return true;
                }
            }

            return false;
        }

        public virtual void Draw(SpriteBatch spriteBatch)
        {
            //spriteBatch.Draw(Game1.rectangle, new Rectangle((int)(_currentPosition.X - Origin.X), (int)(_currentPosition.Y - Origin.Y), Chassis.Width, Chassis.Height), Color.Green);
            //spriteBatch.Draw(Game1.rectangle, new Rectangle((int)(_currentPosition.X - Origin.X), (int)(_currentPosition.Y - Origin.Y), Chassis.Width, Chassis.Height), null, Color.Green, _chassisRotation, Origin,  SpriteEffects.None, 0f);
            spriteBatch.Draw(Chassis, _currentPosition, null, Color.White, _chassisRotation, Origin, 1, SpriteEffects.None, 0f);
            spriteBatch.Draw(Turret, _currentPosition, null, Color.White, CurrentTurretAngle, TurretOrigin, 1, SpriteEffects.None, 0f);
        }
    }
}
