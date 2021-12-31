using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Audio;

namespace Tank_Defence_Game
{
    public class Sound
    {
        public static SoundEffect PlayerShot;
        public static SoundEffect EnemyShot;

        public static SoundEffect Click;
        public static SoundEffect Reload;

        public static SoundEffect Collision;
        public static SoundEffect Destruction;

        public static Song Motion;

        public Sound()
        {

        }

        public static void MotionStart()
        {
            MediaPlayer.Play(Motion);
        }

        public static void MotionStop()
        {
            MediaPlayer.Stop();
        }
    }
}
