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

        public override void DrawParent(SpriteBatch spriteBatch)
        {
        }

        public override void LoadContentParent(ContentManager contentManager)
        {
        }

        public override void UpdateParent(GameTime gameTime)
        {
            physical.Collide();

        }
    }
}
