using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RBGNature.Actor
{
    abstract class BaseActor
    {
        public abstract void LoadContent(ContentManager contentManager);
        public abstract void Update(GameTime gameTime);
        public abstract void Draw(SpriteBatch spriteBatch);

        protected static float LayerDepth(float y)
        {
            return y * 0.0000001f;
        }
    }
}
