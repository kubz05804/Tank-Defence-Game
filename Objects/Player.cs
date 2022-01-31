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

        private SpriteBatch spriteBatch;

        private float previousRotationAngle;

        public SoundEffect ShotSound;
        public SoundEffect ClickSound;
        public SoundEffect ReloadSound;
        public Song MotionSound;
        public float TargetAngle;

        private int moveDirection;
        private int score; public int Score { get; set; }

        public Player(Texture2D chassis, Texture2D turret, SpriteFont healthFont, int tankIndex, SpriteFont reloadingFont, SpriteBatch spriteBatchMainGame)
            : base(chassis, turret, healthFont, tankIndex)
        {
            _reloadTime = (double)Game1.Tanks[tankIndex, 9];
            InitialHealth = (int)Game1.Tanks[tankIndex, 4]; Health = InitialHealth;
            _firepower = (int)Game1.Tanks[tankIndex, 5];
            spriteBatch = spriteBatchMainGame;
            ReloadingFont = reloadingFont;
            velocity = (float)Game1.Tanks[tankIndex, 6];
        }

        public override void Update(GameTime gameTime, Missile missile, List<Missile> missiles, Player player, List<Enemy> enemies)
        {
            //timer += (float)gameTime.ElapsedGameTime.TotalSeconds;
            previousMouseState = currentMouseState;
            currentMouseState = Mouse.GetState();

            if (!_reloaded)
            {
                Timer += (float)gameTime.ElapsedGameTime.TotalMilliseconds;

                if (Timer > _reloadTime * 1000)
                {
                    _reloaded = true;
                    Sound.Reload.Play();
                    Timer = 0;
                }

                if (_reloadTime * 1000 - Timer < 1000)
                    _zero = "0";
                else
                    _zero = "";
            }

            if (currentMouseState.LeftButton == ButtonState.Pressed && previousMouseState.LeftButton == ButtonState.Released && _reloaded)
            {
                missile.AddBullet(missiles, _turretDirection, Gunpoint, CurrentTurretAngle, false, _firepower);
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
            previousRotationAngle = TargetAngle;


            if (Keyboard.GetState().IsKeyDown(Keys.D))
                _chassisRotation += _rotationVelocity;

            if (Keyboard.GetState().IsKeyDown(Keys.A))
                _chassisRotation -= _rotationVelocity;



            _currentChassisDirection = new Vector2((float)Math.Cos(MathHelper.ToRadians(90) - _chassisRotation), -(float)Math.Sin(MathHelper.ToRadians(90) - _chassisRotation)); // Chassis rotation direction
            TargetAngle = (float)(Math.Atan2(Mouse.GetState().Y - _currentPosition.Y, Mouse.GetState().X - _currentPosition.X) + (MathF.PI / 2)); // Turret rotation angle
            _turretDirection = new Vector2((float)Math.Cos(MathHelper.ToRadians(90) - CurrentTurretAngle), -(float)Math.Sin(MathHelper.ToRadians(90) - CurrentTurretAngle));

            if (TargetAngle != CurrentTurretAngle)
            {
                if (CurrentTurretAngle < TargetAngle)
                {
                    if (Math.Abs(TargetAngle - CurrentTurretAngle) < MathF.PI)
                        moveDirection = 1;
                    else
                        moveDirection = -1;
                }
                else
                {
                    if (Math.Abs(TargetAngle - CurrentTurretAngle) < MathF.PI)
                        moveDirection = -1;
                    else
                        moveDirection = 1;
                }
            }

            CurrentTurretAngle += 0.05f * moveDirection;

            if ((TargetAngle > CurrentTurretAngle - 0.055f && TargetAngle < CurrentTurretAngle + 0.055f))
            {
                CurrentTurretAngle = TargetAngle;
            }

            if (CurrentTurretAngle >= 1.5 * MathF.PI)
                CurrentTurretAngle = MathHelper.ToRadians(-90);
            if (CurrentTurretAngle < -MathF.PI / 2)
                CurrentTurretAngle = MathHelper.ToRadians(270);

            Gunpoint = _currentPosition + _turretDirection * 100;

            if (Keyboard.GetState().IsKeyDown(Keys.W) && WithinWindow(1, _currentPosition, _currentChassisDirection * velocity) && !Collision(1, player, enemies))
            {
                _currentPosition += _currentChassisDirection * velocity;
            }

            if (Keyboard.GetState().IsKeyDown(Keys.S) && WithinWindow(-1, _currentPosition, _currentChassisDirection * velocity) && !Collision(-1, player, enemies))
            {
                _currentPosition -= _currentChassisDirection * velocity;
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