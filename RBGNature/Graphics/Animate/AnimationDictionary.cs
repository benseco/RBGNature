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
    public class AnimationDictionary<T>
    {
        private Dictionary<T, Animation> Animations { get; set; }

        public AnimationDictionary()
        {
            Animations = new Dictionary<T, Animation>();
        }

        public void Load(ContentManager contentManager)
        {
            foreach (T key in Animations.Keys)
            {
                Animation animation = Animations[key];
                animation.Load(contentManager);
            }
        }

        public void Add(Animation animation, params T[] keys)
        {
            foreach (T key in keys)
            {
                Animations[key] = animation;
            }
        }
        
        public Animation this[T key]
        {
            get { return Animations[key]; }
            set { Animations[key] = value; }
        }
    }
}
