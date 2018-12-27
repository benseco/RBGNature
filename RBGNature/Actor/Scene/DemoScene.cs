using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using RBGNature.Actor.Enemy;
using RBGNature.Actor.Interactible;
using RBGNature.Actor.Map;
using RBGNature.Graphics.Text;
using RBGNature.Physics;

namespace RBGNature.Actor.Scene
{
    class DemoScene : BaseScene
    {
        const string example = "The {Jitter:quick} {Color.Brown,Bold:brown fox} {Fast.30:jumped} {Wait.1000,Slow.500:...}{Wave:over} the {Slow.500,Color.0|255|127|50:lazy} dog.";

        DemoMap map;
        Player player;

        PhysicsGroup physical;

        FragmentWriter writer;

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
            WalkingEnemy enemy2 = new WalkingEnemy(new Vector2(300, 600), player);
            Item item = new Item(player, new Vector2(100, 500));
            children.Add(map);
            children.Add(player);
            //children.Add(enemy);
            //children.Add(enemy2);
            children.Add(item);

            physical = new PhysicsGroup(PhysicsGroupType.Physical);
            physical.Add(map);
            physical.Add(player);
            //physical.Add(enemy);
            //physical.Add(enemy2);

            writer = new FragmentWriter("Fonts/TooMuchInk", new Rectangle(100, 450, 200, 200), Color.White);
            writer.SetText(Fragment.Parse(example));
        }

        public override bool Dead()
        {
            return false;
        }

        public override void DrawParent(GameTime gameTime, SpriteBatch spriteBatch)
        {
            //writer.Draw(spriteBatch, Camera.Position);
            Dialogue.Draw(spriteBatch, Camera.Position);
        }

        public override void LightParent(SpriteBatch spriteBatch)
        {
        }

        public override void LoadContentParent(ContentManager contentManager)
        {
            writer.Load(contentManager);
            Dialogue.Load(contentManager);
        }

        public override void UpdateParent(GameTime gameTime)
        {
            writer.Update(gameTime);
            Dialogue.Update(gameTime);

            physical.Collide((float)gameTime.ElapsedGameTime.TotalMilliseconds);
            for (int i = children.Count - 1; i >= 0; i--)
            {
                if (children[i].Dead())
                {
                    physical.Remove(children[i] as ICollide);
                    children.RemoveAt(i);
                }
            }

            Color color = Color.Black;
            float lengthOfDay = 12000;
            float timeOfDay = (float)gameTime.TotalGameTime.TotalMilliseconds % lengthOfDay / lengthOfDay;
            if (timeOfDay < .25)
            {
                color = Color.MidnightBlue;
            }
            else if (timeOfDay < .375)
            {
                color = Color.Lerp(Color.MidnightBlue, Color.LightSkyBlue, (timeOfDay - .25f) / .125f);
            }
            else if (timeOfDay < .5)
            {
                color = Color.Lerp(Color.LightSkyBlue, Color.White, (timeOfDay - .375f) / .125f);
            }
            else if (timeOfDay < .75)
            {
                color = Color.White;
            }
            else if (timeOfDay < .875)
            {
                color = Color.Lerp(Color.White, Color.Chocolate, (timeOfDay - .75f) / .125f);
            }
            else
            {
                color = Color.Lerp(Color.Chocolate, Color.MidnightBlue, (timeOfDay - .875f) / .125f);
            }
            Atmosphere = color;
        }
    }
}
