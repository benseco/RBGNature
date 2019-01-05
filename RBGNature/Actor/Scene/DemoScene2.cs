using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace RBGNature.Actor.Scene
{
    class DemoScene2 : BaseScene
    {
        private static DemoScene2 _instance;
        public static DemoScene2 Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new DemoScene2();
                }
                return _instance;
            }
        }

        private DemoScene2()
        {

        }

        public override void Regenerate()
        {
        }

        public override void DrawParent(GameTime gameTime, SpriteBatch spriteBatch)
        {
        }

        public override void LightParent(SpriteBatch spriteBatch)
        {
        }

        public override void LoadContentParent(ContentManager contentManager)
        {
        }

        public override void UpdateParent(GameTime gameTime)
        {
            var kstate = Keyboard.GetState();
            if (kstate.IsKeyDown(Keys.B))
            {
                NextScene = DemoScene.Instance;
            }
        }
    }
}
