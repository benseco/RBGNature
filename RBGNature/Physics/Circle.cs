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
        public float Radius { get; set; }

        public override CollisionResult Collide(Circle c)
        {
            Vector2 vel = Velocity - c.Velocity;
            Vector2 pos = Position - c.Position;

            Vector2 d = ClosestOnSegment(pos, pos + vel, Vector2.Zero);

            double closestDistanceSquared = Vector2.DistanceSquared(d, Vector2.Zero);
            if (closestDistanceSquared <= Math.Pow(Radius + c.Radius, 2))
            { // Collision occured

                // Projection of c center on this circle's velocity vector
                Vector2 a = Vector2.Zero - pos;
                Vector2 b = vel;
                Vector2 projection = pos + (Vector2.Dot(a, b) / b.LengthSquared()) * b;

                // Calculate leg of right triangle: the distance from the closest point where the circles actually collided
                double legSquared = Math.Pow(Radius + c.Radius, 2) - (projection - Vector2.Zero).LengthSquared();

                // The percentage of time in this step that passes before the collision
                float time = (float) (1 - (legSquared / Vector2.DistanceSquared(pos, projection)));

                //c_x, c_y - the position of this circle at time of collision
                Vector2 newPos = Position + Velocity * time;
                Vector2 cNewPos = c.Position + c.Velocity * time;
                
                // Calculate response (bounce)

                //n_x, n_y - the norm of the vector between the two circles
                Vector2 n = Vector2.Normalize(cNewPos - newPos);

                // velocity-mass relation
                float p = 2 * (Vector2.Dot(Velocity, n) - Vector2.Dot(c.Velocity, n)) / (Mass + c.Mass);

                //w_x,w_y - resultant velocity of first circle
                Vector2 newV = Velocity - p * Mass * n;
                Vector2 cNewV = c.Velocity + p * c.Mass * n;

                return new CollisionResult(newPos, newV, cNewPos, cNewV);
            }
            else return CollisionResult.None;
        }

        public override CollisionResult Collide(TriArray triArray)
        {
            //Implemented in TriArray.cs
            //So we negate the impulse
            return triArray.Collide(this).Switch();
        }

        public override CollisionResult Collide(PhysicsObject other)
        {
            return other.Collide(this).Switch();
        }

        /// <summary>
        /// Determines if this Circle intersects a given line segment AB
        /// </summary>
        /// <param name="a">The first endpoint of the line segment AB</param>
        /// <param name="b">The second endpoint of the line segment AB</param>
        /// <returns>True if the circle and line segment intersect with closest point of intersection</returns>
        //public CollisionResult Intersects(Vector2 a, Vector2 b)
        //{
        //    Vector2 d = b - a;
        //    Vector2 lc = Position - a;

        //    //project lc onto d, resulting in vector p
        //    var dLen2 = d.LengthSquared();
        //    Vector2 p = d;
        //    if (dLen2 > 0)
        //    {
        //        p *= Vector2.Dot(lc,d) / dLen2;
        //    }

        //    Vector2 projection = a + p;
            
        //    //check collision
        //    if (Intersects(projection) && p.LengthSquared() <= dLen2 && Vector2.Dot(p, d) >= 0)
        //    {
        //        return new CollisionResult(GetImpulse(projection));
        //    }
        //    //check to see if either end point lies within circle
        //    else if (Intersects(a)) return new CollisionResult(GetImpulse(a));
        //    else if (Intersects(b)) return new CollisionResult(GetImpulse(b));

        //    //otherwise, if we have no collision
        //    return CollisionResult.None;
        //}

        public bool Intersects(Vector2 p)
        {
            return Vector2.DistanceSquared(Position, p) < Radius * Radius;
        }

        public Vector2 GetImpulse(Vector2 p)
        {
            Vector2 cp = p - Position;
            return cp - (Vector2.Normalize(cp) * Radius);
        }

    }
}
