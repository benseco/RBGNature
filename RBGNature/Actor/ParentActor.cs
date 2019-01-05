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
    public abstract class ParentActor : IAct
    {
        protected List<IAct> children = new List<IAct>();

        public void LoadContent(ContentManager contentManager)
        {
            children.ForEach(s => s.LoadContent(contentManager));
            this.LoadContentParent(contentManager);
        }

        public void Update(GameTime gameTime)
        {
            this.UpdateParent(gameTime);
            children.ForEach(s => s.Update(gameTime));
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            children.ForEach(s => s.Draw(gameTime, spriteBatch));
            this.DrawParent(gameTime, spriteBatch);
        }

        public void Light(SpriteBatch spriteBatch)
        {
            children.ForEach(s => s.Light(spriteBatch));
            this.LightParent(spriteBatch);
        }

        public abstract void LoadContentParent(ContentManager contentManager);
        public abstract void UpdateParent(GameTime gameTime);
        public abstract void DrawParent(GameTime gameTime, SpriteBatch spriteBatch);
        public abstract void LightParent(SpriteBatch spriteBatch);
        public abstract bool Dead();

    }
}
