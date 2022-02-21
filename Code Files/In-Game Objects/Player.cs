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
    public class Player : Tank
    {
        private MouseState currentMouseState;
        private MouseState previousMouseState;

        private float previousRotationAngle;

        private float targetAngle;

        private bool armourBoostEquipped; public bool ArmourBoostEquipped { get { return armourBoostEquipped;  } set { armourBoostEquipped = value; } }
        private bool camouflageNetEquipped; public bool CamouflageNetEquipped { get { return camouflageNetEquipped; } set { camouflageNetEquipped = value; } }

        private int score; public int Score { get { return score; } set { score = value; } }

        public Player(Texture2D chassis, Texture2D turret, SpriteFont healthFont, int tankIndex, SpriteFont reloadingFont, SpriteBatch spriteBatchMainGame)
            : base(chassis, turret, healthFont)
        {
            _reloadTime = (double)Game1.Tanks[tankIndex, 9];
            _initialHealth = (int)Game1.Tanks[tankIndex, 4];
            _health = _initialHealth;
            _firepower = (int)Game1.Tanks[tankIndex, 5];
            _velocity = (float)Game1.Tanks[tankIndex, 6];

            ReloadingFont = reloadingFont;

            armourBoostEquipped = false;
            CamouflageNetEquipped = false;
        }

        public override void Update(GameTime gameTime, Missile missile, List<Missile> missiles, Player player, List<Enemy> enemies)
        {
            previousMouseState = currentMouseState;
            currentMouseState = Mouse.GetState();

            if (!_reloaded)
            {
                _timer += (float)gameTime.ElapsedGameTime.TotalMilliseconds;

                if (_timer > _reloadTime * 1000)
                {
                    _reloaded = true;
                    Sound.Reload.Play();
                    _timer = 0;
                }
            }

            if (currentMouseState.LeftButton == ButtonState.Pressed && previousMouseState.LeftButton == ButtonState.Released && _reloaded)
            {
                missile.AddBullet(missiles, _turretDirection, _gunpoint, _currentTurretAngle, false, _firepower);
                Sound.PlayerShot.Play(volume: 0.4f, pitch: 0, pan: 0);

                _reloaded = false;
            }
            else if (currentMouseState.LeftButton == ButtonState.Pressed && previousMouseState.LeftButton == ButtonState.Released && !_reloaded)
            {
                Sound.Click.Play(volume: 0.7f, pitch: 0, pan: 0);
            }

            Motion(player, enemies);
        }

        private void Motion(Player player, List<Enemy> enemies)
        {
            _previousPosition = _currentPosition;
            _previousChassisDirection = _currentChassisDirection;
            _wasMoving = _isMoving;
            previousRotationAngle = targetAngle;


            if (Keyboard.GetState().IsKeyDown(Keys.D))
                _chassisRotation += _rotationVelocity;

            if (Keyboard.GetState().IsKeyDown(Keys.A))
                _chassisRotation -= _rotationVelocity;



            _currentChassisDirection = new Vector2((float)Math.Cos(MathHelper.ToRadians(90) - _chassisRotation), -(float)Math.Sin(MathHelper.ToRadians(90) - _chassisRotation)); // Chassis rotation direction
            targetAngle = (float)(Math.Atan2(Mouse.GetState().Y - _currentPosition.Y, Mouse.GetState().X - _currentPosition.X) + (MathF.PI / 2)); // Turret rotation angle
            _turretDirection = new Vector2((float)Math.Cos(MathHelper.ToRadians(90) - _currentTurretAngle), -(float)Math.Sin(MathHelper.ToRadians(90) - _currentTurretAngle));

            if (targetAngle != _currentTurretAngle)
            {
                if (_currentTurretAngle < targetAngle)
                {
                    if (Math.Abs(targetAngle - _currentTurretAngle) < MathF.PI)
                        _moveDirection = 1;
                    else
                        _moveDirection = -1;
                }
                else
                {
                    if (Math.Abs(targetAngle - _currentTurretAngle) < MathF.PI)
                        _moveDirection = -1;
                    else
                        _moveDirection = 1;
                }
            }

            _currentTurretAngle += 0.05f * _moveDirection;

            if ((targetAngle > _currentTurretAngle - 0.055f && targetAngle < _currentTurretAngle + 0.055f))
            {
                _currentTurretAngle = targetAngle;
            }

            if (_currentTurretAngle >= 1.5 * MathF.PI)
                _currentTurretAngle = MathHelper.ToRadians(-90);
            if (_currentTurretAngle < -MathF.PI / 2)
                _currentTurretAngle = MathHelper.ToRadians(270);

            _gunpoint = _currentPosition + _turretDirection * 100;

            if (Keyboard.GetState().IsKeyDown(Keys.W) && WithinWindow(1, _currentPosition, _currentChassisDirection * _velocity) && !Collision(1, player, enemies))
            {
                _currentPosition += _currentChassisDirection * _velocity;
            }

            if (Keyboard.GetState().IsKeyDown(Keys.S) && WithinWindow(-1, _currentPosition, _currentChassisDirection * _velocity) && !Collision(-1, player, enemies))
            {
                _currentPosition -= _currentChassisDirection * _velocity;
            }


            if (_currentPosition != _previousPosition | _currentChassisDirection != _previousChassisDirection)
                _isMoving = true;
            else
                _isMoving = false;

            if (_isMoving && !_wasMoving)
                Sound.MotionStart();
            if (!_isMoving && _wasMoving)
                Sound.MotionStop();
        }

        private bool WithinWindow(int one, Vector2 origin, Vector2 path)
        {
            Vector2 destination = origin + path * one;

            if (destination.X >= 0 && destination.X <= Game1.windowWidth && destination.Y >= 0 && destination.Y <= Game1.windowHeight)
                return true;
            else
                return false;
        }
    }
}