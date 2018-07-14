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
    abstract class ParentActor : BaseActor
    {
        protected List<BaseActor> children = new List<BaseActor>();

        public sealed override void LoadContent(ContentManager contentManager)
        {
            children.ForEach(s => s.LoadContent(contentManager));
            this.LoadContentParent(contentManager);
        }

        public sealed override void Update(GameTime gameTime)
        {
            children.ForEach(s => s.Update(gameTime));
            this.UpdateParent(gameTime);
        }

        public sealed override void Draw(SpriteBatch spriteBatch)
        {
            children.ForEach(s => s.Draw(spriteBatch));
            this.DrawParent(spriteBatch);
        }

        public abstract void LoadContentParent(ContentManager contentManager);
        public abstract void UpdateParent(GameTime gameTime);
        public abstract void DrawParent(SpriteBatch spriteBatch);

    }
}
