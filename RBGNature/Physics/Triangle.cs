using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace RBGNature.Physics
{
    public struct Triangle
    {
        public Vector2 A;
        public Vector2 B;
        public Vector2 C;

        public Triangle(float ax, float ay, float bx, float by, float cx, float cy)
        {
            A = new Vector2(ax, ay);
            B = new Vector2(bx, by);
            C = new Vector2(cx, cy);
        }

        public bool Intersects(Vector2 p) { return ContainsPoint(p, A, B, C); }

        private bool ContainsPoint(Vector2 p, Vector2 a, Vector2 b, Vector2 c) { return ContainsPoint(p.X, p.Y, a.X, a.Y, b.X, b.Y, c.X, c.Y); }
  
        private bool ContainsPoint(float x, float y, float ax, float ay, float bx, float by, float cx, float cy)
        {
            float A = 0.5f * (-by * cx + ay * (-bx + cx) + ax * (by - cy) + bx * cy);
            int sign = A < 0 ? -1 : 1;
            float s = (ay * cx - ax * cy + (cy - ay) * x + (ax - cx) * y) * sign;
            float t = (ax * by - ay * bx + (ay - by) * x + (bx - ax) * y) * sign;

            return s > 0 && t > 0 && (s + t) < 2 * A * sign;
        }

        public CollisionResult CollideCircleAtTime(Circle circle, out double t)
        {
            t = -1;

            CollisionResult rAB = CollideCircleWithEdgeAtTime(circle, A, B, out double tAB);
            CollisionResult rBC = CollideCircleWithEdgeAtTime(circle, B, C, out double tBC);
            CollisionResult rCA = CollideCircleWithEdgeAtTime(circle, C, A, out double tCA);

            CollisionResult earliestCollision = CollisionResult.None;
            double earliestTime = 2d; //arbitrary

            if (rAB && tAB < earliestTime)
            {
                earliestCollision = rAB;
                earliestTime = tAB;
            }
            if (rBC && tBC < earliestTime)
            {
                earliestCollision = rBC;
                earliestTime = tBC;
            }
            if (rCA && tCA < earliestTime)
            {
                earliestCollision = rCA;
                earliestTime = tCA;
            }

            if (earliestTime < 2d) t = earliestTime;
            return earliestCollision;

        }

        /*public CollisionResult CollideCircleWithEdgeAtTime(Circle circle, Vector2 I, Vector2 J, out double t)
        {
            t = -1;

            // Circle moves along path MN
            Vector2 M = circle.Position;
            Vector2 N = circle.Position + circle.Velocity;
            Vector2 MN = circle.Velocity;

            double timeOfIntersection = -1;

            if (!PhysicsObject.TimeOfIntersection(M, N, I, J, out timeOfIntersection, out double uCA)) return CollisionResult.None;

            // The first colliding edge, in vector form
            Vector2 IJ = J - I;

            // Angle between MN and IJ
            double a = Math.Atan2((MN.X * IJ.Y - MN.Y * IJ.X), Vector2.Dot(MN, IJ));

            // Distance between center of circle and point of intersection at time t
            double distCenterToIntersection = Math.Abs(circle.Radius / Math.Sin(a));

            // Report the actual time of collision given the circle's radius
            t = timeOfIntersection - (distCenterToIntersection / MN.Length());

            if (t >= 0 && t <= 1)
            {
                // First two parameters are zero since we don't need to report collision info for the triangle itself
                //TODO: new velocity for circle is more complex than just Vector2.Negate(MN)
                return new CollisionResult(Vector2.Zero, Vector2.Zero, M + (float)(t - 0.01) * MN, Vector2.Negate(MN));
            }


            return CollisionResult.None;
        }*/

        public CollisionResult CollideCircleWithEdgeAtTime(Circle circle, Vector2 I, Vector2 J, out double t)
        {
            t = -1; //arbitrary error case

            // Circle moves along path MN
            Vector2 M = circle.Position;
            Vector2 N = circle.Position + circle.Velocity;
            Vector2 MN = circle.Velocity;

            // Collide with IJ translated by the normal of the circle's radius in length
            Vector2 IJ = J - I;
            Vector2 normIJ = new Vector2(IJ.Y, -IJ.X); //negate the Y value because the edge runs clockwise
            normIJ.Normalize();
            normIJ *= circle.Radius;
            Vector2 P = I + normIJ;
            Vector2 Q = J + normIJ;

            double timeOfEdgeIntersection = -1;
            bool intersectsEdge = PhysicsObject.TimeOfIntersection(M, N, P, Q, out timeOfEdgeIntersection, out double u);
            if (!intersectsEdge || u < 0 || u > 1) timeOfEdgeIntersection = -1; // Invalid intersections

            // Collide circle as point with a new circle with center I and the same radius as circle (corner circle)
            //TODO: we shouldn't create an object every time - create a struct circle representation or equivalent method
            Circle point = new Circle() { Position = circle.Position, Velocity = circle.Velocity };
            Circle corner = new Circle() { Position = I, Radius = circle.Radius };
            double timeOfCornerIntersection = -1;
            point.CollideCircleAtTime(corner, out timeOfCornerIntersection);
            
            // Earliest wins
            if (timeOfEdgeIntersection >= 0 && timeOfEdgeIntersection <= 1)
            {
                if (timeOfCornerIntersection >= 0 && timeOfCornerIntersection <= 1)
                {
                    if (timeOfEdgeIntersection < timeOfCornerIntersection)
                    {
                        // Collision at edge IJ came first
                        t = timeOfEdgeIntersection;
                        Vector2 newCirclePos = M + (float)t * MN - 0.5f * Vector2.Normalize(MN); //Separation of .5 unit
                        //return new CollisionResult(Vector2.Zero, Vector2.Zero, newCirclePos, Vector2.Negate(MN));
                        return new CollisionResult(Vector2.Zero, Vector2.Zero, newCirclePos, Vector2.Zero);
                    }
                    else
                    {
                        // Collision at corner I came first
                        t = timeOfCornerIntersection;
                        Vector2 newCirclePos = M + (float)t * MN - 0.5f * Vector2.Normalize(MN); //Separation of .5 unit
                        //return new CollisionResult(Vector2.Zero, Vector2.Zero, newCirclePos, Vector2.Negate(MN));
                        return new CollisionResult(Vector2.Zero, Vector2.Zero, newCirclePos, Vector2.Zero);
                    }
                }
                else
                {
                    // Collision only at edge IJ
                    t = timeOfEdgeIntersection;
                    Vector2 newCirclePos = M + (float)t * MN - 0.5f * Vector2.Normalize(MN); //Separation of .5 unit
                    //return new CollisionResult(Vector2.Zero, Vector2.Zero, newCirclePos, Vector2.Negate(MN));
                    return new CollisionResult(Vector2.Zero, Vector2.Zero, newCirclePos, Vector2.Zero);
                }
            }
            else if (timeOfCornerIntersection >= 0 && timeOfCornerIntersection <= 1)
            {
                // Collision only at corner I
                t = timeOfCornerIntersection;
                Vector2 newCirclePos = M + (float)t * MN - 0.5f * Vector2.Normalize(MN); //Separation of .5 unit
                //return new CollisionResult(Vector2.Zero, Vector2.Zero, newCirclePos, Vector2.Negate(MN));
                return new CollisionResult(Vector2.Zero, Vector2.Zero, newCirclePos, Vector2.Zero);
            }

            return CollisionResult.None;
        }


        //public bool Intersects(Circle c)
        //{

        //    CollisionResult ab = c.Intersects(A, B);
        //    CollisionResult bc = c.Intersects(B, C);
        //    CollisionResult ac = c.Intersects(A, C);

        //    float ablen = ab.Impulse.LengthSquared();
        //    float bclen = bc.Impulse.LengthSquared();
        //    float aclen = ac.Impulse.LengthSquared();

        //    if (ablen >= bclen && ablen >= aclen) return 


        //    if (ab.Impulse.LengthSquared() > bc.Impulse.LengthSquared())
        //    {
        //        if (ab.Impulse.LengthSquared)
        //    }

        //    if (c.Intersects(B, C)) return true;
        //    if (c.Intersects(A, C)) return true;


        //    CollisionResult inside = Intersects(c.Position);




        //    if (Intersects(c.Position)) return true;
        //    return false;
        //}

    }
}
