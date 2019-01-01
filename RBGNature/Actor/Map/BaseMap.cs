using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using RBGNature.Physics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RBGNature.Actor.Map
{
    public abstract class BaseMap : IAct
    {
        public abstract bool Dead();
        public abstract void Draw(GameTime gameTime, SpriteBatch spriteBatch);
        public abstract void Light(SpriteBatch spriteBatch);
        public abstract void LoadContent(ContentManager contentManager);
        public abstract void Update(GameTime gameTime);
    }
}
