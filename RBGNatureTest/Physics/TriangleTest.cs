using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Xna.Framework;
using RBGNature.Physics;

namespace RBGNatureTest.Physics
{
    [TestClass]
    public class TriangleTest
    {
        [TestMethod]
        public void CollideCircleAtTime_Top()
        {
            
            Triangle triangle = new Triangle(1,1,5,1,4,4);
            Vector2 center = new Vector2(4, 2);

            for (int i=0; i < 7; i++)
            {
                Vector2 position = new Vector2(i, 0);
                Circle circle = new Circle()
                {
                    Position = position,
                    Velocity = center - position,
                    Radius = 0.5f
                };
                Assert.IsTrue(triangle.CollideCircleAtTime(circle, out double t1));

                position = new Vector2(6, i);
                circle = new Circle()
                {
                    Position = position,
                    Velocity = center - position,
                    Radius = 0.5f
                };
                Assert.IsTrue(triangle.CollideCircleAtTime(circle, out double t2));

                position = new Vector2(6-i, 6);
                circle = new Circle()
                {
                    Position = position,
                    Velocity = center - position,
                    Radius = 0.5f
                };
                Assert.IsTrue(triangle.CollideCircleAtTime(circle, out double t3));

                position = new Vector2(0, 6-i);
                circle = new Circle()
                {
                    Position = position,
                    Velocity = center - position,
                    Radius = 0.5f
                };
                Assert.IsTrue(triangle.CollideCircleAtTime(circle, out double t4));
            }

        }

        [TestMethod]
        public void CollideCircleAtTime_Left()
        {
            Circle circle = new Circle()
            {
                Position = new Vector2(0, 0),
                Velocity = new Vector2(0, 3),
                Radius = 3
            };

            Triangle triangle = new Triangle(2, 4, 4, 6, 2, 8);

            Assert.IsTrue(triangle.CollideCircleAtTime(circle, out double t));
        }

        [TestMethod]
        public void CollideCircleWithEdgeAtTime_Left()
        {
            Circle circle = new Circle()
            {
                Position = new Vector2(0, 0),
                Velocity = new Vector2(0, 3),
                Radius = 3
            };

            Vector2 A = new Vector2(2, 4);
            Vector2 B = new Vector2(4, 6);
            Vector2 C = new Vector2(2, 8);
            Triangle triangle = new Triangle(A.X, A.Y, B.X, B.Y, C.X, C.Y);

            Assert.IsTrue(triangle.CollideCircleWithEdgeAtTime(circle, A, B, out double t));
        }

        [TestMethod]
        public void CollideCircleWithEdgeAtTime_Bottom()
        {
            Circle circle = new Circle()
            {
                Position = new Vector2(10, 22),
                Velocity = new Vector2(0, -4),
                Radius = 10
            };

            Vector2 A = new Vector2(0, 10);
            Vector2 B = new Vector2(10, 0);
            Vector2 C = new Vector2(20, 10);
            Triangle triangle = new Triangle(A.X, A.Y, B.X, B.Y, C.X, C.Y);

            CollisionResult result = triangle.CollideCircleWithEdgeAtTime(circle, C, A, out double t);
            Assert.IsTrue(result);
            Assert.AreEqual(t, 0.5);
        }
    }
}
