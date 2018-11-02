using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RBGNature.Graphics.Animate
{
    public class Animation
    {
        public int FrameTime { get; set; }

        public readonly string TextureId;
        public readonly Rectangle[] Sheet;

        public Texture2D Texture { get; private set; }

        public Animation(string textureId, int frameTime, int frames, int width, int height)
        {
            TextureId = textureId;
            FrameTime = frameTime;

            Sheet = new Rectangle[frames];
            for (int frame = 0; frame < frames; frame++)
            {
                Sheet[frame] = new Rectangle(0, frame * height, width, height);
            }
        }

        public void Load(ContentManager contentManager)
        {
            Texture = contentManager.Load<Texture2D>(TextureId);
        }
    }
}
