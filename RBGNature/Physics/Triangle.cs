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


        public bool Intersects(Circle c)
        {
            if (Intersects(c.Center)) return true;
            if (c.Intersects(A, B)) return true;
            if (c.Intersects(B, C)) return true;
            if (c.Intersects(A, C)) return true;
            return false;
        }

    }
}
