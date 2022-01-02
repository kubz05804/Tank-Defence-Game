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

        public SpriteFont HealthFont;
        public SpriteFont ReloadingFont;
        protected Vector2 _currentPosition; public Vector2 Position { get { return _currentPosition; } set { _currentPosition = value; } }
        protected Vector2 _previousPosition;
        protected Vector2 _currentChassisDirection;
        protected Vector2 _previousChassisDirection;
        protected Vector2 _turretDirection;
        public Vector2 Origin;
        public Vector2 TurretOrigin;
        public Vector2 Gunpoint;

        public static SoundEffect destroy;
        public static SoundEffect EnemyShotSound;
        protected float _chassisRotation; public float Rotation {  get { return _chassisRotation; } }
        public float CurrentTurretAngle = 0;
        protected float _rotationVelocity = 0.03f;
        protected float _reloadTimeLeft;
        protected double _reloadTime; public double ReloadTime { get; set; }
        public float Timer;

        public bool _reloaded;
        protected bool _isMoving;
        protected bool _wasMoving;
        protected bool _enemy;

        protected int moveDirection;
        public int Health;
        public int InitialHealth;
        protected int _firepower;
        protected float velocity; public float Velocity { get; set; }

        public Vector2 pointTopLeft;
        public Vector2 pointTopRight;
        public Vector2 pointBottomRight;
        public Vector2 pointBottomLeft;
        public Rectangle rectangle;

        protected string _zero;

        public Tank(Texture2D chassis, Texture2D turret, SpriteFont healthFont, int tankIndex)
        {
            Chassis = chassis;
            Turret = turret;
            HealthFont = healthFont;
        }

        public virtual void Update(GameTime gameTime, Missile missile, List<Missile> missiles, Player player, List<Enemy> enemies)
        {

        }

        protected bool Collision(int direction, Player player, List<Enemy> enemies)
        {
            var position = _currentPosition;
            var chassisDirection = _currentChassisDirection;

            position += (_currentChassisDirection * direction) * velocity;

            if (_enemy)
            {
                if (Vector2.Distance(position, player.Position) <= player.Chassis.Height - 30)
                {
                    return true;
                }
            }

            foreach (var enemy in enemies)
            {
                if (Vector2.Distance(position, enemy.Position) <= enemy.Chassis.Height && Vector2.Distance(position, enemy.Position) > 10)
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
            spriteBatch.Draw(Chassis, _currentPosition, null, Color.White, _chassisRotation, Origin, 1, SpriteEffects.None, 0f);
            spriteBatch.Draw(Turret, _currentPosition, null, Color.White, CurrentTurretAngle, TurretOrigin, 1, SpriteEffects.None, 0f);

            if (!_reloaded && !_enemy)
            {
                spriteBatch.DrawString(ReloadingFont, "Reloading!", new Vector2(Position.X + 100, Position.Y - 50), Color.Red);
                spriteBatch.DrawString(ReloadingFont, _zero + (((ReloadTime * 1000) - Timer) / 1000).ToString("#.#") + "s left", new Vector2(Position.X + 100, Position.Y - 30), Color.Red);
            }
        }

        public void DrawHealth(SpriteBatch spriteBatch)
        {
            spriteBatch.DrawString(HealthFont, Health + "/" + InitialHealth, new Vector2(Position.X - 100, Position.Y - 100), Color.Red);
        }
    }
}
