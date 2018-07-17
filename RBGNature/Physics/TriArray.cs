using System;
using System.Collections.Generic;
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
                case 0: return new Triangle(j, i, j + w, i, j + w / 2, i + w / 2); //TOP
                case 1: return new Triangle(j, i, j, i + w, j + w / 2, i + w / 2); //LEFT
                case 2: return new Triangle(j, i + w, j + w, i + w, j + w / 2, i + w / 2); //BOTTOM
                case 3: return new Triangle(j + w, i, j + w, i + w, j + w / 2, i + w / 2); //RIGHT
                default: throw new InvalidOperationException(string.Format("Triangle.GetTriangle was called with k={0}. K must be in [0,3].",k));
            }
        }

        public TriArray(int[,,] array)
        {
            this.array = array;
        }

        public override CollisionResult Collide(Circle c)
        {
            int xIndex = (int)(c.Center.X / 20);
            int yIndex = (int)(c.Center.Y / 20);
            int xMin = (int)((c.Center.X - c.Radius) / 20);
            int xMax = (int)((c.Center.X + c.Radius) / 20);
            int yMin = (int)((c.Center.Y - c.Radius) / 20);
            int yMax = (int)((c.Center.Y + c.Radius) / 20);

            Console.Write("Index: " + xIndex + ", " + yIndex);
            Console.Write(" | XBounds: " + xMin + " - " + xMax);
            Console.WriteLine(" | YBounds: " + yMin + " - " + yMax);

            for (int i = yMin; i <= yMax; i++) //row
            {
                if (i < 0 || i >= array.GetLength(0)) continue; // Index is out of bounds

                for (int j = xMin; j <= xMax; j++) //column
                {
                    if (j < 0 || j >= array.GetLength(1)) continue; // Index is out of bounds
                    
                    for (int k = 0; k < 4; k++) //subtriangle
                    {
                        if (array[i, j, k] != 1) continue; //triangle does not collide

                        if (GetTriangle(i, j, k).Intersects(c)) return new CollisionResult(true);
                    }
                }
            }

            return new CollisionResult(false);
        }

        public override CollisionResult Collide(TriArray triArray)
        {
            //This should never occur as of now, so throw error.
            throw new NotImplementedException();
        }

        public override CollisionResult Collide(PhysicsObject other)
        {
            return other.Collide(this);
        }
    }
}
