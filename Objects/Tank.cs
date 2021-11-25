﻿using System;
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

        public Rectangle VehicleBounds
        {
            get
            {
                return new Rectangle((int)_currentPosition.X, (int)_currentPosition.Y, Chassis.Width, Chassis.Height);
            }
        } //https://community.monogame.net/t/why-rotate-a-sprite-is-simple-and-rotate-a-rectangle-is-damn-complex/7694/13

        public Tank(Texture2D chassis, Texture2D turret)
        {
            Chassis = chassis;
            Turret = turret;
        }

        public virtual void Update(GameTime gameTime, Texture2D missile)
        {
            

        }

        public virtual void Draw(SpriteBatch spriteBatch)
        {
            Origin = new Vector2(Chassis.Width / 2, Chassis.Height - 80);
            //spriteBatch.Draw(Game1.rectangle, new Rectangle((int)(_currentPosition.X - Origin.X), (int)(_currentPosition.Y - Origin.Y), Chassis.Width, Chassis.Height), Color.Green);
            spriteBatch.Draw(Game1.rectangle, new Rectangle((int)(_currentPosition.X - Origin.X), (int)(_currentPosition.Y - Origin.Y), Chassis.Width, Chassis.Height), null, Color.Green, _chassisRotation, Origin,  SpriteEffects.None, 0f);
            spriteBatch.Draw(Chassis, _currentPosition, null, Color.White, _chassisRotation, Origin, 1, SpriteEffects.None, 0f);
            spriteBatch.Draw(Turret, _currentPosition + new Vector2(4,0), null, Color.White, CurrentTurretAngle, Origin, 1, SpriteEffects.None, 0f);
        }
    }
}
