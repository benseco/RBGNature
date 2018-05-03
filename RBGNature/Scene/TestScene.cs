using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using RBGNature.Actor;
using RBGNature.Actor.Map;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RBGNature.Scene
{
    class TestScene : BaseScene
    {
        List<BaseActor> actors;

        public TestScene()
        {
            // set up camera
            Camera = new Camera
            {
                //Zoom = 2,
                Origin = new Vector2(320, 180)
            };

            actors = new List<BaseActor>();
            actors.Add(new TestMap());
            actors.Add(new Player(Camera));

        }

        public override void LoadContent(ContentManager contentManager)
        {
            actors.ForEach(a => a.LoadContent(contentManager));
        }

        public override void Update(GameTime gameTime)
        {
            actors.ForEach(a => a.Update(gameTime));

            //if (framecount++ % 120 == 0) System.Console.WriteLine("Camera position: " + camera.Position.X + ", " + camera.Position.Y);

        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            actors.ForEach(a => a.Draw(spriteBatch));
        }
    }
}
