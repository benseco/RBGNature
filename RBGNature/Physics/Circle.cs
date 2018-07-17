using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RBGNature.Physics
{
    public class Circle : PhysicsObject
    {
        public double Radius { get; set; }
        public Vector2 Center { get; set; }

        public override CollisionResult Collide(Circle c)
        {
            if (Vector2.Distance(Center, c.Center) < Radius + c.Radius) return new CollisionResult();
            return new CollisionResult(false);
        }

        public override CollisionResult Collide(TriArray triArray)
        {
            //Implemented in TriArray.cs
            return triArray.Collide(this);
        }

        public override CollisionResult Collide(PhysicsObject other)
        {
            return other.Collide(this);
        }

        /// <summary>
        /// Determines if this Circle intersects a given line segment AB
        /// </summary>
        /// <param name="a">The first endpoint of the line segment AB</param>
        /// <param name="b">The second endpoint of the line segment AB</param>
        /// <returns>True if the circle and line segment intersect</returns>
        public bool Intersects(Vector2 a, Vector2 b)
        {
            //check to see if either end point lies within circle 
            if (Intersects(a) || Intersects(b)) return true;
            
            Vector2 d = b - a;
            Vector2 lc = Center - a;

            //project lc onto d, resulting in vector p
            var dLen2 = d.LengthSquared();
            Vector2 p = d;
            if (dLen2 > 0)
            {
                p *= Vector2.Dot(lc,d) / dLen2;
            }

            Vector2 projection = a + p;
            
            //check collision
            return Intersects(projection) && 
                p.LengthSquared() <= dLen2 && 
                Vector2.Dot(p, d) >= 0;
        }

        public bool Intersects(Vector2 p)
        {
            return Vector2.DistanceSquared(Center, p) < Radius * Radius;
        }

    }
}
