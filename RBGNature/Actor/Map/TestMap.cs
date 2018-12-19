using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RBGNature.Actor.Map
{
    class TestMap : BaseMap
    {
        Texture2D textureMap0;
        Texture2D textureMap1;
        Texture2D textureMap2;

        Texture2D textureWaterworld;

        public override void LoadContent(ContentManager contentManager)
        {
            textureMap0 = contentManager.Load<Texture2D>("Maps/test1/0");
            textureMap1 = contentManager.Load<Texture2D>("Maps/test1/1");
            textureMap2 = contentManager.Load<Texture2D>("Maps/test1/2");

            textureWaterworld = contentManager.Load<Texture2D>("Maps/test2/waterworld");
        }

        public override void Update(GameTime gameTime)
        {
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(textureWaterworld, Vector2.Zero, null, Color.White, 0, Vector2.Zero, Vector2.One, SpriteEffects.None, 0);

            //spriteBatch.Draw(textureMap0, Vector2.Zero, null, Color.White, 0, Vector2.Zero, Vector2.One, SpriteEffects.None, 0);
            //spriteBatch.Draw(textureMap1, Vector2.Zero, null, Color.White, 0, Vector2.Zero, Vector2.One, SpriteEffects.None, .9f);
            //spriteBatch.Draw(textureMap2, Vector2.Zero, null, Color.White, 0, Vector2.Zero, Vector2.One, SpriteEffects.None, 1);
        }

        public override bool Dead()
        {
            return false;
        }

        public override void Light(SpriteBatch spriteBatch)
        {
        }
    }
}
