using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RBGNature.Graphics
{
    public class Animator
    {
        private Rectangle[] Sheet { get; set; }
        private int Frame { get; set; }
        private int ElapsedTime { get; set; }
        private int FrameTime { get; set; }

        public Animator(Rectangle[] sheet, int frameTime)
        {
            Set(sheet, frameTime);
        }

        public void Set(Rectangle[] sheet, int frameTime)
        {
            Sheet = sheet;
            FrameTime = frameTime;
        }

        public Rectangle NextFrame(GameTime gameTime)
        {
            ElapsedTime += gameTime.ElapsedGameTime.Milliseconds;

            while (ElapsedTime > FrameTime)
            {
                Frame++;
                if (Frame > Sheet.Length - 1) Frame = 0;
                ElapsedTime -= FrameTime;
            }

            return Sheet[Frame];
        }
    }
}
