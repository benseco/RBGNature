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

        public override CollisionResult Collide(Circle c)
        {
            //int xIndex = (int)(c.Position.X / 20);
            //int yIndex = (int)(c.Position.Y / 20);
            int xMin = (int)((c.Position.X - c.Radius) / 20) - 1;
            int xMax = (int)((c.Position.X + c.Velocity.X + c.Radius) / 20) + 1;
            int yMin = (int)((c.Position.Y - c.Radius) / 20) - 1;
            int yMax = (int)((c.Position.Y + c.Velocity.Y + c.Radius) / 20) + 1;

            //Console.Write("Index: " + xIndex + ", " + yIndex);
            //Console.Write(" | XBounds: " + xMin + " - " + xMax);
            //Console.WriteLine(" | YBounds: " + yMin + " - " + yMax);

            double time = 1;
            CollisionResult first = CollisionResult.None;

            for (int i = yMin; i <= yMax; i++) //row
            {
                if (i < 0 || i >= array.GetLength(0)) continue; // Index is out of bounds

                for (int j = xMin; j <= xMax; j++) //column
                {
                    if (j < 0 || j >= array.GetLength(1)) continue; // Index is out of bounds
                    
                    for (int k = 0; k < 4; k++) //subtriangle
                    {
                        if (array[i, j, k] != 1) continue; //triangle does not collide
                        
                        CollisionResult result = GetTriangle(i, j, k).CollideCircleAtTime(c, out double cTime);
                        
                        if (result && cTime <= time)
                        {
                            time = cTime;
                            first = result;
                        }
                    }
                }
            }

            return first;
        }

        public override CollisionResult Collide(TriArray triArray)
        {
            //This should never occur as of now, so throw error.
            throw new NotImplementedException();
        }

        public override CollisionResult Collide(PhysicsObject other)
        {
            return other.Collide(this).Switch();
        }
    }
}
