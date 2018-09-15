﻿using System;
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
            PhysicsObject.TimeOfIntersection(circle.Position, circle.Velocity, A, B, out double tAB, out double uAB);
            PhysicsObject.TimeOfIntersection(circle.Position, circle.Velocity, B, C, out double tBC, out double uBC);
            PhysicsObject.TimeOfIntersection(circle.Position, circle.Velocity, C, A, out double tCA, out double uCA);



            // Triangle with edge AB
            // Circle with velocity vector CD and radius R
            // P = intersection of AB and CD
            // A = angle between AP and CP
            // Q = the position of circle center at time of intersection 
            // R / sin(A) = length(PQ)

            // Q = t * AB (t is time of intersection)
            // Edge with earliest t is first collision



            t = 0;
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