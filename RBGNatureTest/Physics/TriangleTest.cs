using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Xna.Framework;
using RBGNature.Actor;
using RBGNature.Actor.Map;
using RBGNature.Physics;

namespace RBGNatureTest.Physics
{
    [TestClass]
    public class TriangleTest
    {
        [TestMethod]
        public void TriArrayErrorCase()
        {
            PhysicsGroup physicsGroup = new PhysicsGroup(PhysicsGroupType.Physical);

            DemoMap demoMap = new DemoMap();
            Player player = new Player(new RBGNature.Camera());

            player.collision = new Circle()
            {
                Radius = 10,
                Mass = 1,
                Position = new Vector2(170.716476f, 303.425659f),
                Velocity = new Vector2(-0.17978698f, 0.0744702f)
            };

            physicsGroup.Add(demoMap);
            physicsGroup.Add(player);

            physicsGroup.Collide(16.6488f);

            player.Update(new GameTime());

            Assert.IsTrue(player.collision.Position.X + player.collision.Position.Y > 460 + Math.Sqrt(2) * 10);
        }
    }
}
