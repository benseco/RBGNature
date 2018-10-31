using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using RBGNature.Actor.Enemy;
using RBGNature.Actor.Map;
using RBGNature.Physics;

namespace RBGNature.Actor.Scene
{
    class DemoScene : BaseScene
    {
        DemoMap map;
        Player player;

        PhysicsGroup physical;

        public DemoScene()
        {
            // set up camera
            Camera = new Camera
            {
                //Zoom = 2,
                Origin = new Vector2(320, 180),
                Position = new Vector2(200, 400)
            };

            map = new DemoMap();
            player = new Player(Camera);
            TestEnemy enemy = new TestEnemy(new Vector2(200, 600), player);

            children.Add(map);
            children.Add(player);
            children.Add(enemy);

            physical = new PhysicsGroup(PhysicsGroupType.Physical);
            physical.Add(map);
            physical.Add(player);
            physical.Add(enemy);
        }

        int textElapsedTime = 0;
        int timeBetweenCharacters = 100;
        public override void DrawParent(GameTime gameTime, SpriteBatch spriteBatch)
        {
            string text = "Hello world!";

            textElapsedTime += gameTime.ElapsedGameTime.Milliseconds;

            int numChars = textElapsedTime / timeBetweenCharacters;
            if (numChars > text.Length)
            {
                numChars = text.Length;
                textElapsedTime = 0;
            }
            text = text.Substring(0, numChars);

            spriteBatch.DrawString(font, text, new Vector2(200, 550), Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 1);
        }

        SpriteFont font;
        public override void LoadContentParent(ContentManager contentManager)
        {
            font = contentManager.Load<SpriteFont>("Fonts/TooMuchInk");
        }

        public override void UpdateParent(GameTime gameTime)
        {
            physical.Collide();

        }
    }
}
