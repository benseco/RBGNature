using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RBGNature.Physics
{
    public class TriArray : PhysicsObject
    {
        private int[,,] array;

        public static Triangle GetTriangle(int i, int j, int k)
        {
            // Multiply by tile width to get top left bound
            const int w = 20;
            i *= w;
            j *= w;

            switch (k)
            {
                //These points should be listed in clockwise order
                case 0: return new Triangle(j, i, j + w, i, j + w / 2, i + w / 2); //TOP
                case 1: return new Triangle(j, i, j + w / 2, i + w / 2, j, i + w); //LEFT
                case 2: return new Triangle(j, i + w, j + w / 2, i + w / 2, j + w, i + w); //BOTTOM
                case 3: return new Triangle(j + w, i, j + w, i + w, j + w / 2, i + w / 2); //RIGHT
                default: throw new InvalidOperationException(string.Format("Triangle.GetTriangle was called with k={0}. K must be in [0,3].",k));
            }
        }

        public TriArray(int[,,] array)
        {
            this.array = array;
        }

        public override CollisionResult Collide(float s, Circle c)
        {
            float xIni = c.Position.X;
            float xFin = c.Position.X + c.Velocity.X * s;
            float yIni = c.Position.Y;
            float yFin = c.Position.Y + c.Velocity.Y * s;

            float xMin = Math.Min(xIni, xFin);
            float xMax = Math.Max(xIni, xFin);
            float yMin = Math.Min(yIni, yFin);
            float yMax = Math.Max(yIni, yFin);

            int leftBound = (int)((xMin - c.Radius) / 20);
            int rightBound = (int)((xMax + c.Radius) / 20);
            int topBound = (int)((yMin - c.Radius) / 20);
            int bottomBound = (int)((yMax + c.Radius) / 20);

            double time = 1;
            CollisionResult first = CollisionResult.None;

            for (int i = topBound; i <= bottomBound; i++) //row
            {
                if (i < 0 || i >= array.GetLength(0)) continue; // Index is out of bounds

                for (int j = leftBound; j <= rightBound; j++) //column
                {
                    if (j < 0 || j >= array.GetLength(1)) continue; // Index is out of bounds
                    
                    for (int k = 0; k < 4; k++) //subtriangle
                    {
                        if (array[i, j, k] != 1) continue; //triangle does not collide
                        
                        CollisionResult result = GetTriangle(i, j, k).CollideCircleAtTime(s, c, out double cTime);

                        if (result && cTime >= 0 && cTime <= time)
                        {
                            time = cTime;
                            first = result;
                        }
                    }
                }
            }

            return first;
        }

        public override CollisionResult Collide(float s, TriArray triArray)
        {
            throw new NotImplementedException();
        }
    }
}
