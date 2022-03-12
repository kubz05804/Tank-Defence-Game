using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Content;

namespace Tank_Defence_Game.Objects
{
    public class Tank
    {
        protected Texture2D _chassis; public Texture2D Chassis { get { return _chassis; } }
        protected Texture2D _turret; public Texture2D Turret { get { return _turret; } }

        protected SpriteFont _font12;

        protected Vector2 _origin; public Vector2 Origin { set { _origin = value; } }
        protected Vector2 _turretOrigin; public Vector2 TurretOrigin { set { _turretOrigin = value; } }
        protected Vector2 _currentPosition; public Vector2 Position { get { return _currentPosition; } set { _currentPosition = value; } }
        protected Vector2 _previousPosition;
        protected Vector2 _currentChassisDirection;
        protected Vector2 _previousChassisDirection;
        protected Vector2 _turretDirection;
        protected Vector2 _gunpoint;
        protected Vector2 _pointerPosition;

        protected string name;

        protected int _firepower; public int Firepower { get { return _firepower; } set { _firepower = value; } }
        protected int _health; public int Health { get { return _health; } set { _health = value; } }
        protected int _initialHealth; public int InitialHealth { get { return _initialHealth; } }
        protected int _moveDirection;

        protected float _velocity; public float Velocity { get { return _velocity; } set { _velocity = value; } }
        protected double _muzzleVelocity; public double MuzzleVelocity { get { return _muzzleVelocity; } set { _muzzleVelocity = value; } }
        protected float _chassisRotation; public float Rotation { get { return _chassisRotation; } }
        protected float _rotationVelocity = 0.03f;
        protected float _currentTurretAngle = 0;
        protected float _timer;

        protected double _reloadTime; public double ReloadTime { get { return _reloadTime; } set { _reloadTime = value; } }

        protected bool _reloaded;
        protected bool _isMoving;
        protected bool _wasMoving;
        protected bool _enemy;

        public Tank()
        {
            //_chassis = MainGame.Content.Load<Texture2D>("Textures/" + Game1.Tanks[tankIndex, 0] + "/chassis");
            //_turret = MainGame.Content.Load<Texture2D>("Textures/" + Game1.Tanks[tankIndex, 0] + "/turret");
            _font12 = MainGame.Content.Load<SpriteFont>("Fonts/font12");

            //_initialHealth = (int)Game1.Tanks[tankIndex, 4];
            //_firepower = (int)Game1.Tanks[tankIndex, 5];
            //_velocity = (float)Game1.Tanks[tankIndex, 6];
            //_reloadTime = (double)Game1.Tanks[tankIndex, 9];
            //_muzzleVelocity = (double)Game1.Tanks[tankIndex, 11];

            //_origin = new Vector2(_chassis.Width / 2, _chassis.Height - (int)Game1.Tanks[tankIndex, 7]);
            //_turretOrigin = new Vector2(_turret.Width / 2, _turret.Height - (int)Game1.Tanks[tankIndex, 8]);

            //_health = _initialHealth;

            //name = (string)Game1.Tanks[tankIndex, 0];
        }

        public virtual void Update(GameTime gameTime, Missile missile, List<Missile> missiles, Player player, List<Enemy> enemies)
        {
            
        }

        protected bool Collision(int direction, Player player, List<Enemy> enemies)
        {
            var position = _currentPosition;
            var chassisDirection = _currentChassisDirection;

            position += (_currentChassisDirection * direction) * _velocity;

            if (_enemy)
            {
                if (Vector2.Distance(position, player.Position) <= player._chassis.Height / 2)
                {
                    return true;
                }
            }

            foreach (var enemy in enemies)
            {
                if (Vector2.Distance(position, enemy.Position) <= enemy._chassis.Height && Vector2.Distance(position, enemy.Position) > 10)
                {
                    return true;
                }
            }

            return false;
        }

        public virtual void Draw(SpriteBatch spriteBatch, GraphicsDevice graphicsDevice)
        {
            spriteBatch.Draw(_chassis, _currentPosition, null, Color.White, _chassisRotation, _origin, 1, SpriteEffects.None, 0f);
            spriteBatch.Draw(_turret, _currentPosition, null, Color.White, _currentTurretAngle, _turretOrigin, 1, SpriteEffects.None, 0f);

            
            var texture = new Texture2D(graphicsDevice, 1, 1);
            texture.SetData(new Color[] { Color.White });

            var colour = Color.Red;

            if (!_enemy)
            {
                colour = Color.Green;

                spriteBatch.Draw(texture, new Rectangle((int)_pointerPosition.X - 2, (int)_pointerPosition.Y - 2, 5, 5), Color.Black);

                if (!_reloaded)
                {
                    var zero = "";
                    var reloadTimeLeft = ((_reloadTime * 1000) - _timer) / 1000;
                    if (reloadTimeLeft < 1)
                        zero = "0";


                    spriteBatch.DrawString(_font12, "Reloading!", _pointerPosition + new Vector2(5, -30), Color.Red);
                    spriteBatch.DrawString(_font12, zero + reloadTimeLeft.ToString("#.#") + "s left", _pointerPosition + new Vector2(20, -10), Color.Red);
                }
            }

            spriteBatch.DrawString(_font12, Health + "/" + InitialHealth, new Vector2(Position.X - 100, Position.Y - 100), colour);
            spriteBatch.DrawString(_font12, name, new Vector2(Position.X - 100, Position.Y - 120), colour);
        }
    }
}
