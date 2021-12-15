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

        protected float _chassisRotation; public float Rotation {  get { return _chassisRotation; } }
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

        public Vector2 pointTopLeft;
        public Vector2 pointTopRight;
        public Vector2 pointBottomRight;
        public Vector2 pointBottomLeft;
        public Rectangle rectangle;
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

        //public bool Bounds(float angle, Vector2 point)
        //{
        //    var topleft = new Vector2(Position.X - Chassis.Width / 2,Position.Y - Chassis.Height / 2);
        //    var topright = topleft + new Vector2(Chassis.Width, 0);
        //    var bottomleft = topleft + new Vector2(0, Chassis.Height);
        //    var bottomright = topleft + new Vector2(Chassis.Width, Chassis.Height);

        //    pointTopLeft = new Vector2((float)(topleft.X * Math.Cos(angle) - topleft.Y * Math.Sin(angle)),(float)(topleft.X*Math.Sin(angle)+ topleft.Y*Math.Cos(angle)));
        //    pointTopRight = new Vector2((float)(topright.X * Math.Cos(angle) - topright.Y * Math.Sin(angle)), (float)(topright.X * Math.Sin(angle) + topright.Y * Math.Cos(angle)));
        //    pointBottomRight = new Vector2((float)(bottomright.X * Math.Cos(angle) - bottomright.Y * Math.Sin(angle)), (float)(bottomright.X * Math.Sin(angle) + bottomright.Y * Math.Cos(angle)));
        //    pointBottomLeft = new Vector2((float)(bottomleft.X * Math.Cos(angle) - bottomleft.Y * Math.Sin(angle)), (float)(bottomleft.X * Math.Sin(angle) + bottomleft.Y * Math.Cos(angle)));

        //    if ((pointTopRight.X - pointTopLeft.X) * (point.Y - pointTopLeft.Y) - (pointTopRight.Y - pointTopLeft.Y) * (point.X - pointTopLeft.X) > 0 &&
        //        (pointBottomRight.X - pointBottomLeft.X) * (point.Y - pointBottomLeft.Y) - (pointBottomRight.Y - pointBottomLeft.Y) * (point.X - pointBottomLeft.X) < 0 &&
        //        (pointBottomLeft.X - pointTopLeft.X) * (point.Y - pointTopLeft.Y) - (pointBottomLeft.Y - pointTopLeft.Y) * (point.X - pointTopLeft.X) < 0 &&
        //        (pointBottomRight.X - pointTopRight.X) * (point.Y - pointTopRight.Y) - (pointBottomRight.Y - pointTopRight.Y) * (point.X - pointTopRight.X) > 0)
        //    {
        //        return true;
        //    }
        //    else
        //    {
        //        return false;
        //    }
        //}

        public virtual void Draw(SpriteBatch spriteBatch)
        {
            //spriteBatch.Draw(Game1.rectangle, new Rectangle((int)(_currentPosition.X - Origin.X), (int)(_currentPosition.Y - Origin.Y), Chassis.Width, Chassis.Height), Color.Green);
            //spriteBatch.Draw(Game1.rectangle, new Rectangle((int)(_currentPosition.X - Origin.X), (int)(_currentPosition.Y - Origin.Y), Chassis.Width, Chassis.Height), null, Color.Green, _chassisRotation, Origin,  SpriteEffects.None, 0f);
            spriteBatch.Draw(Chassis, _currentPosition, null, Color.White, _chassisRotation, Origin, 1, SpriteEffects.None, 0f);
            spriteBatch.Draw(Turret, _currentPosition, null, Color.White, CurrentTurretAngle, TurretOrigin, 1, SpriteEffects.None, 0f);
        }
    }
}
