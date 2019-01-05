using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace RBGNature.Actor.Scene
{
    public class SceneManager : IAct
    {
        public BaseScene Scene { get; private set; }

        public SceneManager(BaseScene scene)
        {
            Scene = scene;
        }

        public bool Dead()
        {
            return false;
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch) => Scene.Draw(gameTime, spriteBatch);

        public void Light(SpriteBatch spriteBatch) => Scene.Light(spriteBatch);

        public void LoadContent(ContentManager contentManager) => Scene.LoadContent(contentManager);

        public void Update(GameTime gameTime)
        {
            if (Scene.NextScene != null)
            {
                BaseScene nextScene = Scene.NextScene;
                Scene.Switched();
                Scene = nextScene;
            }
            Scene.Update(gameTime);
        }
    }
}
