using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RBGNature.Graphics.Animation
{
    public class Animator
    {
        public Animation Animation { get; private set; }
        private int Frame { get; set; }
        private int ElapsedTime { get; set; }

        public Animator(Animation animation) => Animate(animation);

        public void Animate(Animation animation)
        {
            if (Animation == animation) return;
            Animation = animation;
            ElapsedTime = 0;
            Frame = 0;
        }

        public Rectangle NextFrame(GameTime gameTime)
        {
            ElapsedTime += gameTime.ElapsedGameTime.Milliseconds;

            while (ElapsedTime > Animation.FrameTime)
            {
                Frame++;
                if (Frame > Animation.Sheet.Length - 1) Frame = 0;
                ElapsedTime -= Animation.FrameTime;
            }

            return Animation.Sheet[Frame];
        }
    }
}
