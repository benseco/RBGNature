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

        public override CollisionResult Collide(float s, Circle c)
        {
            return CollideCircleAtTime(s, c, out double t);
        }

        public CollisionResult CollideCircleAtTime(float s, Circle c, out double t)
        {
            return CollideCircleAtTime2(s, c, out t);

            t = -1;//arbitrary error case

            Vector2 vel = Velocity * s - c.Velocity * s;
            Vector2 pos = Position - c.Position;

            Vector2 d = ClosestOnSegment(pos, pos + vel, Vector2.Zero);

            double closestDistanceSquared = Vector2.DistanceSquared(d, Vector2.Zero);
            if (closestDistanceSquared <= Math.Pow(Radius + c.Radius, 2))
            { // Collision occured

                // Projection of c center on this circle's velocity vector
                Vector2 a = Vector2.Zero - pos;
                Vector2 b = vel;
                float aDotB = Vector2.Dot(a, b);
                if (aDotB < .0001f) { return CollisionResult.None; } // This can happen if the collision is tangential, so just ignore it for now?
                Vector2 projection = pos + (aDotB / b.LengthSquared()) * b;

                // Calculate leg of right triangle: the distance from the closest point where the circles actually collided
                double legSquared = Math.Pow(Radius + c.Radius, 2) - (projection - Vector2.Zero).LengthSquared();

                // The percentage of time in this step that passes before the collision
                t = 1 - (legSquared / Vector2.DistanceSquared(pos, projection));
                if (t < -1 || t > 1) { return CollisionResult.None; } // When does this happen and why is it bad?

                // The position of the circles at time of collision
                Vector2 newPos = Position + Velocity * s * (float)t;
                Vector2 cNewPos = c.Position + c.Velocity * s * (float)t;

                // Calculate response (bounce)

                // The norm of the vector between the two circles
                Vector2 n = Vector2.Normalize(cNewPos - newPos);


                ////Reflection
                //Vector2 reflection = Velocity - 2 * Vector2.Dot(Velocity, n) * n;
                //Vector2 cReflection = c.Velocity - 2 * Vector2.Dot(c.Velocity, n) * n;

                //float totalMass = Mass + c.Mass;
                //Vector2 lerp = Vector2.Lerp(reflection, Velocity, Mass / totalMass);
                //Vector2 cLerp = Vector2.Lerp(cReflection, c.Velocity, c.Mass / totalMass);

                //Vector2 newV = lerp;
                //Vector2 cNewV = cLerp;

                //impulse magnitude?
                Vector2 deltaV = Velocity - c.Velocity;
                float magnitude = deltaV.Length();

                //Resultant velocities
                Vector2 newV = Velocity - (1 / Mass) * magnitude * n;
                Vector2 cNewV = c.Velocity + (1 / c.Mass) * magnitude * n;


                // Add some separation (.01f is arbitrary ?)
                //newPos = newPos + newV * .01f;
                //cNewPos = cNewPos + cNewV * .01f;

                return new CollisionResult(newPos, newV, cNewPos, cNewV);
            }
            else return CollisionResult.None;
        }

        public CollisionResult CollideCircleAtTime2(float s, Circle other, out double t)
        {
            t = -1; //error case

            //Change frame of reference to this circle
            Vector2 vel = other.Velocity * s - this.Velocity * s;
            Vector2 pos = other.Position - this.Position;

            //Now assume the following about this circle:
            //Radius = this.Radius + other.Radius
            //Position = Vector2.Zero
            //Velocity = Vector2.Zero

            //This circle is colliding with a point:
            //Position = pos
            //Velocity = vel

            Vector2 d = vel; //Ray from start to end
            Vector2 f = pos; //Vector from this circle center to ray start

            float r = Radius + other.Radius;

            // If our circles are overlapping for whatever reason, encourage separation
            float distanceSquared = Vector2.DistanceSquared(Position, other.Position);
            if (distanceSquared < r * r && Vector2.DistanceSquared(Position + Velocity, other.Position + other.Velocity) < distanceSquared)
            {
                float massRatio = Mass / (Mass + other.Mass);
                Vector2 newVel = -pos * (1-massRatio) / s;
                Vector2 cNewVel = pos * (massRatio) / s;
                return new CollisionResult(Position, newVel, other.Position, cNewVel);
            }

            float a = Vector2.Dot(d, d);
            float b = Vector2.Dot(f, d) * 2;
            float c = Vector2.Dot(f, f) - r * r;

            float discriminant = b * b - 4 * a * c;
            if (discriminant < 0)
            {
                // no intersection
                return CollisionResult.None;
            }

            // ray didn't totally miss sphere,
            // so there is a solution to
            // the equation.

            discriminant = (float)Math.Sqrt(discriminant);

            // either solution may be on or off the ray so need to test both
            // t1 is always the smaller value, because BOTH discriminant and
            // a are nonnegative.
            float t1 = (-b - discriminant) / (2 * a);
            float t2 = (-b + discriminant) / (2 * a);

            //if (Math.Round(t1, 2) == 0) t1 = 0;
            //if (Math.Round(t2, 2) == 0) t2 = 0;
            //if (Math.Round(t1, 2) == 1) t1 = 1;
            //if (Math.Round(t2, 2) == 1) t2 = 1;

            // 3x HIT cases:
            //          -o->             --|-->  |            |  --|->
            // Impale(t1 hit,t2 hit), Poke(t1 hit,t2>1), ExitWound(t1<0, t2 hit), 

            // 3x MISS cases:
            //       ->  o                     o ->              | -> |
            // FallShort (t1>1,t2>1), Past (t1<0,t2<0), CompletelyInside(t1<0, t2>1)

            if (t1 >= 0 && t1 <= 1)
            {
                // t1 is the intersection, and it's closer than t2
                // (since t1 uses -b - discriminant)
                // Impale, Poke
                t = t1;

                // The position of the circles at time of collision
                Vector2 newPos = Position + Velocity * s * (float)(t - .005);
                Vector2 cNewPos = other.Position + other.Velocity * s * (float)(t - .005);

                // Calculate response (bounce)
                // The norm of the vector between the two circles
                Vector2 n = Vector2.Normalize(cNewPos - newPos);

                //impulse magnitude?
                Vector2 deltaV = Velocity - other.Velocity;
                float magnitude = deltaV.Length();

                //Resultant velocities
                Vector2 newV = Velocity - (1 / Mass) * magnitude * n;
                Vector2 cNewV = other.Velocity + (1 / other.Mass) * magnitude * n;
                
                return new CollisionResult(newPos, newV, cNewPos, cNewV);
            }

            // here t1 didn't intersect so we are either started
            // inside the sphere or completely past it
            if (t2 >= 0 && t2 <= 1)
            {
                // ExitWound
                //Since we are exiting the circle, don't collide
                return CollisionResult.None;
            }

            // no intn: FallShort, Past, CompletelyInside
            return CollisionResult.None;
        }

        public override CollisionResult Collide(float s, TriArray triArray)
        {
            //Implemented in TriArray.cs
            //So we negate the impulse
            return triArray.Collide(s, this).Switch();
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
