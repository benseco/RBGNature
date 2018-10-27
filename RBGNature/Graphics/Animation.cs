using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RBGNature.Graphics
{
    class Animation<T>
    {
        public Texture2D Texture { get; private set; }
        private Dictionary<T, Rectangle[]> Sheets { get; set; }
        private int Width { get; set; }
        private int Height { get; set; }

        public Animation(int width = 0, int height = 0)
        {
            Sheets = new Dictionary<T, Rectangle[]>();
            Width = width;
            Height = height;
        }

        public void Load(ContentManager contentManager, string assetName)
        {
            Texture = contentManager.Load<Texture2D>(assetName);
        }

        public void Add(T key, int frameStart = 0, int numFrames = 1)
        {
            if (frameStart < 0 || numFrames < 1)
            {
                throw new ArgumentOutOfRangeException();
            }
            
            Rectangle[] rects = new Rectangle[numFrames];

            for (int frame = 0; frame < numFrames; frame++)
            {
                //TODO: Make this support other orientations, multiple sheets per texture
                rects[frame] = new Rectangle(0, frame * Height, Width, Height);
            }
            Sheets.Add(key, rects);
        }

        public Rectangle[] Get(T key)
        {
            return Sheets[key];
        }

    }
}
