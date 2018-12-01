using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RBGNature.Graphics.Animate
{
    public class Animator
    {
        private Animation Animation { get; set; }
        private int Frame { get; set; }
        private int ElapsedTime { get; set; }
        private Animation SetAnimation { get; set; }

        public Texture2D Texture { get { return Animation.Texture; } }

        public Animator(Animation animation) => Set(animation);

        public void Set(Animation animation)
        {
            SetAnimation = animation;
        }

        public void Update(GameTime gameTime)
        {
            if (Animation != SetAnimation)
            {
                Animation = SetAnimation;
                ElapsedTime = 0;
                Frame = 0;
            }
            else
            {
                ElapsedTime += gameTime.ElapsedGameTime.Milliseconds;

                while (ElapsedTime > Animation.FrameTime)
                {
                    Frame++;
                    if (Frame > Animation.Sheet.Length - 1) Frame = 0;
                    ElapsedTime -= Animation.FrameTime;
                }
            }
        }

        public Rectangle NextFrame()
        {
            return Animation.Sheet[Frame];
        }
    }
}
