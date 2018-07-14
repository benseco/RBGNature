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
    static class Tri
    {
        public static bool PIT(Point p, Point a, Point b, Point c) { return PointInTriangle(p.X, p.Y, a.X, a.Y, b.X, b.Y, c.X, c.Y); }

        public static bool PIT(float x, float y, float ax, float ay, float bx, float by, float cx, float cy) { return PointInTriangle(x, y, ax, ay, bx, by, cx, cy); }

        public static bool PointInTriangle(float x, float y, float ax, float ay, float bx, float by, float cx, float cy)
        {
            float A = 0.5f * (-by * cx + ay * (-bx + cx) + ax * (by - cy) + bx * cy);
            int sign = A < 0 ? -1 : 1;
            float s = (ay * cx - ax * cy + (cy - ay) * x + (ax - cx) * y) * sign;
            float t = (ax * by - ay * bx + (ay - by) * x + (bx - ax) * y) * sign;

            return s > 0 && t > 0 && (s + t) < 2 * A * sign;
        }

    }
}
